//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CompteResultat.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class DecomptePrev
    {
        public int Id { get; set; }
        public int ImportId { get; set; }
        public string Dossier { get; set; }
        public string Company { get; set; }
        public string Subsid { get; set; }
        public string CodeCol { get; set; }
        public string Apporteur { get; set; }
        public string Gestionnaire { get; set; }
        public Nullable<System.DateTime> DatePayement { get; set; }
        public Nullable<System.DateTime> DateVirement { get; set; }
        public string AssureurName { get; set; }
        public string ContractId { get; set; }
        public Nullable<System.DateTime> DateSin { get; set; }
        public Nullable<System.DateTime> DebSin { get; set; }
        public Nullable<System.DateTime> FinSin { get; set; }
        public Nullable<System.DateTime> DateExtraction { get; set; }
        public Nullable<double> Total { get; set; }
        public string CauseSinistre { get; set; }
    }
}
