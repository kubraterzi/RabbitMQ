using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQWeb.ExcelCreate.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQWeb.ExcelCreate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {

        private AppDbContext _context;

        public FilesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, int fileId)
        {
            if (file is not { Length: > 0 })
            {
                return BadRequest();
            }

            var userFile = await _context.UserFiles.FirstAsync(x => x.Id == fileId); // fileId ye göre kullanıcıyı getir

            var filePath = userFile.FileName + Path.GetExtension(file.FileName); // filePath içerisine file name i ve uzantısını çek

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files" , filePath); // dosya yolunu tamamen al

            using FileStream stream = new FileStream(path, FileMode.Create); // verilen path e göre bir file oluştur

            await file.CopyToAsync(stream); // file ı stream ile oluşturduğun yere kopyala

            userFile.CreatedDate = DateTime.Now;
            userFile.FilePath = filePath;
            userFile.FileStatus = FileStatus.Created;

            await _context.SaveChangesAsync();

            //SignalR ile notification oluşturulacak

            return Ok();
        }

    }
}
