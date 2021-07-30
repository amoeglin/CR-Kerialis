using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Data;

using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Text.RegularExpressions;
using System.Globalization;

using Excel;
using CompteResultat.DAL;
using CompteResultat.Common;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CompteResultat.BL
{
    public class BLTypePrev
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ExcelPackage ExportTypePrev()
        {
            try
            {
                List<TypePrevoyance> trpePrev = TypePrevoyance.GetTypePrev();

                ExcelPackage pck = new ExcelPackage();
                var ws = pck.Workbook.Worksheets.Add("TypePrevoyance");

                //write the header
                ws.Cells[1, 1].Value = "CodeSinistre";
                ws.Cells[1, 2].Value = "LabelSinistre";

                int row = 2;

                foreach (TypePrevoyance tp in trpePrev)
                {
                    ws.Cells[row, 1].Value = tp.CodeSinistre;
                    ws.Cells[row, 2].Value = tp.LabelSinistre;                   
                    row++;
                }

                return pck;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void RecreateTypePrevoyanceFromSinistre()
        {
            try
            {               
                //Update table TypePrevoyance => search for new entries in SinistrePrev and add the with CodeSinistre: "AUTRES"
                List<string> sinLabelsFromTypePrev = TypePrevoyance.GetSinistreLabels();
                List<string> sinLabelsFromSinPrev = SinistrePrev.GetSinistreLabels();

                //add data to GroupGarantySante
                // ### Paramètres par défaut
                foreach (string item in sinLabelsFromSinPrev)
                {
                    if(!sinLabelsFromTypePrev.Contains(item))
                    {
                        int id = TypePrevoyance.InsertTypePrev(new TypePrevoyance
                        {
                            CodeSinistre = "AUTRES",
                            LabelSinistre = item
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void ImportTypePrev(string excelFilePath, bool firstRowAsColumnNames)
        {
            try
            {
                string codeSinistre;
                string labelSinistre;                

                //read Excel file into datatable
                DataTable dt = G.ExcelToDataTable(excelFilePath, firstRowAsColumnNames);

                // delete all rows in DB Tables with the specified assurName 
                TypePrevoyance.DeleteTypePrevoyance();
                
                foreach (DataRow row in dt.Rows)
                {
                    codeSinistre = row[C.eExcelTypePrev.CodeSinistre.ToString()].ToString();
                    labelSinistre = row[C.eExcelTypePrev.LabelSinistre.ToString()].ToString();
                    
                    int id = TypePrevoyance.InsertTypePrev(new TypePrevoyance
                    {
                        CodeSinistre = codeSinistre,
                        LabelSinistre = labelSinistre
                    });
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }


    }
}
