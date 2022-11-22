using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CompteResultat.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;

namespace CompteResultat.DAL
{
    [MetadataType(typeof(Import.MetaData))]
    public partial class Import
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static List<Import> GetImports()
        {
            return GetImports("Date", "ASC", "All");
        }

        public static List<Import> GetImports(string sortExpression, string sortDirection, string sortExpressionArchived)
        {
            try
            {
                List<Import> imports;

                using (var context = new CompteResultatEntities())
                {
                    if (sortExpressionArchived == "Archived")
                        imports = context.Imports.Where(i => i.Archived == true).ToList();
                    else if (sortExpressionArchived == "NonArchived")
                        imports = context.Imports.Where(i => i.Archived == false).ToList();
                    else
                        imports = context.Imports.ToList();

                    if (sortExpression == "Name")
                    {
                        if(sortDirection == "ASC")
                            imports = imports.OrderBy(i => i.Name).ToList();
                        else
                            imports = imports.OrderByDescending(i => i.Name).ToList();
                    }
                    else if (sortExpression == "UserName")
                    {
                        if (sortDirection == "ASC")
                            imports = imports.OrderBy(i => i.UserName).ToList();
                        else
                            imports = imports.OrderByDescending(i => i.UserName).ToList();
                    }
                    else if (sortExpression == "Archived")
                    {
                        if (sortDirection == "ASC")
                            imports = imports.OrderBy(i => i.Archived).ToList();
                        else
                            imports = imports.OrderByDescending(i => i.Archived).ToList();
                    }
                    else if (sortExpression == "Date")
                    {
                        if (sortDirection == "ASC")
                            imports = imports.OrderBy(i => i.Date).ToList();
                        else
                            imports = imports.OrderByDescending(i => i.Date).ToList();
                    }
                    else if (sortExpression == "ImportPath")
                    {
                        if (sortDirection == "ASC")
                            imports = imports.OrderBy(i => i.ImportPath).ToList();
                        else
                            imports = imports.OrderByDescending(i => i.ImportPath).ToList();
                    }
                    else if (sortExpression == "OnlyArchived")
                    {
                        if (sortDirection == "ASC")
                            imports = imports.Where(i => i.Archived == true).OrderBy(i => i.Date).ToList();
                        else
                            imports = imports.Where(i => i.Archived == true).OrderByDescending(i => i.Date).ToList();
                    }
                    else if (sortExpression == "OnlyNonArchived")
                    {
                        if (sortDirection == "ASC")
                            imports = imports.Where(i => i.Archived == false).OrderBy(i => i.Date).ToList();
                        else
                            imports = imports.Where(i => i.Archived == false).OrderByDescending(i => i.Date).ToList();
                    }
                    else
                        imports = imports.OrderByDescending(i => i.Date).ToList();
                }
                
                return imports;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }
        
        public static List<Import> GetImportsByName(string searchText)
        {
            try
            {
                List<Import> imports;
                using (var context = new CompteResultatEntities())
                {
                    imports = context.Imports.Where(i => i.Name.ToLower().Contains(searchText.ToLower())).OrderByDescending(i => i.Date).ToList();
                }

                return imports;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }
      
        public static List<Import> GetImportsWithoutArchive(string sortExpression)
        {
            try
            {
                List<Import> imports = GetImports(sortExpression, "ASC", "NonArchived"); 
                return imports;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int Insert(Import imp)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {                    
                    context.Imports.Add(imp);
                    context.SaveChanges();

                    return imp.Id;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void UpdateArchivedFlagAndImportName(int id, bool archived, string importName)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    Import imp = context.Imports.Where(i => i.Id == id).ToList()[0];
                    imp.Archived = archived;
                    imp.Name = importName;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void DeleteImportWithId(int importId)
        {
            try
            {
                Import imp;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.Imports.Where(i => i.Id == importId);

                    if (elements.Any())
                    {
                        context.Imports.Remove(elements.First());
                        context.SaveChanges();
                    }                    
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }


        

        //MetaData definition for basic validation
        public class MetaData
        {
            //[Display(Name = "Import Name")]
            //[Required(ErrorMessage = "Please provide a name for the import!")]
            //public string Name { get; set; }

        }





        #region OLD METHODS

        public static void NOTNEEDEDUpdateCorrespondingTablesAfterImport()
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    //Verify Assureur's
                    List<string> existingAss = Assureur.GetUniqueAssNames();
                    List<string> newAss = C_TempOtherFields.GetUniqueAssNames();
                    List<string> assToBeAdded = newAss.Where(i => !existingAss.Contains(i)).ToList();

                    foreach (string ass in assToBeAdded)
                    {
                        Assureur assur = new Assureur { Name = ass };
                        context.Assureurs.Add(assur);

                        int assId = assur.Id;
                        //int assId = Assureur.Insert(new Assureur { Name = ass });
                    }

                    //Verify ContractId's
                    List<string> existingContr = Contract.GetUniqueContractIds();
                    List<string> newContr = C_TempOtherFields.GetUniqueContractIds();
                    List<string> contractsToBeAdded = newContr.Where(i => !existingContr.Contains(i)).ToList();

                    foreach (string contr in contractsToBeAdded)
                    {
                        //get the AssureurName for the given contract from TempTable
                        string assName = C_TempOtherFields.GetAssNamesForContract(contr);
                        if (assName == C.cINVALIDSTRING)
                            throw new Exception("No insurance company was found in the '_TempOtherFields' table for the following contract: " + contr);
                        //with the AssureurName, get the AssId from the Assureur Table
                        int assId = Assureur.GetAssIdForAssName(assName);
                        if (assId == C.cINVALIDID)
                            throw new Exception("No insurance company ID was found in the 'Assureur' table for the following insurance company: " + assName);

                        context.Contracts.Add(new Contract { ContractId = contr });

                        //int contrId = Contract.Insert(new Contract { ContractId = contr, AssureurId = assId });
                    }



                    //save all changes
                    context.SaveChanges();

                } //end of: using (var context...
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        #endregion



    }
}
