using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteShopQuanAo.Models;

namespace WebsiteShopQuanAo.Controllers
{
    public class UserController : Controller
    {
        private QL_ShopQuanAoNuEntities db = new QL_ShopQuanAoNuEntities();

        // GET: User
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string matkhau)
        {
            var user = db.TAI_KHOAN.Where(x => x.USERNAME == username && x.MATKHAU == matkhau && x.VAI_TRO.TENVAI == "user").FirstOrDefault();
            if (user == null)
            {
                ViewBag.Error = "Đăng nhập thất bại!!!";
                return View();
            }

            else
            {
                Session["User"] = user;
                Session["Username"] = user.USERNAME;
                Session["MAKH"] = user.KHACH_HANG.First().MAKH;
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string username, string matkhau, string hoten, string email, string sdt, string diachi)
        {
            try
            {
                db.SP_DANGKY(username,matkhau,hoten,email,sdt,diachi);
                return RedirectToAction("Login", "User");
            }
            catch
            {
                ViewBag.Error = "Username / Email / SĐT đã tồn tại";
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session["User"] = null;
            Session["Username"] = null;
            Session["MAKH"] = null;
            return RedirectToAction("Index", "Home");
        }






        // GET: User/Details/5
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

        // GET: User/Create
        public ActionResult Create()
        {
            ViewBag.MAVT = new SelectList(db.VAI_TRO, "MAVT", "TENVAI");
            return View();
        }

        // POST: User/Create
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

        // GET: User/Edit/5
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

        // POST: User/Edit/5
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

        // GET: User/Delete/5
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

        // POST: User/Delete/5
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
