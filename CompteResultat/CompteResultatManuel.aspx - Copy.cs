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

using CompteResultat.BL;
using CompteResultat.DAL;
using CompteResultat.Common;
using System.Web.DynamicData;
using System.Web.ModelBinding;


namespace CompteResultat
{
    
    public partial class CompteResultatManuel : System.Web.UI.Page
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                tvContracts.Attributes.Add("onclick", "postBackByObject()");

                if (!IsPostBack)
                {
                    PopulateTreeViewControl(C.cINVALIDID);
                }                             
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::Page_Load"); }
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
                    //fill textboxes with values from first planning
                    if (tv.CRPs.Count > 0)
                    {
                        txtStartPeriode.Value = tv.CRPs[0].DebutPeriode.Value.ToString(C.cISODATE);
                        txtEndPeriode.Value = tv.CRPs[0].FinPeriode.Value.ToString(C.cISODATE);
                        txtArretCompte.Value = tv.CRPs[0].DateArret.Value.ToString(C.cISODATE);
                    }

                    cmbCollege.SelectedValue = tv.CRCollegeId.Value.ToString();
                    cmbDetailReport.SelectedValue = tv.CRReportLevelId.Value.ToString();
                    txtNameReport.Value = tv.Name;

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

                    detail += "Entreprise : " + Company.GetCompanyNameForId(tv.CRParentCompId) + Environment.NewLine + Environment.NewLine;
                    if (tv.CRSubsids != null)
                        detail += "Filiales : " + Company.GetCompanyNamesFromIdList(tv.CRSubsids) + Environment.NewLine + Environment.NewLine;
                    if (tv.CRContractIds != null)
                        detail += "Contrats : " + Contract.GetContractNamesFromIdList(tv.CRContractIds) + Environment.NewLine + Environment.NewLine;

                    //if(tv.CRCollegeId.HasValue)
                    //    detail += "Collège : " + College.GetCollegeNameForId(tv.CRCollegeId.Value) + Environment.NewLine + Environment.NewLine;
                    if (tv.CRReportLevelId.HasValue)
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

                    //display parent companies underneath the assureur
                    List<Company> parentCompaniesForAssur = Company.GetParentCompaniesForAssureurId(assurId);
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

