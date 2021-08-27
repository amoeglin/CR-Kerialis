using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Text.RegularExpressions;
using System.IO;
using System.Data;
using System.Threading;

using CompteResultat.DAL;
using CompteResultat.Common;

namespace CompteResultat.BL
{
    public class BLImport
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region PROPERTIES

        public string CSVSep { get; set; }
        public string ImportName { get; set; }
        public string UserName { get; set; }
        public string NewPrestEntCSV { get; set; }
        public string NewPrestProdCSV { get; set; }
        public string NewCotEntCSV { get; set; }
        public string NewCotProdCSV { get; set; }
        public string NewDemoEntCSV { get; set; }
        public string NewDemoProdCSV { get; set; }
        public string NewOtherFieldsCSV { get; set; }
        public string NewCotPrevCSV { get; set; }
        public string NewSinistrePrevCSV { get; set; }
        public string NewDecompPrevCSV { get; set; }
        public string NewProvCSV { get; set; }
        public string NewExpCSV { get; set; }
        public string ConfigStringPrest { get; set; }
        public string ConfigStringDemo { get; set; }
        public string ConfigStringCot { get; set; }
        public string ConfigStringOtherFields { get; set; }
        public string ConfigStringCotPrev { get; set; }
        public string ConfigStringSinistrPrev { get; set; }
        public string ConfigStringDecompPrev { get; set; }
        public string ConfigStringProv { get; set; }
        public string ConfigStringExp { get; set; }
        public string TableForOtherFields { get; set; }
        public string UploadPath { get; set; }
        public string UploadPathPrest { get; set; }
        public string UploadPathCot { get; set; }
        public string UploadPathDemo { get; set; }
        public string UploadPathCotPrev { get; set; }
        public string UploadPathSinistrPrev { get; set; }
        public string UploadPathDecompPrev { get; set; }
        public string UploadPathProv { get; set; }
        public string UploadPathExp { get; set; }
        public int ImportId { get; set; }

        public bool ForceCompanySubsid { get; set; }
        public bool UpdateGroupes { get; set; }
        public bool UpdateExperience { get; set; }
        public bool UpdateCad { get; set; }

        #endregion

        public BLImport(string userName, string newPrestEntCSV, string newPrestProdCSV, string newCotEntCSV, string newCotProdCSV, string newDemoEntCSV, string newDemoProdCSV, string newOtherFieldsCSV,
            string newCotPrevCSV, string newSinistrePrevCSV, string newDecompPrevCSV, string newProvCSV,
            string configStringPrest, string configStringDemo, string configStringCot, string configStringOtherFields,
            string configStringCotPrev, string configStringSinistrPrev, string configStringDecompPrev, string configStringProv,
            string tableForOtherFields, string importName, string csvSep, string uploadPath, 
            string uploadPathPrest, string uploadPathCot, string uploadPathDemo, string uploadPathCotPrev, string uploadPathSinistrPrev, 
            string uploadPathDecompPrev, string uploadPathProv, string newExpCSV, string configStringExp, string uploadPathExp, 
            bool forceCompanySubsid, bool updateGroupes, bool updateExperience, bool updateCad)
        {
            CSVSep = csvSep;
            ImportName = importName;
            UserName = userName;
            NewPrestEntCSV = newPrestEntCSV;
            NewPrestProdCSV = newPrestProdCSV;
            NewCotEntCSV = newCotEntCSV;
            NewCotProdCSV = newCotProdCSV;
            NewDemoEntCSV = newDemoEntCSV;
            NewDemoProdCSV = newDemoProdCSV;
            NewOtherFieldsCSV = newOtherFieldsCSV;
            NewCotPrevCSV = newCotPrevCSV;
            NewSinistrePrevCSV = newSinistrePrevCSV;
            NewDecompPrevCSV = newDecompPrevCSV;
            NewProvCSV = newProvCSV;
            NewExpCSV = newExpCSV;
            ConfigStringPrest = configStringPrest;
            ConfigStringDemo = configStringDemo;
            ConfigStringCot = configStringCot;
            ConfigStringOtherFields = configStringOtherFields;
            ConfigStringCotPrev = configStringCotPrev;
            ConfigStringSinistrPrev = configStringSinistrPrev;
            ConfigStringDecompPrev = configStringDecompPrev;
            ConfigStringProv = configStringProv;
            ConfigStringExp = configStringExp;
            TableForOtherFields = tableForOtherFields;
            UploadPath = uploadPath;
            UploadPathPrest = uploadPathPrest;
            UploadPathCot = uploadPathCot;
            UploadPathDemo = uploadPathDemo;
            UploadPathCotPrev = uploadPathCotPrev;
            UploadPathSinistrPrev = uploadPathSinistrPrev;
            UploadPathDecompPrev = uploadPathDecompPrev;
            UploadPathProv = uploadPathProv;
            UploadPathExp = uploadPathExp;
            ForceCompanySubsid = forceCompanySubsid;
            UpdateExperience = updateExperience;
            UpdateGroupes = updateGroupes;
            UpdateCad = updateCad;
        }

        public BLImport(int importId)
        {
            ImportId = importId;
        }

