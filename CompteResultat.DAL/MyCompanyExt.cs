using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;

namespace CompteResultat.DAL
{
    public partial class MyCompany
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static MyCompany GetMyCompanyInfo()
        {
            try
            {
                MyCompany myComp;

                using (var context = new CompteResultatEntities())
                {
                    myComp = context.MyCompanies.First();
                }

                if (myComp == null)
                    throw new Exception("The 'MyCompany' entity does not contain any data!");

                return myComp;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void UpdateMyCompany(MyCompany newCompany)
        {
            try
            {
                if (newCompany == null)
                    throw new Exception("The 'MyCompany' entity does not contain any data!");

                using (var context = new CompteResultatEntities())
                {
                    MyCompany oldCompany = context.MyCompanies.First();
                    oldCompany.Name = newCompany.Name;
                    oldCompany.Phone = newCompany.Phone;
                    oldCompany.Logo = newCompany.Logo;
                    oldCompany.ServerSMTP = newCompany.ServerSMTP;
                    oldCompany.PassSMTP = newCompany.PassSMTP;
                    oldCompany.Email = newCompany.Email;
                    oldCompany.Address = newCompany.Address;

                    context.SaveChanges();
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
