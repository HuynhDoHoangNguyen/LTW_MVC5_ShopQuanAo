using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebsiteShopQuanAo.Models;

namespace WebsiteShopQuanAo.Areas.Admin.Controllers
{

    public class ThongKeController : Controller
    {
        private QL_ShopQuanAoNuEntities db = new QL_ShopQuanAoNuEntities();

        [HttpGet]
        public ActionResult Index(string filterType = "month",
                                   DateTime? selectedDate = null,
                                   DateTime? selectedMonth = null,
                                   int? selectedYear = null)
        {
            var model = new ThongKeSP
            {
                FilterType = filterType,
                SelectedDate = selectedDate,
                SelectedMonth = selectedMonth ?? DateTime.Now,
                SelectedYear = selectedYear ?? DateTime.Now.Year
            };

            if (filterType == "day")
            {
                LoadDailyStatistics(model, selectedDate ?? DateTime.Now);
            }
            else if (filterType == "month")
            {
                LoadMonthlyStatistics(model, selectedMonth ?? DateTime.Now);
            }
            else if (filterType == "year")
            {
                LoadYearlyStatistics(model, selectedYear ?? DateTime.Now.Year);
            }

            LoadTopProducts(model);

            return View(model);
        }

        private void LoadDailyStatistics(ThongKeSP model, DateTime selectedDate)
        {
            DateTime startDate = selectedDate.Date;
            DateTime endDate = startDate.AddDays(1);

            List<DON_HANG> orders = db.DON_HANG
                .Where(o => o.NGAYDAT >= startDate && o.NGAYDAT < endDate)
                .ToList();

            for (int hour = 0; hour < 24; hour++)
            {
                DateTime hourStart = startDate.AddHours(hour);
                DateTime hourEnd = hourStart.AddHours(1);

                List<DON_HANG> hourOrders = orders
                    .Where(o => o.NGAYDAT >= hourStart && o.NGAYDAT < hourEnd)
                    .ToList();

                List<DON_HANG> successOrders = hourOrders.Where(o => o.TRANGTHAI == true).ToList();
                List<DON_HANG> cancelledOrders = hourOrders.Where(o => o.TRANGTHAI == false).ToList();

                decimal revenue = 0;
                foreach (var order in successOrders)
                {
                    revenue += order.TONGTHANHTIEN ?? 0;
                }

                decimal avgRevenue = 0;
                if (successOrders.Count > 0)
                {
                    avgRevenue = revenue / successOrders.Count;
                }

                model.RevenueDetails.Add(new ChiTietTK
                {
                    Period = $"{hour:D2}:00",
                    TotalOrders = hourOrders.Count,
                    SuccessOrders = successOrders.Count,
                    CancelledOrders = cancelledOrders.Count,
                    Revenue = revenue,
                    AvgRevenue = avgRevenue
                });
            }

            CalculateTotals(model);
        }

        private void LoadMonthlyStatistics(ThongKeSP model, DateTime selectedMonth)
        {
            DateTime startDate = new DateTime(selectedMonth.Year, selectedMonth.Month, 1);
            DateTime endDate = startDate.AddMonths(1);

            List<DON_HANG> orders = db.DON_HANG
                .Where(o => o.NGAYDAT >= startDate && o.NGAYDAT < endDate)
                .ToList();

            int daysInMonth = DateTime.DaysInMonth(selectedMonth.Year, selectedMonth.Month);

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime dayStart = new DateTime(selectedMonth.Year, selectedMonth.Month, day);
                DateTime dayEnd = dayStart.AddDays(1);

                List<DON_HANG> dayOrders = orders
                    .Where(o => o.NGAYDAT >= dayStart && o.NGAYDAT < dayEnd)
                    .ToList();

                List<DON_HANG> successOrders = dayOrders.Where(o => o.TRANGTHAI == true).ToList();
                List<DON_HANG> cancelledOrders = dayOrders.Where(o => o.TRANGTHAI == false).ToList();

                decimal revenue = 0;
                foreach (var order in successOrders)
                {
                    revenue += order.TONGTHANHTIEN ?? 0;
                }

                decimal avgRevenue = 0;
                if (successOrders.Count > 0)
                {
                    avgRevenue = revenue / successOrders.Count;
                }

                model.RevenueDetails.Add(new ChiTietTK
                {
                    Period = dayStart.ToString("dd/MM"),
                    TotalOrders = dayOrders.Count,
                    SuccessOrders = successOrders.Count,
                    CancelledOrders = cancelledOrders.Count,
                    Revenue = revenue,
                    AvgRevenue = avgRevenue
                });
            }

