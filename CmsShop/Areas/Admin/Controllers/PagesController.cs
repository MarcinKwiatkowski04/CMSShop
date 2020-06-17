using CmsShop.Models.Data;
using CmsShop.Models.ViewModels.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace CmsShop.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //Declaration of a PageVM list
            List<PageVM> pagesList;
            
            using (Db db = new Db())
            {
                //initializing list
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            //returning the pages back to the view            
            return View(pagesList);
        }
        // GET: Admin/Pages/AddPage
        public ActionResult AddPage()
        {
            return View();
        }
    }
}