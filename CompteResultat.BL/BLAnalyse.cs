using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.IO;

using CompteResultat.DAL;
using CompteResultat.Common;

namespace CompteResultat.BL
{
    public class BLAnalyse
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<UK_CotSante> itemsCSVCotSante;
        public static List<UK_CotSante> itemsSQLCotSante;
        public static List<UK_PrestSante> itemsCSVPrestSante;
        public static List<UK_PrestSante> itemsSQLPrestSante;
        public static List<UK_DemoSante> itemsCSVDemoSante;
        public static List<UK_DemoSante> itemsSQLDemoSante;
        public static List<UK_CotisatPrev> itemsCSVCotPrev;
        public static List<UK_CotisatPrev> itemsSQLCotPrev;
        public static List<UK_DecompPrev> itemsCSVDecompPrev;
        public static List<UK_DecompPrev> itemsSQLDecompPrev;
        public static List<UK_ProvPrev> itemsCSVProvPrev;
        public static List<UK_ProvPrev> itemsSQLProvPrev;
        public static List<UK_SinistrePrev> itemsCSVSinPrev;
        public static List<UK_SinistrePrev> itemsSQLSinPrev;

        public static int nbRowsCSV = 0;
        public static int nbRowsSQL = 0;
        public static double totalCSV = 0;
        public static double totalSQL = 0;

