using System;
using System.Collections.Generic;
using System.Data;
using CompteResultat.DAL;
using CompteResultat.Common;
using OfficeOpenXml;

namespace CompteResultat.BL
{
    public class BLFraisPrev
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ExcelPackage ExportFraisPrev()
        {
            try
            {
                List<FraisPrevoyance> fraisPrev = FraisPrevoyance.GetFraisPrevoyance();

                ExcelPackage pck = new ExcelPackage();
                var ws = pck.Workbook.Worksheets.Add("FraisPrevoyance");

                //write the header
                ws.Cells[1, 1].Value = "AnneeSurvenance";
                ws.Cells[1, 2].Value = "TypeSinistre";
                ws.Cells[1, 3].Value = "Frais";

                int row = 2;

                foreach (FraisPrevoyance tp in fraisPrev)
                {
                    ws.Cells[row, 1].Value = tp.AnneeSurvenance;
                    ws.Cells[row, 2].Value = tp.TypeSinistre;
                    ws.Cells[row, 3].Value = tp.Frais;
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

        public static void ImportFraisPrev(string excelFilePath, bool firstRowAsColumnNames)
        {
            try
            {
                int anneeSurvenance;
                string typeSinistre;
                double frais;

                //read Excel file into datatable
                DataTable dt = G.ExcelToDataTable(excelFilePath, firstRowAsColumnNames);

                // delete all rows in DB Tables with the specified assurName 
                FraisPrevoyance.DeleteFraisPrevoyance();
                
                foreach (DataRow row in dt.Rows)
                {
                    anneeSurvenance = int.Parse(row["AnneeSurvenance"].ToString());
                    typeSinistre = row["TypeSinistre"].ToString();
                    frais = double.Parse(row["Frais"].ToString());

                    int id = FraisPrevoyance.InsertFraisPrevoyance(new FraisPrevoyance
                    {
                        AnneeSurvenance = anneeSurvenance,
                        TypeSinistre = typeSinistre,
                        Frais = frais
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
