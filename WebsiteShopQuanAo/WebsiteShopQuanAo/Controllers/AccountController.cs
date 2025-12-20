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
        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            // 1. Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return View();
            }

            // 2. Kiểm tra trong Cơ sở dữ liệu
            // Lưu ý: t.MATKHAU nên được mã hóa nếu bạn có làm phần bảo mật
            var user = db.TAI_KHOAN.SingleOrDefault(t => t.USERNAME == username && t.MATKHAU == password);

            if (user != null)
            {
                // 3. Nếu đăng nhập thành công: Lưu thông tin vào Session
                Session["User"] = user;
                Session["UserName"] = user.USERNAME;
                Session["UserRole"] = user.MAVT; // Lưu mã vai trò để phân quyền sau này

                // Chuyển hướng về trang chủ hoặc trang mong muốn
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // 4. Nếu thất bại
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            Session.Clear(); // Xóa sạch session khi đăng xuất
            return RedirectToAction("Login");
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
