using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
//using Microsoft.VisualBasic.FileIO;

using CompteResultat.Common;

namespace CompteResultat.DAL
{
    public class BulkInsert
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static void DoBulkInsert(string csv_file_path, string table)
        {            
            DataTable dt;
            SqlBulkCopy bc;
            long cnt = 0;
            bool createNewTable = true;
            int i = 1;
            string[] colFields = null;

            try
            {                
                //get the header columns
                //### ReadLines keeps file locked => replace this with ReadAllLines
                string header = File.ReadLines(csv_file_path).First();
                if (header != "")
                    colFields = Regex.Split(header, C.cVALSEP);
                else
                    throw new Exception("The provided CSV file does not contain any data: " + csv_file_path);


                //create the DataTable
                dt = new DataTable();

                if (createNewTable)
                {
                    i = 1;
                    foreach (string column in colFields)
                    {
                        DataColumn datacolumn = new DataColumn("Col" + i.ToString());
                        datacolumn.AllowDBNull = true;
                        dt.Columns.Add(datacolumn);
                        i++;
                    }
                    createNewTable = false;                    
                }


                using (var conn = DBHelper.GetSQLConnection())
                {
                    //get all content rows
                    i = 1;
                    foreach (string line in File.ReadLines(csv_file_path))
                    {
                        //skip the header line
                        if (line != "" && i > 1)
                        {
                            // get columns from the line
                            //### Bulk Insert does not support internationalization => all comas (in numbers) need to be replaced by dots
                            //string newLine = line.Replace(",", ".");
                            string newLine = line;
                            colFields = Regex.Split(newLine, C.cVALSEP);

                            foreach (string col in colFields)
                            {
                                //string s2 = s1.length == 0 ? null : s1;
                                //col = "zz";
                            }

                            //set empty strings to null
                            for (int col = 0; col < colFields.Length; col++)
                            {
                                colFields[col] = colFields[col].Length == 0 ? null : colFields[col];
                            }

                            dt.Rows.Add(colFields);
                            cnt++;

                            if (cnt >= C.CO_BulkInsertMaxRows)
                            {
                                bc = new SqlBulkCopy(conn);
                                bc.BulkCopyTimeout = 600;
                                bc.DestinationTableName = table;

                                bc.WriteToServer(dt);
                                dt.Clear();
                                dt = null;

                                cnt = 0;
                                createNewTable = true;
                            }

                            if (createNewTable)
                            {
                                i = 1;
                                dt = new DataTable();
                                foreach (string column in colFields)
                                {
                                    DataColumn datacolumn = new DataColumn("Col" + i.ToString());
                                    datacolumn.AllowDBNull = true;
                                    dt.Columns.Add(datacolumn);
                                    i++;
                                }
                                createNewTable = false;
                            }

                        }
                        i++;
                    }

                    //write the last set of data
                    if (cnt < C.CO_BulkInsertMaxRows && !createNewTable)
                    {
                        //do a bulk insert
                        bc = new SqlBulkCopy(conn);
                        bc.BulkCopyTimeout = 600;
                        bc.DestinationTableName = table;
                        bc.WriteToServer(dt);
                        dt.Clear();
                        dt = null;
                    }

                }
            }
            catch (Exception ex)
            {
                //throw new Exception("Bulk Insert Error: It seems like the data has already been imported into the database. \r\n\r\n" + ex.Message);
                throw new Exception("Bulk Insert Error: " + ex.Message);
            }            
        }

    }
}