        public static void AnalyseData(string importFile, string fileGroup, string fileType, int importId)
        {
            try
            {
                //create objects from CSV
                if (File.Exists(importFile))
                {
                    AnalyseCSV(importFile, fileGroup, fileType);
                    AnalyseSQL(importFile, fileGroup, fileType, importId);                   

                    // totals & #rows are stored in: totalSQL, totalCSV && nbRowsSQL, nbRowsCSV                    
                    int diffRows = nbRowsCSV - nbRowsSQL;
                    double diffAmount = Math.Round(totalCSV - totalSQL, 1);
                    int isDiff = 1; //0: not analysed, 1: ok, 2: KO
                    if (diffRows != 0 || diffAmount !=0) isDiff = 2;

                    // calculate diff & update DB
                    string sqlUpdate = "";
                    using (var context = new CompteResultatEntities())
                    {
                        try
                        {
                            sqlUpdate = $@"UPDATE [CompteResultat].[dbo].[ImportFiles] SET NbRowsDb = {nbRowsSQL}, NbRowsCsv = {nbRowsCSV}, 
                                AmountDb = {totalSQL.ToString().Replace(",", ".")}, AmountCsv = {totalCSV.ToString().Replace(",", ".")}, 
                                DifferenceRows = {diffRows}, DifferenceAmount = {diffAmount.ToString().Replace(",", ".")}, IsDifference = {isDiff}
                                WHERE ImportId = {importId}";
                            sqlUpdate = sqlUpdate.Replace(System.Environment.NewLine, "");
                            context.Database.ExecuteSqlCommand(sqlUpdate);
                        }
                        catch (Exception exSQL)
                        {
                            throw new Exception($@"BLAnalyse: The following DB command failed: {sqlUpdate} - {exSQL.Message}");
                        }
                    }

                    //compare data
                    //var result = itemsSQLCotSante.Except(itemsCSVCotSante, new CotSanteComparer());

                    //write results to csv file

                }
                else
                {
                    throw new Exception($@"BLAnalyse: The import file does not exist: {importFile} ");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void AnalyseCSV(string importFile, string fileGroup, string fileType)
        {
            itemsCSVCotSante = new List<UK_CotSante>();
            itemsCSVPrestSante = new List<UK_PrestSante>();
            itemsCSVDemoSante = new List<UK_DemoSante>();
            itemsCSVCotPrev = new List<UK_CotisatPrev>();
            itemsCSVDecompPrev = new List<UK_DecompPrev>();
            itemsCSVProvPrev = new List<UK_ProvPrev>();
            itemsCSVSinPrev = new List<UK_SinistrePrev>();

            try
            {
                UK_CSV_SQL UKs = GetUK_CSV_SQL(fileGroup, fileType);
                List<string> keys = UKs.UK_CSV.Split(';').ToList();

                string csvHeaders = File.ReadLines(importFile).First();
                List<string> colNames = csvHeaders.Split(';').ToList();
                List<int> indexes = new List<int>();

                foreach (string k in keys)
                {
                    //int index = colNames.FindIndex(x => x.Contains(k));
                    int index = colNames.FindIndex(x => string.Equals(x, k, StringComparison.OrdinalIgnoreCase));
                    if (index == -1)
                    {
                        //test: in the CSV file, change the name of a key column
                        throw new Exception($@"BLAnalyse: The column: {k} cannot be founnd in the file: {importFile}");
                    }
                    indexes.Add(index);
                }

                //read CSV => add object for each line
                int cnt = 1;
                foreach (string line in File.ReadLines(importFile))
                {
                    List<string> cols = line.Split(';').ToList();

                    if (cnt > 1 && line != "" && cols[2].Contains("_ENTREPRISE"))
                    {
                        if (fileGroup == C.cIMPFILEGROUPSANTE)
                        {
                            if (fileType == C.cIMPFILETYPECOT)
                            {
                                //test: delete a key column in the web.config
                                if (indexes.Count() != 5)
                                    throw new Exception($@"BLAnalyse: A required unique key (5) is missing in Web.config (UK_CSV_CotisatSante) for the file: {importFile}");

                                try
                                {
                                    //<add key="UK_CSV_CotisatSante" value="ContractId;Company;CodeCol;Year;CotisationBrute"/>
                                    itemsCSVCotSante.Add(new UK_CotSante
                                    {
                                        RowNumber = cnt,
                                        Id = 0,
                                        ContractId = cols[indexes[0]],
                                        Company = cols[indexes[1]],
                                        CodeCol = cols[indexes[2]],
                                        Year = int.Parse(cols[indexes[3]]),
                                        CotisationBrute = double.Parse(cols[indexes[4]])
                                    });
                                }
                                catch (Exception exUK)
                                {
                                    //test: change one of the data types of any of the key columns - eg: double => string
                                    throw new Exception($@"BLAnalyse: There was a problem loading row #{cnt} of the file: {importFile} into the object UK_CotSante => a data type for one of the key columns seems to be wrong!");
                                }
                            }
                            else if (fileType == C.cIMPFILETYPEDEMO)
                            {
                                if (indexes.Count() != 7)
                                    throw new Exception($@"BLAnalyse: A required unique key (7) is missing in Web.config (UK_CSV_DemoSante) for the file: {importFile}");

                                try
                                {
                                    //set age to 0 if it is negativ
                                    int modifAge = int.Parse(cols[indexes[3]]);
                                    if (modifAge < 0) modifAge = 0;
                                    //ContractId;Company;DateDemo;Age;Sexe;CodeCol;Lien
                                    itemsCSVDemoSante.Add(new UK_DemoSante
                                    {
                                        RowNumber = cnt,
                                        Id = 0,
                                        ContractId = cols[indexes[0]],
                                        Company = cols[indexes[1]],
                                        DateDemo = DateTime.Parse(cols[indexes[2]]),
                                        Age = modifAge,
                                        Sexe = cols[indexes[4]],
                                        CodeCol = cols[indexes[5]],
                                        Lien = cols[indexes[6]]
                                    });
                                }
                                catch (Exception exUK)
                                {
                                    //test: change one of the data types of any of the key columns - eg: double => string
                                    throw new Exception($@"BLAnalyse: There was a problem loading row #{cnt} of the file: {importFile} into the object UK_DemoSante => a data type for one of the key columns seems to be wrong!");
                                }
                            }
                            else if (fileType == C.cIMPFILETYPEPREST)
                            {
                                if (indexes.Count() != 10)
                                    throw new Exception($@"BLAnalyse: A required unique key (10) is missing in Web.config (UK_CSV_PrestSante) for the file: {importFile}");

                                try
                                {
                                    //ContractId;Company;CodeCol;DateSoins;CodeActe;FraisReel;RembSS;RembNous;DatePayment;Beneficiaire
                                    itemsCSVPrestSante.Add(new UK_PrestSante
                                    {
                                        RowNumber = cnt,
                                        Id = 0,
                                        ContractId = cols[indexes[0]],
                                        Company = cols[indexes[1]],
                                        CodeCol = cols[indexes[2]],
                                        DateSoins = DateTime.Parse(cols[indexes[3]]),
                                        CodeActe = cols[indexes[4]],
                                        FraisReel = double.Parse(cols[indexes[5]]),
                                        RembSS = double.Parse(cols[indexes[6]]),
                                        RembNous = double.Parse(cols[indexes[7]]),
                                        DatePayment = DateTime.Parse(cols[indexes[8]]),
                                        Beneficiaire = cols[indexes[9]]
                                    });
                                }
                                catch (Exception exUK)
                                {
                                    //test: change one of the data types of any of the key columns - eg: double => string
                                    throw new Exception($@"BLAnalyse: There was a problem loading row #{cnt} of the file: {importFile} into the object UK_PrestSante => a data type for one of the key columns seems to be wrong!");
                                }
                            }
                        }
                        else
                        {
                            if (fileType == C.cIMPFILETYPECOT)
                            {
                                if (indexes.Count() != 6)
                                    throw new Exception($@"BLAnalyse: A required unique key (6) is missing in Web.config (UK_CSV_CotPrev) for the file: {importFile}");

                                try
                                {
                                    //ContractId;Company;CodeCol;Year;CotisationBrute;CodeGarantie
                                    itemsCSVCotPrev.Add(new UK_CotisatPrev
                                    {
                                        RowNumber = cnt,
                                        Id = 0,
                                        ContractId = cols[indexes[0]],
                                        Company = cols[indexes[1]],
                                        CodeCol = cols[indexes[2]],
                                        Year = int.Parse(cols[indexes[3]]),
                                        CotisationBrute = double.Parse(cols[indexes[4]]),
                                        CodeGarantie = cols[indexes[5]]
                                    });
                                }
                                catch (Exception exUK)
                                {
                                    //test: change one of the data types of any of the key columns - eg: double => string
                                    throw new Exception($@"BLAnalyse: There was a problem loading row #{cnt} of the file: {importFile} into the object UK_CotisatPrev => a data type for one of the key columns seems to be wrong!");
                                }
                            }
                            else if (fileType == C.cIMPFILETYPEDECOMP)
                            {
                                if (indexes.Count() != 10)
                                    throw new Exception($@"BLAnalyse: A required unique key (10) is missing in Web.config (UK_CSV_DecompPrev) for the file: {importFile}");

                                try
                                {
                                    //ContractId;Company;Dossier;CodeCol;DateVirement;DateSin;DebSin;FinSin;Total;CauseSinistre
                                    itemsCSVDecompPrev.Add(new UK_DecompPrev
                                    {
                                        RowNumber = cnt,
                                        Id = 0,
                                        ContractId = cols[indexes[0]],
                                        Company = cols[indexes[1]],
                                        Dossier = cols[indexes[2]],
                                        CodeCol = cols[indexes[3]],
                                        DateVirement = DateTime.Parse(cols[indexes[4]]),
                                        DateSin = DateTime.Parse(cols[indexes[5]]),
                                        DebSin = DateTime.Parse(cols[indexes[6]]),
                                        FinSin = DateTime.Parse(cols[indexes[7]]),
                                        Total = double.Parse(cols[indexes[8]]),
                                        CauseSinistre = cols[indexes[9]]
                                    });
                                }
                                catch (Exception exUK)
                                {
                                    //test: change one of the data types of any of the key columns - eg: double => string
                                    throw new Exception($@"BLAnalyse: There was a problem loading row #{cnt} of the file: {importFile} into the object UK_DecompPrev => a data type for one of the key columns seems to be wrong!");
                                }
                            }
                            else if (fileType == C.cIMPFILETYPEPROVCLOT || fileType == C.cIMPFILETYPEPROVOUV)
                            {
                                if (indexes.Count() != 12)
                                    throw new Exception($@"BLAnalyse: A required unique key (5) is missing in Web.config (UK_CSV_ProvPrev) for the file: {importFile}");

                                try
                                {
                                    //provOuverture: multiply by -1 for: PMPassage, PSAP, PMMgdc, PSI, PMPortab , PM
                                    double modifPm = double.Parse(cols[indexes[6]]);
                                    double modifPmPassage = double.Parse(cols[indexes[7]]);
                                    double modifPsap = double.Parse(cols[indexes[8]]);
                                    double modifPmMgdc = double.Parse(cols[indexes[9]]);
                                    double modifPsi = double.Parse(cols[indexes[10]]);
                                    double modifPmPortabilite = double.Parse(cols[indexes[11]]);

                                    if (fileType == C.cIMPFILETYPEPROVOUV)
                                    {
                                        modifPm = modifPm * -1;
                                        modifPmPassage = modifPmPassage * -1;
                                        modifPsap = modifPsap * -1;
                                        modifPmMgdc = modifPmMgdc * -1;
                                        modifPsi = modifPsi * -1;
                                        modifPmPortabilite = modifPmPortabilite * -1;
                                    }

                                    //ContractId;Company;Dossier;CodeCol;DateSinistre;NatureSinistre;Pm;PmPassage;Psap;PmMgdc;Psi;PmPortabilite
                                    itemsCSVProvPrev.Add(new UK_ProvPrev
                                    {
                                        RowNumber = cnt,
                                        Id = 0,
                                        ContractId = cols[indexes[0]],
                                        Company = cols[indexes[1]],
                                        Dossier = cols[indexes[2]],
                                        CodeCol = cols[indexes[3]],
                                        DateSinistre = DateTime.Parse(cols[indexes[4]]),
                                        NatureSinistre = cols[indexes[5]],
                                        Pm = modifPm,
                                        PmPassage = modifPmPassage,
                                        Psap = modifPsap,
                                        PmMgdc = modifPmMgdc,
                                        Psi = modifPsi,
                                        PmPortabilite = modifPmPortabilite
                                    });
                                }
                                catch (Exception exUK)
                                {
                                    //test: change one of the data types of any of the key columns - eg: double => string
                                    throw new Exception($@"BLAnalyse: There was a problem loading row #{cnt} of the file: {importFile} into the object UK_ProvPrev => a data type for one of the key columns seems to be wrong!");
                                }
                            }
                            else if (fileType == C.cIMPFILETYPESIN)
                            {
                                if (indexes.Count() != 7)
                                    throw new Exception($@"BLAnalyse: A required unique key (7) is missing in Web.config (UK_CSV_SinPrev) for the file: {importFile}");

                                try
                                {
                                    //ContractId;Company;Dossier;CodeCol;Birthdate;DateSinistre;NatureSinistre
                                    itemsCSVSinPrev.Add(new UK_SinistrePrev
                                    {
                                        RowNumber = cnt,
                                        Id = 0,
                                        ContractId = cols[indexes[0]],
                                        Company = cols[indexes[1]],
                                        Dossier = cols[indexes[2]],
                                        CodeCol = cols[indexes[3]],
                                        Birthdate = DateTime.Parse(cols[indexes[4]]),
                                        DateSinistre = DateTime.Parse(cols[indexes[5]]),
                                        NatureSinistre = cols[indexes[6]]
                                    });
                                }
                                catch (Exception exUK)
                                {
                                    //test: change one of the data types of any of the key columns - eg: double => string
                                    throw new Exception($@"BLAnalyse: There was a problem loading row #{cnt} of the file: {importFile} into the object UK_SinistrePrev => a data type for one of the key columns seems to be wrong!");
                                }
                            }
                        }
                    }
                    cnt++;
                }

                //calculate totals & #rows
                if (fileGroup == C.cIMPFILEGROUPSANTE)
                {
                    if (fileType == C.cIMPFILETYPECOT)
                    {
                        totalCSV = itemsCSVCotSante.Sum(x => x.CotisationBrute);
                        nbRowsCSV = itemsCSVCotSante.Count();
                    }
                    else if (fileType == C.cIMPFILETYPEDEMO)
                    {
                        totalCSV = 0;
                        nbRowsCSV = itemsCSVDemoSante.Count();
                    }
                    else if (fileType == C.cIMPFILETYPEPREST)
                    {
                        //### confirm RembNous => ??? FraisReel, RembSS 
                        totalCSV = itemsCSVPrestSante.Sum(x => x.RembNous);
                        nbRowsCSV = itemsCSVPrestSante.Count();
                    }
                }
                else
                {
                    if (fileType == C.cIMPFILETYPECOT)
                    {
                        totalCSV = itemsCSVCotPrev.Sum(x => x.CotisationBrute);
                        nbRowsCSV = itemsCSVCotPrev.Count();
                    }
                    else if (fileType == C.cIMPFILETYPEDECOMP)
                    {
                        totalCSV = itemsCSVDecompPrev.Sum(x => x.Total);
                        nbRowsCSV = itemsCSVDecompPrev.Count();
                    }
                    else if (fileType == C.cIMPFILETYPEPROVCLOT || fileType == C.cIMPFILETYPEPROVOUV)
                    {
                        totalCSV = itemsCSVProvPrev.Sum(x => (x.Pm + x.PmPassage + x.Psap + x.PmMgdc + x.Psi + x.PmPortabilite));
                        nbRowsCSV = itemsCSVProvPrev.Count();
                    }
                    else if (fileType == C.cIMPFILETYPESIN)
                    {
                        totalCSV = 0;
                        nbRowsCSV = itemsCSVSinPrev.Count();
                    }
                }

                totalCSV = Math.Round(totalCSV, 1);                
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }        
        }

        public static void AnalyseSQL(string importFile, string fileGroup, string fileType, int importId)
        {
            itemsSQLCotSante = new List<UK_CotSante>();
            itemsSQLPrestSante = new List<UK_PrestSante>();
            itemsSQLDemoSante = new List<UK_DemoSante>();
            itemsSQLCotPrev = new List<UK_CotisatPrev>();
            itemsSQLDecompPrev = new List<UK_DecompPrev>();
            itemsSQLProvPrev = new List<UK_ProvPrev>();
            itemsSQLSinPrev = new List<UK_SinistrePrev>();

            try
            {
                UK_CSV_SQL UKs = GetUK_CSV_SQL(fileGroup, fileType);
                string sql = UKs.UK_SQL + importId;

                using (var context = new CompteResultatEntities())
                {
                    if (fileGroup == C.cIMPFILEGROUPSANTE)
                    {
                        if (fileType == C.cIMPFILETYPECOT)
                        {
                            try
                            {
                                itemsSQLCotSante = context.Database.SqlQuery<UK_CotSante>(sql)
                                    .Select(d => new UK_CotSante
                                    {
                                        RowNumber = 0,
                                        Id = d.Id,
                                        ContractId = d.ContractId,
                                        Company = d.Company,
                                        CodeCol = d.CodeCol,
                                        Year = d.Year,
                                        CotisationBrute = d.CotisationBrute
                                    }).ToList<UK_CotSante>();

                                totalSQL = itemsSQLCotSante.Sum(x => x.CotisationBrute);
                                nbRowsSQL = itemsSQLCotSante.Count();
                            }
                            catch (Exception exUK)
                            {
                                throw new Exception($@"BLAnalyse: There was a problem executing the sql query: {sql} - {exUK.Message}");
                            }
                        }
                        else if (fileType == C.cIMPFILETYPEDEMO)
                        {
                            try
                            {
                                itemsSQLDemoSante = context.Database.SqlQuery<UK_DemoSante>(sql)
                                    .Select(d => new UK_DemoSante
                                    {
                                        RowNumber = 0,
                                        Id = d.Id,
                                        ContractId = d.ContractId,
                                        Company = d.Company,
                                        CodeCol = d.CodeCol,
                                        DateDemo = d.DateDemo,
                                        Age = d.Age,
                                        Sexe = d.Sexe,
                                        Lien = d.Lien
                                    }).ToList<UK_DemoSante>();

                                totalSQL = 0;
                                nbRowsSQL = itemsSQLDemoSante.Count();
                            }
                            catch (Exception exUK)
                            {
                                throw new Exception($@"BLAnalyse: There was a problem executing the sql query: {sql} - {exUK.Message}");
                            }
                        }
                        else if (fileType == C.cIMPFILETYPEPREST)
                        {
                            try
                            {
                                itemsSQLPrestSante = context.Database.SqlQuery<UK_PrestSante>(sql)
                                    .Select(d => new UK_PrestSante
                                    {
                                        RowNumber = 0,
                                        Id = d.Id,
                                        ContractId = d.ContractId,
                                        Company = d.Company,
                                        CodeCol = d.CodeCol,
                                        DateSoins = d.DateSoins,
                                        CodeActe = d.CodeActe,
                                        FraisReel = d.FraisReel,
                                        RembSS = d.RembSS,
                                        RembNous = d.RembNous,
                                        DatePayment = d.DatePayment,
                                        Beneficiaire = d.Beneficiaire
                                    }).ToList<UK_PrestSante>();

                                //### confirm RembNous => ??? FraisReel, RembSS 
                                totalSQL = itemsSQLPrestSante.Sum(x => x.RembNous);
                                nbRowsSQL = itemsSQLPrestSante.Count();
                            }
                            catch (Exception exUK)
                            {
                                throw new Exception($@"BLAnalyse: There was a problem executing the sql query: {sql} - {exUK.Message}");
                            }
                        }
                    }
                    else
                    {
                        if (fileType == C.cIMPFILETYPECOT)
                        {
                            try
                            {
                                itemsSQLCotPrev = context.Database.SqlQuery<UK_CotisatPrev>(sql)
                                    .Select(d => new UK_CotisatPrev
                                    {
                                        RowNumber = 0,
                                        Id = d.Id,
                                        ContractId = d.ContractId,
                                        Company = d.Company,
                                        CodeCol = d.CodeCol,
                                        Year = d.Year,
                                        CotisationBrute = d.CotisationBrute,
                                        CodeGarantie = d.CodeGarantie
                                    }).ToList<UK_CotisatPrev>();

                                totalSQL = itemsSQLCotPrev.Sum(x => x.CotisationBrute);
                                nbRowsSQL = itemsSQLCotPrev.Count();
                            }
                            catch (Exception exUK)
                            {
                                throw new Exception($@"BLAnalyse: There was a problem executing the sql query: {sql} - {exUK.Message}");
                            }
                        }
                        else if (fileType == C.cIMPFILETYPEDECOMP)
                        {
                            try
                            {
                                itemsSQLDecompPrev = context.Database.SqlQuery<UK_DecompPrev>(sql)
                                    .Select(d => new UK_DecompPrev
                                    {
                                        RowNumber = 0,
                                        Id = d.Id,
                                        ContractId = d.ContractId,
                                        Company = d.Company,
                                        CodeCol = d.CodeCol,
                                        Dossier = d.Dossier,
                                        DateVirement = d.DateVirement,
                                        DateSin = d.DateSin,
                                        DebSin = d.DebSin,
                                        FinSin = d.FinSin,
                                        Total = d.Total,
                                        CauseSinistre = d.CauseSinistre

                                    }).ToList<UK_DecompPrev>();

                                totalSQL = itemsSQLDecompPrev.Sum(x => x.Total);
                                nbRowsSQL = itemsSQLDecompPrev.Count();
                            }
                            catch (Exception exUK)
                            {
                                throw new Exception($@"BLAnalyse: There was a problem executing the sql query: {sql} - {exUK.Message}");
                            }
                        }
                        else if (fileType == C.cIMPFILETYPEPROVCLOT || fileType == C.cIMPFILETYPEPROVOUV)
                        {
                            try
                            {
                                itemsSQLProvPrev = context.Database.SqlQuery<UK_ProvPrev>(sql)
                                    .Select(d => new UK_ProvPrev
                                    {
                                        RowNumber = 0,
                                        Id = d.Id,
                                        ContractId = d.ContractId,
                                        Company = d.Company,
                                        CodeCol = d.CodeCol,
                                        Dossier = d.Dossier,
                                        DateSinistre = d.DateSinistre,
                                        NatureSinistre = d.NatureSinistre,
                                        Pm = d.Pm,
                                        PmPassage = d.PmPassage,
                                        Psap = d.Psap,
                                        PmMgdc = d.PmMgdc,
                                        Psi = d.Psi,
                                        PmPortabilite = d.PmPortabilite
                                    }).ToList<UK_ProvPrev>();

                                totalSQL = itemsSQLProvPrev.Sum(x => (x.Pm + x.PmPassage + x.Psap + x.PmMgdc + x.Psi + x.PmPortabilite));
                                nbRowsSQL = itemsSQLProvPrev.Count();
                            }
                            catch (Exception exUK)
                            {
                                throw new Exception($@"BLAnalyse: There was a problem executing the sql query: {sql} - {exUK.Message}");
                            }
                        }
                        else if (fileType == C.cIMPFILETYPESIN)
                        {
                            try
                            {
                                itemsSQLSinPrev = context.Database.SqlQuery<UK_SinistrePrev>(sql)
                                    .Select(d => new UK_SinistrePrev
                                    {
                                        RowNumber = 1,
                                        Id = d.Id,
                                        ContractId = d.ContractId,
                                        Company = d.Company,
                                        CodeCol = d.CodeCol,
                                        Dossier = d.Dossier,
                                        Birthdate = d.Birthdate,
                                        DateSinistre = d.DateSinistre,
                                        NatureSinistre = d.NatureSinistre
                                    }).ToList<UK_SinistrePrev>();

                                totalSQL = 0;
                                nbRowsSQL = itemsSQLSinPrev.Count();
                            }
                            catch (Exception exUK)
                            {
                                throw new Exception($@"BLAnalyse: There was a problem executing the sql query: {sql} - {exUK.Message}");
                            }
                        }
                    }
                }

                totalSQL = Math.Round(totalSQL, 1);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static bool VerifyIfAnalyseDone(int id)
        {
            bool done = false;
            int cnt = 0;
            using (var context = new CompteResultatEntities())
            {
                string sql = $@"SELECT count(*) FROM ImportFiles WHERE ImportId = {id} AND (NbRowsDb Is NOT NULL AND NbRowsDb <> 0)";
                cnt = context.Database.SqlQuery<int>(sql).First();
            }
            if (cnt > 0) done = true;
            return done;
        }

        public static UK_CSV_SQL GetUK_CSV_SQL(string fileGroup, string fileType)
        {
            UK_CSV_SQL paths = new UK_CSV_SQL();

            if (fileGroup == C.cIMPFILEGROUPSANTE)
            {
                if (fileType == C.cIMPFILETYPECOT)
                {
                    paths.UK_CSV = WebConfigurationManager.AppSettings["UK_CSV_CotisatSante"];
                    paths.UK_SQL = WebConfigurationManager.AppSettings["UK_SQL_CotisatSante"];
                }
                else if (fileType == C.cIMPFILETYPEDEMO)
                {
                    paths.UK_CSV = WebConfigurationManager.AppSettings["UK_CSV_DemoSante"];
                    paths.UK_SQL = WebConfigurationManager.AppSettings["UK_SQL_DemoSante"];
                }
                else if (fileType == C.cIMPFILETYPEPREST)
                {
                    paths.UK_CSV = WebConfigurationManager.AppSettings["UK_CSV_PrestSante"];
                    paths.UK_SQL = WebConfigurationManager.AppSettings["UK_SQL_PrestSante"];
                }
            }
            else
            {
                if (fileType == C.cIMPFILETYPECOT)
                {
                    paths.UK_CSV = WebConfigurationManager.AppSettings["UK_CSV_CotPrev"];
                    paths.UK_SQL = WebConfigurationManager.AppSettings["UK_SQL_CotPrev"];
                }
                else if (fileType == C.cIMPFILETYPEDECOMP)
                {
                    paths.UK_CSV = WebConfigurationManager.AppSettings["UK_CSV_DecompPrev"];
                    paths.UK_SQL = WebConfigurationManager.AppSettings["UK_SQL_DecompPrev"];
                }
                else if (fileType == C.cIMPFILETYPEPROVCLOT || fileType == C.cIMPFILETYPEPROVOUV)
                {
                    paths.UK_CSV = WebConfigurationManager.AppSettings["UK_CSV_ProvPrev"];
                    paths.UK_SQL = WebConfigurationManager.AppSettings["UK_SQL_ProvPrev"];
                }
                else if (fileType == C.cIMPFILETYPESIN)
                {
                    paths.UK_CSV = WebConfigurationManager.AppSettings["UK_CSV_SinPrev"];
                    paths.UK_SQL = WebConfigurationManager.AppSettings["UK_SQL_SinPrev"];
                }
            }

            return paths;
        }

        public static string ManualFileAnalyse(string filePath)
        {
            #region PROPS

            string configStringPrest = WebConfigurationManager.AppSettings[C.eConfigStrings.PrestSante.ToString()];
            string configStringCot = WebConfigurationManager.AppSettings[C.eConfigStrings.CotisatSante.ToString()];
            string configStringDemo = WebConfigurationManager.AppSettings[C.eConfigStrings.Demography.ToString()];
            string configStringCotPrev = WebConfigurationManager.AppSettings[C.eConfigStrings.CotisatPrev.ToString()];
            string configStringSinistrPrev = WebConfigurationManager.AppSettings[C.eConfigStrings.SinistrePrev.ToString()];
            string configStringDecompPrev = WebConfigurationManager.AppSettings[C.eConfigStrings.DecomptePrev.ToString()];
            string configStringProv = WebConfigurationManager.AppSettings[C.eConfigStrings.Provisions.ToString()];
            string configStringExp = WebConfigurationManager.AppSettings[C.eConfigStrings.Experience.ToString()];

            List<string> missingColumns;
            string fileGroup = "";
            string fileType = "";
            bool found = false;
            string res = "";

            #endregion

            if (File.Exists(filePath))
            {
                FileInfo fi = new FileInfo(filePath);

                res = "Détail de l'analys :" + Environment.NewLine + Environment.NewLine;
                res += "Nom du fichier : " + fi.Name + Environment.NewLine;
                res += "Taille du fichier : " + fi.Length + Environment.NewLine;               

                #region File Type

                missingColumns = BLImport.ImportFileVerification(C.eImportFile.PrestaSante, ref filePath, configStringPrest);
                if (missingColumns.Count == 0)
                {
                    fileGroup = C.cIMPFILEGROUPSANTE;
                    fileType = C.cIMPFILETYPEPREST;
                    found = true;
                }
                missingColumns = BLImport.ImportFileVerification(C.eImportFile.CotisatSante, ref filePath, configStringCot);
                if (missingColumns.Count == 0 & !found)
                {
                    fileGroup = C.cIMPFILEGROUPSANTE;
                    fileType = C.cIMPFILETYPECOT;
                    found = true;
                }
                missingColumns = BLImport.ImportFileVerification(C.eImportFile.Demography, ref filePath, configStringDemo);
                if (missingColumns.Count == 0 & !found)
                {
                    fileGroup = C.cIMPFILEGROUPSANTE;
                    fileType = C.cIMPFILETYPEDEMO;
                    found = true;
                }

                //Sinistre Prev == PROV (has add fields) ; 
                missingColumns = BLImport.ImportFileVerification(C.eImportFile.Provisions, ref filePath, configStringProv);
                if (missingColumns.Count == 0 & !found)
                {
                    fileGroup = C.cIMPFILEGROUPPREV;
                    fileType = C.cIMPFILETYPEPROV;
                    found = true;
                }

                missingColumns = BLImport.ImportFileVerification(C.eImportFile.SinistrePrev, ref filePath, configStringSinistrPrev);
                if (missingColumns.Count == 0 & !found)
                {
                    fileGroup = C.cIMPFILEGROUPPREV;
                    fileType = C.cIMPFILETYPESIN;
                    found = true;
                }                

                missingColumns = BLImport.ImportFileVerification(C.eImportFile.CotisatPrev, ref filePath, configStringCotPrev);
                if (missingColumns.Count == 0 & !found)
                {
                    fileGroup = C.cIMPFILEGROUPPREV;
                    fileType = C.cIMPFILETYPECOT;
                    found = true;
                }                

                missingColumns = BLImport.ImportFileVerification(C.eImportFile.DecompPrev, ref filePath, configStringDecompPrev);
                if (missingColumns.Count == 0 & !found)
                {
                    fileGroup = C.cIMPFILEGROUPPREV;
                    fileType = C.cIMPFILETYPEDECOMP;
                    found = true;
                }

                missingColumns = BLImport.ImportFileVerification(C.eImportFile.Exp, ref filePath, configStringExp);
                if (missingColumns.Count == 0 & !found)
                {
                    fileGroup = C.cIMPFILEGROUPPREV;
                    fileType = C.cIMPFILETYPEEXP;
                    found = true;
                }

                #endregion

                //read file & calculate: nbRowsCSV & totalCSV
                AnalyseCSV(filePath, fileGroup, fileType);

                if (fileGroup == C.cIMPFILEGROUPSANTE)
                {
                    res += "Categorie : SANTE" + Environment.NewLine;
                    if (fileType == C.cIMPFILETYPECOT)
                    {
                        res += "Type de fichier : Cotisations" + Environment.NewLine;
                        res += "Nombre des lignes : " + nbRowsCSV + Environment.NewLine;
                        res += "Cotisations brutes : " + totalCSV + Environment.NewLine;
                    }
                    else if (fileType == C.cIMPFILETYPEDEMO)
                    {
                        
                    }
                    else if (fileType == C.cIMPFILETYPEPREST)
                    {
                        
                    }
                }
                else
                {
                    if (fileType == C.cIMPFILETYPECOT)
                    {
                        
                    }
                    else if (fileType == C.cIMPFILETYPEDECOMP)
                    {
                    }
                    else if (fileType == C.cIMPFILETYPEPROVCLOT || fileType == C.cIMPFILETYPEPROVOUV)
                    {
                    }
                    else if (fileType == C.cIMPFILETYPESIN)
                    {
                    }
                }
            }

            return res;
        } 

    }

    public class UK_CSV_SQL
    {
        //Unique key fields for CSV file from Web.config
        public string UK_CSV { get; set; }
        //Unique key SQL queries from Web.config
        public string UK_SQL { get; set; }
    }
}
