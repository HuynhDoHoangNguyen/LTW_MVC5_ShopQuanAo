using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteShopQuanAo.Models
{

    public class ThongKeSP
    {
        public string FilterType { get; set; } = "month";

        public DateTime? SelectedDate { get; set; }
        public DateTime? SelectedMonth { get; set; }
        public int? SelectedYear { get; set; }
        public int TotalSuccessOrders { get; set; }
        public int TotalCancelledOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<ChiTietTK> RevenueDetails { get; set; } = new List<ChiTietTK>();
        public List<TopDT> TopProducts { get; set; } = new List<TopDT>();
    }


    public class ChiTietTK
    {
        public string Period { get; set; }
        public int TotalOrders { get; set; }
        public int SuccessOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal Revenue { get; set; }
        public decimal AvgRevenue { get; set; }
    }
    public class ProductTemp
    {
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }
    public class TopDT
    {

        public int Rank { get; set; }
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }
}
