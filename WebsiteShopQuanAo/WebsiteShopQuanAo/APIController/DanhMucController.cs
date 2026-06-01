using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebsiteShopQuanAo.Models;

namespace WebsiteShopQuanAo.APIController
{
    [RoutePrefix("api/DanhMuc")]
    public class DanhMucController : ApiController
    {
        private QL_ShopQuanAoNuEntities db = new QL_ShopQuanAoNuEntities();

        // GET: api/DanhMuc
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var data = db.DANH_MUC.Select(dm => new
            {
                dm.MADM,
                dm.TENDM,
                dm.MANHOM,
                dm.TRANGTHAI
            }).ToList();

            return Ok(data);
        }

        // POST: api/DanhMuc
        [HttpPost]
        [Route("")]
        public IHttpActionResult Post(DANH_MUC model)
        {
            if (string.IsNullOrWhiteSpace(model.TENDM))
                return BadRequest("Tên danh mục không được rỗng");

            db.SP_DM_ADD(model.TENDM, model.MANHOM);

            return Ok("Thêm danh mục thành công");
        }

        // PUT: api/DanhMuc/DM01
        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult Put(string id, DANH_MUC model)
        {
            var dm = db.DANH_MUC.Find(id);
            if (dm == null) return NotFound();

            dm.TENDM = model.TENDM;
            dm.MANHOM = model.MANHOM;
            dm.TRANGTHAI = model.TRANGTHAI;

            db.SaveChanges();
            return Ok("Cập nhật danh mục thành công");
        }

        // DELETE: api/DanhMuc/DM01
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(string id)
        {
            var dm = db.DANH_MUC.Find(id);
            if (dm == null) return NotFound();

           
            if (dm.SAN_PHAM.Any())
            {
                return BadRequest("Không thể xóa! Đang có sản phẩm thuộc danh mục này.");
            }

            try
            {
                // 2. THỰC HIỆN XÓA
                db.DANH_MUC.Remove(dm); // Lưu ý: Đây là xóa vật lý (Hard delete)
                db.SaveChanges();

                return Ok("Đã xóa danh mục thành công.");
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                // 3. BẮT LỖI KHÓA NGOẠI (Safety Net)
                // Bắt lỗi khi database từ chối lệnh xóa do dính FK ở một bảng nào đó
                return BadRequest("Lỗi ràng buộc dữ liệu: Danh mục đang được sử dụng.");
            }
            catch (Exception ex)
            {
                // Bắt các lỗi hệ thống khác
                return InternalServerError(ex);
            }
        }
    }
}