                        selectedNode.ChildNodes.Add(parentCompNode);
                        parentCompNode.Expand();
                    }

                    selectedNode.Expand();
                    txtDetails.Text = detail;
                    return;
                }

                #endregion

                #region NODETYPE == ParentCompany

                if (tv.NodeType == C.eTVNodeTypes.ParentCompany)
                { 
                    int parentCompanyId = tv.Id;

                    LoadAllCRNodes(selectedNode, tv, parentCompanyId, assurId, detail);
                    //### true - false
                    LoadAllSubsidNodes(selectedNode, tv, assurId, detail, true);
                }

                #endregion

                #region NODETYPE == ChildCompany

                if (tv.NodeType == C.eTVNodeTypes.Subsid)
                {
                    LoadAllContractNodes(selectedNode, tv, assurId, detail);
                }

                #endregion                

            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::tvContracts_SelectedNodeChanged"); }
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

        private void LoadAllSubsidNodes(TreeNode selectedNode, TreeViewTag tv, int assurId, string detail, bool includeContracts)
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

                    List<string> contractsThatBelongToAssur = contractsForComp.Where(i => contractNamesForAssur.Contains(i)).ToList();
                    int cnt = contractsThatBelongToAssur.Count();

                    if (cnt == 0)
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
                    
                    if (!NodeWithTextExistsAlready(selectedNode, childCompNode.Text))
                        selectedNode.ChildNodes.Add(childCompNode);

                    if (includeContracts)
                    {
                        LoadAllContractNodes(childCompNode, tv, assurId, detail);
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

        private void LoadAllContractNodes(TreeNode selectedNode, TreeViewTag tv, int assurId, string detail)
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
                //clear all child nodes
                //selectedNode.ChildNodes.Clear();

                //get all CR's that are linked to the parent company
                List<CompteResult> parentCompCR = CompteResult.GetComptesResultatForParentCompany(parentCompanyId);

                foreach (CompteResult cr in parentCompCR)
                {
                    // *** Create CR Node for Parent Comp
                    tv = new TreeViewTag();
                    tv.Name = cr.Name;
                    tv.Id = cr.Id;
                    tv.CRParentCompId = cr.ParentCompanyId;
                    tv.CRCollegeId = cr.CollegeId;
                    tv.CRContractIds = cr.ContractIds;
                    tv.CRCreationDate = cr.CreationDate;
                    tv.NodeType = C.eTVNodeTypes.CompteResultat;
                    tv.CRReportLevelId = cr.ReportLevelId;
                    tv.CRSubsids = cr.SubsidIds;
                    tv.AssureurId = assurId;

                    tv.TaxDef = cr.TaxDefault;
                    tv.TaxAct = cr.TaxActif;
                    tv.TaxPer = cr.TaxPerif;

                    //verify if the CR belongs to the current Assur
                    if (string.IsNullOrWhiteSpace(cr.AssurIds))
                        return;

                    List<string> assurIds = Regex.Split(cr.AssurIds, C.cVALSEP).ToList();
                    if (!assurIds.Contains(assurId.ToString()))
                        continue;


                    //data from CRPlanning
                        foreach (CRPlanning crp in cr.CRPlannings)
                    {
                        tv.CRPs.Add(new CRPlanning { CRId = cr.Id, DebutPeriode = crp.DebutPeriode, FinPeriode = crp.FinPeriode, DateArret = crp.DateArret });
                    }

                    TreeNode parentCRNode = new TreeNode(cr.Name, tv.GetStringFromObject());
                    parentCRNode.SelectAction = TreeNodeSelectAction.Select;
                    parentCRNode.ImageUrl = C.imageRelFolder + "cr.jpg";

                    //###
                    //if (crId != C.cINVALIDID && crId == cr.Id)
                    //{
                    //    parentCRNode.Selected = true;
                    //    Session["SelectedCompteResultatNode"] = tv;
                    //}


                    if(!NodeWithTextExistsAlready(selectedNode, parentCRNode.Text))
                        selectedNode.ChildNodes.Add(parentCRNode);
                   
                }


                selectedNode.Expand();
                txtDetails.Text = detail;
                return;
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


        protected void cmdCreateCR_Click(object sender, EventArgs e)
        {
            TreeViewTag tv;
            int selectedParentCompanies = 0;
            BLCompteResultat myCR;
            List<BLCompteResultat> crs = new List<BLCompteResultat>();
            List<string> compIds;
            List<string> compNames;
            List<string> subsidIds;
            List<string> subsidNames;
            List<string> contractIds;
            List<string> contractNames;
            List<string> assurIds;
            List<string> assurNames;

            try
            {
                //### Validation
                //If a specific parent company is selected, At least 1 corresponding contract needs to be selected

                bool isModeConso = chkComptesConsol.Checked;

                myCR = new BLCompteResultat();
                compIds = new List<string>();
                compNames = new List<string>();
                assurIds = new List<string>();
                assurNames = new List<string>();
                subsidIds = new List<string>();
                subsidNames = new List<string>();
                contractIds = new List<string>();
                contractNames = new List<string>();

                //find out how many Parent Companie shave been selected - if there is more than one, we need to adapt the CR Names
                foreach (TreeNode nodeAssur in tvContracts.Nodes)
                {
                    if (nodeAssur.Checked)
                    {
                        tv = TreeViewTag.GetObjectFromString(nodeAssur.Value);
                        if (!assurNames.Contains(tv.Name) && !assurIds.Contains(tv.Id.ToString()))
                        {
                            assurNames.Add(tv.Name);
                            assurIds.Add(tv.Id.ToString());
                        }
                    }

                    foreach (TreeNode nodePC in nodeAssur.ChildNodes)
                    {
                        if (nodePC.Checked)
                        {
                            tv = TreeViewTag.GetObjectFromString(nodePC.Value);
                            if (!compNames.Contains(tv.Name) && !compIds.Contains(tv.Id.ToString()))
                            {
                                compNames.Add(tv.Name);
                                compIds.Add(tv.Id.ToString());

                                if (!assurIds.Contains(tv.Id.ToString()))
                                {
                                    //assurNames.Add(tv.Name);
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
                }

                if(compNames.Count == 0) //(selectedParentCompanies == 0)
                    throw new Exception("Vous devriez sélectionner au moins une entreprise pour générer des comptes de résultats !");

                //iterate all selected treenodes - the first lavel is assurance            
                foreach (TreeNode nodeAssur in tvContracts.Nodes)
                {

                    string savePath = "";

                    //next level: parent companies
                    foreach (TreeNode nodePC in nodeAssur.ChildNodes)
                    {
                        if (nodePC.Checked)
                        {
                            tv = TreeViewTag.GetObjectFromString(nodePC.Value);  

                            //FinPeriode = DateTime.ParseExact(txtStartPeriode.Value, "d", null)

                            //add the CRPlanning data                            
                            myCR.CRPlannings.Add(new CRPlanning
                            {
                                DebutPeriode = DateTime.Parse(txtStartPeriode.Value),
                                FinPeriode = DateTime.Parse(txtEndPeriode.Value),
                                DateArret = DateTime.Parse(txtArretCompte.Value)
                            });

                            myCR.ParentCompanyId = tv.Id;
                            myCR.ReportLevelId = int.Parse(cmbDetailReport.SelectedItem.Value);
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

                            savePath = Server.MapPath(C.excelCRFolder);

                            if(!string.IsNullOrWhiteSpace(User.Identity.Name))
                                savePath = Path.Combine(savePath, User.Identity.Name);

                            //### ???
                            if (selectedParentCompanies > 1)
                                myCR.Name = txtNameReport.Value; // + "_" + tv.Name;
                            else
                                myCR.Name = txtNameReport.Value;

                            //next level: contracts belonging to PC and subsids
                            foreach (TreeNode nodeSubsid in nodePC.ChildNodes)
                            {
                                if (nodeSubsid.Checked)
                                {
                                    tv = TreeViewTag.GetObjectFromString(nodeSubsid.Value);

                                    if (tv.NodeType == C.eTVNodeTypes.Subsid)
                                    {
                                        subsidIds.Add(tv.Id.ToString());
                                        subsidNames.Add(tv.Name);

                                        foreach (TreeNode nodeContr in nodeSubsid.ChildNodes)
                                        {
                                            tv = TreeViewTag.GetObjectFromString(nodeContr.Value);
                                            if (nodeContr.Checked)
                                            {
                                                contractIds.Add(tv.Id.ToString());
                                                contractNames.Add(tv.Name);

                                                if (!assurIds.Contains(tv.AssureurId.ToString()))
                                                {
                                                    assurIds.Add(tv.AssureurId.ToString());
                                                    //assurNames.Add(tv.Name);
                                                }
                                            }
                                        }
                                    }
                                    else if (tv.NodeType == C.eTVNodeTypes.Contract)
                                    {
                                        contractIds.Add(tv.Id.ToString());
                                    }
                                }
                            }

                            //verify if all CR's contain at least 1 contractId                            
                            if (contractIds.Count == 0)
                                throw new Exception("Chaque entreprise pour laquelle vous voudrais créer un compte de résultat doit contenir au moins un contrat !");

                            myCR.SubsidIds = string.Join(C.cVALSEP, subsidIds);
                            myCR.SubsidNames = string.Join(C.cVALSEP, subsidNames);
                            myCR.ContractIds = string.Join(C.cVALSEP, contractIds);
                            myCR.ContractNames = string.Join(C.cVALSEP, contractNames);
                            myCR.AssurIds = string.Join(C.cVALSEP, assurIds);
                           // myCR.AssurNames = string.Join(C.cVALSEP, assurNames);

                            //we also add the server path
                            myCR.ExcelTemplatePath = Server.MapPath(C.reportTemplateFolder);
                            myCR.ExcelCRPath = Server.MapPath(C.excelCRFolder);
                            myCR.CalculateProvision = chkCalcProv.Checked;

                            crs.Add(myCR);


                            //Add newly created CR to Tree
                            TreeNode parentCRNode = new TreeNode(myCR.Name, tv.GetStringFromObject());
                            parentCRNode.SelectAction = TreeNodeSelectAction.Select;
                            parentCRNode.ImageUrl = C.imageRelFolder + "cr.jpg";

                            if (!NodeWithTextExistsAlready(nodePC, parentCRNode.Text))
                                nodePC.ChildNodes.AddAt(0, parentCRNode);                            
                        }
                    }
                } //end of assur node iteration

                //Call method in BL & pass the List of CRs
                int crId = 0;
                foreach (BLCompteResultat cr in crs)
                {
                    crId = cr.CreateNewCompteResultat();
                }


                //### Add newly created CR to Tree   
                //PopulateTreeViewControl(crId);

            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::cmdCreateCR_Click"); }
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
                    chkCalcProv.Visible = true;

                    taxControls.Visible = false;                    
                }
                else
                {
                    chkCalcProv.Visible = false;
                    //calculateProv = false;

                    taxControls.Visible = true;
                }
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "ShowHideCalcProv", false); }
        }


        #region OLD METHODS

        private void DownloadFile(string filePath)
        {
            try
            {
                FileInfo file = new FileInfo(filePath);
                if (file.Exists)
                {
                    if (false)
                    {
                        Response.Clear();
                        Response.ClearHeaders();
                        Response.ClearContent();

                        //Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" + file.Name);
                        //Response.AddHeader("Content-Type", "application/Excel");
                        Response.AddHeader("Content-Length", file.Length.ToString());

                        Response.ContentType = "application/ms-excel";


                        Response.Flush();
                        //Response.TransmitFile(file.FullName);
                        Response.WriteFile(file.FullName);
                        Response.End();
                    }


                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.ContentType = "application/vnd.ms-powerpoint.presentation.macroEnabled.12";
                    Response.AppendHeader("content-disposition", "attachment; filename=" + file.Name);

                    string FilePath = Server.MapPath("") + @"\App_Data\ExcelCR\" + file.Name;
                    //Write the file directly to the HTTP content output stream.
                    //Response.WriteFile(file.FullName);
                    //Response.WriteFile(FilePath);
                    Response.TransmitFile(file.FullName);
                    Response.End();
                }
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::DownloadFile", false); }

        }

        private void oldPopulateTreeViewControl(int crId)
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
                    List<string> contractNamesForAssur = Assureur.GetAllContractNamesForAssureur(myAssur.Id);

                    // *** Create Assur Node
                    tv = new TreeViewTag();
                    tv.Name = myAssur.Name;
                    tv.Id = myAssur.Id;
                    tv.NodeType = C.eTVNodeTypes.Assureur;

                    TreeNode assurNode = new TreeNode(myAssur.Name, tv.GetStringFromObject());
                    assurNode.SelectAction = TreeNodeSelectAction.Select;
                    assurNode.ImageUrl = C.imageRelFolder + "assur.png";

                    //get all parent companies that are linked to this assureur
                    List<Company> parentCompaniesForAssur = Company.GetParentCompaniesForAssureur(myAssur);

                    foreach (Company parentComp in parentCompaniesForAssur)
                    {
                        // *** Create Parent Comp Node
                        tv = new TreeViewTag();
                        tv.Name = parentComp.Name;
                        tv.Id = parentComp.Id;
                        tv.NodeType = C.eTVNodeTypes.ParentCompany;

                        TreeNode parentCompNode = new TreeNode(parentComp.Name, tv.GetStringFromObject());
                        parentCompNode.SelectAction = TreeNodeSelectAction.Select;
                        parentCompNode.ImageUrl = C.imageRelFolder + "company.png";
                        parentCompNode.ShowCheckBox = true;

                        //get all CR's that are linked to the parent company
                        List<CompteResult> parentCompCR = CompteResult.GetComptesResultatForParentCompany(parentComp.Id);

                        foreach (CompteResult cr in parentCompCR)
                        {
                            // *** Create CR Node for Parent Comp
                            tv = new TreeViewTag();
                            tv.Name = cr.Name;
                            tv.Id = cr.Id;
                            tv.CRParentCompId = cr.ParentCompanyId;
                            tv.CRCollegeId = cr.CollegeId;
                            tv.CRContractIds = cr.ContractIds;
                            tv.CRCreationDate = cr.CreationDate;
                            tv.NodeType = C.eTVNodeTypes.CompteResultat;
                            tv.CRReportLevelId = cr.ReportLevelId;
                            tv.CRSubsids = cr.SubsidIds;

                            tv.TaxDef = cr.TaxDefault;
                            tv.TaxAct = cr.TaxActif;
                            tv.TaxPer = cr.TaxPerif;

                            //data from CRPlanning
                            foreach (CRPlanning crp in cr.CRPlannings)
                            {
                                tv.CRPs.Add(new CRPlanning { CRId = cr.Id, DebutPeriode = crp.DebutPeriode, FinPeriode = crp.FinPeriode, DateArret = crp.DateArret });
                            }

                            TreeNode parentCRNode = new TreeNode(cr.Name, tv.GetStringFromObject());
                            parentCRNode.SelectAction = TreeNodeSelectAction.Select;
                            parentCRNode.ImageUrl = C.imageRelFolder + "cr.jpg";

                            if (crId != C.cINVALIDID && crId == cr.Id)
                            {
                                parentCRNode.Selected = true;
                                //cmdDeleteCR.Visible = true;
                                //cmdStartExcel.Visible = true;
                                //cmdStartPPT.Visible = true;

                                Session["SelectedCompteResultatNode"] = tv;
                            }

                            parentCompNode.ChildNodes.Add(parentCRNode);
                        }


                        //display contracts underneath the parent company, but only if a certain contract does not belong to a subsid
                        //###                        
                        //List<Contract> contractsParent = Company.GetContractsForCompany(parentComp.Id, true);
                        //if (contractsParent != null)
                        //{
                        //    foreach (Contract contr in contractsParent)
                        //    {
                        //        // *** Create Contract Node
                        //        tv = new TreeViewTag();
                        //        tv.Name = contr.ContractId;
                        //        tv.Id = contr.Id;
                        //        tv.NodeType = C.eTVNodeTypes.Contract;

                        //        TreeNode parentContractNode = new TreeNode(contr.ContractId, tv.GetStringFromObject());
                        //        parentContractNode.SelectAction = TreeNodeSelectAction.Select;
                        //        parentContractNode.ImageUrl = C.imageRelFolder + "contract.png";
                        //        parentContractNode.ShowCheckBox = true;

                        //        parentCompNode.ChildNodes.Add(parentContractNode);
                        //    }
                        //}



                        //get all child companies
                        //### display only child companies that contain contracts for this specific assur
                        List<Company> childComps = Company.GetChildCompanies(parentComp.Id);

                        foreach (Company childComp in childComps)
                        {
                            //### optimize code - reduce DB requests
                            // get all contracts for this child comp => is there at least one contract that belongs to assur - otherwise quit loop                            
                            List<string> contractsForComp = Company.GetContractsNamesForCompany(childComp.Id);

                            List<string> contractsThatBelongToAssur = contractsForComp.Where(i => contractNamesForAssur.Contains(i)).ToList();
                            int cnt = contractsThatBelongToAssur.Count();

                            if (cnt == 0)
                                continue;


                            // *** Create Subsid Node
                            tv = new TreeViewTag();
                            tv.Name = childComp.Name;
                            tv.Id = childComp.Id;
                            tv.ParentCompId = childComp.ParentId;
                            tv.NodeType = C.eTVNodeTypes.Subsid;

                            TreeNode childCompNode = new TreeNode(childComp.Name, tv.GetStringFromObject());
                            childCompNode.SelectAction = TreeNodeSelectAction.Select;
                            childCompNode.ImageUrl = C.imageRelFolder + "subsid.png";
                            childCompNode.ShowCheckBox = true;


                            //### test for each contract if it belongs to the current assureur
                            //display contracts underneath the subsid
                            List<Contract> contracts = Company.GetContractsForCompany(childComp.Id, false);
                            foreach (Contract contr in contracts)
                            {
                                //verify if this contract belongs to the Assureur
                                if (!contractsThatBelongToAssur.Contains(contr.ContractId))
                                    continue;

                                // *** Create Contract Node
                                tv = new TreeViewTag();
                                tv.Name = contr.ContractId;
                                tv.Id = contr.Id;
                                tv.NodeType = C.eTVNodeTypes.Contract;

                                TreeNode contractNode = new TreeNode(contr.ContractId, tv.GetStringFromObject());
                                contractNode.SelectAction = TreeNodeSelectAction.Select;
                                contractNode.ImageUrl = C.imageRelFolder + "contract.png";
                                contractNode.ShowCheckBox = true;

                                childCompNode.ChildNodes.Add(contractNode);
                            }


                            //### add only if there are any contracts
                            parentCompNode.ChildNodes.Add(childCompNode);
                            childCompNode.Expand();

                            // NOTE: we don't display CR's underneath a subsid => all CR's are displayed under the corresponding Parent Company

                        }

                        assurNode.ChildNodes.Add(parentCompNode);
                        parentCompNode.Expand();

                    }

                    tvContracts.Nodes.Add(assurNode);
                    assurNode.Expand();
                }
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "CompteResultatManuel::PopulateTreeViewControl"); }
        }

        #endregion
    }
}