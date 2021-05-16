using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQWeb.WaterMark.Models.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [Column(TypeName="decimal(18,2")]
        public decimal UnitPrice { get; set; }

        [Range(1,100)]
        public int Stock { get; set; }

        [StringLength(300)]
        public string ImageUrl { get; set; }
    }
}
