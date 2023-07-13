using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.ExcelCreate.Hubs;
using RabbitMQ.ExcelCreate.Models;
using RabbitMQ.ExcelCreate.Services;

namespace RabbitMQ.ExcelCreate.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RabbitMQPublisher _rabbitMQPublisher;
        private readonly IHubContext<MyHub> _hubContext;
        public ProductController(AppDbContext context, UserManager<IdentityUser> userManager, RabbitMQPublisher rabbitMqPublisher, IHubContext<MyHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _rabbitMQPublisher = rabbitMqPublisher;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateProductExcel()
        {
            var user =await _userManager.FindByNameAsync(User.Identity.Name);
            var fileName = $"product-excel-{Guid.NewGuid().ToString().Substring(1, 10)}";
            UserFile file = new()
            {
                UserId = user.Id,
                FileName = fileName,
                FileStatus = FileStatus.Creating
            };
            await _context.UserFiles.AddAsync(file);
            await _context.SaveChangesAsync();
            _rabbitMQPublisher.Publish(new CreateExcelMessage()
            {
                FileId = file.Id 
            });
            TempData["StartCreatingExcel"] = true;


            return RedirectToAction(nameof(Files));
        }

        public async Task<IActionResult> Files()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = await _context.UserFiles.Where(x => x.UserId == user.Id).OrderByDescending(x=>x.Id).ToListAsync();
            return View(data);
        }
    }
}
