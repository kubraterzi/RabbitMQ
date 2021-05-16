using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQWeb.ExcelCreate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQWeb.ExcelCreate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host=CreateHostBuilder(args).Build();    
            using(var scope = host.Services.CreateScope()) // bu scope üzerinden startup tarafýnda tanýmlanan AppDbContext servisine eriþip iþlem bittikten sonra eldeki servisler memory den düþüp yer kaplamasýn.
            //using bloklarý içerisinde çaðýrýlan tüm servisler, bloktan çýkýldýðý anda bellekten düþecek. geçici olarak buraya çektik, kullanacaðýz ve sileceðiz.
            {
                var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>(); // GetRequiredService-> mutlaka bu servisin baðlantýlý olduðundan emin olunduðunda kullanýlýr, yoksa hata fýrlatýr.
                // GetService-> eðer çaðýrýlan servis baðlantýlý deðilse hata fýrlatmaz, null döndürür. 

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>(); // UserManager ve IdentityUser ->AspNetCore.Identity kütüphanesinden geliyorlar.
                
                appDbContext.Database.Migrate(); // tablodaki her güncellemede konsoldan update migration demek gerekirdi, uygulama her ayaða kalktýðýnda otomatik update etsin diye bu komutu kullandýk 

                

                if (!appDbContext.Users.Any())
                {
                    // bir SignUp penceresi ve uygulamasý yazmak istemediðimizden, buradan verileri ekliyoruz.
                    userManager.CreateAsync(new IdentityUser() { UserName = "Deneme", Email = "deneme@outlook.com" }, "DenemePassword.12*").Wait(); // IdentityUser içerisine yazmama sebebimiz, Identity nin password ü veritabanýnda hash leyerek saklamasýdýr 

                    userManager.CreateAsync(new IdentityUser() { UserName = "Deneme2", Email = "deneme2@outlook.com" }, "DenemePassword.12*").Wait(); // Wait()-> Asenkron metodu senkron hale getirir.
                }
            }


            host.Run();


        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
