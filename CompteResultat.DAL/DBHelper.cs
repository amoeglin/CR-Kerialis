using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace CompteResultat.DAL
{
    public class DBHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static SqlConnection sqlCn = null;
        private static string connectionString = "";

        public static SqlConnection GetSQLConnection()
        {
            try
            {
               
                try
                {
                    if (connectionString == "")
                        connectionString = ConfigurationManager.ConnectionStrings["CRMainDB"].ConnectionString;
                }
                catch (Exception exConn)
                {
                    throw new Exception("No connection string was provided for: CRMainDB --- Source: ADOCommon.GetSQLConnection");
                }                

                sqlCn = new SqlConnection();
                sqlCn.ConnectionString = connectionString;
                sqlCn.Open();

                return sqlCn;
            }
            catch (Exception ex)
            {
                throw new Exception("The connection with the database cannot be established: " + ex.Message + " --- Source: ADOCommon.GetSQLConnection");
            }
        }

        public static void ExecuteNonQuery(string sqlCommand)
        {
            try
            {
                using (var conn = GetSQLConnection())
                {
                    SqlCommand command = new SqlCommand(sqlCommand, conn);
                    //command.Connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message + " --- Source: ADOCommon.ExecuteNonQuery");
            }
        }



    }
}
