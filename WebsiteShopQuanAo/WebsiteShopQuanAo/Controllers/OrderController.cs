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
    public class OrderController : Controller
    {
        private QL_ShopQuanAoNuEntities db = new QL_ShopQuanAoNuEntities();

        // GET: Order
        public ActionResult Index()
        {
            var dON_HANG = db.DON_HANG.Include(d => d.KHACH_HANG);
            return View(dON_HANG.ToList());
        }

        // GET: Order/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DON_HANG dON_HANG = db.DON_HANG.Find(id);
            if (dON_HANG == null)
            {
                return HttpNotFound();
            }
            return View(dON_HANG);
        }

        // GET: Order/Create
        public ActionResult Create()
        {
            ViewBag.MAKH = new SelectList(db.KHACH_HANG, "MAKH", "HOTEN");
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MADH,MAKH,NGAYDAT,HINHTHUCTHANHTOAN,DIACHIGIAO,GIAMGIA,TONGSOLUONG,TONGTHANHTIEN,TRANGTHAI")] DON_HANG dON_HANG)
        {
            if (ModelState.IsValid)
            {
                db.DON_HANG.Add(dON_HANG);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MAKH = new SelectList(db.KHACH_HANG, "MAKH", "HOTEN", dON_HANG.MAKH);
            return View(dON_HANG);
        }

        // GET: Order/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DON_HANG dON_HANG = db.DON_HANG.Find(id);
            if (dON_HANG == null)
            {
                return HttpNotFound();
            }
            ViewBag.MAKH = new SelectList(db.KHACH_HANG, "MAKH", "HOTEN", dON_HANG.MAKH);
            return View(dON_HANG);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MADH,MAKH,NGAYDAT,HINHTHUCTHANHTOAN,DIACHIGIAO,GIAMGIA,TONGSOLUONG,TONGTHANHTIEN,TRANGTHAI")] DON_HANG dON_HANG)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dON_HANG).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MAKH = new SelectList(db.KHACH_HANG, "MAKH", "HOTEN", dON_HANG.MAKH);
            return View(dON_HANG);
        }

        // GET: Order/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DON_HANG dON_HANG = db.DON_HANG.Find(id);
            if (dON_HANG == null)
            {
                return HttpNotFound();
            }
            return View(dON_HANG);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DON_HANG dON_HANG = db.DON_HANG.Find(id);
            db.DON_HANG.Remove(dON_HANG);
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
