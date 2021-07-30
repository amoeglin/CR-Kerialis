using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Data;

using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Text.RegularExpressions;

using Excel;
using CompteResultat.DAL;
using CompteResultat.Common;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CompteResultat.BL
{
    public class BLGroupsAndGaranties
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void RecreateGroupsGarantiesSanteFromPresta()
        {
            try
            {
                //truncate table GroupGarantySante
                GroupGarantySante.TruncateTable();

                //get data from PrestSante
                List<GroupesGarantiesSante> GroupGarantyList = PrestSante.GetGroupGarantyList();

                //add data to GroupGarantySante
                // ### Paramètres par défaut
                foreach (GroupesGarantiesSante item in GroupGarantyList)
                {
                    int id = GroupGarantySante.InsertGroupGaranty(new GroupGarantySante
                    {
                        AssureurName = item.AssureurName,
                        GroupName = item.GroupName,
                        GarantyName = item.GarantyName,
                        CodeActe = item.CodeActe,
                        OrderNumber = 1
                    });
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void ImportGroupsGarantiesSanteForAssureur(string assureurName, string excelFilePath, bool firstRowAsColumnNames)
        {
            try
            {
                string groupName;
                string garantyName;
                string codeActe;
                int orderNumber;

                //read Excel file into datatable
                DataTable dt = G.ExcelToDataTable(excelFilePath, firstRowAsColumnNames);
                
                // delete all rows in DB Tables with the specified assurName 
                GroupGarantySante.DeleteGroupsWithSpecificAssureurName(assureurName);
                
                foreach (DataRow row in dt.Rows)
                {
                    //### validate => all fields must be specified
                    groupName = row[C.eExcelGroupsGaranties.GroupName.ToString()].ToString();
                    garantyName = row[C.eExcelGroupsGaranties.GarantyName.ToString()].ToString();
                    codeActe = row[C.eExcelGroupsGaranties.CodeActe.ToString()].ToString();
                    
                    if (!Int32.TryParse(row[C.eExcelGroupsGaranties.OrderNumber.ToString()].ToString(), out orderNumber))
                        orderNumber = 9999;

                    int id = GroupGarantySante.InsertGroupGaranty(new GroupGarantySante
                    {
                        AssureurName = assureurName,
                        GroupName = groupName,
                        GarantyName = garantyName,
                        CodeActe = codeActe,
                        OrderNumber = orderNumber
                    });                    
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static ExcelPackage ExportGroupsGarantiesSanteForAssureur(string assureurName)
        {
            try
            {
                List<GroupGarantySante> groupGaranties = GroupGarantySante.GetGroupsAndGarantiesForAssureur(assureurName);
                
                ExcelPackage pck = new ExcelPackage();
                var ws = pck.Workbook.Worksheets.Add(assureurName);

                //write the header
                ws.Cells[1, 1].Value = "GroupName"; 
                ws.Cells[1, 2].Value = "GarantyName";
                ws.Cells[1, 3].Value = "CodeActe"; 
                ws.Cells[1, 4].Value = "OrderNumber"; 

                int row = 2;

                foreach (GroupGarantySante ggs in groupGaranties)
                {                    
                    ws.Cells[row, 1].Value = ggs.GroupName;
                    ws.Cells[row, 2].Value = ggs.GarantyName;
                    ws.Cells[row, 3].Value = ggs.CodeActe;
                    ws.Cells[row, 4].Value = ggs.OrderNumber;

                    row++;   
                }

                return pck;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }
        
        public static GenericClasses GetNormalizedGroupGarantyPair(string assureurName, string codeActe, List<GroupGarantySante> groupSanteListAll)
        {
            try
            {
                GenericClasses myGGPair = new GenericClasses();
                myGGPair.AssureurName = assureurName;
                myGGPair.CodeActe = codeActe;

                string myGroupName = "";
                string myGarName = "";
                string errMessAssId = "No labels for 'GroupSante' and 'GarantySante' were found for the following CodeActe: " + codeActe.ToString() + " and for the following Assureur Name: " + assureurName;
                string errMessAssDefaultId = "No labels for 'GroupSante' and 'GarantySante' were found for the following CodeActe: " + codeActe.ToString() + " and for the default Assureur";


                //var element = groupSanteListAll.Where(g => g.GarantySantes.Any(gar => gar.AssurId == assureurId && gar.CodeActe == codeActe));
                var element = groupSanteListAll.Where(g => g.AssureurName == assureurName && g.CodeActe == codeActe);

                if (element.Any())
                {
                    myGroupName = element.First().GroupName;
                    myGarName = element.First().GarantyName;                    
                }
                else
                {
                    //we didn't fint any Groups - Garanties for a specific AssureurId => maybe no unique Group-Garanty values
                    //have been assigned for this Assureur => check the default Group-Garanty values for Assureur == -1
                    element = groupSanteListAll.Where(g => g.AssureurName == C.cDEFAULTASSUREUR && g.CodeActe == codeActe);
                    if (element.Any())
                    {
                        myGroupName = element.First().GroupName;
                        myGarName = element.First().GarantyName;
                    }
                    else
                    {
                        //no group and garanty were found for the specified codeActe                        
                        throw new Exception(errMessAssDefaultId);
                    }
                }

                myGGPair.GroupName = myGroupName;
                myGGPair.GarantyName = myGarName;

                return myGGPair;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }


        }




        #region OLD FUNCTIONS FOR SEPARATE GROUP & GARANTY Tables

        //public static GroupGarantyPair OLDGetNormalizedGroupGarantyPair(int assureurId, string codeActe, List<GroupSante> groupSanteListAll)
        //{
        //    try
        //    {
        //        GroupGarantyPair myGGPair = new GroupGarantyPair();
        //        //myGGPair.AssureurId = assureurId;
        //        myGGPair.CodeActe = codeActe;

        //        string myGroupName = "";
        //        string myGarName = "";
        //        //string assurName = Assureur.GetAssById(assureurId).Name;
        //        string errMessAssId = "No labels for 'GroupSante' and 'GarantySante' were found for the following CodeActe: " + codeActe.ToString() + " and for the following AssureurId: " + assureurId;
        //        string errMessAssDefaultId = "No labels for 'GroupSante' and 'GarantySante' were found for the following CodeActe: " + codeActe.ToString() + " and for the default AssureurId";


        //        var element = groupSanteListAll.Where(g => g.GarantySantes.Any(gar => gar.AssurId == assureurId && gar.CodeActe == codeActe));
        //        if (element.Any())
        //        {
        //            myGroupName = element.First().Name;

        //            var elemGaranty = element.First().GarantySantes;
        //            if (elemGaranty.Any())
        //            {
        //                myGarName = element.First().GarantySantes.Where(g => g.CodeActe == codeActe).First().Name;
        //                // myGarName = element.First().GarantySantes.First().Name;
        //            }
        //            else
        //                throw new Exception(errMessAssId);
        //        }
        //        else
        //        {
        //            //we didn't fint any Groups - Garanties for a specific AssureurId => maybe no unique Group-Garanty values
        //            //have been assigned for this Assureur => check the default Group-Garanty values for AssureurId == -1
        //            element = groupSanteListAll.Where(g => g.GarantySantes.Any(gar => gar.AssurId == C.cDEFAULTID && gar.CodeActe == codeActe));
        //            if (element.Any())
        //            {
        //                myGroupName = element.First().Name;

        //                var elemGaranty = element.First().GarantySantes;
        //                if (elemGaranty.Any())
        //                {
        //                    myGarName = element.First().GarantySantes.Where(g => g.CodeActe == codeActe).First().Name;
        //                    //myGarName = element.First().GarantySantes.First().Name;
        //                }
        //                else
        //                    throw new Exception(errMessAssDefaultId);
        //            }
        //            else
        //            {
        //                //no group and garanty were found for the specified codeActe                        
        //                throw new Exception(errMessAssDefaultId);
        //            }
        //        }

        //        myGGPair.GroupName = myGroupName;
        //        myGGPair.GarantyName = myGarName;

        //        return myGGPair;

        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message);
        //        throw ex;
        //    }


        //}

        //public static void oldImportGroupsAndGarantiesForAssureur(int assureurId, string excelFilePath, bool firstRowAsColumnNames)
        //{
        //    try
        //    {
        //        //read Excel file into datatable
        //        DataTable dt = G.ExcelToDataTable(excelFilePath, firstRowAsColumnNames);

        //        //get distinct group names 
        //        DataView view = new DataView(dt);

        //        //DataTable distinctGroups = view.ToTable(true, C.eExcelGroupsGaranties.GroupName.ToString(), "Type");

        //        //var groupedTable = dt.AsEnumerable()
        //        //   .GroupBy(a => new {                       
        //        //       Group = a.Field<string>("GroupName")
        //        //   }
        //        //            )
        //        //   .Select(g => g.First())
        //        //   .ToList();


        //        //### the following part should be in the data layer using context object in order to being able to rollback all transactions


        //        // delete all rows in DB Tables with the specified companyId => this will also delete related items in Garantees table
        //        GroupSante.DeleteGroupsWithSpecificAssureurId(assureurId);
                
        //        //add the groups to DB Table - store returned Id & Name in dictionary
        //        Dictionary<string, int> dicGroupSanteIds = new Dictionary<string, int>();
        //        Dictionary<string, int> dicGroupPrevIds = new Dictionary<string, int>();

        //        foreach (DataRow row in dt.Rows)
        //        {
        //            string groupName = row[C.eExcelGroupsGaranties.GroupName.ToString()].ToString();

        //            int orderNumber = 0;

        //            //### order number is not required
        //            //if (!Int32.TryParse(row[C.eExcelGroupsGaranties.Order.ToString()].ToString(), out orderNumber))
        //            //    orderNumber = 9999;

        //            if (groupName != "")
        //            {
        //                if (row[C.eExcelGroupsGaranties.Type.ToString()].ToString().ToLower() == C.eExcelGroupTypes.Sante.ToString().ToLower())
        //                {
        //                    if (!dicGroupSanteIds.ContainsKey(groupName))
        //                    {
        //                        GroupSante group = new GroupSante { AssurId = assureurId, Name = groupName, OrderNumber = orderNumber };
        //                        int id = GroupSante.InsertGroup(group);
        //                        dicGroupSanteIds.Add(groupName, id);
        //                    }
        //                }
        //                else if (row[C.eExcelGroupsGaranties.Type.ToString()].ToString().ToLower() == C.eExcelGroupTypes.Prev.ToString().ToLower())
        //                {
        //                    if (!dicGroupSanteIds.ContainsKey(groupName))
        //                    {
        //                        //GroupPrev group = new GroupPrev { AssurId = assureurId, Name = groupName, OrderNumber = orderNumber };
        //                        //int id = GroupPrev.InsertGroup(group);
        //                        //dicGroupPrevIds.Add(groupName, id);
        //                    }
        //                }
        //            }
        //        }

        //        //Iterate all rows in Excel file and add all garanties to DB Table - for each row, get the GroupId from the dictionary 
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            string codeActe = row[C.eExcelGroupsGaranties.CodeActe.ToString()].ToString();

        //            if (row[C.eExcelGroupsGaranties.Type.ToString()].ToString().ToLower() == C.eExcelGroupTypes.Sante.ToString().ToLower())
        //            {
        //                if (row[C.eExcelGroupsGaranties.GarantyName.ToString()].ToString() != "")
        //                {
        //                    GarantySante garanty = new GarantySante
        //                    {
        //                        GroupSanteId = dicGroupSanteIds[row[C.eExcelGroupsGaranties.GroupName.ToString()].ToString()],
        //                        Name = row[C.eExcelGroupsGaranties.GarantyName.ToString()].ToString(),
        //                        CodeActe = codeActe,
        //                        AssurId = assureurId
        //                    };

        //                    int id = GarantySante.InsertGaranty(garanty);
        //                }
        //            }
        //            else if (row[C.eExcelGroupsGaranties.Type.ToString()].ToString().ToLower() == C.eExcelGroupTypes.Prev.ToString().ToLower())
        //            {
        //                if (row[C.eExcelGroupsGaranties.GarantyName.ToString()].ToString() != "")
        //                {
        //                    //GarantyPrev garanty = new GarantyPrev
        //                    //{
        //                    //    GroupPrevId = dicGroupPrevIds[row[C.eExcelGroupsGaranties.GroupName.ToString()].ToString()],
        //                    //    Name = row[C.eExcelGroupsGaranties.GarantyName.ToString()].ToString(),
        //                    //    CodeActe = codeActe,
        //                    //    AssurId = assureurId
        //                    //};

        //                    //int id = GarantyPrev.InsertGaranty(garanty);
        //                }
        //            }
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message);
        //        throw ex;
        //    }
        //}

        //public static ExcelPackage oldExportGroupsAndGarantiesForAssureur(int assureurId, string assureurName)
        //{
        //    try
        //    {
        //        //get group & garanty data from DB
        //        List<GroupSante> groupsSante = GroupSante.GetGroupsAndGarantiesForAssureur(assureurId);
        //        //List<GroupPrev> groupsPrev = GroupPrev.GetGroupsAndGarantiesForAssureur(assureurId);

        //        //create the Excel File
        //        ExcelPackage pck = new ExcelPackage();
        //        var ws = pck.Workbook.Worksheets.Add(assureurName);

        //        //write the header
        //        ws.Cells[1, 1].Value = C.eExcelGroupsGaranties.Type.ToString();
        //        ws.Cells[1, 2].Value = C.eExcelGroupsGaranties.GroupName.ToString();
        //        ws.Cells[1, 3].Value = C.eExcelGroupsGaranties.GarantyName.ToString();
        //        ws.Cells[1, 4].Value = C.eExcelGroupsGaranties.CodeActe.ToString();

        //        int row = 2;

        //        foreach (GroupSante group in groupsSante)
        //        {
        //            if (group.GarantySantes.Count == 0)
        //            {
        //                ws.Cells[row, 1].Value = C.eExcelGroupTypes.Sante.ToString();
        //                ws.Cells[row, 2].Value = group.Name;
        //                ws.Cells[row, 3].Value = "";
        //                ws.Cells[row, 4].Value = "";

        //                row++;
        //            }
        //            else
        //            {
        //                foreach (GarantySante gar in group.GarantySantes)
        //                {
        //                    //Console.WriteLine("Sante" + " - " + group.Name + " - " + gar.Name);
        //                    ws.Cells[row, 1].Value = C.eExcelGroupTypes.Sante.ToString();
        //                    ws.Cells[row, 2].Value = group.Name;
        //                    ws.Cells[row, 3].Value = gar.Name;
        //                    ws.Cells[row, 4].Value = gar.CodeActe;

        //                    row++;
        //                }
        //            }
        //        }

        //        //add Prev groups and garanties
        //        //foreach (GroupPrev group in groupsPrev)
        //        //{
        //        //    if (group.GarantyPrevs.Count == 0)
        //        //    {
        //        //        ws.Cells[row, 1].Value = C.eExcelGroupTypes.Prev.ToString();
        //        //        ws.Cells[row, 2].Value = group.Name;
        //        //        ws.Cells[row, 3].Value = "";
        //        //        ws.Cells[row, 4].Value = "";

        //        //        row++;
        //        //    }
        //        //    else
        //        //    {
        //        //        //foreach (GarantyPrev gar in group.GarantyPrevs)
        //        //        //{
        //        //        //    //Console.WriteLine("Sante" + " - " + group.Name + " - " + gar.Name);
        //        //        //    ws.Cells[row, 1].Value = C.eExcelGroupTypes.Prev.ToString();
        //        //        //    ws.Cells[row, 2].Value = group.Name;
        //        //        //    ws.Cells[row, 3].Value = gar.Name;
        //        //        //    ws.Cells[row, 4].Value = gar.CodeActe;

        //        //        //    row++;
        //        //        //}
        //        //    }
        //        //}

        //        return pck;

        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message);
        //        throw ex;
        //    }
        //}
        

        #endregion

    }
}
