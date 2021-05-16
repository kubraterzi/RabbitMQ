using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQWeb.ExcelCreate.Models
{
    public enum FileStatus
    {
        Creating,
        Created
    }
    public class UserFile
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime? CreatedDate { get; set; } // Excel dosyasına dönüştürülmek üzere alındığında henüz oluşturulmamış olacağı için nullable tanımladık.
        public FileStatus FileStatus { get; set; }

        [NotMapped] // Entity de bir property tanımlamak istenirse ve veritabanındaki tablosunda yer almasını istemezsek bu attribute kullanılır
        public string GetCreatedDate => CreatedDate.HasValue ? CreatedDate.Value.ToShortDateString() : "-";


    }
}
