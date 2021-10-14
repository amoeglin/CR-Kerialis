using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Web.Configuration;
using System.Threading;

using CompteResultat.BL;
using CompteResultat.DAL;
using CompteResultat.Common;
using System.Web.DynamicData;
using System.Web.ModelBinding;

//test
//test 2
//test 3
namespace CompteResultat
{
    
    public partial class CompteResultatManuel : System.Web.UI.Page
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {              
            try
            { 
                radioReportType.Items[0].Enabled = false;

                tvContracts.Enabled = true;
                tvContracts.Attributes.Add("onclick", "postBackByObject()");

                PrepareUI();

                if (!IsPostBack)
                {
                    PopulateTreeViewControl(C.cINVALIDID);

                    //get date values
                    HttpCookie cookie = Request.Cookies["txtStartPeriode"];
                    string startPeriodeCookieVal = cookie != null ? cookie.Value.Split('=')[1] : "";
                    if (txtStartPeriode.Text == "")
                        txtStartPeriode.Text = startPeriodeCookieVal != "" ? startPeriodeCookieVal : "01/01/2020";

                    cookie = Request.Cookies["txtEndPeriode"];
                    string endPeriodeCookieVal = cookie != null ? cookie.Value.Split('=')[1] : "";
                    if (txtEndPeriode.Text == "")
                        txtEndPeriode.Text = endPeriodeCookieVal != "" ? endPeriodeCookieVal : "01/01/2020";

                    cookie = Request.Cookies["txtArretCompte"];
                    string arretCompteCookieVal = cookie != null ? cookie.Value.Split('=')[1] : "";
                    if (txtArretCompte.Text == "")
                        txtArretCompte.Text = arretCompteCookieVal != "" ? arretCompteCookieVal : "01/01/2020";

                    cookie = Request.Cookies["typeCompte"];
                    string typeCompteCookieVal = cookie != null ? cookie.Value.Split('=')[1] : "";
                    int iTypeCompteVal = 0;
                    if (int.TryParse(typeCompteCookieVal, out iTypeCompteVal))
                        radioTypeComptes.SelectedIndex = iTypeCompteVal;
                    else
                        radioTypeComptes.SelectedIndex = 0;
                }                             
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::Page_Load"); }
        }

        private void PrepareUI()
        {  
            if (radioReportType.SelectedIndex == 0)
            {
                imgReport.ImageUrl = C.imageRelFolder + "report1.png";
                chkSelectAll.Visible = false;
                chkPrev.Visible = false;
                //cmbDetailReport.Enabled = true;
            }
            else
            {
                chkSelectAll.Visible = true;
                chkPrev.Visible = true;
                //cmbDetailReport.Enabled = false;

                if (radioReportType.SelectedIndex == 1)
                    imgReport.ImageUrl = C.imageRelFolder + "report2.png";
                else
                    imgReport.ImageUrl = C.imageRelFolder + "report3.png";
            }
        }     

        protected void Page_PreRenderComplete(object src, EventArgs args)
        {
            ShowHideCalcProv();
        }

        private void PopulateTreeViewControl(int crId)
        {
            //### this method needs to be optimized - there are too many SQL requests - load everything you need : Assur + Contr + Comp 
            // in memory table - maybe use stored proc with joins

            try
            {
                //get all assureurs
                List<Assureur> assur = Assureur.GetAllAssureurs();
                TreeViewTag tv = new TreeViewTag();
                bool CRNodeSelected = false;

                tvContracts.Nodes.Clear();

                foreach (Assureur myAssur in assur)
                {
                    //store the list of contracts for the current assureur
                    List<string> contractNamesForAssur;

                    //### - don't load contracts at this stage
                    //contractNamesForAssur = Assureur.GetAllContractNamesForAssureur(myAssur.Id);

                    // *** Create Assur Node
                    tv = new TreeViewTag();
                    tv.Name = myAssur.Name;
                    tv.Id = myAssur.Id;
                    tv.AssureurId = myAssur.Id;
                    tv.NodeType = C.eTVNodeTypes.Assureur;

                    TreeNode assurNode = new TreeNode(myAssur.Name, tv.GetStringFromObject());
                    assurNode.SelectAction = TreeNodeSelectAction.Select;
                    assurNode.ImageUrl = C.imageRelFolder + "assur.png";
                    //assurNode.ShowCheckBox = true;


                    tvContracts.Nodes.Add(assurNode);
                    assurNode.Expand();
                }
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::PopulateTreeViewControl"); }
        }

        protected void tvContracts_SelectedNodeChanged(object sender, EventArgs e)
        {
            try
            { 
                taxDef.Value = WebConfigurationManager.AppSettings["DefaultTax"];

                //dispaly details of the selected node  
                string nodeVal = tvContracts.SelectedNode.Value;
                TreeViewTag tv = TreeViewTag.GetObjectFromString(nodeVal);
                TreeNode selectedNode =  tvContracts.SelectedNode;
                int assurId = tv.AssureurId;

                Session["SelectedCompteResultatNode"] = tv;
                
                string detail = "Nom : " + tv.Name + Environment.NewLine + Environment.NewLine;

                #region NODETYPE == CompteResultat

                if (tv.NodeType == C.eTVNodeTypes.CompteResultat)
                {
                    //### fix this temporary solution
                    //in certain scenarios various props may not be filled - leave this routine if that's the case
                    if (tv.CRCompanyIds == null)
                        return;

                    //fill textboxes with values from first planning
                    if (tv.CRPs.Count > 0)
                    {
                        txtStartPeriode.Text = tv.CRPs[0].DebutPeriode.Value.ToString(C.cISODATE);
                        //txtStartPeriode2.Value = tv.CRPs[0].DebutPeriode.Value.ToString(C.cISODATE);
                        txtEndPeriode.Text = tv.CRPs[0].FinPeriode.Value.ToString(C.cISODATE);
                        txtArretCompte.Text = tv.CRPs[0].DateArret.Value.ToString(C.cISODATE);                            
                    }

                    cmbCollege.SelectedValue = tv.CRCollegeId.Value.ToString();
                    cmbDetailReport.SelectedValue = tv.CRReportLevelId.Value.ToString();
                    txtNameReport.Value = tv.Name;

                    if (tv.CRReportType.HasValue)
                    { 
                        if (tv.CRReportType == C.eReportTypes.Standard)                        
                            radioReportType.SelectedIndex = 0;
                        
                        else if (tv.CRReportType == C.eReportTypes.GlobalEnt)                        
                            radioReportType.SelectedIndex = 1;
                        
                        else                        
                            radioReportType.SelectedIndex = 2;                        
                    }
                    else                    
                        radioReportType.SelectedIndex = 0;
                   

                    PrepareUI();


                    if (tv.TaxDef.HasValue)
                        taxDef.Value = tv.TaxDef.Value.ToString();
                    else
                        taxDef.Value = WebConfigurationManager.AppSettings["DefaultTax"];

                    if (tv.TaxAct.HasValue)
                        taxAct.Value = tv.TaxAct.Value.ToString();
                    else
                        taxAct.Value = "";

                    if (tv.TaxPer.HasValue)
                        taxPer.Value = tv.TaxPer.Value.ToString();
                    else
                        taxPer.Value = "";

                    ShowHideCalcProv();

                    //cmdDeleteCR.Visible = true;
                    //cmdStartExcel.Visible = true;
                    //cmdStartPPT.Visible = true;

                    detail += "Entreprise(s) : " + Company.GetCompanyNamesFromIdList(tv.CRCompanyIds) + Environment.NewLine + Environment.NewLine;
                    if (tv.CRSubsids != null)
                        detail += "Filiales : " + Company.GetCompanyNamesFromIdList(tv.CRSubsids) + Environment.NewLine + Environment.NewLine;
                    if (tv.CRContractIds != null && radioReportType.SelectedIndex == 0)
                        detail += "Contrats : " + Contract.GetContractNamesFromIdList(tv.CRContractIds) + Environment.NewLine + Environment.NewLine;

                    //if(tv.CRCollegeId.HasValue)
                    //    detail += "Collège : " + College.GetCollegeNameForId(tv.CRCollegeId.Value) + Environment.NewLine + Environment.NewLine;
                    if (tv.CRReportLevelId.HasValue && radioReportType.SelectedIndex == 0)
                        detail += "Modelé rapport : " + ReportTemplate.GetTemplateNameForId(tv.CRReportLevelId.Value) + Environment.NewLine + Environment.NewLine;

                    if (tv.CRCreationDate.HasValue)
                        detail += "Date création : " + tv.CRCreationDate.Value.ToShortDateString() + Environment.NewLine + Environment.NewLine;
                    //detail += "Entreprise : " + tv.CRParentCompId + Environment.NewLine + Environment.NewLine;

                    //Planning details
                    if (tv.CRPs.Count > 0)
                    {
                        detail += "Détail planning : " + Environment.NewLine;
                        detail += "============ " + Environment.NewLine + Environment.NewLine;
                        int cnt = 1;
                        foreach (CRPlanning crp in tv.CRPs)
                        {
                            detail += "Planning No. " + cnt.ToString() + " : " + Environment.NewLine + Environment.NewLine;
                            if (crp.DebutPeriode.HasValue)
                                detail += "Début période : " + crp.DebutPeriode.Value.ToShortDateString() + Environment.NewLine + Environment.NewLine;
                            if (crp.FinPeriode.HasValue)
                                detail += "Fin période : " + crp.FinPeriode.Value.ToShortDateString() + Environment.NewLine + Environment.NewLine;
                            if (crp.DateArret.HasValue)
                                detail += "Date arrêté : " + crp.DateArret.Value.ToShortDateString() + Environment.NewLine + Environment.NewLine;
                        }
                    }

                    txtDetails.Text = detail;
                    return;
                }

                #endregion

                #region NODETYPE == Assureur

                if (tv.NodeType == C.eTVNodeTypes.Assureur)
                {
                    //clear all child nodes
                    selectedNode.ChildNodes.Clear();
                    tvContracts.Enabled = false;

                    //get all CR nodes and parent company nodes
                    LoadAllCRNodes(selectedNode, tv, -1, assurId, detail);

                    //display parent companies underneath the assureur
                    List<Company> parentCompaniesForAssur = Company.GetParentCompaniesForAssureurId(assurId);
                    foreach (Company parentComp in parentCompaniesForAssur)
                    {
                        // *** Create Parent Comp Node
                        tv = new TreeViewTag();
                        tv.Name = parentComp.Name; // parentComp.Name.TrimEnd('_');
                        tv.Id = parentComp.Id;
                        tv.AssureurId = assurId;
                        tv.NodeType = C.eTVNodeTypes.ParentCompany;

                        //TreeNode parentCompNode = new TreeNode(parentComp.Name.TrimEnd('_'), tv.GetStringFromObject());
                        TreeNode parentCompNode = new TreeNode(parentComp.Name, tv.GetStringFromObject());
                        parentCompNode.SelectAction = TreeNodeSelectAction.Select;
                        parentCompNode.ImageUrl = C.imageRelFolder + "company.png";
                        parentCompNode.ShowCheckBox = true;

                        selectedNode.ChildNodes.Add(parentCompNode);
                        parentCompNode.Expand();
                    }

                    selectedNode.Expand();
                    txtDetails.Text = detail;
                    tvContracts.Enabled = true;
                    return;
                }

                #endregion

                #region NODETYPE == ParentCompany

                if (tv.NodeType == C.eTVNodeTypes.ParentCompany)
                { 
                    int parentCompanyId = tv.Id;

                    //get all CR nodes and subsid nodes
                    LoadAllCRNodes(selectedNode, tv, parentCompanyId, assurId, detail);                    
                    LoadAllSubsidNodes(selectedNode, tv, assurId, detail, true, false);
                }

                #endregion

                #region NODETYPE == ChildCompany

                if (tv.NodeType == C.eTVNodeTypes.Subsid)
                {
                    LoadAllContractNodes(selectedNode, tv, assurId, detail, false);
                }

                #endregion                

            }
            catch (Exception ex) { tvContracts.Enabled = true; UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::tvContracts_SelectedNodeChanged"); }
        }

        protected void tvContracts_TreeNodeCheckChanged(object sender, TreeNodeEventArgs e)
        {
            try
            {
                //if a parent company is selected, all dependant subsids & contracts are selected            
                string nodeVal = e.Node.Value;
                TreeViewTag tv = TreeViewTag.GetObjectFromString(nodeVal);
                bool nodeChecked = e.Node.Checked;
                int assurId = tv.AssureurId;
                TreeNode selectedNode = e.Node;

                string detail = "Nom : " + tv.Name + Environment.NewLine + Environment.NewLine;
                int parentCompanyId = tv.Id;
                Session["SelectedCompteResultatNode"] = tv;

                LoadAllCRNodes(selectedNode, tv, parentCompanyId, assurId, detail);
                LoadAllSubsidNodes(selectedNode, tv, assurId, detail, true, false);

                if (tv.NodeType == C.eTVNodeTypes.Assureur)
                {
                    //###load all child & contract nodes
                    //if (nodeChecked)
                    //    LoadAllSubsidNodes(selectedNode, tv, assurId, detail, true);

                    foreach (TreeNode parentNode in e.Node.ChildNodes)
                    {
                        if (parentNode.ShowCheckBox != null && parentNode.ShowCheckBox != false)
                        {
                            parentNode.Checked = nodeChecked;

                            foreach (TreeNode subsidNode in parentNode.ChildNodes)
                            {
                                if (subsidNode.ShowCheckBox != null && subsidNode.ShowCheckBox != false)
                                {
                                    subsidNode.Checked = nodeChecked;

                                    foreach (TreeNode contr in subsidNode.ChildNodes)
                                    {
                                        contr.Checked = nodeChecked;
                                    }
                                }
                            }
                        }
                    }
                }

                if (tv.NodeType == C.eTVNodeTypes.ParentCompany)
                {
                    //###load all child & contract nodes
                    //if (nodeChecked)
                    //    LoadAllSubsidNodes(selectedNode, tv, assurId, detail, true);


                    foreach (TreeNode subsidNode in e.Node.ChildNodes)
                    {
                        if (subsidNode.ShowCheckBox != null && subsidNode.ShowCheckBox != false)
                        {
                            subsidNode.Checked = nodeChecked;

                            foreach (TreeNode contr in subsidNode.ChildNodes)
                            {
                                contr.Checked = nodeChecked;
                            }
                        }
                    }
                }
                //if we check a subsid node, make sure that the corresponding parent node is checked as well
                else if (tv.NodeType == C.eTVNodeTypes.Subsid)
                {
                    e.Node.Parent.Checked = nodeChecked ? true : true;
                    //check all contracts
                    foreach (TreeNode contrNode in e.Node.ChildNodes)
                    {
                        contrNode.Checked = nodeChecked;
                    }
                }
                //if we check a contract node, make sure that the corresponding subsid node is checked as well
                else if (tv.NodeType == C.eTVNodeTypes.Contract)
                {
                    e.Node.Parent.Checked = nodeChecked ? true : true;
                    //check also the PC node
                    e.Node.Parent.Parent.Checked = nodeChecked ? true : true;

                }
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "tvContracts_TreeNodeCheckChanged"); }
        }
        
        private void LoadAllSubsidNodes(TreeNode selectedNode, TreeViewTag tv, int assurId, string detail, bool includeContracts, bool checkNodes)
        {
            try
            {
                //clear all child nodes
                //selectedNode.ChildNodes.Clear();

                int parentCompanyId = tv.Id;

                // GET Child Companies for Parent Company

                List<string> contractNamesForAssur = Assureur.GetAllContractNamesForAssureur(assurId);
                List<Company> childComps = Company.GetChildCompanies(parentCompanyId);

                //### if childComps.count > 0 :: retrieve a list of all contractNames + ContractID && retrieve a second list of all contractID + companyId
                // from those 2 lists, create a list of all ContractName + CompanyId
                //use this to replace the following code line below :: List<string> contractsForComp = Company.GetContractsNamesForCompany(childComp.Id);
                // or, execute an sqlCommand that returns the required data using a join of 2 tables

                foreach (Company childComp in childComps)
                {
                    //### optimize code - reduce DB requests
                    // get all contracts for this child comp => is there at least one contract that belongs to assur - otherwise quit loop                            
                    List<string> contractsForComp = Company.GetContractsNamesForCompany(childComp.Id);

                    int cnt = 0;
                    if (contractsForComp != null)
                    {
                        List<string> contractsThatBelongToAssur = contractsForComp.Where(i => contractNamesForAssur.Contains(i)).ToList();
                        cnt = contractsThatBelongToAssur.Count();

                        if (cnt == 0)
                            continue;
                    }
                    else
                        continue;

                    // *** Create Subsid Node
                    tv = new TreeViewTag();
                    tv.Name = childComp.Name;
                    tv.Id = childComp.Id;
                    tv.ParentCompId = childComp.ParentId;
                    tv.NodeType = C.eTVNodeTypes.Subsid;
                    tv.AssureurId = assurId;

                    TreeNode childCompNode = new TreeNode(childComp.Name, tv.GetStringFromObject());
                    childCompNode.SelectAction = TreeNodeSelectAction.Select;
                    childCompNode.ImageUrl = C.imageRelFolder + "subsid.png";
                    childCompNode.ShowCheckBox = true;

                    if (checkNodes == true)
                        childCompNode.Checked = true;

                    if (!NodeWithTextExistsAlready(selectedNode, childCompNode.Text))
                        selectedNode.ChildNodes.Add(childCompNode);

                    if (includeContracts && cnt>0)
                    {
                        LoadAllContractNodes(childCompNode, tv, assurId, detail, checkNodes);
                        childCompNode.Expand();
                    }                    

                    // NOTE: we don't display CR's underneath a subsid => all CR's are displayed under the corresponding Parent Company
                }

                //selectedNode.Collapse();
                selectedNode.ExpandAll();
                txtDetails.Text = detail;
                return;
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "LoadAllSubsidNodes"); }
        }

        private void LoadAllContractNodes(TreeNode selectedNode, TreeViewTag tv, int assurId, string detail, bool checkNodes)
        {
            try
            {
                //clear all child nodes
                //selectedNode.ChildNodes.Clear();

                int childCompanyId = tv.Id;

                //display contracts underneath the subsid
                List<Contract> contracts = Company.GetContractsForCompany(childCompanyId, false);
                foreach (Contract contr in contracts)
                {
                    //###verify if this contract belongs to the Assureur
                    //if (!contractsThatBelongToAssur.Contains(contr.ContractId))
                    //    continue;

                    // *** Create Contract Node
                    tv = new TreeViewTag();
                    tv.Name = contr.ContractId;
                    tv.Id = contr.Id;
                    tv.NodeType = C.eTVNodeTypes.Contract;
                    tv.AssureurId = assurId;

                    TreeNode contractNode = new TreeNode(contr.ContractId, tv.GetStringFromObject());
                    contractNode.SelectAction = TreeNodeSelectAction.Select;
                    contractNode.ImageUrl = C.imageRelFolder + "contract.png";
                    contractNode.ShowCheckBox = true;

                    if (checkNodes == true)
                        contractNode.Checked = true;

                    if (!NodeWithTextExistsAlready(selectedNode, contractNode.Text))
                    {
                        //selectedNode.ChildNodes.Remove(contractNode);
                        selectedNode.ChildNodes.Add(contractNode);
                        selectedNode.Expand();
                       // bool? exp = selectedNode.Expanded;
                    }
                }

                // NOTE: we don't display CR's underneath a subsid => all CR's are displayed under the corresponding Parent Company
                
                selectedNode.ExpandAll();
                txtDetails.Text = detail;
                return;
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "LoadAllContractNodes"); }
        }
        
