using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteShopQuanAo.Models;

namespace WebsiteShopQuanAo.Controllers
{
    public class CartController : Controller
    {
        private QL_ShopQuanAoNuEntities db = new QL_ShopQuanAoNuEntities();

        // GET: Cart
        public ActionResult Index()
        {
            var gIO_HANG = db.GIO_HANG.Include(g => g.TAI_KHOAN);
            return View(gIO_HANG.ToList());
        }

        // GET: Cart/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GIO_HANG gIO_HANG = db.GIO_HANG.Find(id);
            if (gIO_HANG == null)
            {
                return HttpNotFound();
            }
            return View(gIO_HANG);
        }

        // GET: Cart/Create
        public ActionResult Create()
        {
            ViewBag.USERNAME = new SelectList(db.TAI_KHOAN, "USERNAME", "MATKHAU");
            return View();
        }

        // POST: Cart/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MAGH,USERNAME,NGAYTAO,TRANGTHAI")] GIO_HANG gIO_HANG)
        {
            if (ModelState.IsValid)
            {
                db.GIO_HANG.Add(gIO_HANG);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.USERNAME = new SelectList(db.TAI_KHOAN, "USERNAME", "MATKHAU", gIO_HANG.USERNAME);
            return View(gIO_HANG);
        }

        // GET: Cart/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GIO_HANG gIO_HANG = db.GIO_HANG.Find(id);
            if (gIO_HANG == null)
            {
                return HttpNotFound();
            }
            ViewBag.USERNAME = new SelectList(db.TAI_KHOAN, "USERNAME", "MATKHAU", gIO_HANG.USERNAME);
            return View(gIO_HANG);
        }

        // POST: Cart/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MAGH,USERNAME,NGAYTAO,TRANGTHAI")] GIO_HANG gIO_HANG)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gIO_HANG).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.USERNAME = new SelectList(db.TAI_KHOAN, "USERNAME", "MATKHAU", gIO_HANG.USERNAME);
            return View(gIO_HANG);
        }

        // GET: Cart/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GIO_HANG gIO_HANG = db.GIO_HANG.Find(id);
            if (gIO_HANG == null)
            {
                return HttpNotFound();
            }
            return View(gIO_HANG);
        }

        // POST: Cart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            GIO_HANG gIO_HANG = db.GIO_HANG.Find(id);
            db.GIO_HANG.Remove(gIO_HANG);
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
