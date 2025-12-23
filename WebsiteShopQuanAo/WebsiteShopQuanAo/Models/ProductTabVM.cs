using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteShopQuanAo.Models
{
    public class ProductTabVM
    {
        public string MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }
        public List<ProductItemVM> SanPhams { get; set; }
    }
}