using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompteResultat.BL
{
    public class Groups
    {
        public string Name { get; set; }
        public List<Entrepr> Enterprises { get; set; }
    }

    public class Entrepr
    {
        public string Name { get; set; }
        public string GroupName { get; set; }
        public string RaisonSociale { get; set; }
        public string Structure { get; set; }
    }

    //public class GenericClasses
    //{
    //    public GenericClasses() { }

    //    public string GroupName { get; set; }
    //    public string GarantyName { get; set; }
    //    public string CodeActe { get; set; }
    //    public string AssureurName { get; set; }

    //}

    //public class IMAssurContrIDPair
    //{
    //    public IMAssurContrIDPair() { }

    //    public int IdAssurance { get; set; }
    //    public int IdContract { get; set; }       

    //}

    //public class OtherTableAssurContrPair
    //{
    //    public OtherTableAssurContrPair() { }

    //    public string Assureur { get; set; }
    //    public string ContractId { get; set; }

    //}
}
