using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompteResultat.BL
{
    //********************* SANTE ***************************************

    public class UK_CotSante
    {
        public int RowNumber { get; set; }
        public int Id { get; set; }
        public string ContractId { get; set; }
        public string Company { get; set; }
        public string CodeCol { get; set; }
        public int Year { get; set; }
        public double CotisationBrute { get; set; }
    }

    public class CotSanteComparer : EqualityComparer<UK_CotSante>
    {
        public override bool Equals(UK_CotSante x, UK_CotSante y)
        {
            if (x == null || y == null)
                return x == y;

            return x.ContractId.ToUpper().Trim() == y.ContractId.ToUpper().Trim() && x.Company.ToUpper().Trim() == y.Company.ToUpper().Trim()
                && x.CodeCol.ToUpper().Trim() == y.CodeCol.ToUpper().Trim() && x.Year.ToString().ToUpper().Trim() == y.Year.ToString().ToUpper().Trim()
                && x.CotisationBrute.ToString().ToUpper().Trim() == y.CotisationBrute.ToString().ToUpper().Trim();
        }

        public override int GetHashCode(UK_CotSante obj)
        {
            return obj == null ? 0 : (obj.ContractId.GetHashCode() ^ obj.Company.GetHashCode() ^ obj.CodeCol.GetHashCode() 
                ^ obj.Year.GetHashCode() ^ obj.CotisationBrute.GetHashCode() ); 
        }
    }

    public class UK_PrestSante
    {
        //PrestSant: ContractId,Company,CodeCol,DateSoins,CodeActe,FraisReel,RembSS,RembNous,DatePayment,Beneficiaire
        public int RowNumber { get; set; }
        public int Id { get; set; }
        public string ContractId { get; set; }
        public string Company { get; set; }
        public string CodeCol { get; set; }
        public DateTime DateSoins { get; set; }        
        public string CodeActe { get; set; }
        public double FraisReel { get; set; }
        public double RembSS { get; set; }
        public double RembNous { get; set; }
        public DateTime DatePayment { get; set; }
        public string Beneficiaire { get; set; }
    }

    public class PrestSanteComparer : EqualityComparer<UK_PrestSante>
    {
        public override bool Equals(UK_PrestSante x, UK_PrestSante y)
        {
            if (x == null || y == null)
                return x == y;
            //PrestSant: ContractId,Company,CodeCol,DateSoins,CodeActe,FraisReel,RembSS,RembNous,DatePayment,Beneficiaire

            return x.ContractId.ToString().ToUpper().Trim() == y.ContractId.ToString().ToUpper().Trim() 
                && x.Company.ToString().ToUpper().Trim() == y.Company.ToString().ToUpper().Trim()
                && x.CodeCol.ToString().ToUpper().Trim() == y.CodeCol.ToString().ToUpper().Trim() 
                && x.DateSoins.ToString().ToUpper().Trim() == y.DateSoins.ToString().ToUpper().Trim()
                && x.CodeActe.ToString().ToUpper().Trim() == y.CodeActe.ToString().ToUpper().Trim()
                && x.FraisReel.ToString().ToUpper().Trim() == y.FraisReel.ToString().ToUpper().Trim()
                && x.RembSS.ToString().ToUpper().Trim() == y.RembSS.ToString().ToUpper().Trim()
                && x.RembNous.ToString().ToUpper().Trim() == y.RembNous.ToString().ToUpper().Trim()
                && x.DatePayment.ToString().ToUpper().Trim() == y.DatePayment.ToString().ToUpper().Trim()
                && x.Beneficiaire.ToString().ToUpper().Trim() == y.Beneficiaire.ToString().ToUpper().Trim();
        }

        public override int GetHashCode(UK_PrestSante obj)
        {
            return obj == null ? 0 : (obj.ContractId.GetHashCode() ^ obj.Company.GetHashCode() ^ obj.CodeCol.GetHashCode()
                ^ obj.DateSoins.GetHashCode() ^ obj.CodeActe.GetHashCode() ^ obj.FraisReel.GetHashCode() ^ obj.RembSS.GetHashCode()
                ^ obj.RembNous.GetHashCode() ^ obj.DatePayment.GetHashCode() ^ obj.Beneficiaire.GetHashCode() );
        }
    }

    public class UK_DemoSante
    {
        //DemoSant: ContractId,Company,DateDemo,Age,Sexe,CodeCol,Lien
        public int RowNumber { get; set; }
        public int Id { get; set; }
        public string ContractId { get; set; }
        public string Company { get; set; }
        public DateTime DateDemo { get; set; }
        public int Age { get; set; }
        public string Sexe { get; set; }
        public string CodeCol { get; set; }
        public string Lien { get; set; }
    }

    public class DemoSanteComparer : EqualityComparer<UK_DemoSante>
    {
        public override bool Equals(UK_DemoSante x, UK_DemoSante y)
        {
            if (x == null || y == null)
                return x == y;
            //DemoSant: ContractId,Company,DateDemo,Age,Sexe,CodeCol,Lien
            return x.ContractId.ToUpper().Trim() == y.ContractId.ToUpper().Trim() && x.Company.ToUpper().Trim() == y.Company.ToUpper().Trim()
                && x.DateDemo.ToString().ToUpper().Trim() == y.DateDemo.ToString().ToUpper().Trim() 
                && x.Age.ToString().ToUpper().Trim() == y.Age.ToString().ToUpper().Trim()
                && x.Sexe.ToString().ToUpper().Trim() == y.Sexe.ToString().ToUpper().Trim()
                && x.CodeCol.ToString().ToUpper().Trim() == y.CodeCol.ToString().ToUpper().Trim()
                && x.Lien.ToString().ToUpper().Trim() == y.Lien.ToString().ToUpper().Trim();
        }

        public override int GetHashCode(UK_DemoSante obj)
        {
            return obj == null ? 0 : (obj.ContractId.GetHashCode() ^ obj.Company.GetHashCode() ^ obj.DateDemo.GetHashCode()
                ^ obj.Age.GetHashCode() ^ obj.Sexe.GetHashCode() ^ obj.CodeCol.GetHashCode()
                ^ obj.Lien.GetHashCode() );
        }
    }

    //********************* PREV ***************************************

    public class UK_CotisatPrev
    {
        //CotPrev: ContractId,Company,CodeCol,Year,CotisationBrute,CodeGarantie
        public int RowNumber { get; set; }
        public int Id { get; set; }
        public string ContractId { get; set; }
        public string Company { get; set; }
        public string CodeCol { get; set; }
        public int Year { get; set; }
        public double CotisationBrute { get; set; }
        public string CodeGarantie { get; set; }
    }

    public class CotisatPrevComparer : EqualityComparer<UK_CotisatPrev>
    {
        public override bool Equals(UK_CotisatPrev x, UK_CotisatPrev y)
        {
            if (x == null || y == null)
                return x == y;
            //CotPrev: ContractId,Company,CodeCol,Year,CotisationBrute,CodeGarantie

            return x.ContractId.ToString().ToUpper().Trim() == y.ContractId.ToString().ToUpper().Trim()
                && x.Company.ToString().ToUpper().Trim() == y.Company.ToString().ToUpper().Trim()
                && x.CodeCol.ToString().ToUpper().Trim() == y.CodeCol.ToString().ToUpper().Trim()
                && x.Year.ToString().ToUpper().Trim() == y.Year.ToString().ToUpper().Trim()
                && x.CotisationBrute.ToString().ToUpper().Trim() == y.CotisationBrute.ToString().ToUpper().Trim()
                && x.CodeGarantie.ToString().ToUpper().Trim() == y.CodeGarantie.ToString().ToUpper().Trim();
        }

        public override int GetHashCode(UK_CotisatPrev obj)
        {
            return obj == null ? 0 : (obj.ContractId.GetHashCode() ^ obj.Company.GetHashCode() ^ obj.CodeCol.GetHashCode()
                ^ obj.Year.GetHashCode() ^ obj.CotisationBrute.GetHashCode() ^ obj.CodeGarantie.GetHashCode() );
        }
    }

    public class UK_DecompPrev
    {
        //DecompPrev: ContractId,Company,Dossier,CodeCol,DateVirement,DateSin,DebSin,FinSin,Total,CauseSinistre        
        public int RowNumber { get; set; }
        public int Id { get; set; }
        public string ContractId { get; set; }
        public string Company { get; set; }
        public string Dossier { get; set; }
        public string CodeCol { get; set; }
        public DateTime DateVirement { get; set; }
        public DateTime DateSin { get; set; }
        public DateTime DebSin { get; set; }
        public DateTime FinSin { get; set; }
        public double Total { get; set; }
        public string CauseSinistre { get; set; }
    }

    public class DecompPrevComparer : EqualityComparer<UK_DecompPrev>
    {
        public override bool Equals(UK_DecompPrev x, UK_DecompPrev y)
        {
            if (x == null || y == null)
                return x == y;
            //DecompPrev: ContractId,Company,Dossier,CodeCol,DateVirement,DateSin,DebSin,FinSin,Total,CauseSinistre  

            return x.ContractId.ToString().ToUpper().Trim() == y.ContractId.ToString().ToUpper().Trim()
                && x.Company.ToString().ToUpper().Trim() == y.Company.ToString().ToUpper().Trim()
                && x.Dossier.ToString().ToUpper().Trim() == y.Dossier.ToString().ToUpper().Trim()
                && x.CodeCol.ToString().ToUpper().Trim() == y.CodeCol.ToString().ToUpper().Trim()
                && x.DateVirement.ToString().ToUpper().Trim() == y.DateVirement.ToString().ToUpper().Trim()
                && x.DateSin.ToString().ToUpper().Trim() == y.DateSin.ToString().ToUpper().Trim()
                && x.DebSin.ToString().ToUpper().Trim() == y.DebSin.ToString().ToUpper().Trim()
                && x.FinSin.ToString().ToUpper().Trim() == y.FinSin.ToString().ToUpper().Trim()
                && x.Total.ToString().ToUpper().Trim() == y.Total.ToString().ToUpper().Trim()
                && x.CauseSinistre.ToString().ToUpper().Trim() == y.CauseSinistre.ToString().ToUpper().Trim();
        }

        public override int GetHashCode(UK_DecompPrev obj)
        {
            return obj == null ? 0 : (obj.ContractId.GetHashCode() ^ obj.Company.GetHashCode() ^ obj.Dossier.GetHashCode()
                ^ obj.CodeCol.GetHashCode() ^ obj.DateVirement.GetHashCode() ^ obj.DateSin.GetHashCode() ^ obj.DebSin.GetHashCode()
                ^ obj.FinSin.GetHashCode() ^ obj.Total.GetHashCode() ^ obj.CauseSinistre.GetHashCode());
        }
    }

    public class UK_ProvPrev
    {
        //ProvPrev: ContractId,Company,Dossier,CodeCol,DateSinistre,NatureSinistre,Pm,PmPassage,Psap,PmMgdc,Psi,PmPortabilite        
        public int RowNumber { get; set; }
        public int Id { get; set; }
        public string ContractId { get; set; }
        public string Company { get; set; }
        public string Dossier { get; set; }
        public string CodeCol { get; set; }
        public DateTime DateSinistre { get; set; }
        public string NatureSinistre { get; set; }
        public double Pm { get; set; }
        public double PmPassage { get; set; }
        public double Psap { get; set; }
        public double PmMgdc { get; set; }
        public double Psi { get; set; }
        public double PmPortabilite { get; set; }
    }

    public class ProvPrevComparer : EqualityComparer<UK_ProvPrev>
    {
        public override bool Equals(UK_ProvPrev x, UK_ProvPrev y)
        {
            if (x == null || y == null)
                return x == y;
            //ProvPrev: ContractId,Company,Dossier,CodeCol,DateSinistre,NatureSinistre,Pm,PmPassage,Psap,PmMgdc,Psi,PmPortabilite  

            return x.ContractId.ToString().ToUpper().Trim() == y.ContractId.ToString().ToUpper().Trim()
                && x.Company.ToString().ToUpper().Trim() == y.Company.ToString().ToUpper().Trim()
                && x.Dossier.ToString().ToUpper().Trim() == y.Dossier.ToString().ToUpper().Trim()
                && x.CodeCol.ToString().ToUpper().Trim() == y.CodeCol.ToString().ToUpper().Trim()
                && x.DateSinistre.ToString().ToUpper().Trim() == y.DateSinistre.ToString().ToUpper().Trim()
                && x.NatureSinistre.ToString().ToUpper().Trim() == y.NatureSinistre.ToString().ToUpper().Trim()
                && x.Pm.ToString().ToUpper().Trim() == y.Pm.ToString().ToUpper().Trim()
                && x.PmPassage.ToString().ToUpper().Trim() == y.PmPassage.ToString().ToUpper().Trim()
                && x.Psap.ToString().ToUpper().Trim() == y.Psap.ToString().ToUpper().Trim()
                && x.PmMgdc.ToString().ToUpper().Trim() == y.PmMgdc.ToString().ToUpper().Trim()
                && x.Psi.ToString().ToUpper().Trim() == y.Psi.ToString().ToUpper().Trim()
                && x.PmPortabilite.ToString().ToUpper().Trim() == y.PmPortabilite.ToString().ToUpper().Trim();
        }

        public override int GetHashCode(UK_ProvPrev obj)
        {
            return obj == null ? 0 : (obj.ContractId.GetHashCode() ^ obj.Company.GetHashCode() ^ obj.Dossier.GetHashCode() ^ obj.CodeCol.GetHashCode()
                ^ obj.DateSinistre.GetHashCode() ^ obj.NatureSinistre.GetHashCode() ^ obj.Pm.GetHashCode() ^ obj.PmPassage.GetHashCode()
                ^ obj.Psap.GetHashCode() ^ obj.PmMgdc.GetHashCode() ^ obj.Psi.GetHashCode() ^ obj.PmPortabilite.GetHashCode() );
        }
    }

    public class UK_SinistrePrev
    {
        //SinistrPrev: ContractId,Company,Dossier,CodeCol,Birthdate,DateSinistre,NatureSinistre
        public int RowNumber { get; set; }
        public int Id { get; set; }
        public string ContractId { get; set; }
        public string Company { get; set; }
        public string Dossier { get; set; }
        public string CodeCol { get; set; }
        public DateTime Birthdate { get; set; }
        public DateTime DateSinistre { get; set; }
        public string NatureSinistre { get; set; }       
    }

    public class SinistrePrevComparer : EqualityComparer<UK_SinistrePrev>
    {
        public override bool Equals(UK_SinistrePrev x, UK_SinistrePrev y)
        {
            if (x == null || y == null)
                return x == y;
            //SinistrPrev: ContractId,Company,Dossier,CodeCol,Birthdate,DateSinistre,NatureSinistre

            return x.ContractId.ToString().ToUpper().Trim() == y.ContractId.ToString().ToUpper().Trim()
                && x.Company.ToString().ToUpper().Trim() == y.Company.ToString().ToUpper().Trim()
                && x.Dossier.ToString().ToUpper().Trim() == y.Dossier.ToString().ToUpper().Trim()
                && x.CodeCol.ToString().ToUpper().Trim() == y.CodeCol.ToString().ToUpper().Trim()
                && x.Birthdate.ToString().ToUpper().Trim() == y.Birthdate.ToString().ToUpper().Trim()
                && x.DateSinistre.ToString().ToUpper().Trim() == y.DateSinistre.ToString().ToUpper().Trim()
                && x.NatureSinistre.ToString().ToUpper().Trim() == y.NatureSinistre.ToString().ToUpper().Trim();
        }

        public override int GetHashCode(UK_SinistrePrev obj)
        {
            return obj == null ? 0 : (obj.ContractId.GetHashCode() ^ obj.Company.GetHashCode() ^ obj.Dossier.GetHashCode()
                ^ obj.CodeCol.GetHashCode() ^ obj.Birthdate.GetHashCode() ^ obj.DateSinistre.GetHashCode() ^ obj.NatureSinistre.GetHashCode() );
        }
    }
}
