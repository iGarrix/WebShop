using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ProductsController(ApplicationDbContext db)
        {
            this._db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _db.Products.Include(u => u.Category);
            return View(products);
        }
        bool iscreate = false;

        public IActionResult Upsert(int? id)
        {
            ProductVM productvm = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _db.Category.Select(u => new SelectListItem() { Text = u.Name, Value = u.Id.ToString() }),
            };
            if (id == null)
            {
                iscreate = true;
                return View(productvm);
            }
            else
            {
                productvm.Product = _db.Products.Find(id);
                if (productvm.Product == null)
                {
                    return NotFound();
                }
                iscreate = false;
                return View(productvm);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Product product)
        {
            if (ModelState.IsValid)
            {
                if (iscreate)
                {
                    _db.Products.Add(product);
                    _db.SaveChanges();
                }
                else
                {
                    _db.Products.Update(product);
                    _db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var products = _db.Products.Find(id);
            if (products == null)
            {
                return NotFound();
            }
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var products = _db.Products.Find(id);
            if (products == null)
            {
                return NotFound();
            }

            _db.Products.Remove(products);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
