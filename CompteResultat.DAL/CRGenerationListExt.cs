using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CompteResultat.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;

namespace CompteResultat.DAL
{
    [MetadataType(typeof(Import.MetaData))]
    public partial class CRGenList
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<CRGenList> GetLists()
        {
            return GetLists("Name", "ASC");
        }

        public static List<CRGenList> GetLists(string sortExpression, string sortDirection)
        {
            try
            {
                List<CRGenList> lists;

                using (var context = new CompteResultatEntities())
                {
                    if (sortExpression == "Name")
                    {
                        if(sortDirection == "ASC")
                            lists = context.CRGenLists.OrderBy(i => i.Name).ToList();
                        else
                            lists = context.CRGenLists.OrderByDescending(i => i.Name).ToList();
                    }
                    else if (sortExpression == "UserName")
                    {
                        if (sortDirection == "ASC")
                            lists = context.CRGenLists.OrderBy(i => i.UserName).ToList();
                        else
                            lists = context.CRGenLists.OrderByDescending(i => i.UserName).ToList();
                    }
                    else if (sortExpression == "Type")
                    {
                        if (sortDirection == "ASC")
                            lists = context.CRGenLists.OrderBy(i => i.Type).ToList();
                        else
                            lists = context.CRGenLists.OrderByDescending(i => i.Type).ToList();
                    }                   
                    else
                        lists = context.CRGenLists.OrderBy(i => i.Name).ToList();
                }
                
                return lists;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<CRGenList> GetListsForUser(string userName)
        {
            return GetListsForUser("Name", "ASC", userName);
        }

        public static List<CRGenList> GetListsForUser(string sortExpression, string sortDirection, string userName)
        {
            try
            {
                List<CRGenList> lists;

                using (var context = new CompteResultatEntities())
                {
                    if (sortExpression == "Name")
                    {
                        if (sortDirection == "ASC")
                            lists = context.CRGenLists.Where(l => l.UserName == userName).OrderBy(i => i.Name).ToList();
                        else
                            lists = context.CRGenLists.Where(l => l.UserName == userName).OrderByDescending(i => i.Name).ToList();
                    }
                    else if (sortExpression == "UserName")
                    {
                        if (sortDirection == "ASC")
                            lists = context.CRGenLists.Where(l => l.UserName == userName).OrderBy(i => i.UserName).ToList();
                        else
                            lists = context.CRGenLists.Where(l => l.UserName == userName).OrderByDescending(i => i.UserName).ToList();
                    }
                    else if (sortExpression == "Type")
                    {
                        if (sortDirection == "ASC")
                            lists = context.CRGenLists.Where(l => l.UserName == userName).OrderBy(i => i.Type).ToList();
                        else
                            lists = context.CRGenLists.Where(l => l.UserName == userName).OrderByDescending(i => i.Type).ToList();
                    }
                    else
                        lists = context.CRGenLists.Where(l => l.UserName == userName).OrderBy(i => i.Name).ToList();
                }

                return lists;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<CRGenList> GetListsByName(string searchText)
        {
            try
            {
                List<CRGenList> lists;
                using (var context = new CompteResultatEntities())
                {
                    lists = context.CRGenLists.Where(i => i.Name.ToLower().Contains(searchText.ToLower())).OrderByDescending(i => i.Name).ToList();
                }

                return lists;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static CRGenList GetSingleListName(string listName)
        {
            try
            {
                CRGenList list;
                using (var context = new CompteResultatEntities())
                {
                    var elements = context.CRGenLists.Where(i => i.Name.ToLower() == listName.ToLower());

                    if (elements.Any())
                        list = elements.First();
                    else
                        list = null;
                }

                return list;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static CRGenList GetListById(int id)
        {
            try
            {
                CRGenList list;
                using (var context = new CompteResultatEntities())
                {
                    var elements = context.CRGenLists.Where(i => i.Id == id);

                    if (elements.Any())
                        list = elements.First();
                    else
                        list = null;
                }

                return list;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int Insert(CRGenList list)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {                    
                    context.CRGenLists.Add(list);
                    context.SaveChanges();

                    return list.Id;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void DeleteListWithId(int listId)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    var elements = context.CRGenLists.Where(i => i.Id == listId);

                    if (elements.Any())
                    {
                        context.CRGenLists.Remove(elements.First());
                        context.SaveChanges();
                    }                    
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

                

        //MetaData definition for basic validation
        public class MetaData
        {
            //[Display(Name = "Import Name")]
            //[Required(ErrorMessage = "Please provide a name for the import!")]
            //public string Name { get; set; }

        }

    }
}
