using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteShopQuanAo.Models
{
    public class CheckoutVM
    {
        // thông tin giao hàng
        public string DiaChiGiao { get; set; }

        // COD / BANK
        public string HinhThucThanhToan { get; set; }

        // hiển thị
        public decimal TongTien { get; set; }
        public int TongSoLuong { get; set; }

        // danh sách CTSP trong giỏ
        public List<CHI_TIET_SP> CTSPs { get; set; }
    }

}