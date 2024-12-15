using iTextSharp.text;
using iTextSharp.text.pdf;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Globalization;
using WebBanVang.Models.DTO;

namespace WebBanVang.Models.Domain
{
    public class EmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly IConfiguration _configuration;

        public EmailService(IOptions<SmtpSettings> smtpSettings, IConfiguration configuration)
        {
            _smtpSettings = smtpSettings.Value;
            _configuration = configuration;
        }

        public void SendRevenueReport(List<OrdersDTO> orders, string toEmail, string reportOption)
        {
            var message = new MimeMessage();
            var builder = new BodyBuilder();

            int totalOrders = orders.Count;
            decimal totalRevenue = (decimal)orders.Sum(order => order.PaymentMoney);
            string logoUrl = _configuration["StoreInfo:Logo"];

            string currentDate = reportOption switch
            {
                "today" => DateTime.Now.ToString("dd/MM/yyyy", new CultureInfo("vi-VN")),
                "this-week" => $"Tuần {CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday)} năm {DateTime.Now.Year}",
                "this-month" => $"{DateTime.Now.ToString("MMMM", new CultureInfo("vi-VN"))} {DateTime.Now.Year}",
                "this-year" => DateTime.Now.ToString("yyyy", new CultureInfo("vi-VN")),
                _ => DateTime.Now.ToString("dd/MM/yyyy", new CultureInfo("vi-VN"))
            };

            string subject = reportOption switch
            {
                "today" => "Báo cáo doanh thu từ cửa hàng trang sức Kim Ngân Hoàng hôm nay",
                "this-week" => "Báo cáo doanh từ cửa hàng thu trang sức Kim Ngân Hoàng tuần này",
                "this-month" => "Báo cáo doanh từ cửa hàng thu trang sức Kim Ngân Hoàng tháng này",
                "this-year" => "Báo cáo doanh từ cửa hàng thu trang sức Kim Ngân Hoàng năm nay",
                _ => "Báo cáo doanh thu từ cửa hàng trang sức Kim Ngân Hoàng"
            };

            message.From.Add(new MailboxAddress(subject, _smtpSettings.FromEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            string reportDescription = reportOption switch
            {
                "today" => "hôm nay",
                "this-week" => "tuần này",
                "this-month" => "tháng này",
                "this-year" => "năm nay",
                _ => reportOption
            };

            builder.HtmlBody = $@"
        <html>
        <head>
            <style>
                .email-body {{
                    font-family: Arial, sans-serif;
                    color: #333;
                }}
                .header {{
                    text-align: left;
                }}
                .total {{
                    font-weight: bold;
                    margin-top: 10px;
                }}
                .logo {{
                    width: 700px;
                    height: auto;
                }}
            </style>
        </head>
        <body class='email-body'>
            <div class='header'>
                <img src='{logoUrl}' alt='Logo' class='logo' />
            </div>
            <p>Báo cáo doanh thu từ cửa hàng trang sức Kim Ngân Hoàng cho {reportDescription} ({currentDate}) là:</p>
            <b><p>Tổng đơn hàng: {totalOrders}</p></b>
            <p class='total'>Tổng tiền: {totalRevenue:N} VND</p>
            <p>Xem chi tiết vui lòng tải PDF phía dưới</p>
        </body>
        </html>";

            // Generate PDF
            var pdfBytes = GeneratePdf(orders, totalRevenue, totalOrders, logoUrl, reportDescription, currentDate);


            // Generate PDF
            builder.Attachments.Add("RevenueReport.pdf", pdfBytes, new ContentType("application", "pdf"));

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect(_smtpSettings.Host, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate(_smtpSettings.Username, _smtpSettings.Password);

                client.Send(message);
                client.Disconnect(true);
            }
        }



        private byte[] GeneratePdf(List<OrdersDTO> orders, decimal total, int totalOrders, string logoUrl, string reportDescription, string currentDate)
        {
            using (var ms = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 30, 30, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                try
                {
                    var logo = Image.GetInstance(new Uri(logoUrl));
                    logo.ScaleToFit(700, 225);
                    logo.Alignment = Element.ALIGN_CENTER;
                    document.Add(logo);
                }
                catch (Exception)
                {
                    document.Add(new Paragraph("Logo could not be loaded."));
                }

                // Load the Arial font
                string arialFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                var bfArial = BaseFont.CreateFont(arialFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                Font font = new Font(bfArial, 12);

                string reportTitle = $"Báo cáo doanh thu từ cửa hàng trang sức Kim Ngân Hoàng cho {reportDescription} ({currentDate})";
                document.Add(new Paragraph(reportTitle, font) { Leading = 14 });

                document.Add(new Paragraph(" ")); // Add a blank line

                PdfPTable table = new PdfPTable(new float[] { 1.5f, 3f, 2f, 2f }); // Adjust the widths to accommodate 5 columns
                table.WidthPercentage = 100;

                table.AddCell(new Phrase("Mã đơn hàng", font));
                table.AddCell(new Phrase("Thời Gian", font));
                table.AddCell(new Phrase("Khách hàng", font));
                table.AddCell(new Phrase("Tổng tiền (VND)", font));

                foreach (var order in orders)
                {
                    table.AddCell(new Phrase(order.OrderCode.ToString(), font));
                    table.AddCell(new Phrase(order.OrderDate.ToString("dd/MM/yyyy HH:mm:ss"), font));

                    if (order.Customers != null)
                    {
                        table.AddCell(new Phrase(order.Customers.CustomerName, font));
                    }
                    else if (!string.IsNullOrEmpty(order.CustomerName))
                    {
                        table.AddCell(new Phrase(order.CustomerName, font));
                    }
                    else if (!string.IsNullOrEmpty(order.Email))
                    {
                        table.AddCell(new Phrase(order.Email, font));
                    }
                    else if (!string.IsNullOrEmpty(order.PhoneNumber))
                    {
                        table.AddCell(new Phrase(order.PhoneNumber, font));
                    }
                    else
                    {
                        table.AddCell(new Phrase("Khách vãng lai", font));
                    }

                    table.AddCell(new Phrase(order.PaymentMoney?.ToString("N") ?? "N/A", font));
                }

                PdfPCell totalLabelCell = new PdfPCell(new Phrase($"Tổng đơn hàng: {totalOrders}", font))
                {
                    Colspan = 2, // Spanning across 3 columns
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                table.AddCell(totalLabelCell);

                PdfPCell totalAmountCell = new PdfPCell(new Phrase($"Tổng tiền: {total:N} VND", font))
                {
                    Colspan = 4, // Spanning across 1 column
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                table.AddCell(totalAmountCell);

                document.Add(table);
                document.Close();
                return ms.ToArray();
            }
        }


    }
}
