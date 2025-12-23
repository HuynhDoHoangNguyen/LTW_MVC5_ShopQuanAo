using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteShopQuanAo.Models
{
    public class ProductDetailVM
    {
        public SAN_PHAM SanPham { get; set; }

        public List<HINH_ANH_SP> HinhAnhs { get; set; }

        public List<CHI_TIET_SP> ChiTietSanPhams { get; set; }

     

        public List<ProductItemVM> RelatedProducts { get; set; }
    }
}