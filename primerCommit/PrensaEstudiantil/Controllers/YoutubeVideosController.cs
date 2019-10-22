using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PrensaEstudiantil.Context;
using PrensaEstudiantil.Models;

namespace PrensaEstudiantil.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class YoutubeVideosController : Controller
    {
        private readonly PrensaContext db = new PrensaContext();

        // ViewBagss
        readonly string permissions = "Usted no tiene permisos para ejecutar esta acción.";

        // GET: YoutubeVideos
        public ActionResult Index()
        {
            var videos = db.YoutubeVideos.OrderByDescending(x => x.YoutubeVideoID).Take(30).ToList();

            // ViewBags
            ViewBag.VideosLength = videos.Count;
            if(TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"];
            }

            return View(videos);
        }

        // GET: YoutubeVideos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: YoutubeVideos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "YoutubeVideoID,URL,Name")] YoutubeVideo youtubeVideo)
        {
            if (ModelState.IsValid)
            {
                youtubeVideo.UserPublicationVideo = User.Identity.GetUserName();

                db.YoutubeVideos.Add(youtubeVideo);
                db.SaveChanges();

                // ViewBag
                TempData["Success"] = "Video publicado correctamente";

                return RedirectToAction("Index");
            }

            return View(youtubeVideo);
        }

        // GET: YoutubeVideos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            YoutubeVideo youtubeVideo = db.YoutubeVideos.Find(id);

            if (youtubeVideo == null)
            {
                return HttpNotFound();
            }

            // Check if it's the same User or has persmissions
            if (!User.IsInRole("Admin") && youtubeVideo.UserPublicationVideo != User.Identity.GetUserName())
            {

                TempData["Permissions"] = permissions;

                return RedirectToAction("Admin", "Publications");
            }

            return View(youtubeVideo);
        }

        // POST: YoutubeVideos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "YoutubeVideoID,URL,Name,UserPublicationVideo")] YoutubeVideo youtubeVideo)
        {
            if (ModelState.IsValid)
            {
                youtubeVideo.UserPublicationVideoEdit = User.Identity.GetUserName();

                db.Entry(youtubeVideo).State = EntityState.Modified;
                db.SaveChanges();

                // ViewBag
                TempData["Success"] = "Video actualizado correctamente";

                return RedirectToAction("Index");
            }
            return View(youtubeVideo);
        }

        // GET: YoutubeVideos/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            YoutubeVideo youtubeVideo = db.YoutubeVideos.Find(id);
            if (youtubeVideo == null)
            {
                return HttpNotFound();
            }
            return View(youtubeVideo);
        }

        // POST: YoutubeVideos/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            YoutubeVideo youtubeVideo = db.YoutubeVideos.Find(id);
            db.YoutubeVideos.Remove(youtubeVideo);
            db.SaveChanges();

            // ViewBag
            TempData["Success"] = "Video eliminado correctamente";

            return RedirectToAction("Index");
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
