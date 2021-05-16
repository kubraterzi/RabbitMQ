using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQWeb.ExcelCreate.Models;
using RabbitMQWeb.ExcelCreate.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQWeb.ExcelCreate.Controllers
{

    [Authorize]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly RabbitMQPublisher _rabbitMQPublisher;

        public ProductController(AppDbContext context, UserManager<IdentityUser> userManager, RabbitMQPublisher rabbitMQPublisher)
        {
            _context = context;
            _userManager = userManager;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateProductExcel() // biz doğrudan product tablosunu excel dosyasına döndüreceğimiz için parametre almadık, birden fazla versiyona döndüreceksek parametre olrak verebilirdik
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name); // cookie de mevcut olan User üzerinden ismine ulaşıyoruz. metot asenkron olduğu için metodun imzasnı düzenliyoruz

            var fileName = $"product-excel-{Guid.NewGuid().ToString().Substring(1, 10)}";

            UserFile userFile = new()
            {
                UserId=user.Id,
                FileName=fileName,
                FileStatus= FileStatus.Creating
            };

            await _context.UserFiles.AddAsync(userFile);
            await _context.SaveChangesAsync();

            _rabbitMQPublisher.Publish(new Shared.CreateExcelMessage
            {
                FileId = userFile.Id
            });


            TempData["StartCreatingExcel"] = true; // bir request ten bir requeste data taşımak için kullanılır.

            return RedirectToAction(nameof(Files)); // aynı controller daki farklı bir request yönlendirebiliriz

        }


        public async Task<IActionResult> Files()
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            return View(await _context.UserFiles.Where(x => x.UserId == user.Id).OrderByDescending(x => x.Id).ToListAsync());
        }

    }
}
