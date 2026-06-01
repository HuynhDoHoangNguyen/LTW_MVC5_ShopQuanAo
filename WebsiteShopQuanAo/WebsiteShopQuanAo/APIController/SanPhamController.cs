using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebsiteShopQuanAo.Models;

namespace WebsiteShopQuanAo.APIController
{
    [RoutePrefix("api/SanPham")]
    public class SanPhamController : ApiController
    {
        private QL_ShopQuanAoNuEntities db = new QL_ShopQuanAoNuEntities();

        // GET: api/SanPham
        [HttpGet]
        [Route("")]
        public List<ProductItemVM> Get()
        {
            return db.SAN_PHAM.Select(sp => new ProductItemVM
            {
                MaSanPham = sp.MASP,
                TenSanPham = sp.TENSP,
                TenDanhMuc = sp.DANH_MUC.TENDM,
                MoTa = sp.MOTA,
                GiaBan = sp.CHI_TIET_SP.Min(ct => (decimal?)ct.GIABAN) ?? 0,
                SoLuongTon = sp.SOLUONGTON,
                HinhAnh = sp.HINH_ANH_SP
                        .OrderBy(h => h.TENHINHANH)
                        .Select(h => h.TENHINHANH)
                        .FirstOrDefault()
            }).ToList();
        }

        // POST: api/SanPham?maDanhMuc=DM01
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post(ProductItemVM model, string maDanhMuc)
        {
            if (string.IsNullOrWhiteSpace(model.TenSanPham))
                return BadRequest("Tên sản phẩm không được rỗng");
            if (string.IsNullOrWhiteSpace(maDanhMuc))
                return BadRequest("Mã danh mục không được rỗng");

            db.SP_SANPHAM_ADD(model.TenSanPham, maDanhMuc, model.MoTa);
            return Ok("Thêm sản phẩm thành công ");
        }

        // PUT: api/SanPham/SP01?maDanhMuc=DM01
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(string id, ProductItemVM model, string maDanhMuc)
        {
            var sp = db.SAN_PHAM.Find(id);


            if (sp == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(model.TenSanPham))
                sp.TENSP = model.TenSanPham;

            if (!string.IsNullOrWhiteSpace(maDanhMuc))
                sp.MADM = maDanhMuc;

            if (!string.IsNullOrWhiteSpace(model.MoTa))
                sp.MOTA = model.MoTa;

            if (model.SoLuongTon.HasValue)
                sp.SOLUONGTON = model.SoLuongTon;

            db.SaveChanges();
            return Ok("Cập nhật thành công");
        }


        // DELETE: api/SanPham/SP01
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(string id)
        {
            var sp = db.SAN_PHAM.Find(id);
            var hd = db.CT_DON_HANG.Any(ct => ct.MASP == id); 
            if (sp == null) return NotFound();

            // 1. KIỂM TRA RÀNG BUỘC (Proactive Check)
            // Kiểm tra xem sản phẩm quần áo này đã có chi tiết (size, màu) hay đã được khách mua (nằm trong hóa đơn) chưa
            // Lưu ý: Tên property (CHI_TIET_SP, CTHOADON) cần khớp với navigation property trong model của bạn.
            bool hasDetails = sp.CHI_TIET_SP != null && sp.CHI_TIET_SP.Any();
            
            bool hasOrders = hd; 
            if (hasDetails || hasOrders)
            {
                return BadRequest("Không thể xóa! Sản phẩm này đang có chi tiết sản phẩm hoặc đã nằm trong hóa đơn.");
            }

            try
            {
                // 2. THỰC HIỆN XÓA (Xóa cứng)
                db.SAN_PHAM.Remove(sp);
                db.SaveChanges();
                return Ok("Đã xóa");
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                // 3. BẮT LỖI KHÓA NGOẠI TỪ DATABASE (Safety Net)
                // Đề phòng trường hợp sót bảng liên kết nào đó chưa check ở bước 1
                return BadRequest("Lỗi ràng buộc: Không thể xóa sản phẩm đang được liên kết với dữ liệu khác.");
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

    }
}