            CalculateTotals(model);
        }

        private void LoadYearlyStatistics(ThongKeSP model, int selectedYear)
        {
            DateTime startDate = new DateTime(selectedYear, 1, 1);
            DateTime endDate = startDate.AddYears(1);

            List<DON_HANG> orders = db.DON_HANG
                .Where(o => o.NGAYDAT >= startDate && o.NGAYDAT < endDate)
                .ToList();

            for (int month = 1; month <= 12; month++)
            {
                DateTime monthStart = new DateTime(selectedYear, month, 1);
                DateTime monthEnd = monthStart.AddMonths(1);

                List<DON_HANG> monthOrders = orders
                    .Where(o => o.NGAYDAT >= monthStart && o.NGAYDAT < monthEnd)
                    .ToList();

                List<DON_HANG> successOrders = monthOrders.Where(o => o.TRANGTHAI == true).ToList();
                List<DON_HANG> cancelledOrders = monthOrders.Where(o => o.TRANGTHAI == false).ToList();

                decimal revenue = 0;
                foreach (var order in successOrders)
                {
                    revenue += order.TONGTHANHTIEN ?? 0;
                }

                decimal avgRevenue = 0;
                if (successOrders.Count > 0)
                {
                    avgRevenue = revenue / successOrders.Count;
                }

                model.RevenueDetails.Add(new ChiTietTK
                {
                    Period = $"Tháng {month}",
                    TotalOrders = monthOrders.Count,
                    SuccessOrders = successOrders.Count,
                    CancelledOrders = cancelledOrders.Count,
                    Revenue = revenue,
                    AvgRevenue = avgRevenue
                });
            }

            CalculateTotals(model);
        }

        private void LoadTopProducts(ThongKeSP model)
        {
            List<CT_DON_HANG> allOrderDetails = db.CT_DON_HANG
                .Where(od => od.DON_HANG.TRANGTHAI == true)
                .ToList();
            List<SAN_PHAM> allProducts = db.SAN_PHAM.ToList();

            // Nhóm theo MASP
            var productGroups = new Dictionary<string, ProductTemp>();

            foreach (var detail in allOrderDetails)
            {
                string masp = detail.MASP;
                SAN_PHAM product = allProducts.FirstOrDefault(p => p.MASP == masp);

                if (product != null)
                {
                    string productName = product.TENSP;

                    if (!productGroups.ContainsKey(productName))
                    {
                        productGroups[productName] = new ProductTemp
                        {
                            ProductName = productName,
                            QuantitySold = 0,
                            Revenue = 0
                        };
                    }

                    productGroups[productName].QuantitySold += detail.SOLUONG;
                    productGroups[productName].Revenue += detail.THANHTIEN ?? 0;
                }
            }
            List<ProductTemp> sortedProducts = productGroups.Values
                .OrderByDescending(p => p.QuantitySold)
                .Take(5)
                .ToList();

            // Gán thứ hạng
            int rank = 1;
            foreach (var product in sortedProducts)
            {
                model.TopProducts.Add(new TopDT
                {
                    Rank = rank,
                    ProductName = product.ProductName,
                    QuantitySold = product.QuantitySold,
                    Revenue = product.Revenue
                });
                rank++;
            }
        }


        private void CalculateTotals(ThongKeSP model)
        {
            model.TotalSuccessOrders = 0;
            foreach (var item in model.RevenueDetails)
            {
                model.TotalSuccessOrders += item.SuccessOrders;
            }

            model.TotalCancelledOrders = 0;
            foreach (var item in model.RevenueDetails)
            {
                model.TotalCancelledOrders += item.CancelledOrders;
            }

            model.TotalRevenue = 0;
            foreach (var item in model.RevenueDetails)
            {
                model.TotalRevenue += item.Revenue;
            }

            if (model.TotalSuccessOrders > 0)
            {
                model.AverageOrderValue = model.TotalRevenue / model.TotalSuccessOrders;
            }
            else
            {
                model.AverageOrderValue = 0;
            }
        }

   
    }
}