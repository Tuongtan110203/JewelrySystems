using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;
using WebBanVang.Repository;

namespace WebBanVang.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //  [Authorize(Roles = "Manager")]
    [EnableCors("AllowLocalhost3000And15723035249")]
    //[AllowAnonymous]
    public class RevenueReportController : ControllerBase
    {
        private readonly RevenueService _revenueService;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;

        public RevenueReportController(RevenueService revenueService, EmailService emailService, IConfiguration configuration)
        {
            _revenueService = revenueService;
            _emailService = emailService;
            _configuration = configuration;
        }

        [HttpGet("send-report/{option}")]
        public IActionResult SendReport(string option)
        {
            var ownerEmail = _configuration["StoreInfo:Email"];
            List<OrdersDTO> orders = null;

            switch (option)
            {
                case "today":
                    orders = _revenueService.GetOrdersOfToday();
                    break;
                case "this-week":
                    orders = _revenueService.GetThisWeekOrdersAsync();
                    break;
                case "this-month":
                    orders = _revenueService.GetThisMonthOrdersAsync();
                    break;
                case "this-year":
                    orders = _revenueService.GetThisYearOrdersAsync();
                    break;
                default:
                    return BadRequest("Invalid option. Please use 'today', 'this-week', 'this-month', or 'this- year'.");
            }
            _emailService.SendRevenueReport(orders, ownerEmail, option);
            return Ok("Orders report sent successfully.");
        }

    }
}
