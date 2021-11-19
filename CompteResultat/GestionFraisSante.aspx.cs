using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

using CompteResultat.BL;
using CompteResultat.DAL;
using CompteResultat.Common;
using System.Web.DynamicData;
using System.Web.ModelBinding;
using OfficeOpenXml.Style;
using OfficeOpenXml;


namespace CompteResultat
{
    public partial class GestionFraisSante : System.Web.UI.Page
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                cmdImport.Attributes.Add("onclick", "jQuery('#" + uploadExcel.ClientID + "').click();return false;");                

                if (IsPostBack )
                {                                  
                    //Handle the Delete Event
                    if (Request.Form["cmdDelete"] != null) {  }                       

                    //Handle the import of groups & guarantees
                    if (uploadExcel.PostedFile != null)
                    {
                        if (uploadExcel.PostedFile.FileName.Length > 0)
                        {
                            //Import Garanties
                            string excelFile = Path.GetFileName(uploadExcel.PostedFile.FileName);
                            
                            string excelDirectory = Path.Combine(Request.PhysicalApplicationPath, C.excelFolder);
                            string fullUploadPath = Path.Combine(excelDirectory, excelFile);
                            uploadExcel.PostedFile.SaveAs(fullUploadPath);

                            BLFraisSante.ImportFraisSante(fullUploadPath, true);                            
                        }
                    }                                   
                }
            }
            catch (Exception ex)
            {
                var myCustomValidator = new CustomValidator();
                myCustomValidator.IsValid = false;
                myCustomValidator.ErrorMessage = ex.Message;
                Page.Validators.Add(myCustomValidator);
            }
        }

        public IEnumerable<FraisSante> GetFS()
        {
            try
            {
                return FraisSante.GetFraisSante();
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "GestionFraisSante::GetFS"); return null; }
        }

        protected void rptFS_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Repeater rpt = sender as Repeater; // Get the Repeater control object.
            
            // If the Repeater contains no data.
            if (rpt != null)
            {
                if (e.Item.ItemType == ListItemType.Footer)
                {
                    if (rpt.Items.Count < 1)
                    {
                        rptFS.Visible = false;
                        phHeader.Visible = true;
                    }
                    else
                    {
                        rptFS.Visible = true;
                        phHeader.Visible = false;
                    }
                }
            }
        }

        protected void cmdExport_Click(object sender, EventArgs e)
        {
            string uploadPath = "";

            try
            { 
                ExcelPackage pack = BLFraisSante.ExportFraisSante();
                
                uploadPath = Path.Combine(Request.PhysicalApplicationPath, C.uploadFolder);
                uploadPath = Path.Combine(uploadPath, User.Identity.Name + "_FraisSante.xlsx");
                              
                pack.SaveAs(new FileInfo(uploadPath));

                UICommon.DownloadFile(this.Page, uploadPath);
            }
            catch (Exception ex)
            {                
                if (ex.Message != "Thread was being aborted.")
                    log.Error(ex.Message);

                var myCustomValidator = new CustomValidator();
                myCustomValidator.IsValid = false;
                myCustomValidator.ErrorMessage = ex.Message;
                Page.Validators.Add(myCustomValidator);
            }
        }

        protected void cmdDelete_Click(object sender, EventArgs e)
        {            
            try
            {
                FraisSante.DeleteFraisSante();
                rptFS.DataBind();
            }
            catch (Exception ex)
            {                
                log.Error(ex.Message);

                var myCustomValidator = new CustomValidator();
                myCustomValidator.IsValid = false;
                myCustomValidator.ErrorMessage = ex.Message;
                Page.Validators.Add(myCustomValidator);
            }
        }               
       
    }
}