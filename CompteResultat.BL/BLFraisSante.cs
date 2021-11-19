using System;
using System.Collections.Generic;
using System.Data;
using CompteResultat.DAL;
using CompteResultat.Common;
using OfficeOpenXml;

namespace CompteResultat.BL
{
    public class BLFraisSante
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ExcelPackage ExportFraisSante()
        {
            try
            {
                List<FraisSante> fraisSante = FraisSante.GetFraisSante();

                ExcelPackage pck = new ExcelPackage();
                var ws = pck.Workbook.Worksheets.Add("FraisSante");

                //write the header
                ws.Cells[1, 1].Value = "AnneeSurvenance";
                ws.Cells[1, 2].Value = "TypeFraisTaxes";
                ws.Cells[1, 3].Value = "Frais";

                int row = 2;

                foreach (FraisSante tp in fraisSante)
                {
                    ws.Cells[row, 1].Value = tp.AnneeSurvenance;
                    ws.Cells[row, 2].Value = tp.TypeFraisTaxes;
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

        public static void ImportFraisSante(string excelFilePath, bool firstRowAsColumnNames)
        {
            try
            {
                int anneeSurvenance;
                string typeFraisTaxes;
                double frais;

                //read Excel file into datatable
                DataTable dt = G.ExcelToDataTable(excelFilePath, firstRowAsColumnNames);

                // delete all rows in DB Tables with the specified assurName 
                FraisSante.DeleteFraisSante();
                
                foreach (DataRow row in dt.Rows)
                {
                    anneeSurvenance = int.Parse(row["AnneeSurvenance"].ToString());
                    typeFraisTaxes = row["TypeFraisTaxes"].ToString();
                    frais = double.Parse(row["Frais"].ToString());

                    int id = FraisSante.InsertFraisSante(new FraisSante
                    {
                        AnneeSurvenance = anneeSurvenance,
                        TypeFraisTaxes = typeFraisTaxes,
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
