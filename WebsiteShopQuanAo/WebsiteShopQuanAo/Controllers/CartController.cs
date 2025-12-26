using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
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

        public ActionResult RemoveFromCart(string MaCTSP)
        {
            Dictionary<string, CTGioHang> gioHang = new Dictionary<string, CTGioHang>();

            var ctsp = db.CHI_TIET_SP.FirstOrDefault(x => x.MACTSP == MaCTSP && x.TRANGTHAI == true);
            //Kiểm tra session
            if (Session["Cart"] != null)
            {
                gioHang = (Dictionary<string, CTGioHang>)Session["Cart"];
            }

            if (gioHang.ContainsKey(MaCTSP))
            {
                gioHang.Remove(MaCTSP);
            }
           

            Session["Cart"] = gioHang;

            return RedirectToAction("Index", "Cart");
        }


        public ActionResult ClearCart()
        {
            Dictionary<string, CTGioHang> gioHang = new Dictionary<string, CTGioHang>();

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

        public ActionResult Checkout()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Login", "User");
            }


            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "Cart");
            }

            var cart = (Dictionary<string, CTGioHang>)Session["Cart"];

            if (cart.Count() == 0)
            {
                return RedirectToAction("Index", "Cart");

            }

            var ctspKeys = cart.Keys.ToList();

            var ctspList = db.CHI_TIET_SP.Where(x => ctspKeys.Contains(x.MACTSP)).ToList();

            var vm = new CheckoutVM
            {
                CTSPs = ctspList,
                TongSoLuong = cart.Sum(x => x.Value.SoLuong),
                TongTien = cart.Sum(x => x.Value.SoLuong * x.Value.Gia),
                HinhThucThanhToan = "COD"
            };

            return View(vm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlaceOrder(CheckoutVM model)
        {
            if (Session["Cart"] == null || Session["MAKH"] == null)
                return RedirectToAction("Index", "Cart");

            var cart = (Dictionary<string, CTGioHang>)Session["Cart"];
            string makh = Session["MAKH"].ToString();

            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    // tạo đơn hàng
                    var madhParam = new ObjectParameter("MADH", typeof(string));

                    db.SP_DONHANG_CREATE(
                        makh,
                        model.DiaChiGiao,
                        model.HinhThucThanhToan,
                        madhParam
                    );

                    string madh = madhParam.Value.ToString();


                    // thêm chi tiết đơn hàng
                    foreach (var item in cart)
                    {
                        var ctsp = db.CHI_TIET_SP.First(x => x.MACTSP == item.Key);

                        db.SP_CTDH_ADD(
                            madh,
                            ctsp.MASP,
                            ctsp.MAMAU,
                            ctsp.MASIZE,
                            item.Value.SoLuong
                        );
                    }

                    tran.Commit();
                    Session["Cart"] = null;

                    return RedirectToAction("Success");
                }
                catch
                {
                    var ctspKeys = cart.Keys.ToList();
                    var ctspList = db.CHI_TIET_SP
                                     .Where(x => ctspKeys.Contains(x.MACTSP))
                                     .ToList();

                    model.CTSPs = ctspList;
                    model.TongSoLuong = cart.Sum(x => x.Value.SoLuong);
                    model.TongTien = cart.Sum(x => x.Value.SoLuong * x.Value.Gia);

                    tran.Rollback();
                    ViewBag.Error = "Không đủ tồn kho hoặc lỗi hệ thống";
                    return View("Checkout", model);
                }
            }
        }

        public ActionResult Success()
        {
            if (Session["MAKH"] == null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        public ActionResult MiniCart()
        {
            if (Session["Cart"] == null)
            {
                return PartialView("_MiniCart", new List<CHI_TIET_SP>());
            }
            Dictionary<string, CTGioHang> gioHang = (Dictionary<string, CTGioHang>)Session["Cart"];

            var lstCTSP = db.CHI_TIET_SP.Where(ct => gioHang.Keys.Contains(ct.MACTSP));

            return PartialView("_MiniCart", lstCTSP);
        }

        public ActionResult CartItemCount()
        {
            var cart = Session["GioHang"] as Dictionary<string, CTGioHang>;

            int count = 0;
            if (cart != null)
            {
                count = cart.Count();
            }

            return PartialView("_CartItemCount", count);
        }


        public ActionResult MyOrders()
        {
            if (Session["MAKH"] == null)
                return RedirectToAction("Login", "User");

            string makh = Session["MAKH"].ToString();

            var orders = db.DON_HANG.Where(x => x.MAKH == makh).OrderByDescending(x => x.NGAYDAT).ToList();

            return View(orders);
        }


        public ActionResult OrderDetails(string id)
        {
            if (Session["MAKH"] == null)
                return RedirectToAction("Login", "User");

            if (id == null)
                return HttpNotFound();

            string makh = Session["MAKH"].ToString();

            var order = db.DON_HANG.FirstOrDefault(x => x.MADH == id && x.MAKH == makh);

            if (order == null)
                return HttpNotFound();

            return View(order);
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