        private void LoadAllCRNodes(TreeNode selectedNode, TreeViewTag tv, int parentCompanyId, int assurId, string detail)
        {
            try
            {
                List<CompteResult> CRs = null;

                if (tv.NodeType == C.eTVNodeTypes.Assureur)
                    CRs = CompteResult.GetComptesResultatForAssur(assurId.ToString());
                else if (tv.NodeType == C.eTVNodeTypes.ParentCompany)
                    CRs = CompteResult.GetComptesResultatForParentCompany(parentCompanyId.ToString());
                else
                    return;
                
                foreach (CompteResult cr in CRs)
                {
                    //Validation
                    //verify if the CR belongs to the current Assur
                    if (string.IsNullOrWhiteSpace(cr.AssurIds))
                        return;

                    List<string> assurIds = Regex.Split(cr.AssurIds, C.cVALSEP).ToList();
                    if (!assurIds.Contains(assurId.ToString()))
                        continue;

                    // *** Create CR Node for Parent Comp
                    tv = new TreeViewTag();
                    tv.NodeType = C.eTVNodeTypes.CompteResultat;
                    tv.AssureurId = assurId;
                    tv.ParentCompId = parentCompanyId;

                    tv.Name = cr.Name;
                    tv.Id = cr.Id;                        
                    tv.CRCollegeId = cr.CollegeId;                       
                    tv.CRCreationDate = cr.CreationDate;                        
                    tv.CRReportLevelId = cr.ReportLevelId;
                    tv.CRCompanyIds = cr.CompanyIds;
                    tv.CRSubsids = cr.SubsidIds;
                    tv.CRContractIds = cr.ContractIds;

                    C.eReportTypes repType = C.eReportTypes.Standard;
                    if (cr.ReportType != null)
                        repType = (C.eReportTypes)Enum.Parse(typeof(C.eReportTypes), cr.ReportType);
                    tv.CRReportType = repType;

                    tv.TaxDef = cr.TaxDefault;
                    tv.TaxAct = cr.TaxActif;
                    tv.TaxPer = cr.TaxPerif;

                    //data from CRPlanning
                    foreach (CRPlanning crp in cr.CRPlannings)
                    {
                        tv.CRPs.Add(new CRPlanning { CRId = cr.Id, DebutPeriode = crp.DebutPeriode, FinPeriode = crp.FinPeriode, DateArret = crp.DateArret });
                    }

                    //we create our TreeNode
                    TreeNode crNode = new TreeNode(cr.Name, tv.GetStringFromObject());
                    crNode.SelectAction = TreeNodeSelectAction.Select;

                    crNode.ImageUrl = C.imageRelFolder + "report1.png";
                    if (tv.CRReportType == C.eReportTypes.GlobalEnt)
                        crNode.ImageUrl = C.imageRelFolder + "report2.png";
                    if (tv.CRReportType == C.eReportTypes.GlobalSubsid)
                        crNode.ImageUrl = C.imageRelFolder + "report3.png";

                    if (!NodeWithTextExistsAlready(selectedNode, crNode.Text))
                        selectedNode.ChildNodes.AddAt(0,crNode);                    
                }

                selectedNode.Expand();
                txtDetails.Text = detail;
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "LoadAllCRNodes"); }
        }

        private bool NodeWithTextExistsAlready(TreeNode selectedNode, string nodeText)
        {
            try
            {
                foreach (TreeNode child in selectedNode.ChildNodes)
                {
                    if (child.Text.ToLower() == nodeText.ToLower())
                        return true;
                }

                return false;
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "NodeWithTextExistsAlready::tvContracts_SelectedNodeChanged"); return false; }
        }
        
        protected void cmdTest_Click(object sender, EventArgs e)
        {
            //CadencierIsUpToDate();
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

        protected void cmdCreateCR_Click(object sender, EventArgs e)
        {  
            //RequiredFieldValidator1.ErrorMessage = "Le nom du rapport est obligatoire !";
            if (txtNameReport.Value == "")
            {
                validateReportName.Visible = true;
            }
            else { validateReportName.Visible = false; }

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

            HttpCookie cookieTC = new HttpCookie("typeCompte");
            cookieTC.Values["typeCompte"] = radioTypeComptes.SelectedIndex.ToString();
            Response.Cookies.Add(cookieTC);

            //verify if Cadencier is up to date
            //CadencierIsUpToDate();
            List<int> missingYears = new List<int>();
            bool cadUpToDate = BLCadencier.CadencierIsUpToDate(ref missingYears, txtStartPeriode.Text, txtEndPeriode.Text);

            lblCadencierWarning.Visible = false;
            if (!cadUpToDate && cmbDetailReport.SelectedItem.Text != "Prévoyance")
            {
                string strYears = string.Join(", ", missingYears);
                lblCadencierWarning.Text = "Attention, le cadencier n’est pas à jour pour les année(s) : " + strYears + " !";
                lblCadencierWarning.Visible = true;
            }


            List < BLCompteResultat > crs = CollectData();

            //Call method in BL & pass the List of CRs
            int crId = 0;
            if (crs != null)
            {
                foreach (BLCompteResultat cr in crs)
                {
                    crId = cr.CreateNewCompteResultat();
                }
            }

            //### fix the below code
            //return;

            //delete all CR nodes & re-create them to reflect name changes 
            //make sure the correct parent node of the CR nodes to be deleted is selected
            TreeNode selectedNode = tvContracts.SelectedNode;
            if (selectedNode != null)
            {
                TreeViewTag tv = TreeViewTag.GetObjectFromString(selectedNode.Value);

                if (tv.NodeType == C.eTVNodeTypes.CompteResultat)
                {
                    //we need to move up one level
                    selectedNode = selectedNode.Parent;
                    tv = TreeViewTag.GetObjectFromString(selectedNode.Value);
                }

                //delete all CR nodes
                List<TreeNode> crNodes = new List<TreeNode>();
                foreach (TreeNode crNode in selectedNode.ChildNodes)
                {
                    TreeViewTag tvCR = TreeViewTag.GetObjectFromString(crNode.Value);
                    if (tvCR.NodeType == C.eTVNodeTypes.CompteResultat)
                        crNodes.Add(crNode);

                    //TreeViewTag tvCR = TreeViewTag.GetObjectFromString(nodeCR.Value);
                    //if (tvCR.NodeType == C.eTVNodeTypes.CompteResultat)
                    //    selectedNode.ChildNodes.Remove(nodeCR);             
                }

                if (crNodes.Count > 0)
                {
                    foreach (TreeNode crNode in crNodes)
                        selectedNode.ChildNodes.Remove(crNode);

                    //re-create all CR nodes
                    int assurId = tv.AssureurId;
                    string detail = "Nom : " + tv.Name + Environment.NewLine + Environment.NewLine;
                    int parentCompanyId = -1;
                    if (tv.NodeType == C.eTVNodeTypes.ParentCompany)
                        parentCompanyId = tv.Id;

                    LoadAllCRNodes(selectedNode, tv, parentCompanyId, assurId, detail);
                }
            }
        }
        
        protected List<BLCompteResultat> CollectData()
        {
            TreeViewTag tv;            
            BLCompteResultat myCR = new BLCompteResultat();
            List<BLCompteResultat> crs = new List<BLCompteResultat>();
            List<string> compIds = new List<string>();
            List<string> compNames = new List<string>();
            List<string> subsidIds = new List<string>();
            List<string> subsidNames = new List<string>();
            List<string> contractIds = new List<string>();
            List<string> contractNames = new List<string>();
            List<string> assurIds = new List<string>();
            List<string> assurNames = new List<string>();

            //int selectedParentCompanies = 0;
            //string savePath = "";           

            C.eReportTypes repType = C.eReportTypes.Standard;
            if (radioReportType.SelectedIndex == 1)
                repType = C.eReportTypes.GlobalEnt;
            if (radioReportType.SelectedIndex == 2)
                repType = C.eReportTypes.GlobalSubsid;

            //### modify this
            bool isModeConso = chkComptesConsol.Checked;
            isModeConso = false;
            if (radioReportType.SelectedIndex == 1 || radioReportType.SelectedIndex == 2)
                isModeConso = true;

            try
            {
                #region ITERATE ALL NODES & COLLECT DATA
                //find out how many Parent Companies have been selected - if there is more than one, we need to adapt the CR Names
                foreach (TreeNode nodeAssur in tvContracts.Nodes)
                {
                    //code should never enter this block, because we don't allow assureur's to be selected directly
                    if (nodeAssur.Checked)
                    {
                        tv = TreeViewTag.GetObjectFromString(nodeAssur.Value);
                        if (!assurNames.Contains(tv.Name) && !assurIds.Contains(tv.Id.ToString()))
                        {
                            assurNames.Add(tv.Name);
                            assurIds.Add(tv.Id.ToString());
                        }
                    }

                    //iterate all comp nodes - except if the checkbox 'Select All' has been checked
                    if (!chkSelectAll.Checked)
                    {
                        foreach (TreeNode nodePC in nodeAssur.ChildNodes)
                        {
                            if (nodePC.Checked)
                            {
                                tv = TreeViewTag.GetObjectFromString(nodePC.Value);
                                if (!compNames.Contains(tv.Name) && !compIds.Contains(tv.Id.ToString()))
                                {
                                    compNames.Add(tv.Name);
                                    compIds.Add(tv.Id.ToString());

                                    if (!assurIds.Contains(tv.AssureurId.ToString()))
                                    {
                                        assurNames.Add(nodeAssur.Text);
                                        assurIds.Add(tv.AssureurId.ToString());
                                    }
                                }
                            }

                            foreach (TreeNode nodeSubsid in nodePC.ChildNodes)
                            {
                                if (nodeSubsid.Checked)
                                {
                                    tv = TreeViewTag.GetObjectFromString(nodeSubsid.Value);
                                    if (!subsidNames.Contains(tv.Name) && !subsidIds.Contains(tv.Id.ToString()))
                                    {
                                        subsidNames.Add(tv.Name);
                                        subsidIds.Add(tv.Id.ToString());
                                    }
                                }

                                foreach (TreeNode nodeContr in nodeSubsid.ChildNodes)
                                {
                                    if (nodeContr.Checked)
                                    {
                                        tv = TreeViewTag.GetObjectFromString(nodeContr.Value);
                                        if (!contractNames.Contains(tv.Name) && !contractIds.Contains(tv.Id.ToString()))
                                        {
                                            contractNames.Add(tv.Name);
                                            contractIds.Add(tv.Id.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }  // end of : if (chkSelectAll.Checked)
                }
                #endregion

                if (compNames.Count == 0 && !chkSelectAll.Checked) 
                    throw new Exception("Vous devriez sélectionner au moins une entreprise pour générer des comptes de résultats !");

                if (tvContracts.SelectedNode == null && chkSelectAll.Checked)
                    throw new Exception("Vous devriez sélectionner un assureur pour générer des comptes de résultats !");

                //iterate all selected treenodes - the first level is assurance            
                foreach (TreeNode nodeAssur in tvContracts.Nodes)
                {
                    if (chkSelectAll.Checked)
                    {
                        // get assureurId associated with currently selected node
                        int myassurIds = 0;
                        TreeNode selectedNode = tvContracts.SelectedNode;
                        
                        TreeViewTag seltv = TreeViewTag.GetObjectFromString(selectedNode.Value);
                        myassurIds = seltv.AssureurId;

                        //assurNames.Add(nodeAssur.Text);
                        assurIds.Add(myassurIds.ToString());

                        //get all companies & subsids
                        compIds = Company.GetParentCompanyIdsForAssureurId(myassurIds);
                        compNames = Company.GetParentCompanyNamesForAssureurId(myassurIds);
                        subsidIds = Company.GetSubsidIdsForAssureurId(myassurIds);
                        subsidNames = Company.GetSubsidNamesForAssureurId(myassurIds);

                    }

                    myCR = SetCRDetails(repType);
                    
                    myCR.AssurIds = string.Join(C.cVALSEP, assurIds);
                    myCR.AssurNames = string.Join(C.cVALSEP, assurNames);
                    myCR.ParentCompanyNames = string.Join(C.cVALSEP, compNames);
                    myCR.ParentCompanyIds = string.Join(C.cVALSEP, compIds);
                    myCR.SubsidIds = string.Join(C.cVALSEP, subsidIds);
                    myCR.SubsidNames = string.Join(C.cVALSEP, subsidNames);

                    if (radioReportType.SelectedIndex == 0)
                    {
                        myCR.ContractIds = string.Join(C.cVALSEP, contractIds);
                        myCR.ContractNames = string.Join(C.cVALSEP, contractNames);
                    }

                    //next level: parent companies
                    foreach (TreeNode nodePC in nodeAssur.ChildNodes)
                    {
                        if (nodePC.Checked)
                        {
                            //verify if all CR's contain at least 1 contractId                            
                            if (contractIds.Count == 0 && myCR.ReportType == C.eReportTypes.Standard)
                                throw new Exception("Chaque entreprise pour laquelle vous voulez créer un compte de résultat doit contenir au moins un contrat !");

                            tv = TreeViewTag.GetObjectFromString(nodePC.Value);                                                

                            //Add newly created CR to Tree
                            if ( (compNames.Count == 1 && isModeConso) || (!isModeConso))
                            {
                                crs.Add(myCR);
                                CreateNewReportNode(myCR.Name, tv.GetStringFromObject(), myCR.ReportType, nodePC);
                            }
                                                      
                        } //end current PC node
                    } //end forEach PC node

                    // create a CR node if we are in consolidation mode
                    tv = TreeViewTag.GetObjectFromString(nodeAssur.Value);
                    if (compNames.Count > 1 && isModeConso)
                    {
                        crs.Add(myCR);
                        CreateNewReportNode(myCR.Name, tv.GetStringFromObject(), myCR.ReportType, nodeAssur);
                    }

                } //end of assur node iteration

                return crs;   
            }
            catch (Exception ex) {
                UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::cmdCreateCR_Click");
                return null;
            }
        }

        public BLCompteResultat SetCRDetails(C.eReportTypes repType)
        {
            C.eTypeComptes typeComptes = C.eTypeComptes.Survenance;
            if (radioTypeComptes.SelectedIndex == 1)
                typeComptes = C.eTypeComptes.Comptable;

            BLCompteResultat myCR = new BLCompteResultat();

            int reportLevelId = int.Parse(cmbDetailReport.SelectedItem.Value);
            C.eReportTemplateTypes templateType = ReportTemplate.GetTemplateTypeForId(reportLevelId);            

            string reportName = txtNameReport.Value;
            //if (chkPrev.Checked || reportLevelId == (int) C.eReportTemplateTypes.PREV)
            //    reportName = "PREV_" + reportName;

            myCR.Name = reportName;
            //add the CRPlanning data                            
            myCR.CRPlannings.Add(new CRPlanning
            {
                //DebutPeriode = DateTime.Parse(txtStartPeriode.Value),
                DebutPeriode = DateTime.Parse(txtStartPeriode.Text),
                FinPeriode = DateTime.Parse(txtEndPeriode.Text),
                DateArret = DateTime.Parse(txtArretCompte.Text)
            });

            myCR.ReportType = repType;
            myCR.TypeComptes = typeComptes;
            myCR.ReportLevelId = reportLevelId;
            myCR.CollegeId = int.Parse(cmbCollege.SelectedItem.Value);
            myCR.UserName = User.Identity.Name;
            myCR.IsActive = true;
            myCR.IsAutoGenerated = false;
            myCR.CreationDate = DateTime.Now.Date;

            double tax = 0.0;
            if (double.TryParse(taxDef.Value, out tax))
                myCR.TaxDef = tax;
            if (double.TryParse(taxAct.Value, out tax))
                myCR.TaxAct = tax;
            if (double.TryParse(taxPer.Value, out tax))
                myCR.TaxPer = tax;

            //we also add the server path
            myCR.ExcelTemplatePath = Server.MapPath(C.reportTemplateFolder);
            myCR.ExcelCRPath = Server.MapPath(C.excelCRFolder);
            myCR.CalculateProvision = chkCalcProv.Checked;
            myCR.IsPrev = chkPrev.Checked;

            return myCR;
        }

        public void CreateNewReportNode(string name, string tvString, C.eReportTypes reportType, TreeNode nodePC)
        {
            //Add newly created CR to Tree 
            TreeViewTag tv = new TreeViewTag();
            tv.Name = name;
            tv.NodeType = C.eTVNodeTypes.CompteResultat;
            //### add other props

            TreeNode parentCRNode = new TreeNode(name, tv.GetStringFromObject());
            //TreeNode parentCRNode = new TreeNode(name, tvString);
            parentCRNode.SelectAction = TreeNodeSelectAction.Select;

            //if we are in global mode and several PC's are selected => add new report node under assureur
            parentCRNode.ImageUrl = C.imageRelFolder + "report1.png";
            if (reportType == C.eReportTypes.GlobalEnt)
                parentCRNode.ImageUrl = C.imageRelFolder + "report2.png";
            if (reportType == C.eReportTypes.GlobalSubsid)
                parentCRNode.ImageUrl = C.imageRelFolder + "report3.png";

            if (!NodeWithTextExistsAlready(nodePC, parentCRNode.Text))
                nodePC.ChildNodes.AddAt(0, parentCRNode);
        }

        public List<College> GetColleges()
        {
            try
            {
                return College.GetColleges();
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::GetColleges"); return null; }
        }

        public List<ReportTemplate> GetReportTemplates()
        {
            try
            {
                return ReportTemplate.GetReportTemplates();
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::GetReportTemplates"); return null; }
        }       

        protected void cmdDisplayReport_Click(object sender, EventArgs e)
        {
        }

        protected void cmdDeleteCR_Click(object sender, EventArgs e)
        {
            try
            {
                //get the id of the selected CR node
                TreeViewTag tv = Session["SelectedCompteResultatNode"] as TreeViewTag;

                if (tv != null && tv.NodeType == C.eTVNodeTypes.CompteResultat)
                {
                    BLCompteResultat.DeleteCompteResultat(tv.Id);

                    cmdDeleteCR.Visible = false;
                    cmdStartExcel.Visible = false;
                    cmdStartPPT.Visible = false;

                    PopulateTreeViewControl(C.cINVALIDID);
                }
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::cmdDeleteCR_Click"); }
        }
        
        protected void cmdStartExcel_Click(object sender, EventArgs e)
        {
            try
            {
                //get the id of the selected CR node
                TreeViewTag tv = Session["SelectedCompteResultatNode"] as TreeViewTag;

                if (tv != null && tv.NodeType == C.eTVNodeTypes.CompteResultat)
                {
                    string filePath = Server.MapPath(C.excelCRFolder) + "\\" + tv.Name + C.cEXCELEXTENSION;
                    
                    DownloadExcel(filePath);
                }
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::cmdStartExcel_Click"); }
        }

        protected void cmdStartPPT_Click(object sender, EventArgs e)
        {
            try
            {
                //get the id of the selected CR node
                TreeViewTag tv = Session["SelectedCompteResultatNode"] as TreeViewTag;

                //throw new Exception("test");

                if (tv != null && tv.NodeType == C.eTVNodeTypes.CompteResultat)
                {
                    string filePath = Server.MapPath(C.excelCRFolder) + "\\" + tv.Name + C.cPPTEXTENSION;
                    DownloadPPT(filePath);
                }
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::cmdStartPPT_Click"); }
        }

        protected void tvContracts_Load(object sender, EventArgs e)
        {
            try
            {
                cmdDeleteCR.Visible = false;
                cmdStartExcel.Visible = false;
                cmdStartPPT.Visible = false;

                if (tvContracts.SelectedNode != null)
                {
                    //dispaly details of the selected node  
                    string nodeVal = tvContracts.SelectedNode.Value;
                    TreeViewTag tv = TreeViewTag.GetObjectFromString(nodeVal);

                    if (tv.NodeType == C.eTVNodeTypes.CompteResultat)
                    {
                        cmdDeleteCR.Visible = true;
                        cmdStartExcel.Visible = true;
                        cmdStartPPT.Visible = true;
                    }                    
                }
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::tvContracts_Load"); }

        }        

        private void DownloadExcel(string filePath)
        {
            //### download multiple files: https://forums.asp.net/t/1100257.aspx?Download+Multiple+files

            try
            {
                FileInfo file = new FileInfo(filePath);
                if (file.Exists)
                {
                    //first we download the excel file                  
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();

                    Response.ContentType = "application/vnd.ms-excel.sheet.macroEnabled.12";
                    Response.AppendHeader("content-disposition", "attachment; filename=" + file.Name);
                    Response.AddHeader("Content-Length", file.Length.ToString());

                    Response.Flush();
                    Response.TransmitFile(file.FullName);
                    Response.End();

                    //HttpContext.Current.Response.Flush();
                    //HttpContext.Current.Response.SuppressContent = true;
                    //HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::DownloadExcel", false); }
        }

        private void DownloadPPT(string filePath)
        {
            try
            {
                FileInfo file = new FileInfo(filePath);
                //string pptFile = Path.GetDirectoryName(file.FullName) + Path.GetFileNameWithoutExtension(file.Name) + ".pptm";
                //file = new FileInfo(pptFile);
                
                if (file.Exists)
                {
                    //first we download the excel file                  
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();

                    Response.ContentType = "application/vnd.ms-powerpoint.presentation.macroEnabled.12";
                    Response.AppendHeader("content-disposition", "attachment; filename=" + file.Name);
                    Response.AddHeader("Content-Length", file.Length.ToString());

                    Response.Flush();
                    Response.TransmitFile(file.FullName);
                    Response.End();

                    //HttpContext.Current.Response.Flush();
                    //HttpContext.Current.Response.SuppressContent = true;
                    //HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }           
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::DownloadPPT", false); }
        }

        protected void cmbDetailReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //DropDownList ddl = (DropDownList)sender;  
                ShowHideCalcProv();
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "cmbDetailReport_SelectedIndexChanged", false); }
        }
        
        protected void ShowHideCalcProv()
        {
            try
            {
                if (cmbDetailReport.SelectedItem == null)
                    return;

                taxDef.Value = WebConfigurationManager.AppSettings["DefaultTax"];

                int id = int.Parse(cmbDetailReport.SelectedItem.Value);

                C.eReportTemplateTypes type = ReportTemplate.GetTemplateTypeForId(id);

                if (type == C.eReportTemplateTypes.PREV)
                {
                    radioReportType.SelectedIndex = 0; 
                    chkSelectAll.Enabled = false;
                    chkSelectAll.Checked = false;
                    radioReportType.Enabled = false;
                    chkPrev.Checked = true;

                    chkCalcProv.Visible = true;
                    taxControls.Visible = false;                    
                }
                else if (type == C.eReportTemplateTypes.PREV_GLOBAL)
                {
                    radioReportType.SelectedIndex = 1;
                    chkSelectAll.Enabled = true;
                    chkSelectAll.Checked = true;
                    radioReportType.Enabled = true;
                    chkPrev.Checked = true;

                    chkCalcProv.Visible = true;
                    taxControls.Visible = false;
                }
                else if (type == C.eReportTemplateTypes.SANTE)
                {
                    radioReportType.SelectedIndex = 0;
                    chkSelectAll.Enabled = false;
                    chkSelectAll.Checked = false;
                    radioReportType.Enabled = false;
                    chkPrev.Checked = false;

                    chkCalcProv.Visible = true;

                    taxControls.Visible = false;
                }
                else if (type == C.eReportTemplateTypes.SANTE_GLOBAL)
                {
                    radioReportType.SelectedIndex = 1;
                    chkSelectAll.Enabled = true;
                    chkSelectAll.Checked = true;
                    radioReportType.Enabled = true;
                    chkPrev.Checked = false;

                    chkCalcProv.Visible = false;
                    //calculateProv = false;

                    taxControls.Visible = true;
                }

                PrepareUI();
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "ShowHideCalcProv", false); }
        }

        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            FilterByCompany();
        }

        protected void txtFilterCompany_Change(object sender, EventArgs e)
        {
            FilterByCompany();
        }

        protected void FilterByCompany()
        {
            //entered text
            string searchText = txtCompanyFilter.Text; // ((TextBox)sender).Text;
            TreeNode assurNode = tvContracts.SelectedNode;

            if (tvContracts.SelectedNode == null && tvContracts.Nodes.Count > 0)
            {
                assurNode = tvContracts.Nodes[0];
            } 

            //get paren assureur
            if (assurNode != null)
            {
                TreeViewTag tv = TreeViewTag.GetObjectFromString(assurNode.Value);
                int assurId = tv.AssureurId;

                while (tv.NodeType != C.eTVNodeTypes.Assureur) {
                    assurNode = tvContracts.SelectedNode.Parent;
                    tv = TreeViewTag.GetObjectFromString(assurNode.Value);
                    assurId = tv.AssureurId;
                }

                string detail = "Nom : " + tv.Name + Environment.NewLine + Environment.NewLine;

                //clear all child nodes
                assurNode.ChildNodes.Clear();

                //load new company nodes
                List<Company> parentCompaniesForAssur = Company.GetParentCompaniesForAssureurId(assurId, searchText);
                foreach (Company parentComp in parentCompaniesForAssur)
                {
                    // *** Create Parent Comp Node
                    tv = new TreeViewTag();
                    tv.Name = parentComp.Name;
                    tv.Id = parentComp.Id;
                    tv.AssureurId = assurId;
                    tv.NodeType = C.eTVNodeTypes.ParentCompany;

                    TreeNode parentCompNode = new TreeNode(parentComp.Name, tv.GetStringFromObject());
                    parentCompNode.SelectAction = TreeNodeSelectAction.Select;
                    parentCompNode.ImageUrl = C.imageRelFolder + "company.png";
                    parentCompNode.ShowCheckBox = true;

                    assurNode.ChildNodes.Add(parentCompNode);
                    parentCompNode.Expand();
                }

                assurNode.Expand();
                txtDetails.Text = detail;

            }   
        }

        protected void radioReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            PrepareUI();

            //chkSelectAll.Visible = true;
            //cmbDetailReport.Enabled = false;
            //if (radioReportType.SelectedIndex == 0)
            //{
            //    imgReport.ImageUrl = C.imageRelFolder + "report1.png";
            //    chkSelectAll.Visible = false;
            //    cmbDetailReport.Enabled = true;
            //}

            //if (radioReportType.SelectedIndex == 1) imgReport.ImageUrl = C.imageRelFolder + "report2.png";
            //if (radioReportType.SelectedIndex == 2) imgReport.ImageUrl = C.imageRelFolder + "report3.png";
        }

        protected void cmdSelectAll_Click(object sender, EventArgs e)
        {
            string strNoAssur = UICommon.ShowPopUpMsg("Vous devriez sélectionner un assureur pour utiliser cette fonction !");            
            TreeNode selectedNode = tvContracts.SelectedNode;

            if (selectedNode != null)
            {
                TreeViewTag tv = TreeViewTag.GetObjectFromString(selectedNode.Value);
                int assurId = tv.AssureurId;
                string detail = "Nom : " + tv.Name + Environment.NewLine + Environment.NewLine;

                if (tv.NodeType != C.eTVNodeTypes.Assureur)
                {                    
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "showalert", strNoAssur, true);
                    //throw new Exception("Vous devriez sélectionner un assureur pour utiliser cette fonction !");
                }
                else
                {                    
                    //expand all company nodes
                    foreach (TreeNode nodePC in selectedNode.ChildNodes)
                    {
                        //select parent node & expand it
                        tv = TreeViewTag.GetObjectFromString(nodePC.Value);

                        if (tv.NodeType == C.eTVNodeTypes.ParentCompany)
                        {
                            nodePC.Checked = true;
                            int parentCompanyId = tv.Id;

                            //get all Subsid nodes and subsid nodes
                            LoadAllSubsidNodes(nodePC, tv, assurId, detail, false, true);

                            foreach (TreeNode subsidNode in nodePC.ChildNodes)
                            {
                                if (subsidNode.ShowCheckBox != null && subsidNode.ShowCheckBox != false)
                                {
                                    subsidNode.Checked = true;

                                    LoadAllContractNodes(subsidNode, tv, assurId, detail, true);

                                    foreach (TreeNode contr in subsidNode.ChildNodes)
                                    {
                                        contr.Checked = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "showalert", strNoAssur, true);
            }  
         }



    }
}