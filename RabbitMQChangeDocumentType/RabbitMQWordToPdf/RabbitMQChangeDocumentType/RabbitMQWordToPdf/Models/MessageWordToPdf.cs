using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQWordToPdf.Models
{
    public class MessageWordToPdf
    {
        public Byte[] WordByte { get; set; }
        public string FileName { get; set; }
        public string Email { get; set; }
    }
}
