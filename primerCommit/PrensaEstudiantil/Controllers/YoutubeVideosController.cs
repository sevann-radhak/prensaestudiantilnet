using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PrensaEstudiantil.Context;
using PrensaEstudiantil.Models;

namespace PrensaEstudiantil.Controllers
{
    [Authorize]
    public class YoutubeVideosController : Controller
    {
        private PrensaContext db = new PrensaContext();

        // GET: YoutubeVideos
        public ActionResult Index()
        {
            var videos = db.YoutubeVideos.OrderByDescending(x => x.YoutubeVideoID).ToList();

            ViewBag.VideosLength = videos.Count;

            return View(videos);
        }

        // GET: YoutubeVideos/Details/5
        public ActionResult Details(int? id)
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
                db.YoutubeVideos.Add(youtubeVideo);
                db.SaveChanges();
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
            return View(youtubeVideo);
        }

        // POST: YoutubeVideos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "YoutubeVideoID,URL,Name")] YoutubeVideo youtubeVideo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(youtubeVideo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(youtubeVideo);
        }

        // GET: YoutubeVideos/Delete/5
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
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            YoutubeVideo youtubeVideo = db.YoutubeVideos.Find(id);
            db.YoutubeVideos.Remove(youtubeVideo);
            db.SaveChanges();
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
