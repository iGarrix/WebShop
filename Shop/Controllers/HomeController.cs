using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop.Data;
using Shop.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _db = db;
            this._webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string uploads = webRootPath + ENV.imagepass;

            IEnumerable<Product> products = _db.Products.Include(u => u.Category);
            foreach (var item in products)
            {
                if (item.Image != null)
                {
                    string name = item.Image;
                    string final = Path.Combine(uploads, name);
                    item.Image = final;
                }
            }
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
