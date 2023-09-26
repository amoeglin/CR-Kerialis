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

        #region Global Variables

        int listId;
        C.eExcelGroupTypes listType;
        List<Entrepr> enterprises;
        List<Groups> groups;
        string rootCRAutoPath = "";
        string loggedUser = "";

        protected static bool inProcess = false;
        protected static bool processComplete = false;
        protected static bool processError = false;
        protected static string progressContent;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {              
            try
            {
                rootCRAutoPath = Server.MapPath(C.excelCRAutoFolder);
                loggedUser = User.Identity.Name;

                if (radioMode.SelectedIndex == 0)
                {                    
                    PlaceHolderReportType.Visible = false;
                    panelAuto.Visible = true;
                    panelManuel.Visible = false;
                    panelAuto2.Visible = true;
                    panelManuel2.Visible = false;
                }
                else
                {
                    cmdCreateCR.CssClass = "ButtonBigBlue inline manual"; //to show the old spinner
                    PlaceHolderReportType.Visible = true;
                    panelAuto.Visible = false;
                    panelManuel.Visible = true;
                    panelAuto2.Visible = false;
                    panelManuel2.Visible = true;
                }

                radioReportType.Items[0].Enabled = false;

                tvContracts.Enabled = true;
                tvContracts.Attributes.Add("onclick", "postBackByObject()");

                PrepareUI();

                if (!IsPostBack)
                {
                    RefreshLists("");

                    PopulateTreeViewControl(C.cINVALIDID);

                    //get date values
                    HttpCookie cookie = Request.Cookies["txtStartPeriode"];
                    string startPeriodeCookieVal = cookie != null ? cookie.Value : ""; 
                    if (txtStartPeriode.Text == "")
                        txtStartPeriode.Text = startPeriodeCookieVal != "" ? startPeriodeCookieVal : "01/01/2020";

                    cookie = Request.Cookies["txtEndPeriode"];
                    string endPeriodeCookieVal = cookie != null ? cookie.Value : "";
                    if (txtEndPeriode.Text == "")
                        txtEndPeriode.Text = endPeriodeCookieVal != "" ? endPeriodeCookieVal : "01/01/2020";

                    cookie = Request.Cookies["txtArretCompte"];
                    string arretCompteCookieVal = cookie != null ? cookie.Value : "";
                    if (txtArretCompte.Text == "")
                        txtArretCompte.Text = arretCompteCookieVal != "" ? arretCompteCookieVal : "01/01/2020";

                    cookie = Request.Cookies["typeCompte"];
                    string typeCompteCookieVal = cookie != null ? cookie.Value : "";
                    int iTypeCompteVal = 0;
                    if (int.TryParse(typeCompteCookieVal, out iTypeCompteVal))
                        radioTypeComptes.SelectedIndex = iTypeCompteVal;
                    else
                        radioTypeComptes.SelectedIndex = 0;

                    if (radioTypeComptes.SelectedIndex == 0)
                    {
                        dateArreteCompte.Visible = true;
                        lblDateDebut.InnerText = "Date survenance début :";
                        lblDateFin.InnerText = "Date survenance fin :";
                    }
                    else
                    {
                        dateArreteCompte.Visible = false;
                        lblDateDebut.InnerText = "Date comptable début :";
                        lblDateFin.InnerText = "Date comptable fin :";
                    }
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
            catch (Exception ex) { 
                UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::PopulateTreeViewControl"); 
            }
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
                    bool auto = cr.IsAutoGenerated.HasValue ? cr.IsAutoGenerated.Value : false;
                    if (!auto)
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
                            selectedNode.ChildNodes.AddAt(0, crNode);
                    }
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

        private void oldCadencierIsUpToDate()
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

        private void CheckCadencierUpToDate()
        {
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
        }

        private void SaveDatesAsCookies()
        {
            //save dates as cookie
            if (txtStartPeriode.Text != "")
            {
                HttpCookie cookie = Request.Cookies.Get("txtStartPeriode");
                if (cookie == null)
                {
                    cookie = new HttpCookie("txtStartPeriode");
                    cookie.Expires = new DateTime(2050, 1, 1);
                    cookie.Value = txtStartPeriode.Text;
                    Response.Cookies.Add(cookie);
                } else
                {
                    cookie.Value = txtStartPeriode.Text;
                } 
            }
            if (txtEndPeriode.Text != "")
            {
                HttpCookie cookie = Request.Cookies.Get("txtEndPeriode");
                if (cookie == null)
                {
                    cookie = new HttpCookie("txtEndPeriode");
                    cookie.Expires = new DateTime(2050, 1, 1);
                    cookie.Value = txtEndPeriode.Text;
                    Response.Cookies.Add(cookie);
                }
                else
                {
                    cookie.Value = txtEndPeriode.Text;
                }
            }
            if (txtArretCompte.Text != "")
            {
                HttpCookie cookie = Request.Cookies.Get("txtArretCompte");
                if (cookie == null)
                {
                    cookie = new HttpCookie("txtArretCompte");
                    cookie.Expires = new DateTime(2050, 1, 1);
                    cookie.Value = txtArretCompte.Text;
                    Response.Cookies.Add(cookie);
                }
                else
                {
                    cookie.Value = txtArretCompte.Text;
                }
            }

            HttpCookie cookieTC = new HttpCookie("typeCompte");
            cookieTC.Values["typeCompte"] = radioTypeComptes.SelectedIndex.ToString();
            Response.Cookies.Add(cookieTC);
        }

        private bool CheckFileLocked(string filePath)
        {
            bool fileLocked = false;
            FileInfo fil1 = new FileInfo(filePath);
            if (IsFileLocked(fil1))
            {
                fileLocked = true;
                try
                {
                    lblModalBody.Text = $"Merci de fermer le fichier suivant avant de procéder : <br /> {fil1.FullName}";
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "modalFileOpen", "$('#modalFileOpen').modal();", true);
                    upModal.Update();
                }
                catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::CheckFileLocked"); }
            }
            return fileLocked;
        }

        protected void cmdCreateCR_Click(object sender, EventArgs e)
        {
            if (radioMode.SelectedIndex == 0)
            {
                try
                {
                    SaveDatesAsCookies();

                    inProcess = true;
                    processComplete = false;
                    progressContent = "Progrès...";

                    Timer1.Enabled = true;
                    Thread workerThread = new Thread(new ThreadStart(CreateCRAuto));
                    workerThread.Start();

                    //CreateCRAuto();
                }
                catch (Exception ex)
                {
                    UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::cmdCreateCR_Click");
                }
            }
            else // CR Manual
            {
                try
                {
                    //Validate Report Name - RequiredFieldValidator1.ErrorMessage = "Le nom du rapport est obligatoire !";                
                    if (txtNameReport.Value == "")
                    {
                        validateReportName.Visible = true;
                    }
                    else { validateReportName.Visible = false; }

                    //check if rport files are open
                    string crFilePath = Path.Combine(Server.MapPath(C.excelCRFolder), txtNameReport.Value + ".xlsm");
                    string crFilePathPPT = Path.Combine(Server.MapPath(C.excelCRFolder), txtNameReport.Value + ".pptm");
                    CheckFileLocked(crFilePath);
                    CheckFileLocked(crFilePathPPT);

                    SaveDatesAsCookies();
                    CheckCadencierUpToDate();

                    //collect CR data & create CR's
                    List<BLCompteResultat> crs = CollectData();
                    int crId = 0;
                    if (crs != null)
                    {
                        foreach (BLCompteResultat cr in crs)
                        {
                            crId = cr.CreateNewCompteResultat();
                        }
                    }

                    //delete all CR nodes & re-create them to reflect name changes 
                    try
                    {
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
                    catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::6"); }
                }
                catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::1"); }
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

                    myCR = SetCRDetails(repType, C.excelCRFolder);
                    
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

        public BLCompteResultat SetCRDetails(C.eReportTypes repType, string excelCRPath)
        {
            BLCompteResultat myCR = new BLCompteResultat();

            C.eTypeComptes typeComptes = C.eTypeComptes.Survenance;
            if (radioTypeComptes.SelectedIndex == 1)
                typeComptes = C.eTypeComptes.Comptable;            

            int reportLevelId = int.Parse(cmbDetailReport.SelectedItem.Value);              

            DateTime dArret = DateTime.Parse(txtArretCompte.Text);
            if (radioTypeComptes.SelectedIndex == 1)
                dArret = DateTime.Parse(txtEndPeriode.Text);

            //Set CR fields
            myCR.Name = txtNameReport.Value; 
            myCR.ReportType = repType;
            myCR.ReportLevelId = reportLevelId;
            myCR.TypeComptes = typeComptes;              
            myCR.UserName = loggedUser;            
            myCR.CreationDate = DateTime.Now.Date;

            myCR.CRPlannings.Add(new CRPlanning
            {
                DebutPeriode = DateTime.Parse(txtStartPeriode.Text),
                FinPeriode = DateTime.Parse(txtEndPeriode.Text),
                DateArret = dArret
            });

            myCR.CalculateProvision = chkCalcProv.Checked;
            myCR.IsPrev = chkPrev.Checked;

            //we also add the server path
            myCR.ExcelTemplatePath = Server.MapPath(C.reportTemplateFolder);
            myCR.ExcelCRPath = Server.MapPath(excelCRPath);          

            //minor fields - some of them may no longer be required
            double tax = 0.0;
            if (double.TryParse(taxDef.Value, out tax))
                myCR.TaxDef = tax;
            if (double.TryParse(taxAct.Value, out tax))
                myCR.TaxAct = tax;
            if (double.TryParse(taxPer.Value, out tax))
                myCR.TaxPer = tax;

            myCR.IsActive = true;
            myCR.IsAutoGenerated = false;
            myCR.CollegeId = int.Parse(cmbCollege.SelectedItem.Value);            

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
                var repTempl = ReportTemplate.GetReportTemplates();

                if (radioMode.SelectedIndex == 0)
                {
                    List<ReportTemplate> repModeAuto = new List<ReportTemplate>();
                    foreach (ReportTemplate r in repTempl)
                    {
                        if (r.Type == "SANTE" || r.Type == "PREV") repModeAuto.Add(r);
                    }
                    return repModeAuto;
                }
                else return repTempl;
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
                string repType = cmbDetailReport.SelectedItem.Text;
                if (repType.ToLower().Contains("sant")) listType = C.eExcelGroupTypes.Sante;
                else listType = C.eExcelGroupTypes.Prev;

                ShowHideCalcProv();

                //cmbDetailReport_SelectedIndexChanged
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

        protected void radioTypeComptes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioTypeComptes.SelectedIndex == 0)
            {
                dateArreteCompte.Visible = true;
                lblDateDebut.InnerText = "Date survenance début :";
                lblDateFin.InnerText = "Date survenance fin :";
            }
            else
            {
                dateArreteCompte.Visible = false;
                lblDateDebut.InnerText = "Date comptable début :";
                lblDateFin.InnerText = "Date comptable fin :";
            }

            //SaveParams();
        }

        protected virtual bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException ex)
            {
                if (ex.Message.ToString().ToLower().Contains("could not find file"))
                    return false;
                else
                    return true;
            }

            //file is not locked
            return false;
        }

        // ****************************** NEW FEATURES ******************************

        //Progress:
        //https://stackoverflow.com/questions/27583227/updating-asp-net-updatepanel-during-processing-loop
        //https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.updateprogress?view=netframework-4.8.1
       
        private void CreateCRFolders(string listName, string listType)
        {
            //AutoCR => Sante || Prev => Group || Entreprise => USER[_NAME]_DATETIME
            //string rootCRAutoPath = Server.MapPath(C.excelCRAutoFolder);
            if (!Directory.Exists(rootCRAutoPath))
                Directory.CreateDirectory(rootCRAutoPath);

            if (!Directory.Exists(Path.Combine(rootCRAutoPath, listName)))
                Directory.CreateDirectory(Path.Combine(rootCRAutoPath, listName));

            if (!Directory.Exists(Path.Combine(rootCRAutoPath, listName, listType)))
                Directory.CreateDirectory(Path.Combine(rootCRAutoPath, listName, listType));

            //*****************************
            //if (!Directory.Exists(Path.Combine(rootCRAutoPath, "Sante")))
            //    Directory.CreateDirectory(Path.Combine(rootCRAutoPath, "Sante"));

            //if (!Directory.Exists(Path.Combine(rootCRAutoPath, "Sante", "Group")))
            //    Directory.CreateDirectory(Path.Combine(rootCRAutoPath, "Sante", "Group"));

            //if (!Directory.Exists(Path.Combine(rootCRAutoPath, "Sante", "Entreprise")))
            //    Directory.CreateDirectory(Path.Combine(rootCRAutoPath, "Sante", "Entreprise"));


            //if (!Directory.Exists(Path.Combine(rootCRAutoPath, "Prev")))
            //    Directory.CreateDirectory(Path.Combine(rootCRAutoPath, "Prev"));

            //if (!Directory.Exists(Path.Combine(rootCRAutoPath, "Prev", "Group")))
            //    Directory.CreateDirectory(Path.Combine(rootCRAutoPath, "Prev", "Group"));

            //if (!Directory.Exists(Path.Combine(rootCRAutoPath, "Prev", "Entreprise")))
            //    Directory.CreateDirectory(Path.Combine(rootCRAutoPath, "Prev", "Entreprise"));
        }

        private void CreateCRAuto()
        {
            try
            {
                spinnerPanel.Visible = true;
                
                string currListType = "Sante";
                if (!cmbDetailReport.SelectedItem.Text.ToLower().Contains("sant")) currListType = "Prev";
                string listName = lbListes.SelectedItem.Text;                

                CreateCRFolders(listName, currListType);
                GenerateGroupEntLists();                
                CheckCadencierUpToDate();

                //collect CR data & create CR's
                List<BLCompteResultat> crs = CollectAutoData();
                int crId = 0;
                if (crs != null)
                {
                    //inProcess = true;
                    //processComplete = false;

                    int cnt = 0;
                    foreach (BLCompteResultat cr in crs)
                    {                        
                        progressContent = "Progrès : " + cnt.ToString()  + " / " + crs.Count.ToString() + " comptes de résultats créés";
                        cnt++;

                        //Thread.Sleep(500); 
                        if (cnt == 1)
                            cr.CRAutoId = -1;
                        else
                            cr.CRAutoId = crId;

                        crId = cr.CreateNewCompteResultat();
                    }

                    processComplete = true;                    
                }
            }
            catch (Exception ex) {
                while (ex.InnerException != null) ex = ex.InnerException;
                progressContent = ex.Message;
                processComplete = true;
                processError = true;
            }
        }

        private List<BLCompteResultat> CollectAutoData()
        {
            BLCompteResultat myCR = new BLCompteResultat();
            List<BLCompteResultat> crs = new List<BLCompteResultat>();

            int listId = int.Parse(lbListes.SelectedItem.Value);
            CRGenList list = CRGenList.GetListById(listId);
            string currListName = list.Name;
            string currListOwner = list.UserName;
            string assurType = list.AssurType;

            //TODO reset on each iteration            
            List<string> assurIds = new List<string>();
            List<string> assurNames = new List<string>();

            C.eReportTypes repType = C.eReportTypes.Standard;
            if (radioReportType.SelectedIndex == 1)
                repType = C.eReportTypes.GlobalEnt;
            if (radioReportType.SelectedIndex == 2)
                repType = C.eReportTypes.GlobalSubsid;

            string repSantePrev = cmbDetailReport.SelectedItem.Text;
            if (repSantePrev.ToLower().Contains("sant")) listType = C.eExcelGroupTypes.Sante;
            else listType = C.eExcelGroupTypes.Prev;

            string currListType = "Sante";
            if (!cmbDetailReport.SelectedItem.Text.ToLower().Contains("sant")) currListType = "Prev";

            string repName = txtNameReport.Value != "" ? txtNameReport.Value + "_" : "";
            string reportFolder = repName + DateTime.Now.ToString("s").Replace(":", "-"); //loggedUser
            string reportPath = "";

            //we assume it is always KERIALIS_ENTREPRISE => KERIALIS_PRODUIT does not containcompany names => only contact names
            string idAssSanteEnt = Assureur.GetAssIdForAssName("KERIALIS_ENTREPRISE").ToString();
            string idAssPrevEnt = Assureur.GetAssIdForAssName("KERIALIS_PREVOYANCE_ENTREPRISE").ToString();
            string idAssSanteProd = Assureur.GetAssIdForAssName("KERIALIS_PRODUIT").ToString();
            string idAssPrevProd = Assureur.GetAssIdForAssName("KERIALIS_PREVOYANCE_PRODUIT").ToString();

            if (listType == C.eExcelGroupTypes.Sante)
            {
                if (assurType.ToLower().Contains("prod"))
                {
                    assurNames.Add("KERIALIS_PRODUIT");
                    assurIds.Add(idAssSanteProd);
                }
                else
                {
                    assurNames.Add("KERIALIS_ENTREPRISE");
                    assurIds.Add(idAssSanteEnt);
                }
            }
            else //PREV
            {
                if (assurType.ToLower().Contains("prod"))
                {
                    assurNames.Add("KERIALIS_PREVOYANCE_PRODUIT");
                    assurIds.Add(idAssPrevProd);
                }
                else
                {
                    assurNames.Add("KERIALIS_PREVOYANCE_ENTREPRISE");
                    assurIds.Add(idAssPrevEnt);
                }                
            }

            try
            {
                if (chkGroups.Checked)
                {  
                    if (!Directory.Exists(Path.Combine(rootCRAutoPath, currListName, currListType, "Group")))
                        Directory.CreateDirectory(Path.Combine(rootCRAutoPath, currListName, currListType, "Group"));

                    reportPath = Path.Combine(rootCRAutoPath, currListName, "Sante", "Group", reportFolder);
                    if (listType != C.eExcelGroupTypes.Sante) reportPath = Path.Combine(rootCRAutoPath, currListName, "Prev", "Group", reportFolder);
                    
                    foreach (Groups g in groups)
                    {
                        List<string> compIds = new List<string>();
                        List<string> compNames = new List<string>();
                        List<string> subsidIds = new List<string>();
                        List<string> subsidNames = new List<string>();
                        List<string> contractIds = new List<string>();
                        List<string> contractNames = new List<string>();

                        foreach(Entrepr e in g.Enterprises)
                        {
                            int id = Company.GetCompanyIdForName(e.Name);
                            if (!compNames.Contains(e.Name) && !compIds.Contains(id.ToString()))
                            {
                                compNames.Add(e.Name);
                                compIds.Add(id.ToString());

                                //get all subsids
                                List<Company> subsids = Company.GetChildCompanies(id);
                                foreach (Company sub in subsids)
                                {
                                    if (!subsidNames.Contains(sub.Name) && !subsidIds.Contains(sub.Id.ToString()))
                                    {
                                        subsidNames.Add(sub.Name);
                                        subsidIds.Add(sub.Id.ToString());
                                    }

                                    //get all contracts
                                    List<Contract> contracts = Company.GetContractsForCompany(sub.Id, false);
                                    foreach (Contract contr in contracts)
                                    {
                                        if (!contractNames.Contains(contr.ContractId) && !contractIds.Contains(contr.Id.ToString()))
                                        {
                                            contractNames.Add(contr.ContractId);
                                            contractIds.Add(contr.Id.ToString());
                                        }
                                    }
                                }
                            }
                        }

                        myCR = SetCRDetails(repType, C.excelCRAutoFolder);

                        myCR.AssurIds = string.Join(C.cVALSEP, assurIds);
                        myCR.AssurNames = string.Join(C.cVALSEP, assurNames);
                        myCR.ParentCompanyNames = string.Join(C.cVALSEP, compNames);
                        myCR.ParentCompanyIds = string.Join(C.cVALSEP, compIds);
                        myCR.SubsidIds = string.Join(C.cVALSEP, subsidIds);
                        myCR.SubsidNames = string.Join(C.cVALSEP, subsidNames);
                        myCR.ContractIds = string.Join(C.cVALSEP, contractIds);
                        myCR.ContractNames = string.Join(C.cVALSEP, contractNames);

                        myCR.Name = g.Name;
                        myCR.ListName = currListName;
                        myCR.LevelGrEnt = "Group";
                        myCR.CRGroups = g;
                        myCR.ExcelCRPath = reportPath;
                        myCR.IsAutoGenerated = true;

                        crs.Add(myCR);
                    }
                }

                if (chkEnterprises.Checked)
                {
                    if (!Directory.Exists(Path.Combine(rootCRAutoPath, currListName, currListType, "Entreprise")))
                        Directory.CreateDirectory(Path.Combine(rootCRAutoPath, currListName, currListType, "Entreprise"));

                    reportPath = Path.Combine(rootCRAutoPath, currListName, "Sante", "Entreprise", reportFolder);
                    if (listType != C.eExcelGroupTypes.Sante) reportPath = Path.Combine(rootCRAutoPath, currListName, "Prev", "Entreprise", reportFolder);
                                        
                    //create a report for each Entrepr => search for all subsids & all contracts
                    foreach (Entrepr e in enterprises)
                    {
                        List<string> compIds = new List<string>();
                        List<string> compNames = new List<string>();
                        List<string> subsidIds = new List<string>();
                        List<string> subsidNames = new List<string>();
                        List<string> contractIds = new List<string>();
                        List<string> contractNames = new List<string>();

                        int id = Company.GetCompanyIdForName(e.Name);
                        if (!compNames.Contains(e.Name) && !compIds.Contains(id.ToString()))
                        {
                            compNames.Add(e.Name);                            
                            compIds.Add(id.ToString());

                            //get all subsids
                            List<Company> subsids = Company.GetChildCompanies(id);
                            foreach(Company sub in subsids)
                            {
                                if (!subsidNames.Contains(sub.Name) && !subsidIds.Contains(sub.Id.ToString()))
                                {
                                    subsidNames.Add(sub.Name);
                                    subsidIds.Add(sub.Id.ToString());
                                }

                                //get all contracts
                                List<Contract> contracts = Company.GetContractsForCompany(sub.Id, false);
                                foreach (Contract contr in contracts)
                                {
                                    if (!contractNames.Contains(contr.ContractId) && !contractIds.Contains(contr.Id.ToString()))
                                    {
                                        contractNames.Add(contr.ContractId);
                                        contractIds.Add(contr.Id.ToString());
                                    }
                                }
                            }
                        }

                        myCR = SetCRDetails(repType, C.excelCRAutoFolder);

                        myCR.AssurIds = string.Join(C.cVALSEP, assurIds);
                        myCR.AssurNames = string.Join(C.cVALSEP, assurNames);
                        myCR.ParentCompanyNames = string.Join(C.cVALSEP, compNames);
                        myCR.ParentCompanyIds = string.Join(C.cVALSEP, compIds);
                        myCR.SubsidIds = string.Join(C.cVALSEP, subsidIds);
                        myCR.SubsidNames = string.Join(C.cVALSEP, subsidNames);
                        myCR.ContractIds = string.Join(C.cVALSEP, contractIds);
                        myCR.ContractNames = string.Join(C.cVALSEP, contractNames);

                        myCR.Name = e.Name;
                        myCR.ListName = currListName;
                        myCR.LevelGrEnt = "Entreprise";
                        myCR.CREntrepr = e;
                        myCR.ExcelCRPath = reportPath;
                        myCR.IsAutoGenerated = true;

                        crs.Add(myCR);                                                
                    }
                }

                return crs;
            }
            catch (Exception ex)
            {
                UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::CollectAutoData");
                return null;
            }
        }

        protected void GenerateGroupEntLists()
        {
            enterprises = new List<Entrepr>();
            groups = new List<Groups>();

            //rptListe.DataBind();
            bool res = chkGroups.Checked;

            foreach (RepeaterItem ri in rptListe.Items)
            {
                CheckBox chk = ri.FindControl("chk1") as CheckBox;
                Label lblGroup = ri.FindControl("lblGroupe") as Label;
                Label lblEnt = ri.FindControl("lblEnterprise") as Label;
                Label lblRaison = ri.FindControl("lblRaison") as Label;
                Label lblStructure = ri.FindControl("lblStructure") as Label;
                string currGroup = "";
                string currEnt = "";
                string currRaison = "";
                string currStruct = "";
                int currIndex;
                int currIndexE;

                if (chk != null)
                {
                    if (chk.Checked)
                    {
                        if (lblGroup != null)
                        {
                            if (lblGroup.Text != string.Empty)
                            {
                                currGroup = lblGroup.Text;
                                currIndex = groups.FindIndex(item => item.Name == currGroup);
                                if (currIndex < 0)
                                {
                                    groups.Add(new Groups { Name = currGroup, Enterprises = new List<Entrepr>() });
                                }
                            }

                            //handle enterprises
                            if (lblEnt != null)
                            {
                                if (lblRaison.Text != string.Empty) currRaison = lblRaison.Text;
                                if (lblStructure.Text != string.Empty) currStruct = lblStructure.Text;

                                if (lblEnt.Text != string.Empty)
                                {
                                    currEnt = lblEnt.Text;
                                    currIndexE = enterprises.FindIndex(item => item.Name == currEnt);
                                    
                                    if (currIndexE < 0)
                                    {
                                        //groups.Add(new Group { Name = currGroup, Enterprises = new List<Entrepr>() });
                                        enterprises.Add(new Entrepr { Name = currEnt, GroupName= currGroup, RaisonSociale = currRaison, Structure = currStruct });
                                    }

                                    //add Enterprise to Group
                                    currIndex = groups.FindIndex(item => item.Name == currGroup);
                                    if (currIndex >= 0)
                                    {
                                        //List<string> ents = groups[currIndex].Enterprises;
                                        List<Entrepr> ents = groups[currIndex].Enterprises;
                                        int index = ents.FindIndex(item => item.Name == currEnt);
                                        if (index < 0)
                                        {
                                            //groups[currIndex].Enterprises.Add(currEnt);
                                            groups[currIndex].Enterprises.Add(new Entrepr { Name = currEnt, GroupName= currGroup, RaisonSociale = currRaison, Structure = currStruct });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #region AUTO MODE => UI

        public IEnumerable<CRGenList> GetLists()
        {
            return CRGenList.GetLists();
        }

        public List<CRGenListComp> GetGroupEntreprise()
        {
            try
            {
                if (listId != -1) return CRGenListComp.GetByCRListId(listId);
                else return null;
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::GetGroupEntreprise"); return null; }
        }

        protected void radioMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioMode.SelectedIndex == 0)
            {
                panelAuto.Visible = true;
                panelManuel.Visible = false;
                panelAuto2.Visible = true;
                panelManuel2.Visible = false;
            }
            if (radioMode.SelectedIndex == 1)
            {
                panelManuel.Visible = true;
                panelAuto.Visible = false;
                panelManuel2.Visible = true;
                panelAuto2.Visible = false;
            }
            cmbDetailReport.DataBind();

        }

        private void RefreshLists(string myListName)
        {
            lbListes.DataSource = GetLists();
            lbListes.DataBind();

            if (lbListes.Items.Count > 0)
            {
                if (myListName != "")
                {
                    lbListes.SelectedIndex = lbListes.Items.IndexOf(lbListes.Items.FindByText(myListName));
                }
                else
                {
                    lbListes.SelectedIndex = 0;
                }

                listId = int.Parse(lbListes.SelectedItem.Value);
                FillListRepeater();
            }
        }

        private void FillListRepeater()
        {
            if (lbListes.SelectedItem != null)
            {
                rptListe.DataBind();

                int id = int.Parse(lbListes.SelectedItem.Value);
                CRGenList list = CRGenList.GetListById(id);

                if (list.Type.ToLower().Contains("sant"))
                {
                    listType = C.eExcelGroupTypes.Sante;
                    var reportItem = cmbDetailReport.Items.FindByText("Santé - 1 an de survenance");
                    if(reportItem != null)
                    {
                        cmbDetailReport.SelectedValue = reportItem.Value;
                    }
                }
                else
                {
                    listType = C.eExcelGroupTypes.Prev;
                    var reportItem = cmbDetailReport.Items.FindByText("Prévoyance");
                    if (reportItem != null)
                    {
                        cmbDetailReport.SelectedValue = reportItem.Value;
                    }
                }
            } 
        }

        protected void lbListes_SelectedIndexChanged(object sender, EventArgs e)
        {
            listId = int.Parse(lbListes.SelectedItem.Value);
            FillListRepeater();
        }

        protected void rptListe_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Repeater rpt = sender as Repeater;

            if (rpt != null)
            {
                //if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                //{
                //    // automatically select all checkboxes
                //    var chk = (CheckBox)e.Item.FindControl("chk1");
                //    if (chk != null)
                //    {
                //        chk.Checked = true;
                //    }
                //}

                if (e.Item.ItemType == ListItemType.Footer)
                {
                    if (rpt.Items.Count < 1)
                    {
                        rptListe.Visible = false;
                        phHeader.Visible = true;
                    }
                    else
                    {
                        rptListe.Visible = true;
                        phHeader.Visible = false;
                    }
                }
            }
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            if (inProcess)
            {
                if (lblProgress.Text != progressContent)
                    lblProgress.Text = progressContent;
            }

            if (processComplete)
            {
                inProcess = false;
                Timer1.Enabled = false;
                spinnerPanel.Visible = false;

                if (!processError)
                {
                    lblProgress.ForeColor = System.Drawing.Color.FromArgb(0,117,255);
                    lblProgress.Font.Size = FontUnit.Large;
                    lblProgress.Text = "";
                } else
                {                    
                    lblProgress.ForeColor = System.Drawing.Color.Red;
                    lblProgress.Font.Size = FontUnit.Smaller; 
                    lblProgress.Text = progressContent;
                }
            }
        }

        #endregion
      
        
        //### TEST CODE => DELETE
        protected void TEST_Click(object sender, EventArgs e)
        {
            //GenerateGroupEntLists();
            try
            {
                //throw new Exception("ERROR!!!");
            }
            catch (Exception ex) {
                UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::CreateCRAuto");
            }
        }

       
    }

    
}