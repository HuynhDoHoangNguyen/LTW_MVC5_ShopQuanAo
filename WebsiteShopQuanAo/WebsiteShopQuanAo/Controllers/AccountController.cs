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
    public class AccountController : Controller
    {
        private QL_ShopQuanAoNuEntities db = new QL_ShopQuanAoNuEntities();

        // GET: Account
        public ActionResult Index()
        {
            var tAI_KHOAN = db.TAI_KHOAN.Include(t => t.VAI_TRO);
            return View(tAI_KHOAN.ToList());
        }

        // GET: Account/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TAI_KHOAN tAI_KHOAN = db.TAI_KHOAN.Find(id);
            if (tAI_KHOAN == null)
            {
                return HttpNotFound();
            }
            return View(tAI_KHOAN);
        }

        // GET: Account/Create
        public ActionResult Create()
        {
            ViewBag.MAVT = new SelectList(db.VAI_TRO, "MAVT", "TENVAI");
            return View();
        }

        // POST: Account/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "USERNAME,MATKHAU,MAVT,TRANGTHAI")] TAI_KHOAN tAI_KHOAN)
        {
            if (ModelState.IsValid)
            {
                db.TAI_KHOAN.Add(tAI_KHOAN);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MAVT = new SelectList(db.VAI_TRO, "MAVT", "TENVAI", tAI_KHOAN.MAVT);
            return View(tAI_KHOAN);
        }

        // GET: Account/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TAI_KHOAN tAI_KHOAN = db.TAI_KHOAN.Find(id);
            if (tAI_KHOAN == null)
            {
                return HttpNotFound();
            }
            ViewBag.MAVT = new SelectList(db.VAI_TRO, "MAVT", "TENVAI", tAI_KHOAN.MAVT);
            return View(tAI_KHOAN);
        }

        // POST: Account/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "USERNAME,MATKHAU,MAVT,TRANGTHAI")] TAI_KHOAN tAI_KHOAN)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tAI_KHOAN).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MAVT = new SelectList(db.VAI_TRO, "MAVT", "TENVAI", tAI_KHOAN.MAVT);
            return View(tAI_KHOAN);
        }

        // GET: Account/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TAI_KHOAN tAI_KHOAN = db.TAI_KHOAN.Find(id);
            if (tAI_KHOAN == null)
            {
                return HttpNotFound();
            }
            return View(tAI_KHOAN);
        }

        // POST: Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TAI_KHOAN tAI_KHOAN = db.TAI_KHOAN.Find(id);
            db.TAI_KHOAN.Remove(tAI_KHOAN);
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
