using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TourBooking.Views
{
    public partial class TourSearchView : UserControl
    {
        public TourSearchView()
        {
            InitializeComponent();

            // Nạp dữ liệu thiết kế cứng để test giao diện
            LoadDesignTourData();
        }

        private void LoadDesignTourData()
        {
            try
            {
                var toursInDb = new List<TourDesignModel>
                {
                    new TourDesignModel { TourCode = "HL-2023-001", TourName = "Khám phá Vịnh Hạ Long - 3 Ngày 2 Đêm", Description = "Hà Nội - Hạ Long - Tuần Châu", StartDate = new DateTime(2023, 10, 15), TotalSlots = 20, AvailableSlots = 12, PriceAdult = 4500000 },
                    new TourDesignModel { TourCode = "DN-2023-042", TourName = "Đà Nẵng - Hội An - Bà Nà Hills", Description = "Đà Nẵng - Ngũ Hành Sơn - Hội An", StartDate = new DateTime(2023, 10, 18), TotalSlots = 20, AvailableSlots = 3, PriceAdult = 3800000 },
                    new TourDesignModel { TourCode = "PQ-2023-109", TourName = "Nghỉ dưỡng Đảo Ngọc Phú Quốc", Description = "Bãi Sao - VinWonders - Safari", StartDate = new DateTime(2023, 10, 20), TotalSlots = 20, AvailableSlots = 0, PriceAdult = 6200000 },
                    new TourDesignModel { TourCode = "SP-2023-015", TourName = "Chinh phục đỉnh Fansipan - Sapa", Description = "Bản Cát Cát - Thác Bạc - Fansipan", StartDate = new DateTime(2023, 10, 25), TotalSlots = 30, AvailableSlots = 25, PriceAdult = 2900000 }
                };

                // ĐÃ ĐỔI SANG: TourSearchDesignRowModel
                var displayList = toursInDb.Select(t => {
                    string statusText = $"Còn {t.AvailableSlots} chỗ";
                    string statusColor = "#12B76A";

                    if (t.AvailableSlots == 0)
                    {
                        statusText = "Hết chỗ";
                        statusColor = "#C5221F";
                    }
                    else if (t.AvailableSlots <= 5)
                    {
                        statusText = $"Gần hết (Còn {t.AvailableSlots} chỗ)";
                        statusColor = "#F79009";
                    }

                    return new TourSearchDesignRowModel
                    {
                        TourCode = t.TourCode,
                        TourName = t.TourName,
                        Description = t.Description,
                        StartDate = t.StartDate,
                        TotalSlots = t.TotalSlots,
                        AvailableSlots = t.AvailableSlots,
                        PriceAdult = t.PriceAdult,
                        SlotStatusText = statusText,
                        StatusColor = statusColor
                    };
                }).ToList();

                txtTourCount.Text = $"Hiển thị: {displayList.Count} tour";

                dgTourSearch.ItemsSource = displayList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi giao diện: {ex.Message}", "Hệ thống cảnh báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadDesignTourData();
        }
    }

    public class TourDesignModel
    {
        public string TourCode { get; set; }
        public string TourName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
        public decimal PriceAdult { get; set; }
    }

    /// <summary>
    /// ĐÃ ĐỔI TÊN THÀNH CÔNG ĐỂ TRÁNH XUNG ĐỘT HỆ THỐNG
    /// </summary>
    public class TourSearchDesignRowModel
    {
        public string TourCode { get; set; }
        public string TourName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
        public decimal PriceAdult { get; set; }
        public string SlotStatusText { get; set; }
        public string StatusColor { get; set; }
    }
}