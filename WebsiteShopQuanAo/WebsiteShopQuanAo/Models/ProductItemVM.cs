using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteShopQuanAo.Models
{
    public class ProductItemVM
    {

        public string MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public string TenDanhMuc { get; set; }

        public decimal GiaGoc { get; set; }
        public decimal GiaBan { get; set; }

        

        public List<string> Images { get; set; }

    }

}