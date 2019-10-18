using PrensaEstudiantil.Context;
using PrensaEstudiantil.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PrensaEstudiantil.Controllers
{
    public class PublicationImagesController : Controller
    {
        private PrensaContext db = new PrensaContext();

        // GET: PublicationImages
        public ActionResult Index()
        {
            var publicationImages = db.PublicationImages.Include(p => p.Publication);
            return View(publicationImages.ToList());
        }

        // GET: PublicationImages
        // Returns the images associated with a publication ID
        public ActionResult ImagesByPublicationId(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Select images associated to the id given
            var publicationImages = db.PublicationImages.Include(p => p.Publication).Where(x => x.ID == id);

            var currentPublication = db.Publications.Find(id);

            // Construct viewbag for principal image of the publication
            Session["CurrentPublication"] = currentPublication;
            Session["CurrentPublicationId"] = currentPublication.ID;
            Session["CurrentPublicationTitle"] = currentPublication.Title;

            ViewBag.CurrentPublication = Session["CurrentPublication"];
            ViewBag.CurrentPublicationId = Session["CurrentPublicationId"];
            ViewBag.CurrentPublicationTitle = Session["CurrentPublicationTitle"];

            return View(publicationImages.ToList());
        }

        // GET: PublicationImages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PublicationImage publicationImage = db.PublicationImages.Find(id);
            if (publicationImage == null)
            {
                return HttpNotFound();
            }
            return View(publicationImage);
        }

        // GET: PublicationImages/Create
        public ActionResult Create()
        {
            ViewBag.ID = new SelectList(db.Publications, "ID", "Title");
            return View();
        }

        // POST: PublicationImages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Description,ID")] PublicationImage publicationImage, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // Verify if there is an associated image
                if (ImageFile != null && ImageFile.ContentLength > 0)
                    try
                    {
                        // Construct the name to the image file
                        string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                        string extension = Path.GetExtension(ImageFile.FileName);

                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        publicationImage.ImagePath = "/Content/Images/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/Content/Images/"), fileName);

                        // Save the image in server
                        ImageFile.SaveAs(fileName);

                        // Save publication in BD
                        db.PublicationImages.Add(publicationImage);
                        db.SaveChanges();

                        ViewBag.Message = "Imagen cargada exitosamente";

                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    }
                else
                {
                    ViewBag.Message = "No se ha encontrado un archivo válido.";
                }

                return RedirectToAction("ImagesByPublicationId/" + publicationImage.ID);
            }

            // Construct viewbag for principal image of the publication
            Session["CurrentPublication"] = db.Publications.Find(publicationImage.ID);

            //ViewBag.ID = new SelectList(db.Publications, "ID", "Title", publicationImage.ID);
            return View(publicationImage);
        }

        // POST: PublicationImages/ChangePrincipalImage
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePrincipalImage(string ImagePath, int ID, HttpPostedFileBase ImageFile)
        {
            // Verify if there is an associated image
            if (ImageFile != null && ImageFile.ContentLength > 0)
                try
                {
                    // Delete file in server
                    var fullPath = Server.MapPath(ImagePath);

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                        ImagePath = Path.Combine(Server.MapPath(""), ImagePath);

                        // Save the image in server
                        ImageFile.SaveAs(fullPath);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "No se ha encontrado un archivo válido.";
            }

            // Construct viewbag for principal image of the publication
            Session["CurrentPublication"] = db.Publications.Find(ID);

            return RedirectToAction("ImagesByPublicationId/" + ID);

            //ViewBag.ID = new SelectList(db.Publications, "ID", "Title", publicationImage.ID);
            //return View(publicationImage);
        }

        // GET: PublicationImages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PublicationImage publicationImage = db.PublicationImages.Find(id);
            if (publicationImage == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = new SelectList(db.Publications, "ID", "Title", publicationImage.ID);
            return View(publicationImage);
        }

        // POST: PublicationImages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PublicationImagenID,ImagePath,Description,ID")] PublicationImage publicationImage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(publicationImage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID = new SelectList(db.Publications, "ID", "Title", publicationImage.ID);
            return View(publicationImage);
        }

        // GET: PublicationImages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PublicationImage publicationImage = db.PublicationImages.Find(id);

            if (publicationImage == null)
            {
                return HttpNotFound();
            }
            return View(publicationImage);
        }

        // POST: PublicationImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string ImagePath)
        {
            PublicationImage publicationImage = db.PublicationImages.Find(id);

            try
            {
                // Delete from DB
                db.PublicationImages.Remove(publicationImage);
                db.SaveChanges();

                // Delete file in server
                var fullPath = Server.MapPath(ImagePath);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    ViewBag.Message = "Archivo eliminado correctamente";
                }

                // Construct viewbag for principal image of the publication
                ViewBag.CurrentPublication = Session["CurrentPublication"];
                ViewBag.CurrentPublicationId = Session["CurrentPublicationId"];
                ViewBag.CurrentPublicationTitle = Session["CurrentPublicationTitle"];
            }
            catch (Exception)
            {
                ViewBag.Message = "Error eliminando el archivo del servidor";
                return View();
            }

            //return RedirectToAction("Index");
            return RedirectToAction("ImagesByPublicationId/" + publicationImage.ID);
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
