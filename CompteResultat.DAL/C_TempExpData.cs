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
    
    public partial class C_TempExpData
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> Au { get; set; }
        public string Contrat { get; set; }
        public string CodCol { get; set; }
        public Nullable<int> AnneeExp { get; set; }
        public string LibActe { get; set; }
        public string LibFam { get; set; }
        public string TypeCas { get; set; }
        public Nullable<int> NombreActe { get; set; }
        public Nullable<double> Fraisreel { get; set; }
        public Nullable<double> Rembss { get; set; }
        public Nullable<double> RembAnnexe { get; set; }
        public Nullable<double> RembNous { get; set; }
        public string Reseau { get; set; }
        public Nullable<double> MinFr { get; set; }
        public Nullable<double> MaxFr { get; set; }
        public Nullable<double> MinNous { get; set; }
        public Nullable<double> MaxNous { get; set; }
        public Nullable<int> ImportId { get; set; }
        public string AssureurName { get; set; }
    }
}
