using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Models.ViewModels
{
    public class ProductUserVM
    {
        public ProductUserVM()
        {
            AppUser = new AppUser();
            ProductList = new List<Product>();

        }

        public AppUser AppUser { get; set; }
        public IList<Product> ProductList { get; set; }
    }
}