        public void DoImport()
        {
            try
            {
                // ***** Add a new line to the Import table and get the corresponding Id

                Import imp = new Import { Name = ImportName, Date = DateTime.Today.Date, UserName = UserName };
                int importId = Import.Insert(imp);
                ImportId = importId;


                // ***** Transform the uploaded csv files so they correspond with our DB schema

                #region INSERT KEY DATA FROM IMPORT FILES INTO TEMP TABLE

                //Iterate all imported files => add unique combination of: Assur-Contr-Comp-Subsid and add them to a List<string>
                List<string> dataAssurContrCompSubsid = new List<string>();

                //get header for temp table from Web.config
                dataAssurContrCompSubsid.Add(C.eDBTempOtherFieldsColumns.ImportId.ToString() + C.cVALSEP + C.eDBTempOtherFieldsColumns.AssureurName.ToString() + C.cVALSEP +
                    C.eDBTempOtherFieldsColumns.ContractId.ToString() + C.cVALSEP + C.eDBTempOtherFieldsColumns.Company.ToString() + C.cVALSEP +
                    C.eDBTempOtherFieldsColumns.Subsid.ToString() + C.cVALSEP + C.eDBTempOtherFieldsColumns.Gestionnaire.ToString() + C.cVALSEP +
                    C.eDBTempOtherFieldsColumns.Apporteur.ToString());

                
                if (File.Exists(UploadPathPrest))
                {
                    //we need to perform 2 imports for the following 2 assureurs : ASSUREEUR_PRODUIT && ASSUREUR_ENTREPRISE
                    //TransformFile(UploadPathPrest, CSVSep, ConfigStringPrest, NewPrestCSV, "Id", importId, C.eImportFile.PrestaSante, ForceCompanySubsid);
                    //Thread.Sleep(500);

                    //Entreprise
                    TransformFile(UploadPathPrest, CSVSep, ConfigStringPrest, NewPrestEntCSV, "Id", importId, C.eImportFile.PrestaSante, false);
                    Thread.Sleep(500);
                    if (File.Exists(NewPrestEntCSV))
                        G.GetAssurContrCompSubsidFromCSV(ref dataAssurContrCompSubsid, NewPrestEntCSV, ImportId);

                    //Produit
                    TransformFile(UploadPathPrest, CSVSep, ConfigStringPrest, NewPrestEntCSV, "Id", importId, C.eImportFile.PrestaSante, true);
                    Thread.Sleep(500);
                    if (File.Exists(NewPrestEntCSV))
                        G.GetAssurContrCompSubsidFromCSV(ref dataAssurContrCompSubsid, NewPrestEntCSV, ImportId);
                }

                if (File.Exists(UploadPathCot))
                {
                    TransformFile(UploadPathCot, CSVSep, ConfigStringCot, NewCotEntCSV, "Id", importId, C.eImportFile.CotisatSante, false);
                    Thread.Sleep(500);
                    if (File.Exists(NewCotEntCSV))
                        G.GetAssurContrCompSubsidFromCSV(ref dataAssurContrCompSubsid, NewCotEntCSV, ImportId);

                    TransformFile(UploadPathCot, CSVSep, ConfigStringCot, NewCotEntCSV, "Id", importId, C.eImportFile.CotisatSante, true);
                    Thread.Sleep(500);
                    if (File.Exists(NewCotEntCSV))
                        G.GetAssurContrCompSubsidFromCSV(ref dataAssurContrCompSubsid, NewCotEntCSV, ImportId);
                }

                if (File.Exists(UploadPathDemo))
                {
                    TransformFile(UploadPathDemo, CSVSep, ConfigStringDemo, NewDemoEntCSV, "Id", importId, C.eImportFile.Demography, false);
                    Thread.Sleep(500);
                    if (File.Exists(NewDemoEntCSV))
                        G.GetAssurContrCompSubsidFromCSV(ref dataAssurContrCompSubsid, NewDemoEntCSV, ImportId);

                    TransformFile(UploadPathDemo, CSVSep, ConfigStringDemo, NewDemoEntCSV, "Id", importId, C.eImportFile.Demography, true);
                    Thread.Sleep(500);
                    if (File.Exists(NewDemoEntCSV))
                        G.GetAssurContrCompSubsidFromCSV(ref dataAssurContrCompSubsid, NewDemoEntCSV, ImportId);
                }

                if (File.Exists(UploadPathCotPrev))
                {
                    TransformFile(UploadPathCotPrev, CSVSep, ConfigStringCotPrev, NewCotPrevCSV, "Id", importId, C.eImportFile.CotisatPrev, false);
                    Thread.Sleep(500);
                    if (File.Exists(NewCotPrevCSV))
                        G.GetAssurContrCompSubsidFromCSV(ref dataAssurContrCompSubsid, NewCotPrevCSV, ImportId);

                    TransformFile(UploadPathCotPrev, CSVSep, ConfigStringCotPrev, NewCotPrevCSV, "Id", importId, C.eImportFile.CotisatPrev, true);
                    Thread.Sleep(500);
                    if (File.Exists(NewCotPrevCSV))
                        G.GetAssurContrCompSubsidFromCSV(ref dataAssurContrCompSubsid, NewCotPrevCSV, ImportId);
                }

                if (File.Exists(UploadPathSinistrPrev))
                {
                    TransformFile(UploadPathSinistrPrev, CSVSep, ConfigStringSinistrPrev, NewSinistrePrevCSV, "Id", importId, C.eImportFile.SinistrePrev, false);
                    Thread.Sleep(500);
                    if (File.Exists(NewSinistrePrevCSV))
                        G.GetAssurContrCompSubsidFromCSV(ref dataAssurContrCompSubsid, NewSinistrePrevCSV, ImportId);

                    TransformFile(UploadPathSinistrPrev, CSVSep, ConfigStringSinistrPrev, NewSinistrePrevCSV, "Id", importId, C.eImportFile.SinistrePrev, true);
                    Thread.Sleep(500);
                    if (File.Exists(NewSinistrePrevCSV))
                        G.GetAssurContrCompSubsidFromCSV(ref dataAssurContrCompSubsid, NewSinistrePrevCSV, ImportId);
                }

                if (File.Exists(UploadPathDecompPrev))
                {
                    TransformFile(UploadPathDecompPrev, CSVSep, ConfigStringDecompPrev, NewDecompPrevCSV, "Id", importId, C.eImportFile.DecompPrev, false);
                    Thread.Sleep(500);
                    if (File.Exists(NewDecompPrevCSV))
                        G.GetAssurContrCompSubsidFromCSV(ref dataAssurContrCompSubsid, NewDecompPrevCSV, ImportId);

                    TransformFile(UploadPathDecompPrev, CSVSep, ConfigStringDecompPrev, NewDecompPrevCSV, "Id", importId, C.eImportFile.DecompPrev, true);
                    Thread.Sleep(500);
                    if (File.Exists(NewDecompPrevCSV))
                        G.GetAssurContrCompSubsidFromCSV(ref dataAssurContrCompSubsid, NewDecompPrevCSV, ImportId);
                }

                if (File.Exists(UploadPathProv))
                {
                    TransformFile(UploadPathProv, CSVSep, ConfigStringProv, NewProvCSV, "Id", importId, C.eImportFile.Provisions, false);
                    Thread.Sleep(500);
                    if (File.Exists(NewProvCSV))
                        G.GetAssurContrCompSubsidFromCSV(ref dataAssurContrCompSubsid, NewProvCSV, ImportId);

                    TransformFile(UploadPathProv, CSVSep, ConfigStringProv, NewProvCSV, "Id", importId, C.eImportFile.Provisions, true);
                    Thread.Sleep(500);
                    if (File.Exists(NewProvCSV))
                        G.GetAssurContrCompSubsidFromCSV(ref dataAssurContrCompSubsid, NewProvCSV, ImportId);
                }

                //experience Data
                if (File.Exists(UploadPathExp))
                {
                    TransformFile(UploadPathExp, CSVSep, ConfigStringExp, NewExpCSV, "Id", importId);                  
                }


                // create the NewOtherFieldsCSV
                if(dataAssurContrCompSubsid.Count > 1)
                    G.CreateCSVFromStringList(dataAssurContrCompSubsid, NewOtherFieldsCSV);


                //### NO LONGER NEEDED ?
                //Create the csv file that will be imported into the temporary TempOtherFields Table
                //if (TableForOtherFields == C.eImportFile.PrestaSante.ToString())
                //    TransformFile(UploadPathPrest, CSVSep, ConfigStringOtherFields, NewOtherFieldsCSV, importId: importId);
                //else if (TableForOtherFields == C.eImportFile.CotisatSante.ToString())
                //    TransformFile(UploadPathCot, CSVSep, ConfigStringOtherFields, NewOtherFieldsCSV, importId: importId);
                //else if (TableForOtherFields == C.eImportFile.Demography.ToString())
                //    TransformFile(UploadPathDemo, CSVSep, ConfigStringOtherFields, NewOtherFieldsCSV, importId: importId);
                //else
                //    throw new Exception("The parameter for 'TableForOtherFields' in the config file is not defined or contains an invalid value!");


                if (File.Exists(NewOtherFieldsCSV))
                    BulkInsert.DoBulkInsert(NewOtherFieldsCSV, C.eDBTempTables._TempOtherFields.ToString());

                #endregion


                // ***** Update additional Tables: Assur, Company, Contract 

                //***************************************************************************************************
                //*** Update the following tables with items from the imported Temp table: Assureur, Contract & Company (Parent + Subsid)
                //***************************************************************************************************

                #region UPDATING TABLES: ASSUREUR, CONTRACT & COMPANY

                //Verify Assureur's
                List<string> existingAss = Assureur.GetUniqueAssNames();
                List<string> newAss = C_TempOtherFields.GetUniqueAssNames();
                List<string> assToBeAdded = newAss.Where(i => !existingAss.Contains(i)).ToList();

                foreach (string ass in assToBeAdded)
                {
                    int assId = Assureur.Insert(new Assureur { Name = ass, ImportId = importId });
                }

                //Verify ContractId's
                List<string> existingContr = Contract.GetUniqueContractIds();
                List<string> newContr = C_TempOtherFields.GetUniqueContractIds();
                List<string> contractsToBeAdded = newContr.Where(i => !existingContr.Contains(i)).ToList();

                foreach (string contr in contractsToBeAdded)
                {
                    int contrId = Contract.Insert(new Contract { ContractId = contr, ImportId = importId });
                }

                //Verify Parent Comp
                List<string> existingParComp = Company.GetUniqueParentCompanies();
                List<string> newParComp = C_TempOtherFields.GetUniqueParentCompanies();
                List<string> compsToBeAdded = newParComp.Where(i => !existingParComp.Contains(i)).ToList();

                foreach (string comp in compsToBeAdded)
                {
                    int compId = Company.Insert(new Company { Name = comp, ImportId = importId });
                }

                //Verify Subsid
                List<string> existingSubsid = Company.GetUniqueSubsids();
                List<string> newSubsid = C_TempOtherFields.GetAllSubsids();
                List<string> subsToBeAdded = newSubsid.Where(i => !existingSubsid.Contains(i)).ToList();

                //we test all subsids to take duplicate subsids into account
                foreach (string sub in newSubsid)
                {
                    //get the name of the associated Parent Company
                    List<string> parCompNames = C_TempOtherFields.GetParentCompanyNamesForSubsid(sub);
                    if (parCompNames == null)
                        throw new Exception("No parent company was found in the '_TempOtherFields' table for the following subsidiary: " + sub);

                    foreach (string parComp in parCompNames)
                    {
                        //get the id of the associated Parent Company
                        List<int> parCompIds = Company.GetCompIdsForParentCompName(parComp);
                        if (parCompIds == null)
                            throw new Exception("No parent company ID was found in the 'Company' table for the following parent company: " + parComp);

                        //verify if the parent companyId & subsid name combination already exists
                        foreach (int comp in parCompIds)
                        {
                            if (!Company.SubsidCompIdPairExists(sub, comp))
                            {
                                //now we add the subsidiary
                                int compId = Company.Insert(new Company { ImportId = importId, Name = sub, ParentId = comp });
                            }
                        }
                    }
                }                

                #endregion


                //***************************************************************************************************
                //*** Update the junction tables with items from the imported Temp table: Assureur, Contract & Company 
                //***************************************************************************************************

                #region UPDATING RELATIONSHIP TABLES: ASSUR-CONTRACT && CONTRACT-COMPANY 

                //Get all Assureur-Contract Pairs, Contract-Company Pairs & Contract-Subsid Pairs from the Temp Table
                List<OtherTableAssurContrPair> ACPairTempTable = C_TempOtherFields.GetAssurContrList();
                List<OtherTableContrCompPair> CCPairTempTable = C_TempOtherFields.GetContrCompList();
                List<OtherTableContrSubsidPair> CSPairTempTable = C_TempOtherFields.GetContrSubsidList();

                //Prepare the lists with the ID pairs from the relationship tables
                List<IMAssurContrIDPair> ACIDPair = Assureur.GetAssurContrIDPairsFromIMTable();
                List<IMContrCompIDPair> CCIDPair = Contract.GetContrCompIDPairsFromIMTable();

                //create dictionaries of ID-Name pairs => we need this later to get the corresponding ID values for Assureurs,
                // Contracts, Companies & Subsids that are in the Temp Table
                List<Assureur> allAssur = Assureur.GetAllAssureurs();
                Dictionary<int, string> dictAssurNameId = new Dictionary<int, string>();
                foreach (Assureur ass in allAssur)
                    dictAssurNameId.Add(ass.Id, ass.Name);

                List<Contract> allContr = Contract.GetContracts();
                Dictionary<int, string> dictContrNameId = new Dictionary<int, string>();
                foreach (Contract contr in allContr)
                    dictContrNameId.Add(contr.Id, contr.ContractId);

                List<Company> allComp = Company.GetParentCompanies();
                Dictionary<int, string> dictCompNameId = new Dictionary<int, string>();
                foreach (Company comp in allComp)
                    dictCompNameId.Add(comp.Id, comp.Name);


                //##### for the second param below, add something like comp.Name##comp.ImportId
                //that way, we can match a specific subsid to the current importId => line 350
                List<Company> allSubsid = Company.GetAllSubsids();
                Dictionary<int, string> dictSubsidNameId = new Dictionary<int, string>();
                foreach (Company comp in allSubsid)
                    dictSubsidNameId.Add(comp.Id, comp.Name);
                    //dictSubsidNameId.Add(comp.Id, comp.Name + "##" + comp.ImportId);


                //Translate the AssurName-Contract pairs from the TempOtherFields table into ID pairs
                List<IMAssurContrIDPair> ACIDPairFromTempTable = new List<IMAssurContrIDPair>();

                foreach (OtherTableAssurContrPair item in ACPairTempTable)
                {
                    int assId = dictAssurNameId.FirstOrDefault(x => x.Value.ToLower() == item.Assureur.ToLower()).Key;
                    int contrId = dictContrNameId.FirstOrDefault(x => x.Value.ToLower() == item.ContractId.ToLower()).Key;

                    if (assId != 0 && contrId != 0)
                        ACIDPairFromTempTable.Add(new IMAssurContrIDPair { IdAssurance = assId, IdContract = contrId });
                }

                //translate the Contract-Company pairs from the TempOtherFields table into ID pairs
                List<IMContrCompIDPair> CCIDPairFromTempTable = new List<IMContrCompIDPair>();

                foreach (OtherTableContrCompPair item in CCPairTempTable)
                {
                    int contrId = dictContrNameId.FirstOrDefault(x => x.Value.ToLower() == item.ContractId.ToLower()).Key;
                    int compId = dictCompNameId.FirstOrDefault(x => x.Value.ToLower() == item.Company.ToLower()).Key;

                    if (compId != 0 && contrId != 0)
                        CCIDPairFromTempTable.Add(new IMContrCompIDPair { IdContract = contrId, IdCompany = compId });
                }

                //translate the Contract-Subsid pairs from the TempOtherFields table into ID pairs
                List<IMContrCompIDPair> CSIDPairFromTempTable = new List<IMContrCompIDPair>();

                //##### this can cause problems, because we can have 2 or more subsids with the same name that belong to 
                //different companies => when we search for a subsidid, we should also take the current importid into account
                //in order to find the subsid id of the current import => modify also line 310

                foreach (OtherTableContrSubsidPair item in CSPairTempTable)
                {
                    //string searchItem = item.Subsid.ToLower() + "##" + ImportId;
                    int contrIdold = dictContrNameId.FirstOrDefault(x => x.Value.ToLower() == item.ContractId.ToLower()).Key;
                    List<int> contrIds = dictContrNameId.Where(x => x.Value.ToLower() == item.ContractId.ToLower()).Select(x => x.Key).ToList();
                    List<string> ourParentCompanies = C_TempOtherFields.GetParentCompanyNamesForSubsid(item.Subsid);

                    //int subsidId = dictSubsidNameId.FirstOrDefault(x => x.Value.ToLower() == item.Subsid.ToLower()).Key;
                    //var values = dictionary.Where(x => someKeys.Contains(x.Key)).Select(x => x.Value);
                    //var keys = dictionary.Where(x => someValues.Contains(x.Value)).Select(x => x.Key);

                    List<int> subsidIds = dictSubsidNameId.Where(x => x.Value.ToLower() == item.Subsid.ToLower()).Select(x => x.Key).ToList();
                    foreach (int subsidId in subsidIds)
                    {
                        int? parentCompId = Company.GetParentIdForSubsid(subsidId);
                        if (parentCompId.HasValue)
                        {
                            string parentCompName = Company.GetCompanyNameForId(parentCompId.Value);

                            foreach (string parComp in ourParentCompanies)
                            {
                                if (parentCompName == parComp)
                                {
                                    foreach (int contrId in contrIds)
                                    {
                                        if (subsidId != 0 && contrId != 0)
                                            CSIDPairFromTempTable.Add(new IMContrCompIDPair { IdContract = contrId, IdCompany = subsidId });
                                    }
                                }
                            }
                        }
                    }
                }


                //1: Add Contracts to Assureur in IM Table

                //evaluate which of those pairs are not already in the intermediate table
                foreach (IMAssurContrIDPair item in ACIDPairFromTempTable)
                {
                    if (item != null && !ACIDPair.Contains(item))
                    {
                        //insert item into IM Table
                        Assureur.InsertIDPairIntoIMTable(item);
                        ACIDPair.Add(new IMAssurContrIDPair { IdAssurance = item.IdAssurance, IdContract = item.IdContract });
                    }
                }

                //2: Add Company to Contracts in IM Table
                foreach (IMContrCompIDPair item in CCIDPairFromTempTable)
                {
                    if (item != null && !CCIDPair.Contains(item))
                    {
                        //insert item into IM Table
                        Contract.InsertIDPairIntoIMTable(item);
                        CCIDPair.Add(new IMContrCompIDPair { IdCompany = item.IdCompany, IdContract = item.IdContract });
                    }
                }

                //3: Add Subsid to Contracts in IM Table
                foreach (IMContrCompIDPair item in CSIDPairFromTempTable)
                {
                    if (item != null && !CCIDPair.Contains(item))
                    {
                        //insert item into IM Table
                        Contract.InsertIDPairIntoIMTable(item);
                        CCIDPair.Add(new IMContrCompIDPair { IdCompany = item.IdCompany, IdContract = item.IdContract });
                    }
                }

                #endregion


                #region OLD CODE FOR FILLING RELATIONSHIP TABLES: ASSUR-CONTRACT && CONTRACT-COMPANY

                ////Add Contracts to Assureur
                //foreach (string ass in newAss)
                //{
                //    List<string> contrNames = C_TempOtherFields.GetAllContractNamesForAssureur(ass);
                //    foreach (string contr in contrNames)
                //    {
                //        if (contr != null && contr != "" && ass != null && ass != "")
                //            Assureur.TryAddContractToAss(contr, ass);
                //    }
                //}

                ////Add Assureurs to Contract => normally there should be no need to add several Ass to one Contract
                //foreach (string contr in newContr)
                //{
                //    List<string> assNames = C_TempOtherFields.GetAllAssNamesForContract(contr);
                //    foreach (string ass in assNames)
                //    {
                //        if (ass != null && ass != "" && contr != null && contr != "")
                //            Contract.TryAddAssToContract(contr, ass);
                //    }
                //}

                ////Add Contracts to Parent Company
                //foreach (string comp in newParComp)
                //{
                //    List<string> contrNames = C_TempOtherFields.GetAllContractNamesForCompany(comp);
                //    foreach (string contr in contrNames)
                //    {
                //        if (contr != null && contr != "" && comp != null && comp != "")
                //            Company.TryAddContrToCompany(contr, comp, true);
                //    }
                //}

                ////Add Contracts to Subsid 
                //foreach (string comp in newSubsid)
                //{
                //    List<string> contrNames = C_TempOtherFields.GetAllContractNamesForSubsid(comp);
                //    foreach (string contr in contrNames)
                //    {
                //        if (contr != null && contr != "" && comp != null && comp != "")
                //            Company.TryAddContrToCompany(contr, comp, false);
                //    }
                //}

                ////Add Companies to Contract 
                //foreach (string contr in newContr)
                //{
                //    List<string> parentCompNames = C_TempOtherFields.GetAllParentCompForContract(contr);
                //    List<string> subsidNames = C_TempOtherFields.GetAllSubsidForContract(contr);

                //    foreach (string comp in parentCompNames)
                //    {
                //        if (contr != null && contr != "" && comp != null && comp != "")
                //            Contract.TryAddCompanyToContract(contr, comp);
                //    }

                //    foreach (string comp in subsidNames)
                //    {
                //        if (contr != null && contr != "" && comp != null && comp != "")
                //            Contract.TryAddCompanyToContract(contr, comp);
                //    }
                //}


                #endregion


                // ### *****Alert user about missing data in Tables: College, Groups & Garanties, Company, Assureur


                // ***** BulkInsert data to Tables: Prest, Cotisat, Demo
                if (File.Exists(NewDemoEntCSV) && !UploadPathDemo.EndsWith(UserName + "_"))
                    BulkInsert.DoBulkInsert(NewDemoEntCSV, C.eDBImportTables.Demography.ToString());
                if (File.Exists(NewDemoProdCSV) && !UploadPathDemo.EndsWith(UserName + "_"))
                    BulkInsert.DoBulkInsert(NewDemoProdCSV, C.eDBImportTables.Demography.ToString());

                if (File.Exists(NewPrestEntCSV) && !UploadPathPrest.EndsWith(UserName + "_"))
                    BulkInsert.DoBulkInsert(NewPrestEntCSV, C.eDBImportTables.PrestSante.ToString());
                if (File.Exists(NewPrestProdCSV) && !UploadPathPrest.EndsWith(UserName + "_"))
                    BulkInsert.DoBulkInsert(NewPrestProdCSV, C.eDBImportTables.PrestSante.ToString());

                if (File.Exists(NewCotEntCSV) && !UploadPathCot.EndsWith(UserName + "_"))
                    BulkInsert.DoBulkInsert(NewCotEntCSV, C.eDBImportTables.CotisatSante.ToString());
                if (File.Exists(NewCotProdCSV) && !UploadPathCot.EndsWith(UserName + "_"))
                    BulkInsert.DoBulkInsert(NewCotProdCSV, C.eDBImportTables.CotisatSante.ToString());

                if (File.Exists(NewCotPrevCSV) && !UploadPathCotPrev.EndsWith(UserName + "_"))
                    BulkInsert.DoBulkInsert(NewCotPrevCSV, C.eDBImportTables.CotisatPrev.ToString());
                if (File.Exists(NewSinistrePrevCSV) && !UploadPathSinistrPrev.EndsWith(UserName + "_"))
                    BulkInsert.DoBulkInsert(NewSinistrePrevCSV, C.eDBImportTables.SinistrePrev.ToString());
                if (File.Exists(NewDecompPrevCSV) && !UploadPathDecompPrev.EndsWith(UserName + "_"))
                    BulkInsert.DoBulkInsert(NewDecompPrevCSV, C.eDBImportTables.DecomptePrev.ToString());
                if (File.Exists(NewProvCSV) && !UploadPathProv.EndsWith(UserName + "_"))
                    BulkInsert.DoBulkInsert(NewProvCSV, C.eDBImportTables.ProvPrev.ToString());


                //### before inserting, the Exp file needs to be transformed: certain cols need to be modified
                if (File.Exists(NewExpCSV) && !UploadPathExp.EndsWith(UserName + "_"))
                {
                    ModifyExperienceFile(NewExpCSV);  
                    Thread.Sleep(500);
                    BulkInsert.DoBulkInsert(NewExpCSV, C.eDBImportTables._TempExpData.ToString());
                }


                //iterate Demo Table with ImportId => update Presta & Cotisat Tables : 'WithOption' field
                List<DemoSanteWithOptionInfo> demoWithOption = Demography.GetDemoDataWithOptionInfoTrue(importId);

                //UPDATE PrestSante SET WithOption = 'True' WHERE ImportId = " + importId + " AND ContractId IN ('aaa', 'bbb', 'ccc')"
                
                if (demoWithOption.Count > 0)
                {
                    StringBuilder sb = new StringBuilder("UPDATE PrestSante SET WithOption = 'True' WHERE ImportId = " + importId + " AND ContractId IN (");

                    foreach (DemoSanteWithOptionInfo data in demoWithOption)
                    {
                        sb.Append("'" + data.ContractId + "',");
                    }

                    string sql = sb.ToString();
                    sql = sql.Remove(sql.Length - 1);
                    sql = sql + ")";

                    PrestSante.UpdateWithOptionField(sql);
                    sql = sql.Replace("PrestSante", "CotisatSante");
                    PrestSante.UpdateWithOptionField(sql);
                }

                //foreach (DemoSanteWithOptionInfo data in demoWithOption)
                //{
                //    PrestSante.UpdateOptionField(importId, data.ContractId);
                //    CotisatSante.UpdateOptionField(importId, data.ContractId);
                //}


                // ### maybe do something with the data in the _TempOtherFields Table => calculations

                if(UpdateGroupes)
                    BLGroupsAndGaranties.RecreateGroupsGarantiesSanteFromPresta();

                if (UpdateExperience)
                    BLExperience.RecreateExperienceFromPresta();

                if (UpdateCad)
                    BLCadencier.RecreateCadencier();

                //everything went well, we delete the corresponding data from the Temp Table
                C_TempOtherFields.DeleteRowsWithImportId(ImportId);

                //Delete originally uploaded as well as the converted CSV Files
                CleanupImportFiles(UploadPath, UserName);

            }
            catch (Exception ex)
            {
                //rollback all operations using the importId : Assureur, Contract, Company
                // 2 * Presta, 2 * Cotisat , Demo, _TempOtherFields
                
                try
                {
                    if (ImportId != 0)
                    {
                        CleanTablesForSpecificImportID(ImportId, true);
                        CleanupImportFiles(UploadPath, UserName);
                    }
                }
                catch (Exception rbEx)
                {
                    log.Error(rbEx.Message);
                    throw rbEx;
                }
                finally
                {
                    log.Error(ex.Message);
                    throw ex;
                }
            }
        }       

