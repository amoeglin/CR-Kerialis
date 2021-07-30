using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

using CompteResultat.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;

namespace CompteResultat.DAL
{
    [MetadataType(typeof(Contract.MetaData))]
    public partial class Contract
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static List<Contract> GetContracts()
        {
            try
            {
                List<Contract> contracts;

                using (var context = new CompteResultatEntities())
                {
                    contracts = context.Contracts.ToList();
                }

                return contracts;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetUniqueContractIds()
        {
            try
            {
                List<string> contractIds;

                using (var context = new CompteResultatEntities())
                {
                    contractIds = context.Contracts.Where(c => c.ContractId != null && c.ContractId != "").Select(c => c.ContractId).Distinct().ToList();
                }

                return contractIds;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int GetContrIdForContrName(string contractName)
        {
            try
            {
                int contrId;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.Contracts.Where(c => c.ContractId == contractName).Select(c => c.Id);

                    if (elements.Any())
                        contrId = elements.First();
                    else
                        contrId = C.cINVALIDID;
                }

                return contrId;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static string GetContractNameForId(int id)
        {
            try
            {
                string name;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.Contracts.Where(c => c.Id == id).Select(c => c.ContractId);

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

        public static string oldGetContractNamesFromIdList(string Ids)
        {
            List<string> contrNames = new List<string>();
            List<string> idList = Regex.Split(Ids, C.cVALSEP).ToList();
            int id = 0;

            foreach (string contrId in idList)
            {
                if (int.TryParse(contrId, out id))
                {
                    string myContr = Contract.GetContractNameForId(id);
                    if (myContr != C.cINVALIDSTRING)
                        contrNames.Add(myContr);
                }
            }

            return string.Join(C.cVALSEP, contrNames.Distinct());
        }

        public static string GetContractNamesFromIdList(string contractIdList)
        {
            List<string> contrNames = new List<string>();
            List<ContrNameIDPair> contrIdPAirs = new List<ContrNameIDPair>();
            List<string> idList = Regex.Split(contractIdList, C.cVALSEP).ToList();
            int id = 0;

            try
            {
                //load all company-id pairs into a List : ContrNameIDPair
                using (var context = new CompteResultatEntities())
                {
                    contrIdPAirs = context.Contracts.Select(c => new ContrNameIDPair { ContrId = c.Id, ContrName = c.ContractId }).ToList();
                }

                //create a list of company names
                foreach (string contrId in idList)
                {
                    if (int.TryParse(contrId, out id))
                    {
                        string myContract = "";
                        var vContr = contrIdPAirs.Where(c => c.ContrId == id).ToList();

                        if (vContr.Any())
                            myContract = vContr.Select(c => c.ContrName).First().ToString();

                        if (myContract != C.cINVALIDSTRING)
                            contrNames.Add(myContract);
                    }
                }

                return string.Join(C.cVALSEP, contrNames.Distinct());
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static string GetAllContractNames()
        {
            List<string> contrIds = GetUniqueContractIds();            
            return string.Join(C.cVALSEP, contrIds.Distinct()); ;
        }

        public static List<IMContrCompIDPair> GetContrCompIDPairsFromIMTable()
        {
            try
            {
                List<IMContrCompIDPair> CCIDPair;

                using (var context = new CompteResultatEntities())
                {
                    CCIDPair = context.Database.SqlQuery<IMContrCompIDPair>("select * from dbo.IMContrComp").ToList<IMContrCompIDPair>();
                }

                return CCIDPair;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int TryAddCompanyToContract(string contrName, string comp)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    Contract myContr;
                    int compId = C.cINVALIDID;

                    //get the Contract
                    var elements = context.Contracts.Where(c => (c.ContractId == contrName ));

                    if (elements.Any())
                    {
                        //there should only be 1 contract with that name
                        myContr = elements.First();

                        if (elements.Count() > 1)
                            log.Error("There are more than 1 contracts with the same name in the Contract Table: " + contrName);


                        //try to find the company 
                        var foundComp = myContr.Companies.Where(c => c.Name == comp);

                        if (!foundComp.Any())
                        {
                            //we did not find the company - so we need to add it to the contract
                            var elemComp = context.Companies.Where(c => c.Name == comp);
                            if (elemComp.Any())
                            {
                                Company myComp = elemComp.First();
                                myContr.Companies.Add(myComp);
                                compId = myComp.Id;

                                context.SaveChanges();                                
                            }
                            else
                            {
                                //we did not find the COMPANY => raise error
                                throw new Exception("The Company with the name: '" + comp + "' was not found in the Company Table!");
                            }
                        }
                    }
                    else
                    {
                        //we did not find the Contract => raise error
                        throw new Exception("The Contract with the name: '" + contrName + "' was not found in the Contract Table!");
                    }

                    return compId;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int TryAddAssToContract(string contrName, string ass)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    Contract myContr;
                    int assId = C.cINVALIDID;

                    //get the Contract
                    var elements = context.Contracts.Where(c => (c.ContractId == contrName));

                    if (elements.Any())
                    {
                        //there should only be 1 contract with that name
                        myContr = elements.First();

                        if (elements.Count() > 1)
                            log.Error("There are more than 1 contracts with the same name in the Contract Table: " + contrName);


                        //try to find the assureur 
                        var foundAss = myContr.Assureurs.Where(a => a.Name == ass);

                        if (!foundAss.Any())
                        {
                            //we did not find the company - so we need to add it to the contract
                            var elemAss = context.Assureurs.Where(a => a.Name == ass);
                            if (elemAss.Any())
                            {
                                Assureur myAss = elemAss.First();
                                myContr.Assureurs.Add(myAss);
                                assId = myAss.Id;

                                context.SaveChanges();
                            }
                            else
                            {
                                //we did not find the Assureur => raise error
                                throw new Exception("The Assureur with the name: '" + ass + "' was not found in the Assureur Table!");
                            }
                        }
                    }
                    else
                    {
                        //we did not find the Contract => raise error
                        throw new Exception("The Contract with the name: '" + contrName + "' was not found in the Contract Table!");
                    }

                    return assId;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int Insert(Contract contr)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.Contracts.Add(contr);
                    context.SaveChanges();

                    return contr.Id;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void InsertIDPairIntoIMTable(IMContrCompIDPair idPair)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    string sql = "INSERT INTO dbo.IMContrComp (IdContract, IdCompany)  VALUES(@idContr, @idComp)";

                    List<SqlParameter> parameterList = new List<SqlParameter>();                    
                    parameterList.Add(new SqlParameter("@idContr", idPair.IdContract));
                    parameterList.Add(new SqlParameter("@idComp", idPair.IdCompany));
                    SqlParameter[] parameters = parameterList.ToArray();

                    context.Database.ExecuteSqlCommand(sql, parameters);
                }
            }
            catch (Exception ex)
            {
                //###
                if (!ex.Message.Contains("Violation of PRIMARY KEY constraint"))
                {
                    log.Error(ex.Message);
                    throw ex;
                }                
            }
        }

        public static void DeleteRowsWithImportId(int importId)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.Contracts.RemoveRange(context.Contracts.Where(c => c.ImportId == importId));
                    context.SaveChanges();
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
            //[Display(Name = "Email address")]
            //[Required(ErrorMessage = "The email address is required")]
            //public string Email { get; set; }

        }
    }
}
