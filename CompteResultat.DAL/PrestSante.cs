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
    
    public partial class PrestSante
    {
        public int Id { get; set; }
        public int ImportId { get; set; }
        public string ContractId { get; set; }
        public string CodeCol { get; set; }
        public Nullable<System.DateTime> DateSoins { get; set; }
        public string CodeActe { get; set; }
        public Nullable<int> NombreActe { get; set; }
        public Nullable<double> FraisReel { get; set; }
        public Nullable<double> RembSS { get; set; }
        public Nullable<double> RembAnnexe { get; set; }
        public Nullable<double> RembNous { get; set; }
        public Nullable<System.DateTime> DateVision { get; set; }
        public string AssureurName { get; set; }
        public string GroupName { get; set; }
        public string GarantyName { get; set; }
        public string CAS { get; set; }
        public string Reseau { get; set; }
        public string Company { get; set; }
        public string Subsid { get; set; }
        public Nullable<System.DateTime> DatePayment { get; set; }
        public Nullable<double> PrixUnit { get; set; }
        public string WithOption { get; set; }
        public string Beneficiaire { get; set; }
        public string BO1 { get; set; }
        public string BO2 { get; set; }
        public Nullable<int> Test { get; set; }
    }
}
