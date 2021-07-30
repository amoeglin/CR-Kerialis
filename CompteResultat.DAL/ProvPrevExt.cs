using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using CompteResultat.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;

namespace CompteResultat.DAL
{
    [MetadataType(typeof(DecomptePrev.MetaData))]
    public partial class ProvPrev
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<ProvPrev> GetProvPrevForContracts(List<string> assurList, List<string> parentCompanyList, List<string> companyList,
            List<string> contrIds, string college, DateTime debutPeriod, DateTime finPeriod, DateTime dateArret)
        {
            try
            {
                List<ProvPrev> provPrev;

                using (var context = new CompteResultatEntities())
                {
                    //selection with DateProvision
                    provPrev = context.ProvPrevs.Where(pp => assurList.Contains(pp.AssureurName) && parentCompanyList.Contains(pp.Company)
                        && companyList.Contains(pp.Subsid) && contrIds.Contains(pp.ContractId)
                        && pp.DateSinistre >= debutPeriod && pp.DateSinistre <= finPeriod
                        && pp.DateProvision == dateArret).ToList();
                }

                return provPrev;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }
    }

    public class MetaData
    {
        //[Display(Name = "Email address")]
        //[Required(ErrorMessage = "The email address is required")]
        //public string Email { get; set; }

    }
}
