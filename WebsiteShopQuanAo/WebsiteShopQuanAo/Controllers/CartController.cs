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
            
            if (Session["Cart"] == null)
            {
                return View(new List<CHI_TIET_SP>());
            }
            Dictionary<string, CTGioHang> gioHang = (Dictionary<string, CTGioHang>)Session["Cart"];

            var lstCTSP = db.CHI_TIET_SP.Where(ct => gioHang.Keys.Contains(ct.MACTSP));

            return View(lstCTSP.ToList());
        }

        [HttpPost]
        public ActionResult AddToCart(CTGioHang model)
        {

            // tạo giỏ hàng để lưu tạm
            Dictionary<string, CTGioHang> gioHang = new Dictionary<string, CTGioHang>();

            var ctsp = db.CHI_TIET_SP.FirstOrDefault(x => x.MACTSP == model.MaCTSP && x.TRANGTHAI == true);
            //Kiểm tra session
            if (Session["Cart"] != null)
            {
                gioHang = (Dictionary<string, CTGioHang>)Session["Cart"];
            }

            if (gioHang.ContainsKey(model.MaCTSP))
            {
                int tongSoLuong = gioHang[model.MaCTSP].SoLuong + model.SoLuong;

                // Không cho vượt tồn
                if (tongSoLuong > ctsp.SOLUONGTON)
                    gioHang[model.MaCTSP].SoLuong = ctsp.SOLUONGTON.Value;
                else
                    gioHang[model.MaCTSP].SoLuong = tongSoLuong;
            }
            else
            {
                gioHang.Add(model.MaCTSP, model);
            }

            Session["Cart"] = gioHang;

            return RedirectToAction("Index", "Cart");
        }


        // GET: Cart/Details/5
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

        // GET: Cart/Create
        public ActionResult Create()
        {
            ViewBag.MADM = new SelectList(db.DANH_MUC, "MADM", "TENDM");
            return View();
        }

        // POST: Cart/Create
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

        // GET: Cart/Edit/5
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

        // POST: Cart/Edit/5
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

        // GET: Cart/Delete/5
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

        // POST: Cart/Delete/5
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
