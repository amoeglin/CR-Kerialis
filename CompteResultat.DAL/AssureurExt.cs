using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using CompteResultat.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;

namespace CompteResultat.DAL
{
    [MetadataType(typeof(Assureur.MetaData))]
    public partial class Assureur
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<Assureur> GetAllAssureurs()
        {
            try
            {
                List<Assureur> assur;

                using (var context = new CompteResultatEntities())
                {
                    assur = context.Assureurs.Include("Contracts").ToList();
                   
                }

                //if (assur == null || assur.Count == 0)
                //    throw new Exception("The 'Assureur' entity does not contain any data!");

                return assur;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<Assureur> GetAssureursWithoutGroupsAndGarantiesSante()
        {
            try
            {
                List<Assureur> assur;
                List<string> assurNames = new List<string>();

                using (var context = new CompteResultatEntities())
                {
                    //first, get unique assureur names from groups & garanties table
                    assurNames = context.GroupGarantySantes.Where(a=>a.AssureurName != C.cDEFAULTASSUREUR).
                        OrderBy(a=>a.AssureurName).Select(a => a.AssureurName).Distinct().ToList();
                    
                    assur = context.Assureurs.Where(a => !assurNames.Contains(a.Name)).ToList();
                }

                return assur;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<Assureur> GetAssureursWithoutCadencier()
        {
            try
            {
                List<Assureur> assur;
                List<string> assurNames = new List<string>();

                using (var context = new CompteResultatEntities())
                {
                    //first, get unique assureur names from cadencier table
                    assurNames = context.Cadenciers.Where(a => a.AssureurName != C.cDEFAULTASSUREUR).
                        OrderBy(a => a.AssureurName).Select(a => a.AssureurName).Distinct().ToList();

                    assur = context.Assureurs.Where(a => !assurNames.Contains(a.Name)).ToList();
                }

                return assur;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetUniqueAssNames()
        {
            try
            {
                List<string> assNames;

                using (var context = new CompteResultatEntities())
                {
                    assNames = context.Assureurs.Where(c => c.Name != null && c.Name != "").Select(c => c.Name).Distinct().ToList();
                }

                return assNames;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetProductAssNames()
        {
            try
            {
                List<string> assNames;

                using (var context = new CompteResultatEntities())
                {
                    assNames = context.Assureurs.Where(c => c.Name.Contains(C.cASSTYPEPRODUCT)).Select(c => c.Name).Distinct().ToList();
                }

                return assNames;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetEnterpriseAssNamesByType(string assType)
        {
            try
            {
                List<string> assNames;

                using (var context = new CompteResultatEntities())
                {
                    assNames = context.Assureurs.Where(c => c.Name.Contains(assType)).Select(c => c.Name).Distinct().ToList();
                }

                return assNames;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static Assureur GetAssById(int id)
        {
            try
            {
                Assureur ass;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.Assureurs.Include("Contracts").Where(a => a.Id == id);

                    if (elements.Any())
                        ass = elements.First();
                    else
                        ass = null;
                }

                return ass;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static Assureur GetAssByName(string name)
        {
            try
            {
                Assureur ass;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.Assureurs.Where(a => a.Name == name);

                    if (elements.Any())
                        ass = elements.First();
                    else
                        ass = null;
                }

                return ass;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int GetAssIdForAssName(string assName)
        {
            try
            {
                int assurId;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.Assureurs.Where(a => a.Name == assName).Select(a => a.Id);

                    if (elements.Any())
                        assurId = elements.First();
                    else
                        assurId = C.cINVALIDID;

                }

                return assurId;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<Contract> GetAllContractsForAssureur(int assurId)
        {
            try
            {
                List<Contract> contracts = null;

                using (var context = new CompteResultatEntities())
                {
                    Assureur myAssur;
                    //IEnumerable<Contract> foundContracts;

                    //get the company
                    var elements = context.Assureurs.Where(a => a.Id == assurId);

                    if (elements.Any())
                    {
                        //there should only be 1 company with that name
                        myAssur = elements.First();

                        //try to find the contract
                        var foundContracts = myAssur.Contracts;

                        if (foundContracts.Any())                        
                            contracts = foundContracts.ToList();                       
                    }
                    else
                    {
                        //we did not find the company => raise error
                        throw new Exception("The Assureur with the Id: '" + assurId + "' was not found in the Company Table!");
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

        public static List<string> GetAllContractNamesForAssureur(int assurId)
        {
            try
            {
                List<string> contracts = null;

                using (var context = new CompteResultatEntities())
                {
                    Assureur myAssur;

                    //get the company
                    var elements = context.Assureurs.Where(a => a.Id == assurId);

                    if (elements.Any())
                    {
                        //there should only be 1 company with that name
                        myAssur = elements.First();

                        //try to find the contract
                        var foundContracts = myAssur.Contracts;

                        if (foundContracts.Any())
                            contracts = foundContracts.Select(c=>c.ContractId).ToList();
                    }
                    else
                    {
                        //we did not find the company => raise error
                        throw new Exception("The Assureur with the Id: '" + assurId + "' was not found in the Company Table!");
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


        public static List<IMAssurContrIDPair> GetAssurContrIDPairsFromIMTable()
        {
            try
            {
                List<IMAssurContrIDPair> ACIDPair;

                using (var context = new CompteResultatEntities())
                {
                    ACIDPair = context.Database.SqlQuery<IMAssurContrIDPair>("select * from dbo.IMAssContr").ToList<IMAssurContrIDPair>();
                }

                return ACIDPair;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }


        public static int Insert(Assureur ass)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.Assureurs.Add(ass);
                    context.SaveChanges();

                    return ass.Id;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void InsertIDPairIntoIMTable(IMAssurContrIDPair idPair)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    string sql = "INSERT INTO dbo.IMAssContr (IdAssurance, IdContract)  VALUES(@idAssur, @idContr)";

                    List<SqlParameter> parameterList = new List<SqlParameter>();
                    parameterList.Add(new SqlParameter("@idAssur", idPair.IdAssurance));
                    parameterList.Add(new SqlParameter("@idContr", idPair.IdContract));                    
                    SqlParameter[] parameters = parameterList.ToArray();

                    context.Database.ExecuteSqlCommand(sql, parameters);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int TryAddContractToAss(string contrName, string ass)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    Assureur myAss;
                    int contrId = C.cINVALIDID;

                    //get the Contract
                    var elements = context.Assureurs.Where(a => (a.Name == ass));

                    if (elements.Any())
                    {
                        //there should only be 1 contract with that name
                        myAss = elements.First();

                        if (elements.Count() > 1)
                            log.Error("There are more than 1 assureurs with the same name in the Assureur Table: " + ass);


                        //try to find the contract 
                        var foundContr = myAss.Contracts.Where(c => c.ContractId == contrName);

                        if (!foundContr.Any())
                        {
                            //we did not find the contract - so we need to add it to the assureur
                            var elemContr = context.Contracts.Where(c => c.ContractId == contrName);
                            if (elemContr.Any())
                            {
                                Contract myContr = elemContr.First();
                                myAss.Contracts.Add(myContr);
                                contrId = myContr.Id;

                                context.SaveChanges();
                            }
                            else
                            {
                                //we did not find the Contract => raise error
                                throw new Exception("The Contract with the name: '" + contrName + "' was not found in the Contract Table!");
                            }
                        }
                    }
                    else
                    {
                        //we did not find the Contract => raise error
                        throw new Exception("The Contract with the name: '" + contrName + "' was not found in the Contract Table!");
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

        public static int AddContractToAss(Contract contr, int assId)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    Assureur myAss;
                    var elements = context.Assureurs.Where(a => a.Id == assId);

                    if (elements.Any())
                    {
                        myAss = elements.First();
                        myAss.Contracts.Add(contr);

                        context.SaveChanges();

                        return contr.Id;
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

        public static void DeleteRowsWithImportId(int importId)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.Assureurs.RemoveRange(context.Assureurs.Where(a => a.ImportId == importId));
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
