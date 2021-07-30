using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data;
using Excel;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using GenericParsing;


namespace CompteResultat.Common
{
    public static class G
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        #region FILES & FOLDERS

        public static C.eFileType GetFileType(string filePath)
        {
            try
            {
                string myExt = Path.GetExtension(filePath).ToLower();
                C.eFileType myFileType = C.eFileType.Other;

                switch (myExt)
                {
                    case ".csv":
                        myFileType = C.eFileType.CSV;
                        break;
                    case ".xlsx":
                    case ".xls":
                        myFileType = C.eFileType.Excel;
                        break;
                    case ".xlsm":
                        myFileType = C.eFileType.ExcelMacro;
                        break;
                    case ".ppt":
                        myFileType = C.eFileType.PPT;
                        break;
                    case ".pptm":
                        myFileType = C.eFileType.PPTMacro;
                        break;
                    default:
                        myFileType = C.eFileType.Other;
                        break;
                }

                return myFileType;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void SaveDataTableAsFile(DataTable myDT, string filePath)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                int cnt = 0;
                WriteFileContent(filePath, "");

                IEnumerable<string> columnNames = myDT.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                sb.AppendLine(string.Join(C.cVALSEP, columnNames));

                foreach (DataRow row in myDT.Rows)
                {
                    cnt++;
                    IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                    string str = string.Join(C.cVALSEP, fields);
                    str = str.Replace("\n\n", "");
                    sb.AppendLine(str);
                    
                    if (cnt > 1000)
                    {
                        AppendFileContent(filePath, sb.ToString());
                        cnt = 0;
                        sb = new StringBuilder();
                    }
                }

                AppendFileContent(filePath, sb.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetRelativeFolderPath(string folderName)
        {
            string relFolder = folderName;

            if (relFolder != "")
            {
                if (!folderName.StartsWith("~/"))
                    relFolder = "~/" + relFolder;

                if (!relFolder.EndsWith("/"))
                    relFolder += "/";

                return relFolder;
            }
            else return "";
        }

        public static string GetFileContent(string filename)
        {
            if (File.Exists(filename))
            {
                TextReader tr = new StreamReader(filename);
                string cont = tr.ReadToEnd();
                tr.Close();
                return cont;
            }
            else { return ""; }
        }

        public static void WriteFileContent(string filename, string content)
        {
            TextWriter tw = new StreamWriter(filename, false);
            tw.Write(content);
            tw.Close();
        }

        public static void WriteFileContent(string filename, StringBuilder sb, bool append=false)
        {
            StreamWriter sw = new StreamWriter(filename, append);
            while (sb.Length > 30000)
            {
                sw.Write(sb.ToString(0, 30000));
                sb.Remove(0, 30000);
            }
            sw.Write(sb.ToString());
            sw.Close();            
        }

        public static void AppendFileContent(string filename, string content)
        {
            TextWriter tw = new StreamWriter(filename, true);
            tw.Write(content);
            tw.Close();
        }

        public static string GetRandomKey(int bytelength)
        {
            byte[] buff = new byte[bytelength];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buff);
            StringBuilder sb = new StringBuilder(bytelength * 2);
            for (int i = 0; i < buff.Length; i++)
                sb.Append(string.Format("{0:X2}", buff[i]));
            return sb.ToString();
        }

        #endregion

        #region EXCEL & CSV FUNCTIONS

        public static void CreateCSVFromStringList(List<string> data, string filePath)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                foreach (string line in data)
                {
                    if(!string.IsNullOrWhiteSpace(line))
                        sb.AppendLine(line);
                }

                G.WriteFileContent(filePath, sb.ToString());
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void GetAssurContrCompSubsidFromCSV(ref List<string> data, string filePath, int importId = 0)
        {
            GenericParser parser;

            try
            {
                string assur, contr, comp, subsid;
                string accs;

                using (parser = new GenericParser())
                {
                    parser.SetDataSource(filePath);

                    parser.ColumnDelimiter = C.cVALSEP.ToCharArray()[0];
                    parser.FirstRowHasHeader = true;
                    //parser.SkipStartingDataRows = 10;
                    //parser.MaxBufferSize = 4096;
                    //parser.MaxRows = 500;
                    //parser.TextQualifier = '\"';

                    while (parser.Read())
                    {
                        assur = parser[C.eDBTempOtherFieldsColumns.AssureurName.ToString()];
                        contr = parser[C.eDBTempOtherFieldsColumns.ContractId.ToString()];
                        comp = parser[C.eDBTempOtherFieldsColumns.Company.ToString()];
                        subsid = parser[C.eDBTempOtherFieldsColumns.Subsid.ToString()];

                        accs = importId.ToString() + C.cVALSEP + assur + C.cVALSEP + contr + C.cVALSEP + comp + C.cVALSEP + subsid + C.cVALSEP + C.cVALSEP;

                        if (!string.IsNullOrWhiteSpace(accs) && !data.Contains(accs))
                            data.Add(accs);
                    }
                }
            }
            catch (Exception ex)
            {                
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static DataTable ExcelToDataTable(string excelFilePath, bool firstRowAsColumnNames)
        {
            DataTable dt = null;
            IExcelDataReader excelReader = null;

            try
            {
                using (FileStream stream = File.Open(excelFilePath, FileMode.Open, FileAccess.Read))
                {

                    if (Path.GetExtension(excelFilePath).ToLower() == ".xls")
                        excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                    else if (Path.GetExtension(excelFilePath).ToLower() == ".xlsx")
                        excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    else
                        throw new Exception("The provided file is not a valid Excel file!");
                }

                //DataSet - Create column names from first row
                if (firstRowAsColumnNames)
                    excelReader.IsFirstRowAsColumnNames = true;
                else
                    excelReader.IsFirstRowAsColumnNames = false;

                DataSet result = excelReader.AsDataSet();
                dt = result.Tables[0];

                //Free resources 
                excelReader.Close();

                return dt;
            }
            catch (Exception ex)
            {
                if (!excelReader.IsClosed)
                    excelReader.Close();

                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetExcelHeader(string excelFilePath)
        {
            IExcelDataReader excelReader = null;
            List<string> cols = new List<string>();

            try
            {
                using (FileStream stream = File.Open(excelFilePath, FileMode.Open, FileAccess.Read))
                {

                    if (Path.GetExtension(excelFilePath).ToLower() == ".xls")
                        excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                    else if (Path.GetExtension(excelFilePath).ToLower() == ".xlsx")
                        excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    else
                        throw new Exception("The provided file is not a valid Excel file!");
                }

                //excelReader.IsFirstRowAsColumnNames = true; 

                if (excelReader.Read())
                {
                    for (int i = 0; i < excelReader.FieldCount; i++)
                    {
                        string colVal = excelReader.GetValue(i).ToString();
                        if (!string.IsNullOrEmpty(colVal) && !string.IsNullOrWhiteSpace(colVal))
                            cols.Add(colVal);
                    }
                }

                //Free resources 
                excelReader.Close();

                return cols;
            }
            catch (Exception ex)
            {
                if (excelReader != null)
                {
                    if (!excelReader.IsClosed)
                        excelReader.Close();
                }

                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetCSVHeader(string csvFilePath)
        {            
            List<string> cols = new List<string>();

            try
            {
                string csvCols = File.ReadLines(csvFilePath).First();

                if(!string.IsNullOrWhiteSpace(csvCols))                    
                    cols = Regex.Split(csvCols, C.cVALSEP).ToList();                

                return cols;
            }
            catch (Exception ex)
            {                
                log.Error(ex.Message);
                throw ex;
            }
        }

        #endregion


        public static void ReleaseObject(object obj)
        {
            //Use: releaseObject(xlApp);
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }



    }
}
