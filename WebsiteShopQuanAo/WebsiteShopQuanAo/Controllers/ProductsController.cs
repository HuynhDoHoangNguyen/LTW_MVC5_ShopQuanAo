using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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


        public ActionResult Index(string kw, string maDanhMuc, decimal? min, decimal? max, int page = 1)
        {
            int pageSize = 6;

            // ViewBag để giữ filter khi click menu / phân trang
            ViewBag.Keyword = kw;
            ViewBag.MaDanhMuc = maDanhMuc;
            ViewBag.Min = min;
            ViewBag.Max = max;

            var query = db.SAN_PHAM.Where(sp => sp.TRANGTHAI == true);

            if (!string.IsNullOrWhiteSpace(kw))
                query = query.Where(sp => sp.TENSP.Contains(kw));

            if (!string.IsNullOrWhiteSpace(maDanhMuc))
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

                // lọc giá
                if (min.HasValue && giaGoc < min.Value) continue;
                if (max.HasValue && giaGoc > max.Value) continue;


                var hinhanh = sp.HINH_ANH_SP
                    .Where(h => h.TRANGTHAI == true && !string.IsNullOrEmpty(h.TENHINHANH))
                    .Select(h => Url.Content("~/UI_User/assets/img/product/small-product/" + h.TENHINHANH.Trim()))
                    .ToList();
                if (!hinhanh.Any())
                {
                    hinhanh.Add(Url.Content("~/UI_User/assets/img/product/small-product/product1.jpg"));
                }


                products.Add(new ProductItemVM
                {
                    MaSanPham = sp.MASP,
                    TenSanPham = sp.TENSP,
                    TenDanhMuc = sp.DANH_MUC?.TENDM,
                    GiaGoc = giaGoc,
                    GiaBan = giaGoc,
                    HinhAnh = hinhanh
                });
            }

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