        public static void oldCleanTablesForSpecificImportID(int importId, bool isRollback)
        {
            try
            {
                //https://stackoverflow.com/questions/42372677/entity-framework-execute-multiple-commands-in-one-round-trip

                string sql = "";
               
                sql += @"DELETE FROM PrestSante WHERE ImportId = {0};";
                sql += @"DELETE FROM CotisatSante WHERE ImportId = {0};";
                sql += @"DELETE FROM Demography WHERE ImportId = {0};";

                sql += @"DELETE FROM SinistrePrev WHERE ImportId = {0};";
                sql += @"DELETE FROM CotisatPrev WHERE ImportId = {0};";
                sql += @"DELETE FROM DecomptePrev WHERE ImportId = {0};";

                sql += @"DELETE FROM _TempExpData WHERE ImportId = {0};";

                sql += @"TRUNCATE TABLE _TempOtherFields;";
                sql += @"DELETE FROM Import WHERE Id = {0};";

                using (var context = new CompteResultatEntities())
                {
                    context.Database.ExecuteSqlCommand(sql, importId);
                }

                if (isRollback)
                {
                    Assureur.DeleteRowsWithImportId(importId);
                    Contract.DeleteRowsWithImportId(importId);
                    Company.DeleteRowsWithImportId(importId);
                }


                if (false)
                {
                    //Delete Data Sante                
                    PrestSante.DeleteRowsWithImportId(importId);
                    CotisatSante.DeleteRowsWithImportId(importId);
                    Demography.DeleteRowsWithImportId(importId);

                    //Delete Data Prev
                    SinistrePrev.DeleteRowsWithImportId(importId);
                    CotisatPrev.DeleteRowsWithImportId(importId);
                    DecomptePrev.DeleteRowsWithImportId(importId);

                    C_TempOtherFields.DeleteRowsWithImportId(importId);

                    if (isRollback)
                    {
                        Assureur.DeleteRowsWithImportId(importId);
                        Contract.DeleteRowsWithImportId(importId);
                        Company.DeleteRowsWithImportId(importId);
                    }

                    Import.DeleteImportWithId(importId);

                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void CleanTablesForSpecificImportID(int importId, bool isRollback)
        {
            try
            {
                //https://stackoverflow.com/questions/42372677/entity-framework-execute-multiple-commands-in-one-round-trip

                string sql = "";                

                using (var context = new CompteResultatEntities())
                {
                    sql = @"DELETE FROM PrestSante WHERE ImportId = {0};";
                    context.Database.ExecuteSqlCommand(sql, importId);

                    sql = @"DELETE FROM CotisatSante WHERE ImportId = {0};";
                    context.Database.ExecuteSqlCommand(sql, importId);

                    sql = @"DELETE FROM Demography WHERE ImportId = {0};";
                    context.Database.ExecuteSqlCommand(sql, importId);

                    sql = @"DELETE FROM SinistrePrev WHERE ImportId = {0};";
                    context.Database.ExecuteSqlCommand(sql, importId);

                    sql = @"DELETE FROM CotisatPrev WHERE ImportId = {0};";
                    context.Database.ExecuteSqlCommand(sql, importId);

                    sql = @"DELETE FROM DecomptePrev WHERE ImportId = {0};";
                    context.Database.ExecuteSqlCommand(sql, importId);

                    sql = @"DELETE FROM ProvPrev WHERE ImportId = {0};";
                    context.Database.ExecuteSqlCommand(sql, importId);

                    sql = @"TRUNCATE TABLE _TempOtherFields;";
                    context.Database.ExecuteSqlCommand(sql, importId);

                    sql = @"DELETE FROM Import WHERE Id = {0};";
                    context.Database.ExecuteSqlCommand(sql, importId);
                }

                if (isRollback)
                {
                    Assureur.DeleteRowsWithImportId(importId);
                    Contract.DeleteRowsWithImportId(importId);
                    Company.DeleteRowsWithImportId(importId);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void CleanupImportFiles(string UploadPath, string UserName)
        { 
            try
            {
                //delete all files with the specified user prefix
                List<string> fileList = Directory.GetFiles(UploadPath, UserName + "_*", SearchOption.TopDirectoryOnly).ToList();
                foreach (string fle in fileList)
                {
                    if(File.Exists(fle))
                        File.Delete(fle);
                }

                //if (File.Exists(UploadPathPrest)) File.Delete(UploadPathPrest);
                //if (File.Exists(UploadPathCot)) File.Delete(UploadPathCot);
                //if (File.Exists(UploadPathDemo)) File.Delete(UploadPathDemo);

                //if (File.Exists(NewPrestCSV)) File.Delete(NewPrestCSV);
                //if (File.Exists(NewCotCSV)) File.Delete(NewCotCSV);
                //if (File.Exists(NewDemoCSV)) File.Delete(NewDemoCSV);
                //if (File.Exists(NewOtherFieldsCSV)) File.Delete(NewOtherFieldsCSV);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void TransformFile(string inputFile, string csvSep, string configString,
            string saveLocation, string leadingIdField = "", int importId = 0, C.eImportFile impFile= C.eImportFile.PrestaSante, bool forceCompanySubsid = false)
        {
            var myKey = 0;
            var myKey2 = 0;

            try
            {
                //save the new CSV file
                if (File.Exists(saveLocation) && !forceCompanySubsid)
                {
                    File.Delete(saveLocation);
                }

                string newHeaderLine = "";
                int cnt = 0;

                if (leadingIdField != "")
                    newHeaderLine = leadingIdField + C.cVALSEP;

                if (importId != 0)
                    newHeaderLine += "ImportId" + C.cVALSEP;


                //create a Dict that contains the mappings of the imported Excel file                
                //read the Header columns from the input file into a dictionary : 0-ColName... 
                Dictionary<int, string> dictImportFileFields = new Dictionary<int, string>();

                string csvHeaders = File.ReadLines(inputFile).First();
                string[] cols = Regex.Split(csvHeaders, csvSep);

                cnt = 0;
                foreach (string col in cols)
                {
                    if (col != "")
                    {
                        dictImportFileFields.Add(cnt, col.Trim());
                        cnt++;
                    }
                }


                //<add key="Prestations" value="Id =>Field2;Name =>Field4;Email;NumbEmployees=>Field1;" />

                //Read the config string & create the mapping table (which is a dictionary) :
                //key == position in DB Table (0, 1, 2, 3...) -- value == corresponding field index from imported csv file (3, 6, 0, 2...)
                //that way, we end up with a file that maps exactly the DB Table structure & we can do a direct bulk insert

                List<string> lstNewColHeaders = new List<string>();
                Dictionary<int, int> dictConfigMappings = new Dictionary<int, int>();

                cols = Regex.Split(configString, C.cVALSEP);
                string field = "";
                string val = "";
                cnt = 0;

                foreach (string col in cols)
                {
                    if (col != "")
                    {
                        if (col.Contains(C.cMAP))
                        {
                            field = Regex.Split(col, C.cMAP)[0].Trim();
                            val = Regex.Split(col, C.cMAP)[1].Trim();

                            newHeaderLine += field + C.cVALSEP;
                        }
                        //else if (col.Contains(C.cEQUAL))
                        //{
                        //    field = Regex.Split(col, C.cEQUAL)[0].Trim();
                        //    val = Regex.Split(col, C.cEQUAL)[1].Trim();

                        //    newHeaderLine += field + C.cVALSEP;
                        //}
                        else
                        {
                            field = col.Trim();
                            val = C.cEMPTY;

                            newHeaderLine += field + C.cVALSEP;
                        }

                        lstNewColHeaders.Add(field);

                        if (val != C.cEMPTY)
                        {
                            var key = dictImportFileFields.FirstOrDefault(x => x.Value.ToLower() == val.ToLower()).Key;
                            dictConfigMappings.Add(cnt, key);
                        }
                        else
                            dictConfigMappings.Add(cnt, C.cEMPTYVAL);

                        cnt++;
                    }
                }

                //Trim the header line if there is a trailing ;
                if (newHeaderLine.EndsWith(C.cVALSEP))
                    newHeaderLine = newHeaderLine.Remove(newHeaderLine.Length - 1);
                
                //read columns from csv file
                StringBuilder sb = new StringBuilder();
                string newLine = "";
                Dictionary<int, string> dictInpFileLine = new Dictionary<int, string>();
                cnt = 0;

                if(!forceCompanySubsid)
                    sb.AppendLine(newHeaderLine);

                foreach (string line in File.ReadLines(inputFile))
                {
                    if (line != "" & cnt > 0)
                    {
                        if (leadingIdField != "")
                            newLine = C.cVALSEP;

                        if (importId != 0)
                            newLine += importId.ToString() + C.cVALSEP;

                        // get cell values from the line
                        cols = Regex.Split(line, C.cVALSEP);

                        foreach (KeyValuePair<int, int> entry in dictConfigMappings)
                        {
                            //string contractId = 
                            if (entry.Value == C.cEMPTYVAL)
                                newLine += ";";
                            else
                            {
                                string myValue = "";                                

                                // if checkbox "Contrat sans entreprise et filiale" selected:
                                if (forceCompanySubsid && (entry.Key == 2 || entry.Key == 3))
                                {
                                    //newLine += cols[1].Trim() + C.cVALSEP;
                                    myValue = cols[1].Trim() + "_";
                                }
                                else
                                {
                                    //newLine += cols[entry.Value].Trim() + C.cVALSEP;
                                    myValue = cols[entry.Value].Trim();
                                }

                                //rename contract for PRODUCTS => add _ at the end
                                if (forceCompanySubsid && (entry.Key == 1))
                                {
                                    myValue = cols[1].Trim() + "_";
                                }

                                //change to upper case
                                if (entry.Key == 0 || entry.Key == 1 || entry.Key == 2 || entry.Key == 3)
                                {
                                    myValue = myValue.ToUpper();
                                }

                                if(impFile == C.eImportFile.PrestaSante && (entry.Key == 8 || entry.Key == 9))
                                {
                                    myValue = myValue.ToUpper();
                                }

                                //change name of assureur to ASS_PRODUIT or ASS_ENTREPRISE for PrestaSante, CotisatSante or Demo 
                                //### change this => don't use entry.Key == 5 => specify name
                                if ( (impFile == C.eImportFile.PrestaSante || impFile == C.eImportFile.CotisatSante || impFile == C.eImportFile.Demography
                                    || impFile == C.eImportFile.CotisatPrev || impFile == C.eImportFile.DecompPrev || impFile == C.eImportFile.SinistrePrev || impFile == C.eImportFile.Provisions)
                                    && (entry.Key == 0 ))
                                {
                                    if(forceCompanySubsid)
                                        myValue = myValue.ToUpper() + C.cASSTYPEPRODUCT;
                                    else
                                        myValue = myValue.ToUpper() + C.cASSTYPEENTERPRISE;
                                }

                                //force nombreActe = 1 if it is 0
                                myKey = dictImportFileFields.FirstOrDefault(x => x.Value == "NombreActe").Key; //10
                                if ((impFile == C.eImportFile.PrestaSante) && (entry.Key == myKey))
                                {
                                    if (myValue == "0")
                                        myValue = "1";
                                }

                                //force Age in Demo to 0 if negativ
                                myKey = dictImportFileFields.FirstOrDefault(x => x.Value == "Age").Key;
                                if ((impFile == C.eImportFile.Demography) && (entry.Key == myKey))
                                {
                                    if (int.Parse(myValue) < 0)
                                        myValue = "0";
                                }

                                newLine += myValue + C.cVALSEP;

                            }
                        }

                        if (newLine.EndsWith(C.cVALSEP))
                            newLine = newLine.Substring(0, newLine.Length - 1);

                        sb.AppendLine(newLine);
                    }

                    cnt++;
                    newLine = "";

                    if (cnt > 10000)
                    {
                        cnt = 1;
                        G.WriteFileContent(saveLocation, sb, true);
                        sb.Clear();
                    }
                }

                G.WriteFileContent(saveLocation, sb, true);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ModifyExperienceFile(string inputFile)
        {
            try
            {
                string newFile = inputFile.Replace(".csv", "2.csv");  
                if(File.Exists(newFile)) File.Delete(newFile);
                File.Copy(inputFile, newFile);
                File.Delete(inputFile);

                int cnt = 0;
                StringBuilder sb = new StringBuilder();
                string newLine = "";

                string assur = "";
                string libActe = "";
                string libFam = "";
                List<GroupGarantySante> ggs = null;

                foreach (string line in File.ReadLines(newFile))
                {
                    if (line != "" & cnt == 0)
                    {
                        sb.AppendLine(line);
                    }

                    if (line != "" & cnt > 0)
                    {
                        // get cell values from the line
                        string[] cols = Regex.Split(line, C.cVALSEP);
                        newLine = "";
                        int colCount = 0;

                        string codeActe = cols[6];                        

                        if (assur != cols[7] || ggs == null)
                        {
                            assur = cols[7];
                            ggs = GroupGarantySante.GetGroupsAndGarantiesForAssureur(assur);
                        }

                        var elem = ggs.Find(item => item.CodeActe == codeActe);
                        if (elem != null)
                        {
                            libActe = elem.GarantyName;
                            libFam = elem.GroupName;
                        }
                        else
                        {
                            //### log an error - the codeActe was not found
                            string errMess = string.Format("Le code acte suivant n'est pas defini : {0} ", codeActe);
                            throw new Exception(errMess);
                        }

                        foreach (string col in cols)
                        {
                            string myCol = col;

                            //get the year from the column: AnneeExp
                            if (colCount == 5)
                            {
                                DateTime date = Convert.ToDateTime(col);
                                int year = date.Year;
                                myCol = year.ToString();
                            }

                            //Get LibActe 
                            if (colCount == 6 )
                            {
                                myCol = libActe;                                
                            }

                            //Get LibFam
                            if (colCount == 7)
                            {
                                myCol = libFam;
                            }

                            //set CAS & Reseau
                            if (colCount == 8 || colCount == 14)
                            {
                                myCol = "VRAI";
                                if (col != "VRAI")
                                    myCol = "FAUX";
                            }

                            //set Min & Max values
                            if (colCount == 15 || colCount == 16 || colCount == 17 || colCount == 18)
                            {
                                myCol = "0";
                            }


                            newLine += myCol + C.cVALSEP;

                            colCount++;
                        }                               

                        if (newLine.EndsWith(C.cVALSEP))
                            newLine = newLine.Substring(0, newLine.Length - 1);

                        sb.AppendLine(newLine);
                    }

                    cnt++;
                    newLine = "";

                    if (cnt > 1000)
                    {
                        cnt = 1;
                        G.WriteFileContent(inputFile, sb, true);
                        sb.Clear();
                    }
                }

                G.WriteFileContent(inputFile, sb, true);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<string> ValidateImportFileColumns(List<string> colNames, string configString)
        {
            try
            {                
                //read the Header columns from the input file into a dictionary : 0-ColName... 
                Dictionary<int, string> dictImportFileFields = new Dictionary<int, string>();

                List<string> lstNewColHeaders = new List<string>();
                List<string> missingColumns = new List<string>();
                Dictionary<int, int> dictConfigMappings = new Dictionary<int, int>();

                string[] cols = Regex.Split(configString, C.cVALSEP);
                string requiredColumn = "";               
                
                foreach (string col in cols)
                {
                    if (col != "")
                    {
                        if (col.Contains(C.cMAP))
                        {
                            requiredColumn = Regex.Split(col, C.cMAP)[1].Trim();
                            
                            //if ( colNames.Any(x => x == requiredColumn))  
                            if(colNames.FirstOrDefault(x => x.Equals(requiredColumn, StringComparison.OrdinalIgnoreCase)) == null)
                                missingColumns.Add(requiredColumn);
                        }
                    }
                }   

                return missingColumns;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<string> ImportFileVerification(C.eImportFile importFileType, ref string filePath, string configString)
        {
            try
            {                
                C.eFileType myFileType = G.GetFileType(filePath);
                List<string> cols;
                List<string> missingColumns;

                //check file extension
                if (myFileType == C.eFileType.Excel)
                {
                    //check some columns and verify if this is a Presta File
                    cols = G.GetExcelHeader(filePath);
                    string saveCSVFilePath = filePath.Replace(Path.GetExtension(filePath), ".csv");

                    missingColumns = BLImport.ValidateImportFileColumns(cols, configString);
                    
                    //convert Excel to CSV 
                    DataTable myDT = G.ExcelToDataTable(filePath, true);
                    G.SaveDataTableAsFile(myDT, saveCSVFilePath);

                    filePath = saveCSVFilePath;
                }
                else if (myFileType == C.eFileType.CSV)
                {
                    cols = G.GetCSVHeader(filePath);
                    missingColumns = BLImport.ValidateImportFileColumns(cols, configString);                    
                }
                else
                {
                    throw new Exception("Il faudra spécifier un fichier du type .csv, .xls ou .xlsx dans le champ 'Fichier CSV Prestations' !");
                }

                return missingColumns;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
