using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using WebsiteShopQuanAo.Models;

namespace WebsiteShopQuanAo.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        private QL_ShopQuanAoNuEntities db = new QL_ShopQuanAoNuEntities();
        public ActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn(string Email, string Password)
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ Email và Mật khẩu!";
                return View();
            }
            var admin =db.TAI_KHOAN.FirstOrDefault(t => t.USERNAME == Email && t.MATKHAU == Password && t.MAVT == "VT01");
         
            if (admin != null)
            {
                Session["AdminName"] = "Quản Trị Viên";
                Session["AdminEmail"] = admin.USERNAME;
                return RedirectToAction("Index", "Admin");
            }
            ViewBag.Error = "Email hoặc mật khẩu không chính xác!";
            return View();
        }
        // GET: Admin/Admin
        public ActionResult LogOut()
        {
            Session.Remove("AdminName");
            Session.Remove("AdminEmail");

            Session.Clear(); 
            Session.Abandon();
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        public ActionResult Index()
        {
            var sAN_PHAM = db.SAN_PHAM.Include(s => s.DANH_MUC).Include(s => s.HINH_ANH_SP);
            return View(sAN_PHAM.ToList());
        }
        public ActionResult StatusShop(string searchString, string idDanhMuc)
        {
            var tongKho = db.CHI_TIET_SP
                            .Where(t => t.MASP != null && t.GIABAN != null)
                            .GroupBy(t => t.MASP)
                            .Select(k => new { MASP = k.Key, GiaTriKho = k.Sum(t => (decimal?)t.GIABAN * t.SOLUONGTON) });

            decimal? tongGiaTriKho = tongKho.Sum(t => t.GiaTriKho);
            ViewBag.TongKho = (tongGiaTriKho ?? 0) / 1000000;
            ViewBag.idDanhMuc = new SelectList(db.DANH_MUC, "MADM", "TENDM");

            var sAN_PHAM = db.SAN_PHAM.Include(s => s.DANH_MUC).Include(s => s.HINH_ANH_SP).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                sAN_PHAM = sAN_PHAM.Where(s => s.TENSP.Contains(searchString) || s.MASP.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(idDanhMuc))
            {
                sAN_PHAM = sAN_PHAM.Where(s => s.MADM == idDanhMuc);
            }

            return View(sAN_PHAM.ToList());
        }

        // GET: Admin/Admin/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SAN_PHAM sAN_PHAM = db.SAN_PHAM.Find(id);
            if (sAN_PHAM == null)
            {
                return HttpNotFound();
            }
            return View(sAN_PHAM);
        }

        // GET: Admin/Admin/Create
        public ActionResult Create()
        {
            ViewBag.MADM = new SelectList(db.DANH_MUC, "MADM", "TENDM");
            return View();
        }

        // POST: Admin/Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "MASP,TENSP,MADM,MOTA,SOLUONGTON,TRANGTHAI")] SAN_PHAM sAN_PHAM)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.SAN_PHAM.Add(sAN_PHAM);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.MADM = new SelectList(db.DANH_MUC, "MADM", "TENDM", sAN_PHAM.MADM);
        //    return View(sAN_PHAM);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MASP,TENSP,MADM,MOTA,SOLUONGTON,TRANGTHAI")] SAN_PHAM sAN_PHAM, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {

                db.SAN_PHAM.Add(sAN_PHAM);
                db.SaveChanges();

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var lastImage = db.HINH_ANH_SP.OrderByDescending(h => h.MAHINH).FirstOrDefault();
                    int nextHANumber = 1;

                    if (lastImage != null && !string.IsNullOrEmpty(lastImage.MAHINH) && lastImage.MAHINH.StartsWith("HA"))
                    {
                        string numPart = lastImage.MAHINH.Substring(2);
                        if (int.TryParse(numPart, out int currentNumber))
                        {
                            nextHANumber = currentNumber + 1;
                        }
                    }
                    string maHinhAnhMoi = "HA" + nextHANumber.ToString("000");

                    string uploadPath = Server.MapPath("~/UI_User/assets/img/product/small-product/");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    string fileName = maHinhAnhMoi + "_" + sAN_PHAM.MASP + Path.GetExtension(imageFile.FileName);
                    string filePath = Path.Combine(uploadPath, fileName);
                    imageFile.SaveAs(filePath);
                    var hinhAnh = new HINH_ANH_SP
                    {
                        MAHINH = maHinhAnhMoi,   
                        MASP = sAN_PHAM.MASP,   
                        TENHINHANH = fileName,

                    };
                    db.HINH_ANH_SP.Add(hinhAnh);
                    db.SaveChanges();
                }

                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                return RedirectToAction("StatusShop");
            }

            ViewBag.MADM = new SelectList(db.DANH_MUC, "MADM", "TENDM", sAN_PHAM.MADM);
            return View(sAN_PHAM);
        }
        // GET: Admin/Admin/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SAN_PHAM sAN_PHAM = db.SAN_PHAM.Find(id);
            if (sAN_PHAM == null)
            {
                return HttpNotFound();
            }
            ViewBag.MADM = new SelectList(db.DANH_MUC, "MADM", "TENDM", sAN_PHAM.MADM);
            return View(sAN_PHAM);
        }

        // POST: Admin/Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "MASP,TENSP,MADM,MOTA,SOLUONGTON,TRANGTHAI")] SAN_PHAM sAN_PHAM)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(sAN_PHAM).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.MADM = new SelectList(db.DANH_MUC, "MADM", "TENDM", sAN_PHAM.MADM);
        //    return View(sAN_PHAM);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MASP,TENSP,MADM,MOTA,SOLUONGTON,TRANGTHAI")] SAN_PHAM sAN_PHAM, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                var productInDb = db.SAN_PHAM.Find(sAN_PHAM.MASP);
                if (productInDb == null) return HttpNotFound();
                productInDb.TENSP = sAN_PHAM.TENSP;
                productInDb.MADM = sAN_PHAM.MADM;
                productInDb.MOTA = sAN_PHAM.MOTA;
                productInDb.SOLUONGTON = sAN_PHAM.SOLUONGTON;
                productInDb.TRANGTHAI = sAN_PHAM.TRANGTHAI;

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string uploadPath = Server.MapPath("~/UI_User/assets/img/product/small-product/");
                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                    var oldImage = db.HINH_ANH_SP.FirstOrDefault(h => h.MASP == sAN_PHAM.MASP);
                    if (oldImage != null)
                    {
                        string oldFilePath = Path.Combine(uploadPath, oldImage.TENHINHANH);
                        if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
                        db.HINH_ANH_SP.Remove(oldImage);
                        db.SaveChanges();
                    }
                    var lastImage = db.HINH_ANH_SP.OrderByDescending(h => h.MAHINH).FirstOrDefault();
                    int nextHANumber = 1;
                    if (lastImage != null && lastImage.MAHINH.StartsWith("HA"))
                    {
                        int.TryParse(lastImage.MAHINH.Substring(2), out nextHANumber);
                        nextHANumber++;
                    }
                    string maHinhMoi = "HA" + nextHANumber.ToString("000");
                    string fileName = maHinhMoi + "_" + sAN_PHAM.MASP + Path.GetExtension(imageFile.FileName);
                    string filePath = Path.Combine(uploadPath, fileName);
                    imageFile.SaveAs(filePath);
                    var newHinh = new HINH_ANH_SP
                    {
                        MAHINH = maHinhMoi,
                        MASP = sAN_PHAM.MASP,
                        TENHINHANH = fileName,
                        TRANGTHAI = true
                    };
                    db.HINH_ANH_SP.Add(newHinh);
                }
                try
                {
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                    return RedirectToAction("StatusShop");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }
            ViewBag.MADM = new SelectList(db.DANH_MUC, "MADM", "TENDM", sAN_PHAM.MADM);
            return View(sAN_PHAM);
        }
        // GET: Admin/Admin/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SAN_PHAM sAN_PHAM = db.SAN_PHAM.Find(id);
            if (sAN_PHAM == null)
            {
                return HttpNotFound();
            }
            return View(sAN_PHAM);
        }

        // POST: Admin/Admin/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    SAN_PHAM sAN_PHAM = db.SAN_PHAM.Find(id);
        //    db.SAN_PHAM.Remove(sAN_PHAM);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        // POST: Admin/Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            SAN_PHAM sAN_PHAM = db.SAN_PHAM.Include(s => s.HINH_ANH_SP).FirstOrDefault(s => s.MASP == id);

            if (sAN_PHAM != null)
            {
                string uploadPath = Server.MapPath("~/UI_User/assets/img/product/small-product/");
                var dsHinhAnh = sAN_PHAM.HINH_ANH_SP.ToList();

                foreach (var hinh in dsHinhAnh)
                {
                    string filePath = Path.Combine(uploadPath, hinh.TENHINHANH);

                    if (System.IO.File.Exists(filePath))
                    {
                        try
                        {
                            System.IO.File.Delete(filePath);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    db.HINH_ANH_SP.Remove(hinh);
                }

                db.SAN_PHAM.Remove(sAN_PHAM);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Xóa sản phẩm và tất cả dữ liệu hình ảnh thành công!";
            }

            return RedirectToAction("StatusShop");
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
