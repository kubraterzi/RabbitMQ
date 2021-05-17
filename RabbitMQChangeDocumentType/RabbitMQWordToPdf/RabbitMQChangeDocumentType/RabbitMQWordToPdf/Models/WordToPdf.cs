using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RabbitMQWordToPdf.Models
{
    public class WordToPdf
    {
        public string Email { get; set; }
        public IFormFile File { get; set; }
    }
}
