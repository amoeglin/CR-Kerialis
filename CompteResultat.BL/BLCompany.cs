using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Text.RegularExpressions;

using CompteResultat.DAL;
using CompteResultat.Common;


namespace CompteResultat.BL
{
    public class BLCompany 
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public BLCompany()
        {             
        }       

        public IEnumerable<Company> GetCompanies()
        {            
            try
            {
                // Handle business logic

                // just for testing, lets assume we have an error and we send it back to the presentation layer
                bool error = false;
                if (error)
                    throw new TestException("An error was generated in BLCompany :: GetCompanies");

                return GetCompaniesByName("");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public IEnumerable<Company> GetCompaniesByName(string companySearchString)
        {
            try
            {
                // Handle business logic
                if (String.IsNullOrWhiteSpace(companySearchString))
                {
                    companySearchString = "";
                }

                return Company.GetCompaniesByName(companySearchString);
            }                        
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
}

        public static string GetCompanyNamesFromIdList(string Ids)
        {
            List<string> compNames = new List<string>();
            List<string> idList = Regex.Split(Ids, C.cVALSEP).ToList();
            foreach (string companyId in idList)
            {
                compNames.Add(Company.GetCompanyNameForId(int.Parse(companyId)));
            }

            return string.Join(C.cVALSEP, compNames);
        }

        public void InsertCompany(Company company)
        {
            try
            {
                if (company == null)
                    throw new Exception("The 'Company' entity does not contain any data!");


                // ### additional validation that could be done:
                if (company.Name.Length < 3)
                {  
                    //throw new DbEntityValidationException("The name must have at least 3 characters");
                    //throw new Exception("The name must have at least 3 characters");
                }
                
                Company.Insert(company);
            }            
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public void DeleteCompany(Company company)
        {
            try
            {
                if (company == null)
                    throw new Exception("The 'Company' entity does not contain any data!");

                //Handle business rules...

                Company.DeleteCompany(company);
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

                //Handle business rules...


                Company.UpdateCompany(newCompany);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }


        public static string CreateCompanyValueString(Company comp)
        {
            string valString = "";

            valString += C.eCompanyProperties.Id.ToString() + "::" + comp.Id + ";";
            valString += C.eCompanyProperties.Name.ToString() + "::" + comp.Name + ";";
            valString += C.eCompanyProperties.Address.ToString() + "::" + comp.Address + ";";
            valString += C.eCompanyProperties.Logo.ToString() + "::" + comp.Logo + ";";
            valString += C.eCompanyProperties.Email.ToString() + "::" + comp.Email + ";";
            valString += C.eCompanyProperties.Telephone.ToString() + "::" + comp.Telephone + ";";
            valString += C.eCompanyProperties.ContactName.ToString() + "::" + comp.ContactName + ";";
            valString += C.eCompanyProperties.ParentId.ToString() + "::" + comp.ParentId + ";";
            valString += C.eCompanyProperties.Type.ToString() + "::" + comp.Type;

            return valString;
        }

        public static Dictionary<int, string> GetCompanyPropsAsDictionary(string companyProps)
        {
            try
            {
                Dictionary<int, string> dictCompany = new Dictionary<int, string>();
                //dictCompany.Add((int)C.eCompanyProperties.Name, "Name Value");
                //string name = dictCompany[(int)C.eCompanyProperties.Name];

                string[] items = Regex.Split(companyProps, ";");
                foreach (string item in items)
                {
                    string key = Regex.Split(item, "::")[0];
                    string val = Regex.Split(item, "::")[1];

                    C.eCompanyProperties prop = (C.eCompanyProperties)System.Enum.Parse(typeof(C.eCompanyProperties), key);
                    dictCompany.Add((int)prop, val);
                }

                return dictCompany;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return null;
            }
        }

        




        #region OLD FUNCTIONS

        public void UpdateCompany_OLD(Company company)
        {
            try
            {
                //context.Companies.AddOrUpdate(company);
                //context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                //### CAREFUL !!! This type of exception is handled automatically if DynamicFields are used in the .aspx


                //here we can treat other error types and write to a log file...
                //normally there should not be any need to handle DbEntityValidationException   

                string mess = "";

                foreach (var eve in ex.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        mess += ve.ErrorMessage + Environment.NewLine;
                    }
                }

                Exception e1 = new Exception(mess);
                throw e1;
            }
            catch (Exception ex)
            {
                //Include catch blocks for specific exceptions first,
                //and handle or log the error as appropriate in each.
                //Include a generic catch block like this one last.
                throw ex;
            }
        }





        #endregion

    }
}
