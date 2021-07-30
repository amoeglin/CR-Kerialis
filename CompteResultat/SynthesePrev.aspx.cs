using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CompteResultat.BL;
using CompteResultat.DAL;
using CompteResultat.Common;
using System.Web.Configuration;
using System.Data;
using System.Globalization;

namespace CompteResultat
{
    public partial class SynthesePrev : System.Web.UI.Page
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {  
                HttpCookie cookie = Request.Cookies["txtNumberEnt"];
                string numberEntCookieVal = cookie != null ? cookie.Value.Split('=')[1] : "";
                if (txtNumberEnt.Text == "")
                    txtNumberEnt.Text = numberEntCookieVal != "" ? numberEntCookieVal : WebConfigurationManager.AppSettings["SyntheseNumberofCompaniesToDisplay"];

                //get date values
                cookie = Request.Cookies["txtStartPeriode"];
                string startPeriodeCookieVal = cookie != null ? cookie.Value.Split('=')[1] : "";
                if (txtStartPeriode.Text == "")
                    txtStartPeriode.Text = startPeriodeCookieVal != "" ? startPeriodeCookieVal : "01/01/2020";

                cookie = Request.Cookies["txtEndPeriode"];
                string endPeriodeCookieVal = cookie != null ? cookie.Value.Split('=')[1] : "";
                if (txtEndPeriode.Text == "")
                    txtEndPeriode.Text = endPeriodeCookieVal != "" ? endPeriodeCookieVal : "2020-01-01";

                cookie = Request.Cookies["txtArretCompte"];
                string arretCompteCookieVal = cookie != null ? cookie.Value.Split('=')[1] : "";
                if (txtArretCompte.Text == "")
                    txtArretCompte.Text = arretCompteCookieVal != "" ? arretCompteCookieVal : "2020-01-01";

                lblHeaderSynthese.Text = txtNumberEnt.Text + " Comptes de resultats sante avec les prestations triees par ordre decroissant :";

                if (!IsPostBack)
                {                    
                }
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "SyntheseSante::Page_Load"); }
        }

        public IEnumerable<Synthese> GetSynthese()
        {
            return null;
            //string assurName = "_entreprise";

            //try
            //{
            //    if (cmbAssureur.SelectedItem != null)
            //        assurName = cmbAssureur.SelectedItem.Text.ToString();

            //    List<ExcelGlobalPrestaData> globalPresta = new List<ExcelGlobalPrestaData>();
            //    List<ExcelGlobalPrestaData> globalCotisatCumul = new List<ExcelGlobalPrestaData>();
            //    DateTime debutPeriode = DateTime.Parse(txtStartPeriode.Text);
            //    DateTime finPeriode = DateTime.Parse(txtEndPeriode.Text);
            //    DateTime dateArret = DateTime.Parse(txtArretCompte.Text);                
            //    C.eReportTypes reportType = C.eReportTypes.GlobalSynthese;
            //    int numberTopPerteLoss = int.Parse(txtNumberEnt.Text);

            //    DataTable syntheseTable = new DataTable();
            //    DataView dv;
            //    if (assurName.ToLower().Contains("_entreprise"))
            //    {
            //        syntheseTable = ExcelSheetHandler.GetSyntheseTable(debutPeriode, finPeriode, dateArret, reportType, false, true);                    
            //    }
            //    else
            //    {
            //        syntheseTable = ExcelSheetHandler.GetSyntheseTable(debutPeriode, finPeriode, dateArret, reportType, true, true);
            //    }

            //    if (syntheseTable.Rows.Count != 0)
            //    {
            //        dv = syntheseTable.AsDataView();

            //        List<Synthese> synths = GetSynths(dv, numberTopPerteLoss, "RNous desc");

            //        return synths;
            //    }
            //    else
            //        return null;
            //}
            //catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "SyntheseSante::GetSynthese"); return null; }
        }

        private List<Synthese> GetSynths(DataView dv, int numberTopPerteLoss, string sort)
        {
            List<Synthese> synths = new List<Synthese>();

            dv.Sort = sort;
            int cnt = 1;
            var nfi = new NumberFormatInfo { NumberGroupSeparator = " " };

            foreach (DataRowView rowView in dv)
            {
                if (cnt > numberTopPerteLoss)
                    break;

                DataRow row = rowView.Row;

                double dRatio = double.Parse(row["Ratio"].ToString()) * 100;
                double dChargement = double.Parse(row["TauxChargement"].ToString()) * 100;

                //string gainLoss = double.Parse(row["GainLoss"].ToString()).ToString("0.##") + " €";

                synths.Add(new Synthese
                {
                    Assur = row["Assureur"].ToString(),
                    Company = row["Company"].ToString(),
                    Annee = int.Parse(row["YearSurv"].ToString()),
                    Prestations = double.Parse(row["RNous"].ToString()).ToString("#,0", nfi) + " €", // row["RNous"].ToString() + " €",
                    Provisions = double.Parse(row["Provisions"].ToString()).ToString("#,0", nfi) + " €",  //row["Provisions"].ToString() + " €",
                    CotBrut = double.Parse(row["CotBrut"].ToString()).ToString("#,0", nfi) + " €",   //row["CotBrut"].ToString() + " €",
                    Chargements = dChargement.ToString() + " %",
                    CotNet = double.Parse(row["CotNet"].ToString()).ToString("#,0", nfi) + " €",  //  row["CotNet"].ToString() + " €",
                    Ratio = dRatio.ToString() + " %",
                    GainLoss = double.Parse(row["GainLoss"].ToString()).ToString("#,0", nfi) + " €",
                    CoeffProv = row["CoeffCad"].ToString() ,
                    FR = double.Parse(row["FR"].ToString()).ToString("#,0", nfi) + " €",  //         row["FR"].ToString() + " €",
                    RSS = double.Parse(row["RSS"].ToString()).ToString("#,0", nfi) + " €", // row["RSS"].ToString() + " €",
                    RAnnexe = double.Parse(row["RAnnexe"].ToString()).ToString("#,0", nfi) + " €",  //     row["RAnnexe"].ToString() + " €",
                    DateArrete = DateTime.Parse(row["DateArret"].ToString()).ToShortDateString()
                });

                cnt++;
            }

            return synths;
        }

        protected void cmbAssureur_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetSynthese();
        }

        protected void txtNumberEnt_TextChanged(object sender, EventArgs e)
        {           
            //validate input for txtNumberEnt.Text
            lblNumbEntWarning.Visible = false;
            int numberEnt;
            bool success = Int32.TryParse(txtNumberEnt.Text, out numberEnt);
            if (success)
            {
                HttpCookie cookie = new HttpCookie("txtNumberEnt");
                cookie.Values["txtNumberEnt"] = txtNumberEnt.Text;
                Response.Cookies.Add(cookie);

                GetSynthese();
            }
            else
            {
                //display warning                
                lblNumbEntWarning.Text = "Please anter an integer value !";
                lblNumbEntWarning.Visible = true;
            }
        }

        protected void rptSynthese_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Repeater rpt = sender as Repeater; 
            
            // If the Repeater contains no data.
            if (rpt != null)
            {
                if (e.Item.ItemType == ListItemType.Footer)
                {                    
                    if (rpt.Items.Count < 1)
                    {
                        rptSynthese.Visible = false;
                        phHeader.Visible = true;
                    }
                    else
                    {
                        rptSynthese.Visible = true;
                        phHeader.Visible = false;
                    }
                }
            }
        }

        public List<Assureur> GetAssureurs()
        {
            try
            {
                List<Assureur> assur;
                assur = Assureur.GetAllAssureurs();
                return assur;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);

                var myCustomValidator = new CustomValidator();
                myCustomValidator.IsValid = false;
                myCustomValidator.ErrorMessage = ex.Message;
                Page.Validators.Add(myCustomValidator);

                return null;
            }
        }

        private void CadencierIsUpToDate()
        {
            List<string> assureurs = Assureur.GetAllAssureurs().Select(x => x.Name).Distinct().ToList();
            List<Cadencier> cadencierAll = new List<Cadencier>();
            List<Cadencier> cadencierForAssureur = new List<Cadencier>();
            cadencierAll = Cadencier.GetCadencierForAssureur(C.cDEFAULTASSUREUR);

            foreach (string assurName in assureurs)
            {
                if (assurName != C.cDEFAULTASSUREUR)
                {
                    cadencierForAssureur = Cadencier.GetCadencierForAssureur(assurName);
                    cadencierAll.AddRange(cadencierForAssureur);
                }
            }

            DateTime debutPeriode = DateTime.Parse(txtStartPeriode.Text);
            DateTime finPeriode = DateTime.Parse(txtEndPeriode.Text);
            List<int> years = new List<int>();
            for (int i = 0; i <= finPeriode.Year - debutPeriode.Year; i++)
            {
                years.Add(debutPeriode.Year + i);
            }

            List<int> missingYears = new List<int>();
            bool cadExists = true;
            foreach (int year in years)
            {
                var res = cadencierAll.Where(c => c.Year == year);
                if (!res.Any())
                {
                    missingYears.Add(year);
                    cadExists = false;
                }
            }

            lblCadencierWarning.Visible = false;
            if (!cadExists)
            {
                string strYears = string.Join(", ", missingYears);
                lblCadencierWarning.Text = "Attention, le cadencier n’est pas à jour pour les année(s) : " + strYears + " !";
                lblCadencierWarning.Visible = true;

            }
        }

        private void SaveParams()
        {
            //save dates
            if (txtStartPeriode.Text != "")
            {
                HttpCookie cookie = new HttpCookie("txtStartPeriode");
                cookie.Values["txtStartPeriode"] = txtStartPeriode.Text;
                Response.Cookies.Add(cookie);
            }
            if (txtEndPeriode.Text != "")
            {
                HttpCookie cookie = new HttpCookie("txtEndPeriode");
                cookie.Values["txtEndPeriode"] = txtEndPeriode.Text;
                Response.Cookies.Add(cookie);
            }
            if (txtArretCompte.Text != "")
            {
                HttpCookie cookie = new HttpCookie("txtArretCompte");
                cookie.Values["txtArretCompte"] = txtArretCompte.Text;
                Response.Cookies.Add(cookie);
            }
        }

        protected void cmdcreate_Click(object sender, EventArgs e)
        {
            SaveParams();

            //verify if Cadencier is up to date
            //List<int> missingYears = new List<int>();
            //bool cadUpToDate = BLCadencier.CadencierIsUpToDate(ref missingYears, txtStartPeriode.Text, txtEndPeriode.Text);

            //lblCadencierWarning.Visible = false;
            //if (!cadUpToDate)
            //{
            //    string strYears = string.Join(", ", missingYears);
            //    lblCadencierWarning.Text = "Attention, le cadencier n’est pas à jour pour les année(s) : " + strYears + " !";
            //    lblCadencierWarning.Visible = true;
            //}

            //define all required variables
            var selectedAssName = cmbAssureur.SelectedItem.Text;
            var selectedAssId = cmbAssureur.SelectedItem.Value;

            string reportName = "";
            string dateTimeToday = DateTime.Now.ToString("s").Replace(":", "-");
            BLCompteResultat myCR = null; 
            List<string> assurNames = new List<string>();

            // SYNTHESE => generate 2 types of datasets: GLOBAL SOCIETE PRODUIT && GLOBAL SOCIETE ENTERPRISE 
            if (chkSynthese.Checked)
            {
                //GLOBAL SOCIETE PRODUIT
                //reportName = "SYNTHESE_SANTE_" + dateTimeToday;
                //C.eReportTypes repType = C.eReportTypes.GlobalSynthese;
                //C.eReportTemplateTypes repTemplate = C.eReportTemplateTypes.SANTE_SYNT;
                //assurNames = Assureur.GetEnterpriseAssNamesByType(C.cASSTYPEPRODUCT);

                //myCR = SetCRDetails(repType, repTemplate, reportName);
                //SetGenericCRParams(ref myCR, assurNames);

                //myCR.CreateNewCompteResultat(true);
            }
            
            // GLOBAL SOCIETE ENTERPRISE => get all assureurs that end with _ENTREPRISE => get all Comps & Subsids for those Assur
            if (chkGlobalEnt.Checked)
            {
                reportName = "_PREV_GLOBAL_SOCIETE_ENTREPRISE_" + dateTimeToday;
                C.eReportTypes repType = C.eReportTypes.GlobalEnt;
                C.eReportTemplateTypes repTemplate = C.eReportTemplateTypes.PREV_GLOBAL;
                assurNames = Assureur.GetEnterpriseAssNamesByType(C.cASSTYPEENTERPRISEPREV);

                myCR = SetCRDetails(repType, repTemplate, reportName);
                SetGenericCRParams(ref myCR, assurNames);
                myCR.CreateNewCompteResultat(true);
            }

        }

        private void SetGenericCRParams(ref BLCompteResultat myCR, List<string> assurNames)
        {
            List<string> assurIds = new List<string>();
            List<string> compIds = new List<string>();
            List<string> compNames = new List<string>();
            List<string> subsidIds = new List<string>();
            List<string> subsidNames = new List<string>();
            List<string> contractIds = new List<string>();
            List<string> contractNames = new List<string>();

            foreach (string ass in assurNames)
            {
                int myAssId = Assureur.GetAssIdForAssName(ass);
                assurIds.Add(myAssId.ToString());
                
                List<Contract> myContracts = Assureur.GetAllContractsForAssureur(myAssId);
                foreach(Contract contr in myContracts)
                {
                    contractNames.Add(contr.ContractId);
                    contractIds.Add(contr.Id.ToString());
                }            
            }            

            //get all companies & subsids
            foreach (string assId in assurIds)
            {
                int myassurId = int.Parse(assId);
                compIds.AddRange(Company.GetParentCompanyIdsForAssureurId(myassurId));
                compNames.AddRange(Company.GetParentCompanyNamesForAssureurId(myassurId));
                subsidIds.AddRange(Company.GetSubsidIdsForAssureurId(myassurId));
                subsidNames.AddRange(Company.GetSubsidNamesForAssureurId(myassurId));
            }            

            myCR.AssurIds = string.Join(C.cVALSEP, assurIds);
            myCR.AssurNames = string.Join(C.cVALSEP, assurNames);
            myCR.ParentCompanyNames = string.Join(C.cVALSEP, compNames);
            myCR.ParentCompanyIds = string.Join(C.cVALSEP, compIds);
            myCR.SubsidIds = string.Join(C.cVALSEP, subsidIds);
            myCR.SubsidNames = string.Join(C.cVALSEP, subsidNames);
            myCR.ContractIds = string.Join(C.cVALSEP, contractIds);
            myCR.ContractNames = string.Join(C.cVALSEP, contractNames);
        }

        public BLCompteResultat SetCRDetails(C.eReportTypes repType, C.eReportTemplateTypes templateType, string reportName)
        {
            BLCompteResultat myCR = new BLCompteResultat();
            
            int reportLevelId = ReportTemplate.GetTemplateIdForType(templateType.ToString()); 
            
            myCR.Name = reportName;                           
            myCR.CRPlannings.Add(new CRPlanning
            {
                DebutPeriode = DateTime.Parse(txtStartPeriode.Text),
                FinPeriode = DateTime.Parse(txtEndPeriode.Text),
                DateArret = DateTime.Parse(txtArretCompte.Text)
            });

            myCR.ReportType = repType;
            myCR.ReportLevelId = reportLevelId;
            myCR.UserName = User.Identity.Name;
            myCR.IsActive = true;
            myCR.IsAutoGenerated = false;
            myCR.CreationDate = DateTime.Now.Date;

            double tax = 0.0;
            myCR.TaxDef = tax;
            myCR.TaxAct = tax;
            myCR.TaxPer = tax;
            myCR.NumberTopPerteLoss = int.Parse(txtNumberEnt.Text);

            //we also add the server path
            myCR.ExcelTemplatePath = Server.MapPath(C.reportTemplateFolder);
            myCR.ExcelCRPath = Server.MapPath(C.excelCRFolder);
            myCR.CalculateProvision = chkCalcProv.Checked;

            return myCR;
        }

        protected void txtStartPeriode_TextChanged(object sender, EventArgs e)
        {
            //SaveParams();
        }

        protected void txtEndPeriode_TextChanged(object sender, EventArgs e)
        {
            //SaveParams();
        }

        protected void txtArretCompte_TextChanged(object sender, EventArgs e)
        {
            //SaveParams();
        }
    }
}