using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PrensaEstudiantil.Context;
using PrensaEstudiantil.Models;
using Microsoft.AspNet.Identity;

namespace PrensaEstudiantil.Controllers
{
    public class PublicationsController : Controller
    {
        private readonly PrensaContext db = new PrensaContext();

        // ViewBagss
        readonly string permissions = "Usted no tiene permisos para ejecutar esta acción.";

        // GET: Publications
        public ActionResult Index()
        {
            //var publications = db.Publications.OrderByDescending(x => x.Date).Take(10);
            var publications = db.Publications.Where(x => x.CategoryID == 1).OrderByDescending(x => x.Date).Take(21);
            var videos = db.YoutubeVideos.OrderByDescending(x => x.YoutubeVideoID).Take(5).ToList();
            var columnas = db.Publications.Where(x => x.CategoryID == 2).OrderByDescending(x => x.Date).Take(3).ToList();

            List<YoutubeVideo> videosArray = videos;
            List<Publication> columnasArray = columnas;

            ViewBag.Videos = videosArray;
            ViewBag.Columnas = columnasArray;
            ViewBag.ColumnasLength = columnasArray.Count;

            return View(publications.ToList());
        }

        // GET: Publications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Publication publication = db.Publications.Find(id);

            if (publication == null)
            {
                return HttpNotFound();
            }

            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"].ToString();
            }

            // Get images associated to current publication
            var images = db.PublicationImages.Where(x => x.ID == id).OrderByDescending(x => x.PublicationImagenID).ToList();

            ViewBag.Images = images;

            return View(publication);
        }

        // GET: Publications/Create
        [Authorize(Roles = "Admin, User")]
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Description");
            return View();
        }

        // POST: Publications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, User")]
        public ActionResult Create([Bind(Include = "ID,Title,Header,Body,Footer,Date,LastUpdate,Author,ImageDescription,CategoryID")] Publication publication, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Set the current date and time and default values
                publication.Date = DateTime.Now;
                publication.UserPublication = User.Identity.GetUserName();

                // Verify if there is an associated image
                if (ImageFile != null && ImageFile.ContentLength > 0)
                    try
                    {
                        // Construct the name to the image file
                        string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                        string extension = Path.GetExtension(ImageFile.FileName);

                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        publication.ImagePath = "/Content/Images/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);

                        // Save the image in server
                        ImageFile.SaveAs(fileName);

                        // Set null value for publications different of Opinion Category (CategoryID = 2) 
                        if (publication.CategoryID == 2)
                        {
                            publication.Author = publication.Author.ToUpper();
                            publication.ImageDescription = null;
                        }
                        else
                        {
                            publication.Author = null;
                        }

                        // Save publication in BD
                        db.Publications.Add(publication);
                        db.SaveChanges();

                        // ViewBags
                        TempData["Success"] = "Publicación grabada exitosamente";

                        return RedirectToAction("Details/" + publication.ID);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Description", publication.CategoryID);

                        return View(publication);
                    }
                else
                {
                    ViewBag.Message = "No se ha encontrado un archivo válido.";
                    ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Description", publication.CategoryID);

                    return View(publication);
                }
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Description", publication.CategoryID);
            return View(publication);
        }

        // GET: Publications/Edit/5
        [Authorize(Roles = "Admin, User")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Publication publication = db.Publications.Find(id);

            if (publication == null)
            {
                return HttpNotFound();
            }

            // Check if it's the same User or has persmissions
            if (!User.IsInRole("Admin") && publication.UserPublication != User.Identity.GetUserName())
            {
                //var publications = db.Publications.OrderByDescending(x => x.Date).Take(20).ToList();

                //ViewBag.Permissions = permissions;
                //ViewBag.AdminLength = publications.Count;

                // ViewBags
                TempData["Permissions"] = permissions;

                return RedirectToAction("Admin");

                //return View("Admin", publications);
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Description", publication.CategoryID);

            return View(publication);
        }

        // POST: Publications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, User")]
        public ActionResult Edit([Bind(Include = "ID,Title,Header,Body,Footer,Date,ImagePath,ImageDescription,Author,UserPublication,LastUpdate,CategoryID")] Publication publication)
        {
            if (ModelState.IsValid)
            {
                // Set the current date and time for Update Date
                publication.LastUpdate = DateTime.Now;
                publication.UserPublicationEdit = User.Identity.GetUserName();

                // Set null value for publications different of Opinion Category (CategoryID = 2) 
                if (publication.CategoryID == 2)
                {
                    publication.Author = publication.Author.ToUpper();
                }
                else
                {
                    publication.Author = null;
                }

                // Modify and save changes
                db.Entry(publication).State = EntityState.Modified;
                db.SaveChanges();

                // ViewBags
                TempData["Success"] = "Publicación actualizada exitosamente";

                return RedirectToAction("Details/" + publication.ID);
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Description", publication.CategoryID);
            return View(publication);
        }

        // GET: Publications/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Publication publication = db.Publications.Find(id);
            if (publication == null)
            {
                return HttpNotFound();
            }
            return View(publication);
        }

        // POST: Publications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Publication publication = db.Publications.Find(id);
            db.Publications.Remove(publication);
            db.SaveChanges();

            // ViewBags
            TempData["Success"] = "Publicación eliminada correctamente.";

            return RedirectToAction("Admin");
        }

        // Publications for admin view
        [Authorize(Roles = "Admin, SuperAdmin, User")]
        public ActionResult Admin()
        {
            var publications = db.Publications.OrderByDescending(x => x.Date).Take(20).ToList();

            // ViewBags
            ViewBag.AdminLength = publications.Count;

            if (TempData["Permissions"] != null)
            {
                ViewBag.Permissions = TempData["Permissions"].ToString();
            }

            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"].ToString();
            }

            return View(publications);
        }

        // Publications of Opinion Category (CategoryID = 4)
        public ActionResult Opinion()
        {
            var publications = db.Publications.Where(x => x.CategoryID == 2).OrderByDescending(x => x.Date).Take(21).ToList();

            ViewBag.OpinionLength = publications.Count;

            return View(publications);
        }

        // Publications of Biosociedad Category
        public ActionResult Biosociedad()
        {
            var publications = db.Publications.Where(x => x.CategoryID == 4).OrderByDescending(x => x.Date).Take(21).ToList();

            ViewBag.BiosociedadLength = publications.Count;

            return View(publications);
        }

        // Publications of Social Category 
        public ActionResult Social()
        {
            var publications = db.Publications.Where(x => x.CategoryID == 3).OrderByDescending(x => x.Date).Take(21).ToList();

            ViewBag.SocialLength = publications.Count;

            return View(publications);
        }

        // GET: Videos
        public ActionResult Videos()
        {
            var videos = db.YoutubeVideos.OrderByDescending(x => x.YoutubeVideoID).Take(21).ToList();

            ViewBag.VideosLength = videos.Count;

            if (TempData["Permissions"] != null)
            {
                ViewBag.Permissions = TempData["Permissions"].ToString();
            }

            return View(videos);
        }

        // Add images to publication at creating moment
        //[Authorize(Roles = "Admin")]
        //public ActionResult AddImage(PublicationImage publicationImage)
        //{
        //    return View(publicationImage);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
