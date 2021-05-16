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
            using(var scope = host.Services.CreateScope()) // bu scope �zerinden startup taraf�nda tan�mlanan AppDbContext servisine eri�ip i�lem bittikten sonra eldeki servisler memory den d���p yer kaplamas�n.
            //using bloklar� i�erisinde �a��r�lan t�m servisler, bloktan ��k�ld��� anda bellekten d��ecek. ge�ici olarak buraya �ektik, kullanaca��z ve silece�iz.
            {
                var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>(); // GetRequiredService-> mutlaka bu servisin ba�lant�l� oldu�undan emin olundu�unda kullan�l�r, yoksa hata f�rlat�r.
                // GetService-> e�er �a��r�lan servis ba�lant�l� de�ilse hata f�rlatmaz, null d�nd�r�r. 

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>(); // UserManager ve IdentityUser ->AspNetCore.Identity k�t�phanesinden geliyorlar.
                
                appDbContext.Database.Migrate(); // tablodaki her g�ncellemede konsoldan update migration demek gerekirdi, uygulama her aya�a kalkt���nda otomatik update etsin diye bu komutu kulland�k 

                

                if (!appDbContext.Users.Any())
                {
                    // bir SignUp penceresi ve uygulamas� yazmak istemedi�imizden, buradan verileri ekliyoruz.
                    userManager.CreateAsync(new IdentityUser() { UserName = "Deneme", Email = "deneme@outlook.com" }, "DenemePassword.12*").Wait(); // IdentityUser i�erisine yazmama sebebimiz, Identity nin password � veritaban�nda hash leyerek saklamas�d�r 

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
