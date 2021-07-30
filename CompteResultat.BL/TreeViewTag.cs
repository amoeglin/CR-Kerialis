using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using CompteResultat.Common;
using CompteResultat.DAL;

namespace CompteResultat.BL
{
    [Serializable()]
    public class TreeViewTag
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public C.eTVNodeTypes NodeType { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }

        //Properties for  Assureur
        public int AssureurId { get; set; }

        //Properties for Subsid
        public int? ParentCompId { get; set; }

        //Properties for CR
        public string CRCompanyIds { get; set; }
        public string CRSubsids { get; set; }
        public string CRContractIds { get; set; }
        public int? CRCollegeId { get; set; }
        public int? CRReportLevelId { get; set; }
        public DateTime? CRCreationDate { get; set; }
        public C.eReportTypes? CRReportType { get; set; }

        public double? TaxDef { get; set; }
        public double? TaxAct { get; set; }
        public double? TaxPer { get; set; }


        //Planning Data
        public List<CRPlanning> CRPs;

        //public DateTime? CRDateDebut { get; set; }
        //public DateTime? CRDateFin { get; set; }
        //public DateTime? CRDateArrete { get; set; }


        public TreeViewTag()
        {
            CRPs = new List<CRPlanning>();
        }

        public string GetStringFromObject()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this);
                return json;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static TreeViewTag GetObjectFromString(string objString)
        {
            try
            {
                TreeViewTag tvTag = JsonConvert.DeserializeObject<TreeViewTag>(objString);
                return tvTag;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        
    }
}
