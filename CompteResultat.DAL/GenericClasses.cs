using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompteResultat.DAL
{
    public class GenericClasses
    {
        public GenericClasses() { }

        public string GroupName { get; set; }
        public string GarantyName { get; set; }
        public string CodeActe { get; set; }
        public string AssureurName { get; set; }

    }

    public class GroupesGarantiesSante
    {
        public string AssureurName { get; set; }
        public string GroupName { get; set; }
        public string GarantyName { get; set; }
        public string CodeActe { get; set; }
        public int OrderNumber { get; set; }
    }

    public class TypePrev
    {
        public string CodeSinistre { get; set; }
        public string LabelSinistre { get; set; }
    }

    public class ExcelGlobalDecompteData
    {
        public string Assureur { get; set; }
        public string Company { get; set; }
        public string Subsid { get; set; }
        public int YearSurv { get; set; }
        public double? FR { get; set; }

        public double? RSS { get; set; }
        public double? RAnnexe { get; set; }
        public double? RNous { get; set; }
        public double Provisions { get; set; }
        public double CotBrute { get; set; }
        public string TaxTotal { get; set; }
        public string TaxDefault { get; set; }
        public string TaxActive { get; set; }
        public double CotNet { get; set; }
        public double Ratio { get; set; }
        public double GainLoss { get; set; }
        public DateTime DateArret { get; set; }
        public double? Coef { get; set; }
    }

    public class ExcelGlobalSinistreData
    {
        public string Assureur { get; set; }
        public string Company { get; set; }
        public string Subsid { get; set; }
        public int YearSurv { get; set; }
        public double? FR { get; set; }
        public double? RSS { get; set; }
        public double? RAnnexe { get; set; }
        public double? RNous { get; set; }
        public double Provisions { get; set; }
        public double CotBrut { get; set; }
        public string TaxTotal { get; set; }
        public string TaxDefault { get; set; }
        public string TaxActive { get; set; }
        public double CotNet { get; set; }
        public double Ratio { get; set; }
        public double GainLoss { get; set; }
        public DateTime DateArret { get; set; }

        public double? Coef { get; set; }
    }

    public class TopGroupNames
    {
        public string GroupName { get; set; }
        public double? TotalRembNous { get; set; }
    }

    public class ExcelGlobalPrestaData
    {
        public string Assureur { get; set; }        
        public string Company { get; set; }
        public string Subsid { get; set; }
        public string Contract { get; set; }
        public int YearSurv { get; set; }
        public double? FR { get; set; }
        public double? RSS { get; set; }
        public double? RAnnexe { get; set; }
        public double? RNous { get; set; }
        public double Provisions { get; set; }
        public double CotBrut { get; set; }
        public string TaxTotal { get; set; }
        public string TaxDefault { get; set; }
        public string TaxActive { get; set; }
        public double CotNet { get; set; }
        public double Ratio { get; set; }
        public double GainLoss { get; set; }
        public DateTime DateArret { get; set; }
        public double? Coef { get; set; }
        public double TauxChargement { get; set; }
    }

    public class Synthese
    {
        public string Assur { get; set; }
        public string Company { get; set; }
        public int Annee { get; set; }
        public string Prestations { get; set; }
        public string Provisions { get; set; }
        public string CotBrut { get; set; }
        public string Chargements { get; set; }
        public string CotNet { get; set; }
        public string Ratio { get; set; }
        public string GainLoss { get; set; }
        public string CoeffProv { get; set; }
        public string FR { get; set; }
        public string RSS { get; set; }
        public string RAnnexe { get; set; }
        public string DateArrete { get; set; }
    }

    public class CumulPresta
    {
        public string AssureurName { get; set; }
        public int AnneeSoins { get; set; }
        public int MoisReglement { get; set; }
        public double? SommePresta { get; set; }
    }

    public class NumberBenefs
    {
        public string Benef { get; set; }
        public int Number { get; set; }
    }

    public class ExcelGlobalCotisatData
    {
        public string Assureur { get; set; }
        public string Company { get; set; }
        public string Subsid { get; set; }
        public int YearSurv { get; set; }
        public double? Cotisat { get; set; }
        public double? CotisatBrute { get; set; }
    }

    public class IMAssurContrIDPair : IEquatable<IMAssurContrIDPair>
    {
        public IMAssurContrIDPair() { }

        public int IdAssurance { get; set; }
        public int IdContract { get; set; }

        public override string ToString()
        {
            return IdAssurance + "-" + IdContract;
        }

        //public bool Equals(IMAssurContrIDPair other)
        //{
        //    if (other == null) return false;
        //    return (this.IdAssurance.Equals(other.IdAssurance) && this.IdContract.Equals(other.IdContract));
        //}

        public bool Equals(IMAssurContrIDPair other)
        {
            if (this.IdAssurance == other.IdAssurance && this.IdContract == other.IdContract)            
                return true;            
            else           
                return false;            
        }

        //public override bool Equals(object obj)
        //{
        //    if (obj == null) return false;
        //    IMAssurContrIDPair objAsPart = obj as IMAssurContrIDPair;

        //    if (objAsPart == null) return false;
        //    else return Equals(objAsPart);
        //}

       

        //Using:
        //parts.Contains(new Part { PartId = 1734, PartName = "" });

        // Find items where name contains "seat".
        //parts.Find(x => x.PartName.Contains("seat"));

        // Check if an item with Id 1444 exists.
        //parts.Exists(x => x.PartId == 1444);

    }

    public class IMContrCompIDPair : IEquatable<IMContrCompIDPair>
    {
        public IMContrCompIDPair() { }
        
        public int IdContract { get; set; }
        public int IdCompany { get; set; }

        public override string ToString()
        {
            return IdContract + "-" + IdCompany;
        }

        public bool Equals(IMContrCompIDPair other)
        {
            if (this.IdContract == other.IdContract && this.IdCompany == other.IdCompany)
                return true;
            else
                return false;
        }
    }

    public class CompNameIDPair : IEquatable<CompNameIDPair>
    {
        public CompNameIDPair() { }

        public string CompanyName { get; set; }
        public int CompanyId { get; set; }

        public override string ToString()
        {
            return CompanyId.ToString() + "-" + CompanyName;
        }

        public bool Equals(CompNameIDPair other)
        {
            if (this.CompanyId == other.CompanyId && this.CompanyName == other.CompanyName)
                return true;
            else
                return false;
        }
    }

    public class ContrNameIDPair : IEquatable<ContrNameIDPair>
    {
        public ContrNameIDPair() { }

        public string ContrName { get; set; }
        public int ContrId { get; set; }

        public override string ToString()
        {
            return ContrId.ToString() + "-" + ContrName;
        }

        public bool Equals(ContrNameIDPair other)
        {
            if (this.ContrId == other.ContrId && this.ContrName == other.ContrName)
                return true;
            else
                return false;
        }
    }

    public class OtherTableAssurContrPair
    {
        public OtherTableAssurContrPair() { }

        public string Assureur { get; set; }
        public string ContractId { get; set; }
    }

    public class OtherTableContrCompPair
    {
        public OtherTableContrCompPair() { }
        
        public string ContractId { get; set; }
        public string Company { get; set; }

    }

    public class OtherTableContrSubsidPair
    {
        public OtherTableContrSubsidPair() { }

        public string ContractId { get; set; }
        public string Subsid { get; set; }

    }

    public class DecomptePrevReduced
    {
        public DecomptePrevReduced() { }

        public string Dossier { get; set; }
        public double? Total { get; set; }
        public DateTime? DatePayement { get; set; }
        public DateTime? DebSin { get; set; }
        public DateTime? FinSin { get; set; }
    }

    public class DemoSanteWithOptionInfo
    {
        public DemoSanteWithOptionInfo() { }

        public string ContractId { get; set; }
        public string WithOption { get; set; }
    }

    public class PrestSanteContrIdCount
    {
        public PrestSanteContrIdCount() { }

        public string ContractId { get; set; }
        public long Count { get; set; }
    }

    public class ExcelPrestaSheet
    {        
        public int ImportId { get; set; }
        public string AssureurName { get; set; }
        public int AnneeExp { get; set; }
        public int AnneeDateSoins { get; set; }

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

    public partial class SinistreEtProv
    {
        public int Id { get; set; }
        public int ImportId { get; set; }
        public string AssureurName { get; set; }
        public string ContractId { get; set; }
        public string Company { get; set; }
        public string Subsid { get; set; }
        public string Dossier { get; set; }
        public string CodeCol { get; set; }
        public string Title { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime? Birthdate { get; set; }
        public DateTime? DateSinistre { get; set; }
        public string NatureSinistre { get; set; }
        public string CauseSinistre { get; set; }
        public DateTime? DebVal { get; set; }
        public DateTime? FinVal { get; set; }
        public DateTime? DateRecep { get; set; }
        public DateTime? DateRechute { get; set; }
        public DateTime? DateClo { get; set; }
        public string MotifClo { get; set; }
        public DateTime? DatePayment { get; set; }
        public DateTime? DateProvision { get; set; }
        public string Matricule { get; set; }
        public double? Pm { get; set; }
        public double? PmPassage { get; set; }
        public double? Psap { get; set; }
        public double? PmMgdc { get; set; }
        public double? Psi { get; set; }
    }
}
