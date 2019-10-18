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

namespace PrensaEstudiantil.Controllers
{
    public class PublicationsController : Controller
    {
        private PrensaContext db = new PrensaContext();

        // GET: Publications
        public ActionResult Index()
        {
            //var publications = db.Publications.OrderByDescending(x => x.Date).Take(10);
            var publications = db.Publications.Where(x => x.CategoryID == 1).OrderByDescending(x => x.Date).Take(21);
            var videos = db.YoutubeVideos.OrderByDescending(x => x.YoutubeVideoID).Take(5).ToList();
            var columnas = db.Publications.Where(x => x.CategoryID == 4).OrderByDescending(x => x.Date).Take(3).ToList();

            List<YoutubeVideo> videosArray = videos;
            List<Publication> columnasArray = columnas;

            ViewBag.Videos = videosArray;
            ViewBag.Columnas = columnasArray;

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

            // Get images associated to current publication
            var images = db.PublicationImages.Where(x => x.ID == id).OrderByDescending(x => x.PublicationImagenID).ToList();

            ViewBag.Images = images;

            return View(publication);
        }

        // GET: Publications/Create
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
        public ActionResult Create([Bind(Include = "ID,Title,Header,Body,Footer,Date,LastUpdate,Author,ImageDescription,CategoryID")] Publication publication, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Set the current date and time
                publication.Date = DateTime.Now;

                // Set initial status to 1 (Creada, pendiente de completar)
                //publication.Status = 1;

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

                        // Set null value for publications different of Opinion Category (CategoryID = 4) 
                        if (publication.CategoryID == 4)
                        {
                            publication.Author = publication.Author.ToUpper();
                        }
                        else
                        {
                            publication.Author = null;
                        }

                        // Save publication in BD
                        db.Publications.Add(publication);
                        db.SaveChanges();

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

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Description", publication.CategoryID);

            return View(publication);
        }

        // POST: Publications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Header,Body,Footer,Date,ImagePath,ImageDescription,Author,LastUpdate,CategoryID")] Publication publication)
        {
            if (ModelState.IsValid)
            {
                // Set the current date and time for Update Date
                publication.LastUpdate = DateTime.Now;

                // Set null value for publications different of Opinion Category (CategoryID = 4) 
                if (publication.CategoryID == 4)
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

                return RedirectToAction("Details/" + publication.ID);
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Description", publication.CategoryID);
            return View(publication);
        }

        // GET: Publications/Delete/5
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
        public ActionResult DeleteConfirmed(int id)
        {
            Publication publication = db.Publications.Find(id);
            db.Publications.Remove(publication);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Publications for admin view
        public ActionResult Admin()
        {
            var publications = db.Publications.OrderByDescending(x => x.Date).Take(20);

            return View(publications.ToList());
        }

        // Publications of Opinion Category (CategoryID = 4)
        public ActionResult Opinion()
        {
            var publications = db.Publications.Where(x => x.CategoryID == 4).OrderByDescending(x => x.Date).Take(21);

            return View(publications.ToList());
        }

        // Publications of Biosociedad Category
        public ActionResult Biosociedad()
        {
            var publications = db.Publications.Where(x => x.CategoryID == 3).OrderByDescending(x => x.Date).Take(21);

            return View(publications.ToList());
        }

        // Publications of Social Category 
        public ActionResult Social()
        {
            var publications = db.Publications.Where(x => x.CategoryID == 5).OrderByDescending(x => x.Date).Take(21);

            return View(publications.ToList());
        }
        
        // GET: Videos
        public ActionResult Videos()
        {
            var videos = db.YoutubeVideos.OrderByDescending(x => x.YoutubeVideoID).Take(21).ToList();

            return View(videos);
        }

        // Add images to publication at creating moment
        public ActionResult AddImage(PublicationImage publicationImage)
        {
            return View(publicationImage);
        }

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
