using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CompteResultat.DAL;
using System.Threading;

namespace CompteResultat
{
    public partial class Purge : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void cmdPurge_Click(object sender, EventArgs e)
        {        
            using (var context = new CompteResultatEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Database.ExecuteSqlCommand("truncate table[CompteResultat].[dbo].[Cadencier]");
                        context.Database.ExecuteSqlCommand("truncate table[CompteResultat].[dbo].[IMAssContr]");
                        
                        context.Database.ExecuteSqlCommand("truncate table[CompteResultat].[dbo].[IMContrComp]");
                        context.Database.ExecuteSqlCommand("truncate table[CompteResultat].[dbo].[IMContrDistrib]");
                        context.Database.ExecuteSqlCommand("truncate table[CompteResultat].[dbo].[IMReAssAss]");
                        context.Database.ExecuteSqlCommand("truncate table[CompteResultat].[dbo].[CotisatPrev]");
                        context.Database.ExecuteSqlCommand("truncate table[CompteResultat].[dbo].[CotisatSante]");
                        context.Database.ExecuteSqlCommand("truncate table[CompteResultat].[dbo].[DecomptePrev]");
                        context.Database.ExecuteSqlCommand("truncate table[CompteResultat].[dbo].[Demography]");
                        
                        context.Database.ExecuteSqlCommand("truncate table[CompteResultat].[dbo].[PrestSante]");
                        context.Database.ExecuteSqlCommand("truncate table[CompteResultat].[dbo].[SinistrePrev]");
                        context.Database.ExecuteSqlCommand("truncate table[CompteResultat].[dbo].[ProvPrev]");
                        context.Database.ExecuteSqlCommand("truncate table[CompteResultat].[dbo].[_TempOtherFields]");
                        context.Database.ExecuteSqlCommand("Delete FROM[CompteResultat].[dbo].[Contract]");
                        context.Database.ExecuteSqlCommand("Delete FROM[CompteResultat].[dbo].[CompteResult]");
                        context.Database.ExecuteSqlCommand("Delete FROM[CompteResultat].[dbo].[Company]");
                        context.Database.ExecuteSqlCommand("truncate table[CompteResultat].[dbo].[CRPlanning]");
                        context.Database.ExecuteSqlCommand("DELETE FROM[CompteResultat].[dbo].[ReAssureur]");
                        context.Database.ExecuteSqlCommand("DELETE FROM[CompteResultat].[dbo].[Assureur]");

                        context.Database.ExecuteSqlCommand("truncate table[CompteResultat].[dbo].[_TempExpData]");
                        context.Database.ExecuteSqlCommand("truncate table[CompteResultat].[dbo].[GroupGarantySante]");

                        //SELECT Id FROM Import WHERE Id NOT IN (SELECT DISTINCT ImportId from ImportFiles)
                        //context.Database.ExecuteSqlCommand("truncate table[CompteResultat].[dbo].[Import]");
                        context.Database.ExecuteSqlCommand("Delete FROM Import WHERE Id IN (SELECT Id FROM Import WHERE Id NOT IN (SELECT DISTINCT ImportId from ImportFiles))");

                        //For all imports, set Archived to true - all associated data is already deleted
                        context.Database.ExecuteSqlCommand("UPDATE Import SET Archived = 1");

                        //context.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }           
           
        }
    }
}