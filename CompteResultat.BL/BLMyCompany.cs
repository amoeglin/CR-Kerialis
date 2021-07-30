using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CompteResultat.DAL;
using System.Data.Entity;

namespace CompteResultat.BL
{
    public class BLMyCompany //: IDisposable
    {
        //private static CompteResultatEntities context = new CompteResultatEntities();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public BLMyCompany()
        {
        }

        public static MyCompany GetMyCompanyInfo()
        {
            try
            {                
                return MyCompany.GetMyCompanyInfo();
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

                //Handle business rules...


                MyCompany.UpdateMyCompany(newCompany);                
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }


        #region DISPOSE

        //private bool disposedValue = false;

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!this.disposedValue)
        //    {
        //        if (disposing)
        //        {
        //            context.Dispose();
        //        }
        //    }
        //    this.disposedValue = true;
        //}

        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        #endregion


    }


}

