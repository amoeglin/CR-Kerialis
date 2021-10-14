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
    public class BLCadencier
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static void ImportCadencierForAssureur(string assureurName, string excelFilePath, bool firstRowAsColumnNames)
        {
            try
            {
                int year;
                DateTime debutSurv;
                DateTime finSurv;
                int month;
                double cumul;

                //read Excel file into datatable
                DataTable dt = G.ExcelToDataTable(excelFilePath, firstRowAsColumnNames);

                // delete all rows in DB Tables with the specified assurName 
                Cadencier.DeleteCadencierWithSpecificAssureurName(assureurName);

                foreach (DataRow row in dt.Rows)
                {
                    //### validate => all fields must be specified                    
                    //codeActe = row[C.eExcelGroupsGaranties.CodeActe.ToString()].ToString();

                    if (!Int32.TryParse(row[C.eExcelCadencier.Year.ToString()].ToString(), out year))
                        throw new Exception("One of the provided 'Year' values is not valid for the Cadencier you are trying to import !");

                    if (!Int32.TryParse(row[C.eExcelCadencier.Month.ToString()].ToString(), out month))
                        throw new Exception("One of the provided 'Month' values is not valid for the Cadencier you are trying to import !");

                    if (!double.TryParse(row[C.eExcelCadencier.Cumul.ToString()].ToString(), out cumul))
                        throw new Exception("One of the provided 'Cumul' values is not valid for the Cadencier you are trying to import !");

                    if (!DateTime.TryParse(row[C.eExcelCadencier.DebutSurvenance.ToString()].ToString(), out debutSurv))
                        throw new Exception("One of the provided 'DebutSurvenance' values is not valid for the Cadencier you are trying to import !");

                    if (!DateTime.TryParse(row[C.eExcelCadencier.FinSurvenance.ToString()].ToString(), out finSurv))
                        throw new Exception("One of the provided 'FinSurvenance' values is not valid for the Cadencier you are trying to import !");

                    int id = Cadencier.InsertCadencier(new Cadencier
                    {
                        AssureurName = assureurName,
                        Year=year,
                        DebutSurvenance=debutSurv,
                        FinSurvenance=finSurv,
                        Month=month,
                        Cumul=cumul                        
                    });
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static ExcelPackage ExportCadencierForAssureur(string assureurName)
        {
            try
            {
                List<Cadencier> cad = Cadencier.GetCadencierForAssureur(assureurName);

                ExcelPackage pck = new ExcelPackage();
                var ws = pck.Workbook.Worksheets.Add(assureurName);

                //write the header
                //ws.Cells["A3"].Style.Numberformat.Format = "yyyy-mm-dd";
                //ws.Column(2).Style.Numberformat.Format = "dd-mm-yyyy";
                //ws.Column(3).Style.Numberformat.Format = "dd-mm-yyyy";
                ws.Column(2).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                ws.Column(3).Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;

                ws.Cells[1, 1].Value = "Year";
                ws.Cells[1, 2].Value = "DebutSurvenance";
                ws.Cells[1, 3].Value = "FinSurvenance";
                ws.Cells[1, 4].Value = "Month";
                ws.Cells[1, 5].Value = "Cumul";

                int row = 2;

                foreach (Cadencier c in cad)
                {
                    ws.Cells[row, 1].Value = c.Year;
                    ws.Cells[row, 2].Value = c.DebutSurvenance;
                    ws.Cells[row, 3].Value = c.FinSurvenance;
                    ws.Cells[row, 4].Value = c.Month;
                    ws.Cells[row, 5].Value = c.Cumul;

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

        public static void RecreateCadencier()
        {
            try
            {                
                List<double?> lstSommePresta = new List<double?>();
                double? res = 0;
                double? cumulTotal = 0;
                double? sommePresta = 0;
                double? coeffPSAP = 0;
                //string assName = "";

                //get Presta
                List<CumulPresta> presta = PrestSante.CumulPrestaData();
                List<string> assNames = presta.GroupBy(p => new { p.AssureurName })
                    .Select(g => g.Key.AssureurName)
                    .ToList();
                List<int> years = presta.GroupBy(p => new { p.AnneeSoins })
                    .Select(g => g.Key.AnneeSoins)
                    .ToList();

                //if (presta.Any())
                //    assName = presta.First().AssureurName;

                //get last year
                if (years.Count == 0)
                    return;

                int maxYear = years.Max();

                foreach (string assName in assNames)
                {
                    lstSommePresta = new List<double?>();
                    cumulTotal = 0;

                    List<double> yearsCumul = new List<double>(years.Count);
                    foreach (int year in years)
                    {
                        yearsCumul.Add(0);
                    }

                    //delete Cad for specific year
                    Cadencier.DeleteCadencierForSpecificYear(maxYear, assName);

                    //re-create Cad for that year
                    int maxMonth = presta.Max(p => p.MoisReglement);

                    for (int i = 1; i <= maxMonth; i++)
                    {
                        sommePresta = 0;
                        int yearCount = 0;
                        foreach (int year in years)
                        {
                            res = presta.Where(p => p.AssureurName == assName && p.AnneeSoins == year && p.MoisReglement == i).Select(p => p.SommePresta).FirstOrDefault().HasValue ?
                                presta.Where(p => p.AssureurName == assName && p.AnneeSoins == year && p.MoisReglement == i).Select(p => p.SommePresta).FirstOrDefault() : 0;

                            sommePresta += res.HasValue ? res.Value : 0;

                            yearsCumul[yearCount] += res.HasValue ? res.Value : 0;

                            yearCount++;
                        }

                        lstSommePresta.Add(sommePresta.HasValue ? sommePresta.Value : 0);
                    }

                    //calculate CumulTotal => somme des prestations 
                    foreach (double annualTotal in yearsCumul)
                    {
                        cumulTotal += annualTotal;
                    }

                    //complete missing columns                
                    double? cumPresta = 0;
                    int month = 1;
                    foreach (double sumPresta in lstSommePresta)
                    {
                        //presta moyenne %
                        if (cumulTotal != 0)
                        {
                            //presta cumulées %
                            cumPresta += sumPresta * 100 / cumulTotal;
                        }

                        //Coeff PSAP %
                        if (cumPresta != 0)
                        {
                            coeffPSAP = (100 - cumPresta) / cumPresta;
                        }
                        else
                        {
                            coeffPSAP = 0;
                        }

                        int id = Cadencier.InsertCadencier(new Cadencier
                        {
                            AssureurName = assName,
                            Year = maxYear,
                            DebutSurvenance = new DateTime(maxYear, 1, 1),
                            FinSurvenance = new DateTime(maxYear, 12, 31),
                            Month = month,
                            Cumul = Math.Round(coeffPSAP.Value, 4)
                        });

                        month++;
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static bool CadencierIsUpToDate(ref List<int> missingYears, string txtStartPeriode, string txtEndPeriode)
        {
            DateTime debutPeriode = DateTime.Parse(txtStartPeriode);
            DateTime finPeriode = DateTime.Parse(txtEndPeriode);

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
            List<int> years = new List<int>();
            for (int i = 0; i <= finPeriode.Year - debutPeriode.Year; i++)
            {
                years.Add(debutPeriode.Year + i);
            }
            
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

            return cadExists;
        }

        public static ExcelPackage ExportCadencier()
        {
            try
            {
                int colCounter = 2;
                double cumulTotal = 0;
                double? sommePresta = 0;
                double? res = 0;

                //get Presta
                List<CumulPresta> presta = PrestSante.CumulPrestaData();
                List<string> assNames = presta.GroupBy(p => new { p.AssureurName })
                    .Select(g => g.Key.AssureurName)
                    .ToList();
                List<int> years = presta.GroupBy(p => new { p.AnneeSoins })
                    .Select(g => g.Key.AnneeSoins)
                    .ToList();

                List<double> yearsCumul = new List<double>(years.Count);
                foreach (int year in years)
                {
                    yearsCumul.Add(0);
                }

                int maxMonth = presta.Max(p => p.MoisReglement);

                List<double> lstSommePresta = new List<double>();

                ExcelPackage pck = new ExcelPackage();
                var ws = pck.Workbook.Worksheets.Add("PRESTATIONS");

                //write the header 
                ws.Cells[1, 1].Value = "Mois de règlement";
                foreach (int year in years)
                {
                    ws.Cells[1, colCounter].Value = year.ToString();
                    colCounter++;
                }
                ws.Cells[1, colCounter++].Value = "Somme des prestations";

                ws.Column(colCounter).Style.Numberformat.Format = "#0\\.00%";
                ws.Cells[1, colCounter++].Value = "Prestations moyenne";

                ws.Column(colCounter).Style.Numberformat.Format = "#0\\.00%";
                ws.Cells[1, colCounter++].Value = "Prestations cumulées";

                ws.Column(colCounter).Style.Numberformat.Format = "#0\\.00%";
                ws.Cells[1, colCounter++].Value = "Taux PSAP";

                ws.Cells[1, colCounter++].Value = "Coefficient PSAP";

                int row = 2;

                for(int i=1; i<=maxMonth; i++)
                {
                    colCounter = 2;
                    sommePresta = 0;

                    ws.Cells[row, 1].Value = i.ToString();
                    int yearCount = 0;
                    foreach (int year in years)
                    {
                        //### get values for all assureurs
                        res = 0;
                        foreach (string assName in assNames)
                        {
                            res += presta.Where(p => p.AssureurName == assName && p.AnneeSoins == year && p.MoisReglement == i).Select(p => p.SommePresta).FirstOrDefault().HasValue ?
                            presta.Where(p => p.AssureurName == assName && p.AnneeSoins == year && p.MoisReglement == i).Select(p => p.SommePresta).FirstOrDefault() : 0;
                        }

                        ws.Cells[row, colCounter].Value = res;
                        sommePresta += res;

                        yearsCumul[yearCount] += res.HasValue ? res.Value : 0;

                        yearCount++;
                        colCounter++;
                    }

                    //Somme Presta
                    ws.Cells[row, colCounter++].Value = sommePresta;
                    lstSommePresta.Add(sommePresta.HasValue ? sommePresta.Value : 0);

                    row++;
                }

                //calculate CumulTotal => somme des prestations && write the last line => cumul
                colCounter = 2;
                foreach (double annualTotal in yearsCumul)
                {
                    ws.Cells[row, colCounter].Value = annualTotal;

                    cumulTotal += annualTotal;
                    colCounter++;
                }

                ws.Cells[row, colCounter++].Value = cumulTotal;
                ws.Cells[row, colCounter].Value = 100;

                //complete missing columns
                row = 2;
                double cumPresta = 0;
                foreach (double sumPresta in lstSommePresta)
                {
                    //presta moyenne %
                    if (cumulTotal != 0)
                    {
                        ws.Cells[row, colCounter].Value = sumPresta * 100 / cumulTotal;
                        //presta cumulées %
                        cumPresta += sumPresta * 100 / cumulTotal;
                        ws.Cells[row, colCounter + 1].Value = cumPresta;
                    } else
                    {
                        ws.Cells[row, colCounter].Value = 0;
                        ws.Cells[row, colCounter + 1].Value = 0;
                    }                    

                    //Taux PSAP %
                    ws.Cells[row, colCounter + 2].Value = 100-cumPresta;

                    //Coeff PSAP %
                    if (cumPresta != 0) {
                        ws.Cells[row, colCounter + 3].Value = (100 - cumPresta) / cumPresta;
                    } else
                    {
                        ws.Cells[row, colCounter + 3].Value = 0;
                    }

                    row++;
                }                

                //Format table => first line bold
                ws.Row(1).Style.Font.Bold = true;
                ws.Row(1).Style.Font.Size = 12;

                //last column Yellow et bold
                for (int i = 1; i <= maxMonth + 1; i++)
                {
                    ws.Cells[i, colCounter + 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //ws.Cells[i, colCounter + 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
                    ws.Cells[i, colCounter + 3].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                    ws.Cells[i, colCounter + 3].Style.Font.Bold = true;
                    ws.Cells[i, colCounter + 3].Style.Numberformat.Format = "0.0000";
                }

                //years in header, background yellow
                for (int i = 2; i <= years.Count+1; i++)
                {
                    ws.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[1, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);

                    ws.Cells[maxMonth + 2, i].Style.Numberformat.Format = "0";
                }
                ws.Cells[maxMonth + 2, years.Count + 2].Style.Numberformat.Format = "0";

                //last line (totals): bold
                ws.Cells[maxMonth + 2, 1].Value = "Cumul";
                ws.Row(maxMonth + 2).Style.Font.Bold = true;
                ws.Row(maxMonth + 2).Style.Font.Size = 12;

                //autofit column width
                for (int i = 1; i <= colCounter + 3; i++)
                {
                    ws.Column(i).AutoFit();
                }


                return pck;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

    }
}
