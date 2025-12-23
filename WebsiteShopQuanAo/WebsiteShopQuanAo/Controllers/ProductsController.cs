using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebsiteShopQuanAo.Models;

namespace WebsiteShopQuanAo.Controllers
{
    public class ProductsController : Controller
    {
        private readonly QL_ShopQuanAoNuEntities db = new QL_ShopQuanAoNuEntities();


        // GET: Products
        public ActionResult Index(string kw, string khoanggia, string maDanhMuc, int page = 1)
        {
            decimal minPrice = 0;
            decimal maxPrice = 0;
            int pageSize = 6;

            // lấy khoảng giá
            if (!string.IsNullOrEmpty(khoanggia))
            {
                var parts = khoanggia.Split('-');
                if (parts.Length == 2)
                {
                    minPrice = decimal.Parse(parts[0].Replace("₫", "").Replace(".", "").Replace(",", "").Trim());
                    maxPrice = decimal.Parse(parts[1].Replace("₫", "").Replace(".", "").Replace(",", "").Trim());
                }
            }

            // lấy sản phẩm
            var query = db.SAN_PHAM.Where(sp => sp.TRANGTHAI == true);

            if (!string.IsNullOrEmpty(kw))
                query = query.Where(sp => sp.TENSP.Contains(kw));

            if (!string.IsNullOrEmpty(maDanhMuc))
                query = query.Where(sp => sp.MADM == maDanhMuc);

            var sanPhams = query.ToList();

            var products = new List<ProductItemVM>();

            foreach (var sp in sanPhams)
            {
                var ctsp = sp.CHI_TIET_SP
                    .Where(ct => ct.TRANGTHAI == true && ct.GIABAN.HasValue)
                    .ToList();

                if (!ctsp.Any())
                    continue;

                decimal giaGoc = ctsp.Min(ct => ct.GIABAN.Value);

                // ====== LỌC GIÁ (KHÔNG KHUYẾN MÃI) ======
                if (minPrice > 0 && giaGoc < minPrice)
                    continue;

                if (maxPrice > 0 && giaGoc > maxPrice)
                    continue;

                // lấy 1 ảnh đầu tiên (nếu có)
                // đổi x.DUONG_DAN thành đúng tên cột đường dẫn ảnh của bạn (Url/TENHINH/...)
                ////string image = sp.HINH_ANH_SP?
                ////    .Select(x => x.DUONG_DAN)
                ////    .FirstOrDefault();

                products.Add(new ProductItemVM
                {
                    MaSanPham = sp.MASP,
                    TenSanPham = sp.TENSP,
                    TenDanhMuc = sp.DANH_MUC?.TENDM,
                    GiaGoc = giaGoc,
                    GiaBan = giaGoc,
                 
                });
            }

            // TODO: phân trang tuỳ bạn (Skip/Take)
            return View(products);
        }



        public PartialViewResult Sidebar()
        {
            var data = db.NHOM_DANH_MUC
                .Where(n => n.TRANGTHAI == true)
                .ToList();

            return PartialView("_Sidebar", data);
        }
        // GET: Products/Details/5
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

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.MADM = new SelectList(db.DANH_MUC, "MADM", "TENDM");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MASP,TENSP,MADM,MOTA,SOLUONGTON,TRANGTHAI")] SAN_PHAM sAN_PHAM)
        {
            if (ModelState.IsValid)
            {
                db.SAN_PHAM.Add(sAN_PHAM);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MADM = new SelectList(db.DANH_MUC, "MADM", "TENDM", sAN_PHAM.MADM);
            return View(sAN_PHAM);
        }

        // GET: Products/Edit/5
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

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MASP,TENSP,MADM,MOTA,SOLUONGTON,TRANGTHAI")] SAN_PHAM sAN_PHAM)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sAN_PHAM).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MADM = new SelectList(db.DANH_MUC, "MADM", "TENDM", sAN_PHAM.MADM);
            return View(sAN_PHAM);
        }

        // GET: Products/Delete/5
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

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            SAN_PHAM sAN_PHAM = db.SAN_PHAM.Find(id);
            db.SAN_PHAM.Remove(sAN_PHAM);
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
