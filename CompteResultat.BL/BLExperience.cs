using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using CompteResultat.DAL;
using CompteResultat.Common;
using OfficeOpenXml;

namespace CompteResultat.BL
{
    public class BLExperience
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static void ImportExperienceForAssureur(string assureurName, string excelFilePath, bool firstRowAsColumnNames)
        {
            try
            {
                int importId;
                string assName;
                DateTime au;
                string contrat;
                string codCol;
                int anneeExp;
                string libActe;
                string libFam;
                string typeCas;
                int numActe;
                double fraisReel;
                double rembSS;
                double rembAnnexe;
                double rembNous;
                string reseau;
                double minFR;
                double maxFR;
                double minNous;
                double maxNous;

                //read Excel file into datatable
                DataTable dt = G.ExcelToDataTable(excelFilePath, firstRowAsColumnNames);

                // delete all rows in DB Tables with the specified assurName 
                C_TempExpData.DeleteExperienceWithSpecificAssureurName(assureurName);

                foreach (DataRow row in dt.Rows)
                {
                    //### validate => all fields must be specified                    
                    //codeActe = row[C.eExcelGroupsGaranties.CodeActe.ToString()].ToString();

                    if (!Int32.TryParse(row[0].ToString(), out importId))
                        throw new Exception("One of the provided 'ImportId' values is not valid for the Experience data you are trying to import !");
                    assName = row[1].ToString();
                    if (!DateTime.TryParse(row[2].ToString(), out au))
                        throw new Exception("One of the provided 'Au' values is not valid for the Experience data you are trying to import !");
                    contrat = row[3].ToString();
                    codCol = row[4].ToString();
                    if (!Int32.TryParse(row[5].ToString(), out anneeExp))
                        throw new Exception("One of the provided 'AnneeExp' values is not valid for the Experience data you are trying to import !");
                    libActe = row[6].ToString();
                    libFam = row[7].ToString();
                    typeCas = row[8].ToString();
                    if (!Int32.TryParse(row[9].ToString(), out numActe))
                        throw new Exception("One of the provided 'NombreActe' values is not valid for the Experience data you are trying to import !");
                    if (!double.TryParse(row[10].ToString(), out fraisReel))
                        throw new Exception("One of the provided 'FraisReel' values is not valid for the Experience data you are trying to import !");
                    if (!double.TryParse(row[11].ToString(), out rembSS))
                        throw new Exception("One of the provided 'RembSS' values is not valid for the Experience data you are trying to import !");
                    if (!double.TryParse(row[12].ToString(), out rembAnnexe))
                        throw new Exception("One of the provided 'RembAnnexe' values is not valid for the Experience data you are trying to import !");
                    if (!double.TryParse(row[13].ToString(), out rembNous))
                        throw new Exception("One of the provided 'RembNous' values is not valid for the Experience data you are trying to import !");
                    reseau = row[14].ToString();
                    if (!double.TryParse(row[15].ToString(), out minFR))
                        throw new Exception("One of the provided 'MinFR' values is not valid for the Experience data you are trying to import !");
                    if (!double.TryParse(row[16].ToString(), out maxFR))
                        throw new Exception("One of the provided 'MaxFR' values is not valid for the Experience data you are trying to import !");
                    if (!double.TryParse(row[17].ToString(), out minNous))
                        throw new Exception("One of the provided 'MinNous' values is not valid for the Experience data you are trying to import !");
                    if (!double.TryParse(row[18].ToString(), out maxNous))
                        throw new Exception("One of the provided 'MaxNous' values is not valid for the Experience data you are trying to import !");

                    int id = C_TempExpData.InsertExp(new C_TempExpData
                    {
                        ImportId = importId,
                        AssureurName = assureurName,
                        Au=au,
                        Contrat=contrat,
                        CodCol=codCol,
                        AnneeExp=anneeExp,
                        LibActe=libActe,
                        LibFam=libFam,
                        TypeCas=typeCas,
                        NombreActe=numActe,
                        Fraisreel=fraisReel,
                        Rembss=rembSS,
                        RembAnnexe=rembAnnexe,
                        RembNous=rembNous,
                        Reseau=reseau,
                        MinFr=minFR,
                        MaxFr=maxFR,
                        MinNous=minNous,
                        MaxNous=maxNous                                                
                    });
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static ExcelPackage ExportExperienceForAssureur(string assureurName)
        {
            try
            {
                List<C_TempExpData> exp = C_TempExpData.GetExpDataForAssureur(assureurName);

                ExcelPackage pck = new ExcelPackage();
                var ws = pck.Workbook.Worksheets.Add(assureurName);

                //write the header
                //ws.Column(2).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                ws.Column(3).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;

                ws.Cells[1, 1].Value = "Import Id";
                ws.Cells[1, 2].Value = "Nom Assureur";
                ws.Cells[1, 3].Value = "Au";
                ws.Cells[1, 4].Value = "Contrat";
                ws.Cells[1, 5].Value = "Code College";
                ws.Cells[1, 6].Value = "Annee Experience";
                ws.Cells[1, 7].Value = "Libelle Acte";
                ws.Cells[1, 8].Value = "Libelle Famille";
                ws.Cells[1, 9].Value = "Type CAS";
                ws.Cells[1, 10].Value = "Nombre Acte";
                ws.Cells[1, 11].Value = "Frais Reel";
                ws.Cells[1, 12].Value = "Remb SS";
                ws.Cells[1, 13].Value = "Remb Annexe";
                ws.Cells[1, 14].Value = "Remb Nous";
                ws.Cells[1, 15].Value = "Reseau";
                ws.Cells[1, 16].Value = "Min FR";
                ws.Cells[1, 17].Value = "Max FR";
                ws.Cells[1, 18].Value = "Min Nous";
                ws.Cells[1, 19].Value = "Max Nous";

                int row = 2;

                foreach (C_TempExpData c in exp)
                {
                    ws.Cells[row, 1].Value = c.ImportId;
                    ws.Cells[row, 2].Value = c.AssureurName;
                    ws.Cells[row, 3].Value = c.Au;
                    ws.Cells[row, 4].Value = c.Contrat;
                    ws.Cells[row, 5].Value = c.CodCol;

                    ws.Cells[row, 6].Value = c.AnneeExp;
                    ws.Cells[row, 7].Value = c.LibActe;
                    ws.Cells[row, 8].Value = c.LibFam;
                    ws.Cells[row, 9].Value = c.TypeCas;
                    ws.Cells[row, 10].Value = c.NombreActe;

                    ws.Cells[row, 11].Value = c.Fraisreel;
                    ws.Cells[row, 12].Value = c.Rembss;
                    ws.Cells[row, 13].Value = c.RembAnnexe;
                    ws.Cells[row, 14].Value = c.RembNous;
                    ws.Cells[row, 15].Value = c.Reseau;

                    ws.Cells[row, 16].Value = c.MinFr;
                    ws.Cells[row, 17].Value = c.MaxFr;
                    ws.Cells[row, 18].Value = c.MinNous;
                    ws.Cells[row, 19].Value = c.MaxNous;                    

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

        public static void RecreateExperienceFromPresta()
        {
            try
            {
                List<PrestSante> prestaData = new List<PrestSante>();
                List<PrestSante> prestaDataNormalized = new List<PrestSante>();
                List<ExcelPrestaSheet> excelPrestDataLarge = new List<ExcelPrestaSheet>();

                //truncate table Experience
                C_TempExpData.TruncateTable();
                 
                //get data from PrestSante
                //List<PrestSante> PrestaData = PrestSante.GetPrestations();               
                long cnt = PrestSante.TotalPrestaCount();
                long offset = 0;
                int maxLines = 50000;

                try
                {
                    do
                    {
                        prestaData = PrestSante.GetPrestationsPaged(offset, maxLines);
                        offset += maxLines;
                        prestaDataNormalized = BLCompteResultat.NormalizeGroupGarantyLabelsInPrestaTable(prestaData);

                    } while (offset < cnt);
                } catch (Exception ex)
                { log.Error("1" + ex.Message + ex.InnerException); }

                //add data to _TempExpData  
                try
                {
                    excelPrestDataLarge = ExcelSheetHandler.GenerateModifiedPrestDataComplete();
                }
                catch (Exception ex)
                { log.Error("2" + ex.Message + ex.InnerException); }

                try
                {
                    //save data to table: _TempExpData
                    foreach (ExcelPrestaSheet item in excelPrestDataLarge)
                    {
                        int id = C_TempExpData.InsertExp(new C_TempExpData
                        {
                            ImportId = item.ImportId,
                            AssureurName = item.AssureurName,
                            Au = item.DateVision.HasValue ? item.DateVision.Value : DateTime.MinValue,
                            Contrat = item.ContractId,
                            CodCol = item.CodeCol,
                            AnneeExp = item.DateSoins.HasValue ? item.DateSoins.Value.Year : 0,
                            LibActe = item.GarantyName,
                            LibFam = item.GroupName,
                            TypeCas = item.CAS,
                            NombreActe = item.NombreActe,
                            Fraisreel = item.FraisReel,
                            Rembss = item.RembSS,
                            RembAnnexe = item.RembAnnexe,
                            RembNous = item.RembNous,
                            Reseau = item.Reseau,
                            MinFr = item.MinFR,
                            MaxFr = item.MaxFR,
                            MinNous = item.MinNous,
                            MaxNous = item.MaxNous
                        });
                    }
                }
                catch (Exception ex)
                { log.Error("3" + ex.Message + ex.InnerException); }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + ex.InnerException);
                throw ex;
            }
        }

    }
}
