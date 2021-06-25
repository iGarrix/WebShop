using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(1,1000000, ErrorMessage = "Value is more then tyda syda")]
        public string Price { get; set; }
        [Required]
        public string Model { get; set; }


        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
