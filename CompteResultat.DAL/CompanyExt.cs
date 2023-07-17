using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using CompteResultat.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Data.Entity;

namespace CompteResultat.DAL
{
    [MetadataType(typeof(Company.MetaData))]
    public partial class Company : IValidatableObject
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        //Additional validation

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //###
            if (Name == Email)
            {
                yield return new ValidationResult
                 ("Name cannot be the same as Email", new[] { "Name", "Email" });
            }
        }


        //Data Methods

        public static Company GetCompanyInfo(string companyName)
        {
            try
            {
                Company myComp = null;
                
                using (var context = new CompteResultatEntities())
                {
                    var elements = context.Companies.Where(c => c.Name == companyName);

                    if (elements.Any())
                    {
                        //there should only be 1 company with that name
                        myComp = elements.First();
                    }                    
                }
                return myComp;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int GetimportIdForCompanyId(int companyId)
        {
            try
            {
                int  importId = 0;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.Companies.Where(c => c.Id == companyId);

                    if (elements.Any())
                    {
                        //there should only be 1 company with that name
                        importId = elements.First().ImportId.HasValue ? elements.First().ImportId.Value : 0;
                    }
                }
                return importId;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static bool SubsidCompIdPairExists(string subsid, int compId)
        {
            try
            {
                bool exists;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.Companies.Where(c => c.ParentId == compId && c.Name == subsid);

                    if (elements.Any())
                        exists = true;
                    else
                        exists = false;
                }

                return exists;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static IEnumerable<Company> GetCompaniesByName(string companySearchString)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    return context.Companies.Where(c => c.Name.Contains(companySearchString)).ToList();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetUniqueParentCompanies()
        {
            try
            {
                List<string> comps;

                using (var context = new CompteResultatEntities())
                {
                    comps = context.Companies.Where(c => c.ParentId == null  && c.Name != null && c.Name != "").OrderBy(c => c.Name).Select(c => c.Name).Distinct().ToList();
                }

                return comps;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        //public static List<string> GetUniqueParentCompanyContractIdPairs()
        //{
        //    try
        //    {
        //        List<string> comps;

        //        using (var context = new CompteResultatEntities())
        //        {                    
        //            comps = context.Companies.Where(c => c.ParentId == null).Select(c => c.Contract.ContractId + C.cSTRINGSEP + c.Name ).Distinct().ToList();
        //        }

        //        return comps;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message);
        //        throw ex;
        //    }
        //}

        public static List<string> GetUniqueSubsids()
        {
            try
            {
                List<string> comps;

                using (var context = new CompteResultatEntities())
                {
                    comps = context.Companies.Where(c => c.ParentId != null && c.Name != null && c.Name != "").OrderBy(c => c.Name).Select(c => c.Name).Distinct().ToList();
                }

                return comps;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        //public static List<string> GetUniqueSubsidContractIdPairs()
        //{
        //    try
        //    {
        //        List<string> comps;

        //        using (var context = new CompteResultatEntities())
        //        {
        //            comps = context.Companies.Where(c => c.ParentId != null).Select(c => c.Contract.ContractId + C.cSTRINGSEP + c.Name).Distinct().ToList();
        //        }

        //        return comps;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message);
        //        throw ex;
        //    }
        //}

        public static List<Company> GetParentCompanies()
        {
            try
            {
                List<Company> companies;

                using (var context = new CompteResultatEntities())
                {
                    companies = context.Companies.Where(c => c.ParentId == null).OrderBy(c => c.Name).ToList();
                }

                if (companies == null)
                    throw new Exception("The 'Company' entity does not contain any data!");

                return companies;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<Company> GetParentCompaniesForAssureur(Assureur assur)
        {
            try
            {
                List<Company> companies;

                using (var context = new CompteResultatEntities())
                {
                    List<int> listOfContractIds = assur.Contracts.Select(c => c.Id).ToList();
                    //var listOfContractIds = assur.Contracts.Select(c => c.Id);
                    
                    //companies = context.Companies.Where(c => (c.ParentId == null && listOfContractIds.Contains((int)c.ContractId))).OrderBy(c => c.Name).ToList();
                                        
                    companies = context.Companies.Where(c => (c.ParentId == null && 
                        listOfContractIds.Intersect(c.Contracts.Select(cont => cont.Id).ToList()).Any()  )).OrderBy(c => c.Name).ToList();                   
                }

                if (companies == null)
                    throw new Exception("The 'Company' entity does not contain any data!");

                return companies;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }


        //####

        public static List<string> GetParentCompanyNamesForAssureurId(int assurId)
        {
            try
            {
                List<Company> parentComps;
                List<string> companies = new List<string>();
                Assureur assur = Assureur.GetAssById(assurId);

                using (var context = new CompteResultatEntities())
                {
                    List<int> listOfContractIds = assur.Contracts.Select(c => c.Id).ToList();         
                    parentComps = GetParentCompaniesWithContracts();

                    foreach (Company c in parentComps)
                    {
                        bool hasCommonContracts = listOfContractIds.Intersect(c.Contracts.Select(cont => cont.Id).ToList()).Any();
                        if (hasCommonContracts)
                            companies.Add(c.Name);
                    }

                    //companies = context.Companies.Where(c => (c.ParentId == null &&
                    //    listOfContractIds.Intersect(c.Contracts.Select(cont => cont.Id).ToList()).Any()  ) )
                    //    .OrderBy(c => c.Name).ToList();
                }

                if (companies == null)
                    throw new Exception("The 'Company' entity does not contain any data!");

                return companies;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        private static List<Company> GetParentCompaniesWithContracts()
        {
            try
            {
                List<Company> parentComps;
                using (var context = new CompteResultatEntities())
                {
                    //use eager loading to get contract data related to each company                    
                    parentComps = context.Companies
                        .Where(c => (c.ParentId == null))
                        .Include(c => c.Contracts)
                        .OrderBy(c => c.Name)
                        .ToList();
                }                

                return parentComps;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetParentCompanyIdsForAssureurId(int assurId)
        {
            try
            {
                List<string> companies = new List<string>();
                Assureur assur = Assureur.GetAssById(assurId);

                using (var context = new CompteResultatEntities())
                {
                    List<int> listOfContractIds = assur.Contracts.Select(c => c.Id).ToList();

                    //List<Company> parentComps = GetParentCompaniesWithContracts();
                    //foreach (Company c in parentComps)
                    //{
                    //    if (listOfContractIds.Intersect(c.Contracts.Select(cont => cont.Id).ToList()).Any())
                    //        companies.Add(c.Id.ToString());
                    //}

                    //### this may cause problems - use code above
                    companies = context.Companies.Where(c => (c.ParentId == null &&
                    listOfContractIds.Intersect(c.Contracts.Select(cont => cont.Id).ToList()).Any())).Select(c => c.Id.ToString()).ToList();
                }

                if (companies == null)
                    throw new Exception("The 'Company' entity does not contain any data!");

                return companies;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetSubsidNamesForAssureurId(int assurId)
        {
            try
            {
                List<string> companies;
                Assureur assur = Assureur.GetAssById(assurId);

                using (var context = new CompteResultatEntities())
                {
                    List<int> listOfContractIds = assur.Contracts.Select(c => c.Id).ToList();

                    companies = context.Companies.Where(c => (c.ParentId != null &&
                    listOfContractIds.Intersect(c.Contracts.Select(cont => cont.Id).ToList()).Any())).Select(c => c.Name).ToList();
                }

                if (companies == null)
                    throw new Exception("The 'Company' entity does not contain any data!");

                return companies;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetSubsidIdsForAssureurId(int assurId)
        {
            try
            {
                List<string> companies;
                Assureur assur = Assureur.GetAssById(assurId);

                using (var context = new CompteResultatEntities())
                {
                    List<int> listOfContractIds = assur.Contracts.Select(c => c.Id).ToList();

                    companies = context.Companies.Where(c => (c.ParentId != null &&
                    listOfContractIds.Intersect(c.Contracts.Select(cont => cont.Id).ToList()).Any())).Select(c => c.Id.ToString()).ToList();
                }

                if (companies == null)
                    throw new Exception("The 'Company' entity does not contain any data!");

                return companies;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }


        //####

        public static List<Company> GetParentCompaniesForAssureurId(int assurId, string filter = "")
        {
            try
            {
                List<Company> companies;
                Assureur assur = Assureur.GetAssById(assurId);

                using (var context = new CompteResultatEntities())
                {
                    List<int> listOfContractIds = assur.Contracts.Select(c => c.Id).ToList();

                    if (filter == "")
                    {
                        companies = context.Companies.Where(c => (c.ParentId == null && 
                        listOfContractIds.Intersect(c.Contracts.Select(cont => cont.Id).ToList()).Any())).OrderBy(c => c.Name).ToList();
                    }
                    else
                    {
                        companies = context.Companies.Where(c => (c.ParentId == null && c.Name.ToLower().Contains(filter.ToLower()) &&
                        listOfContractIds.Intersect(c.Contracts.Select(cont => cont.Id).ToList()).Any())).OrderBy(c => c.Name).ToList();
                    }
                }

                if (companies == null)
                    throw new Exception("The 'Company' entity does not contain any data!");

                return companies;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<Company> GetParentCompaniesForAssureurIdORIG(int assurId)
        {
            try
            {
                List<Company> companies;
                Assureur assur = Assureur.GetAssById(assurId);

                using (var context = new CompteResultatEntities())
                {
                    List<int> listOfContractIds = assur.Contracts.Select(c => c.Id).ToList();

                    companies = context.Companies.Where(c => (c.ParentId == null && 
                        listOfContractIds.Intersect(c.Contracts.Select(cont => cont.Id).ToList()).Any())).OrderBy(c => c.Name).ToList();
                }

                if (companies == null)
                    throw new Exception("The 'Company' entity does not contain any data!");

                return companies;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<Contract> GetContractsForCompany(int compId, bool isParentCompany)
        {
            try
            {
                List<Contract> contracts = null; 

                using (var context = new CompteResultatEntities())
                {
                    Company myComp;
                    IEnumerable<Contract> foundContracts; 

                    //get the company
                    var elements = context.Companies.Where(c => c.Id == compId);

                    if (elements.Any())
                    {
                        //there should only be 1 company with that name
                        myComp = elements.First();
                        
                        //try to find the contract 
                        if (isParentCompany)
                            //don't provide contracts that are associated with a subsid
                            foundContracts = myComp.Contracts.Where(contr => contr.Companies.Count <2);
                        else
                            foundContracts = myComp.Contracts;

                        if (foundContracts.Any())
                        {
                            contracts = foundContracts.ToList();                            
                        }
                    }
                    else
                    {
                        //we did not find the company => raise error
                        throw new Exception("The Company with the Id: '" + compId + "' was not found in the Company Table!");
                    }

                    return contracts;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetContractsNamesForCompany(int compId)
        {
            try
            {
                List<string> contracts = null;

                using (var context = new CompteResultatEntities())
                {
                    Company myComp;                    

                    //get the company
                    var elements = context.Companies.Where(c => c.Id == compId);

                    if (elements.Any())
                    {
                        //there should only be 1 company with that name
                        myComp = elements.First();
                        
                        var foundContracts = myComp.Contracts;

                        if (foundContracts.Any())                        
                            contracts = foundContracts.Select(c=>c.ContractId).ToList();
                    }
                    else
                    {
                        //we did not find the company => raise error
                        throw new Exception("The Company with the Id: '" + compId + "' was not found in the Company Table!");
                    }

                    return contracts;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }


        public static List<Company> GetChildCompanies(int parentId)
        {
            try
            {
                List<Company> companies;

                using (var context = new CompteResultatEntities())
                {
                    companies = context.Companies.Where(c => c.ParentId == parentId).ToList();
                }

                if (companies == null)
                    throw new Exception("The 'Company' entity does not contain any data!");

                return companies;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<Company> GetAllSubsids()
        {
            try
            {
                List<Company> companies;

                using (var context = new CompteResultatEntities())
                {
                    companies = context.Companies.Where(c => c.ParentId != null).OrderBy(c => c.Name).ToList();
                }

                //if (companies == null)
                //    throw new Exception("The 'Company' entity does not contain any data!");

                return companies;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<int> GetCompIdsForParentCompName(string companyName)
        {
            try
            {
                List<int> compIds;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.Companies.Where(c => c.Name == companyName && c.ParentId == null).Select(c => c.Id);

                    if (elements.Any())
                        compIds = elements.ToList(); // elements.First();
                    else
                        compIds = null; //  C.cINVALIDID;
                }

                return compIds;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static string GetCompanyNameForId(int id)
        {
            try
            {
                string name;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.Companies.Where(c => c.Id == id).Select(c => c.Name);

                    if (elements.Any())
                        name = elements.First();
                    else
                        name = C.cINVALIDSTRING;
                }

                return name;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int GetCompanyIdForName(string compName)
        {
            try
            {
                int id;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.Companies.Where(c => c.Name.ToLower() == compName.ToLower()).Select(c => c.Id);

                    if (elements.Any())
                        id = elements.First();
                    else
                        id = -1;
                }

                return id;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int? GetParentIdForSubsid(int subsidId)
        {
            try
            {
                int? id;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.Companies.Where(c => c.Id == subsidId).Select(c => c.ParentId);

                    if (elements.Any())
                        id = elements.First();
                    else
                        id = C.cINVALIDID;
                }

                return id;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static string oldGetCompanyNamesFromIdList(string companyIdList)
        {
            List<string> compNames = new List<string>();
            List<string> idList = Regex.Split(companyIdList, C.cVALSEP).ToList();
            int id = 0;

            foreach (string compId in idList)
            {
                if (int.TryParse(compId, out id))
                {
                    string myCompany = Company.GetCompanyNameForId(id);
                    if (myCompany != C.cINVALIDSTRING)
                        compNames.Add(myCompany);
                }
            }

            return string.Join(C.cVALSEP, compNames.Distinct());
        }

        public static string GetCompanyNamesFromIdList(string companyIdList)
        {
            List<string> compNames = new List<string>();
            List<CompNameIDPair> compIdPAirs = new List<CompNameIDPair>();
            List<string> idList = Regex.Split(companyIdList, C.cVALSEP).ToList();
            int id = 0;

            try
            {
                //load all company-id pairs into a List : CompNameIDPair
                using (var context = new CompteResultatEntities())
                {
                    compIdPAirs = context.Companies.Select(c => new CompNameIDPair { CompanyId = c.Id, CompanyName = c.Name }).ToList();
                }

                //create a list of company names
                foreach (string compId in idList)
                {
                    if (int.TryParse(compId, out id))
                    {
                        string myCompany = "";                     
                        var vComp = compIdPAirs.Where(c => c.CompanyId == id).ToList();

                        if (vComp.Any())                        
                            myCompany = vComp.Select(c => c.CompanyName).First().ToString();                        
                        
                        if (myCompany != C.cINVALIDSTRING)
                            compNames.Add(myCompany);
                    }
                }

                return string.Join(C.cVALSEP, compNames.Distinct());
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int AddContrToCompany(Contract contr, int compId)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    Company myComp;
                    int contrId;

                    var elements = context.Companies.Where(c => c.Id == compId);

                    if (elements.Any())
                    {
                        myComp = elements.First();

                        //verify if the ContractName already exists in the Contracts Table
                        var elemCont = context.Contracts.Where(c => c.ContractId == contr.ContractId);
                        if (elemCont.Any())
                        {
                            Contract myContr = elemCont.First();
                            myComp.Contracts.Add(myContr);
                            contrId = myContr.Id;
                        }
                        else
                        {
                            //### we should never get here
                            myComp.Contracts.Add(contr);
                            contrId = contr.Id;
                        }

                        context.SaveChanges();

                        return contrId;
                    }
                    else
                        return C.cINVALIDID;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int TryAddContrToCompany(string contrName, string comp, bool isParentCompany)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    Company myComp;
                    int contrId  = C.cINVALIDID;

                    //get the parent company
                    var elements = context.Companies.Where(c => (c.Name == comp && (isParentCompany ? c.ParentId == null : c.ParentId != null ) ));
                    
                    if (elements.Any())
                    {
                        //there should only be 1 company with that name
                        myComp = elements.First();

                        if(elements.Count() > 1)
                            log.Error("There are more than 1 companies with the same name in the Company Table: " + comp);


                        //try to find the contract 
                        var foundContract = myComp.Contracts.Where(contr => contr.ContractId == contrName);

                        if (!foundContract.Any())
                        {
                            //we did not find the contract - so we need to add it to the company
                            var elemContr = context.Contracts.Where(c => c.ContractId == contrName);
                            if (elemContr.Any())
                            {
                                Contract myContr = elemContr.First();
                                myComp.Contracts.Add(myContr);
                                contrId = myContr.Id;

                                context.SaveChanges();
                            }
                            else
                            {
                                //we did not find the contract => raise error
                                throw new Exception("The Contract with the name: '" + contrName + "' was not found in the Contract Table!");
                            }
                        }
                    }
                    else
                    {
                        //we did not find the parent company => raise error
                        throw new Exception("The Parent Company with the name: '" + comp + "' was not found in the Company Table!");
                    }

                    return contrId;
                }  
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }


        public static void UpdateCompany(Company newCompany)
        {
            try
            {
                if (newCompany == null)
                    throw new Exception("The 'Company' entity does not contain any data!");

                using (var context = new CompteResultatEntities())
                {
                    Company oldCompany = context.Companies.Where(c => c.Id == newCompany.Id).First();

                    oldCompany.Name = newCompany.Name;
                    oldCompany.Telephone = newCompany.Telephone;
                    oldCompany.Logo = newCompany.Logo;
                    oldCompany.ContactName = newCompany.ContactName;
                    oldCompany.Email = newCompany.Email;
                    oldCompany.Address = newCompany.Address;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void DeleteCompany(Company company)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.Companies.Attach(company);
                    context.Companies.Remove(company);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void DeleteRowsWithImportId(int importId)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.Companies.RemoveRange(context.Companies.Where(a => a.ImportId == importId));
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int Insert(Company company)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.Companies.Add(company);
                    context.SaveChanges();

                    return company.Id;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }



        #region OLD METHODS

        //public static void InsertCompany(Company company)
        //{
        //    try
        //    {
        //        using (var context = new CompteResultatEntities())
        //        {
        //            company.Id = GenerateCompanyID();

        //            context.Companies.Add(company);
        //            context.SaveChanges();
        //        }                
        //    }
        //    catch (DbEntityValidationException ex)
        //    {
        //        //here we can treat other error types and write to a log file...
        //        //normally there should not be any need to handle DbEntityValidationException   

        //        string mess = "";

        //        foreach (var eve in ex.EntityValidationErrors)
        //        {
        //            System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
        //                eve.Entry.Entity.GetType().Name, eve.Entry.State);


        //            foreach (var ve in eve.ValidationErrors)
        //            {
        //                System.Diagnostics.Debug.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
        //                    ve.PropertyName,
        //                    eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
        //                    ve.ErrorMessage);

        //                mess = ve.ErrorMessage;
        //            }
        //        }

        //        Exception e1 = new Exception(mess);
        //        throw e1;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message);
        //        throw ex;
        //    }
        //}
        
        //private static Int32 GenerateCompanyID()
        //{
        //    Int32 maxCompanyID = 0;

        //    var company = (from d in GetCompaniesByName("")
        //                   orderby d.Id descending
        //                   select d).FirstOrDefault();
        //    if (company != null)
        //    {
        //        maxCompanyID = company.Id + 1;
        //    }

        //    return maxCompanyID;
        //}

        //public static List<string> GetCompanyNameListForIds(string companyIdList)
        //{
        //    try
        //    {
        //        List<string> companyNames = new List<string>();
        //        List<string> companyIds = Regex.Split(companyIdList, C.cVALSEP).ToList();

        //        int id = 0;
        //        string compName = "";

        //        foreach (string compId in companyIds)
        //        {
        //            if (int.TryParse(compId, out id))
        //            {
        //                compName = GetCompanyNameForId(id);

        //                if (compName != C.cINVALIDSTRING)
        //                    companyNames.Add(compName);
        //            }
        //        }

        //        return companyNames;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message);
        //        throw ex;
        //    }
        //}

        #endregion



        //MetaData definition for basic validation

        public class MetaData
        {
            //[Display(Name = "Email address")]
            //[Required(ErrorMessage = "The email address is required")]
            //public string Email { get; set; }

            //[Display(Name = "Number of Employees")]
            //[Required(ErrorMessage = "The Number of Employees field is required")]
            //[Range(2, 50, ErrorMessage = "Min=2 - Max=50")]
            //public int NumbEmployees { get; set; }

        }
    }
}
