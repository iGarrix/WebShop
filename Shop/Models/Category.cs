using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models
{
    public class Category
    {
        public Category()
        {
            Products = new HashSet<Product>(); 
        }
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [DisplayName("Show Order")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Show order must be more then 0")]
        public int ShowOrder { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
