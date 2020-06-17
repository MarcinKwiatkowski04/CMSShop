using CmsShop.Models.Data;
using CmsShop.Models.ViewModels.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.UI;

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
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }
        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Checking if the addPage form was completed correctly; checking it's state
            if (!ModelState.IsValid)
            {
                //if the form is not completed correctly - we return the model back to the client, without erasing any data
                return View(model);
            }
            using (Db db = new Db())
            {
                string slug;

                //initializing PageDTO
                PageDTO dto = new PageDTO();

                
                //adding a title as a page address in case this field is not completed 
                if (string.IsNullOrEmpty(model.Slug))
                {
                    //making sure that page link meets all the needed requirements
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }
                //preventing adding multiple page titles and slugs
                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "Ten tytuł już istnieje w bazie");
                    return View(model);
                }
                if (db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Ten adres już istnieje w bazie");
                    return View(model);
                }
                dto.Title = model.Title;
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 1000;

                //DTO save
                db.Pages.Add(dto);
                db.SaveChanges();
            }
            TempData["SM"] = "Dodałeś nową stronę!";

            return RedirectToAction("AddPage");
        }
    }
}