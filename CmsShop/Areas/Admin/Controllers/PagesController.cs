using CmsShop.Models.Data;
using CmsShop.Models.ViewModels.Pages;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Infrastructure.MappingViews;
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



        // GET: Admin/Pages/EditPage
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            PageVM model;

            using (Db db = new Db())
            {
                //getting a page from the database instead of creating a whole new procedure
                PageDTO dto = db.Pages.Find(id);
                //checking whether this page is existing and valid in the database
                if (dto == null)
                {
                    return Content("Podana strona nie istnieje");
                }
                model = new PageVM(dto);
            }

            return View(model);
        }
        // POST: Admin/Pages/EditPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                //get id of a page that we want to edit
                int id = model.Id;
                //initializing slug below in case to make sure that following path(1), it is that one specific case 
                string slug = "home";
                // getting a specific page to edit
                PageDTO dto = db.Pages.Find(id);

                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {

                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        //path(1) - Slug is empty -> therefore is has a "home" value
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }
                //checking page and slug uniqueness
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title) ||
                    db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Strona, lub adres już istnieje w bazie");
                }
                //page modification
                dto.Title = model.Title;
                dto.Slug = slug;
                dto.HasSidebar = model.HasSidebar;
                dto.Body = model.Body;

                //saving an edited page to the database
                db.SaveChanges();
            }
            //setting a short message about editing the website
            TempData["SM"] = "Strona została edytowana!";

            //redirecting to the editpage endpoint
            return RedirectToAction("EditPage");
        }
        // GET: Admin/Pages/Details/id
        [HttpGet]
        public ActionResult Details(int id)
        {
            //declaration of pageViewModel
            PageVM model;
            using (Db db = new Db())
            {
                //getting a page by id, to show it's details
                PageDTO dto = db.Pages.Find(id);

                //checking if there is a page with a given id
                if (dto == null)
                {
                    return Content("Strona nie istnieje");
                }
                //initializing PageVM by above website
                model = new PageVM(dto);

            }
            return View(model);
        }
        // GET: Admin/Pages/Delete/id
        [HttpGet]
        public ActionResult Delete(int id)
        {
            using (Db db = new Db())
            {
                //getting a page from the database
                PageDTO dto = db.Pages.Find(id);
                //actual deleting a page 
                db.Pages.Remove(dto);
                //saving changes
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        // POST: Admin/Pages/ReorderPages
        [HttpPost]
        public ActionResult ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {
                int count = 1;
                PageDTO dto;
                //pages sorting and saving them to the database
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;
                    db.SaveChanges();
                    count++;
                }
            }

            return View();
        }
        // GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            SidebarVM model;
            using (Db db = new Db())
            {
                SidebarDTO dto = db.Sidebar.Find(1);
                //model initializing
                model = new SidebarVM(dto);

            }


            return View(model);

        }
        // POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {
                //getting sidebarDTO
                SidebarDTO dto = db.Sidebar.Find(1);
                //modifying sidebar
                dto.Body = model.Body;
                db.SaveChanges();
            }
            //alert that the sidebar has been modified
            TempData["SM"] = "Pasek boczny został zmodyfikowany";
            return RedirectToAction("EditSidebar");

        }

    }
}
