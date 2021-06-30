using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductsController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            this._db = db;
            this._webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _db.Products.Include(u => u.Category);
            return View(products);
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productvm = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _db.Category.Select(u => new SelectListItem() { Text = u.Name, Value = u.Id.ToString() }),
            };
            if (id == null)
            {
                return View(productvm);
            }
            else
            {
                productvm.Product = _db.Products.Find(id);
                if (productvm.Product == null)
                {
                    return NotFound();
                }
                return View(productvm);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                string uploads = webRootPath + ENV.imagepass;
                string FileName = Guid.NewGuid().ToString();
                string FileExt = Path.GetExtension(files[0].FileName);

                if (productVM.Product.Id == 0)
                {                 
                    using (var fs = new FileStream(Path.Combine(uploads, FileName + FileExt), FileMode.Create))
                    {
                        files[0].CopyTo(fs);
                    }

                    productVM.Product.Image = FileName + FileExt;
                    _db.Products.Add(productVM.Product);
                }
                else
                {
                    var formObj = _db.Products.AsNoTracking().FirstOrDefault(u => u.Id == productVM.Product.Id);
                    if (files.Count > 0)
                    {
                        var oldFile = Path.Combine(uploads, formObj.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }
                        using (var fs = new FileStream(Path.Combine(uploads, FileName + FileExt), FileMode.Create))
                        {
                            files[0].CopyTo(fs);
                        }
                        productVM.Product.Image = FileName + FileExt;
                    }
                    else
                    {
                        productVM.Product.Image = formObj.Image;

                    }
                    _db.Products.Update(productVM.Product);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            productVM.CategorySelectList = _db.Category.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString()
            });
            return View(productVM);
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
            string webRootPath = _webHostEnvironment.WebRootPath;
            string uploads = webRootPath + ENV.imagepass;
            string path = Path.Combine(uploads, products.Image);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            _db.Products.Remove(products);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
