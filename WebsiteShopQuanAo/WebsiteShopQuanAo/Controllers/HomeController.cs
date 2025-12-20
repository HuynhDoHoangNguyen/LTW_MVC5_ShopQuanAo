using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteShopQuanAo.Models;

namespace WebsiteShopQuanAo.Controllers
{
    public class HomeController : Controller
    {
        private QL_ShopQuanAoNuEntities db = new QL_ShopQuanAoNuEntities();

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Blank()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(string USERNAME, string MATKHAU, string CMATKHAU)
        {
            var cus = db.TAI_KHOAN.Where(t => t.USERNAME == USERNAME).FirstOrDefault();
            if(cus != null)
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                return View();
            }
            if(MATKHAU != CMATKHAU)
            {
                ViewBag.Error = "Mật khẩu không khớp!";
                return View();
            }

            var newUser = new TAI_KHOAN
            {
                USERNAME = USERNAME,
                MATKHAU = MATKHAU,
                MAVT = "VT02", // Vai trò khách hàng
                TRANGTHAI = db.TAI_KHOAN.FirstOrDefault(x => x.MAVT == "VT02").TRANGTHAI
            };

            db.TAI_KHOAN.Add(newUser);

            db.SaveChanges();

            ViewBag.Error = "Dang ky thanh cong!";

            return RedirectToAction("Login", "Home");
        }
        public ActionResult Login()
        {
            return View();
        }
        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string USERNAME, string MATKHAU)
        {
            // 1. Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(USERNAME) || string.IsNullOrEmpty(MATKHAU))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return View();
            }

            // 2. Kiểm tra trong Cơ sở dữ liệu
            // Lưu ý: t.MATKHAU nên được mã hóa nếu bạn có làm phần bảo mật
            var user = db.TAI_KHOAN.FirstOrDefault(t => t.USERNAME == USERNAME && t.MATKHAU == MATKHAU);

            if (user != null)
            {
                // 3. Nếu đăng nhập thành công: Lưu thông tin vào Session
                Session["User"] = user;
                Session["UserName"] = user.USERNAME;
                Session["UserRole"] = user.MAVT; // Lưu mã vai trò để phân quyền sau này

                // Chuyển hướng về trang chủ hoặc trang mong muốn
                return RedirectToAction("Products", "Home");
            }
            else
            {
                // 4. Nếu thất bại
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }
        }

        public ActionResult Products(string id)
        {
            if (id != null)
            {
                return View(db.SAN_PHAM.Where(x => x.MADM == id).ToList());
            }
            return View(db.SAN_PHAM.ToList());
        }
        public ActionResult DanhMuc()
        {
            return PartialView(db.DANH_MUC.ToList());
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