using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompteResultat.BL
{
    public class ExcelPrestaSheet_OLD
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public int ImportId { get; set; }
        public string AssureurName { get; set; }
        public int AnneeExp { get; set; }

        public DateTime? DateVision { get; set; }
        public string ContractId { get; set; }
        public string CodeCol { get; set; }
        public DateTime? DateSoins { get; set; }
        public string CodeActe { get; set; }
        public string GroupName { get; set; }
        public string GarantyName { get; set; }
        public string CAS { get; set; }
        public int? NombreActe { get; set; }
        public double? FraisReel { get; set; }
        public double? RembSS { get; set; }
        public double? RembAnnexe { get; set; }
        public double? RembNous { get; set; }
        public string Reseau { get; set; }
        public double? MinFR { get; set; }
        public double? MaxFR { get; set; }
        public double? MinNous { get; set; }
        public double? MaxNous { get; set; }
        public string BO1 { get; set; }
        public string BO2 { get; set; }

    }
}
