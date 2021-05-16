using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RabbitMQWeb.ExcelCreate.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQWeb.ExcelCreate.Models
{
    public class AccountController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager; // burada bir bayinin de üyeliği olabilirdi o zaman IdentityUser yerine bayiliğin user ını verebilirdik. Şuan bizde bir tane user türü var. Identity kütüphanesinden aldık.

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

       
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string password)
        {
            var hasUser = await _userManager.FindByEmailAsync(Email); // email e göre kullanıcı çeker

            if (hasUser == null)
            {
                return View();
            }
             
            //kulanıcı varsa aşağıdaki satıra geçer. Alt satırda password hash leyen bir metot vardır, bilgiler ona gönderilir ve o hash leme yapar.
            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, password, true, false); // true-> 60 gün cookie saklansın. false -> şifreyi 3 kez yanlış girdiğinde hesap kilitlenmesin

            //kullanıcı adı olmasına rağmen başarısız olduysa, passwrod hatalı demektir.
            if (!signInResult.Succeeded)
            {
                return View();
            }

            return RedirectToAction(nameof(HomeController.Index), "Home"); // doğrudan Home da yazılabilirdi, olur da index sayfasının ismi değiştirilirse hata versin diye yazdık
        }
    }
}
