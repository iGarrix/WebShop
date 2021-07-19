using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Models.ViewModels;
using Shop.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CartController(ApplicationDbContext db, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENV.SessionCard) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENV.SessionCard).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(ENV.SessionCard);
            }
            List<Product> products = new List<Product>();
            foreach (var item in shoppingCartList)
            {
                Product product = _db.Products.Include(u => u.Category).FirstOrDefault(i => i.Id == item.ProductId);
                products.Add(product);
            }
            return View(products);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ActionName("Index")]
        //public IActionResult IndexPost()
        //{
        //    return RedirectToAction(nameof(Order));
        //}

        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENV.SessionCard) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENV.SessionCard).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(ENV.SessionCard);
            }

            var removeItem = shoppingCartList.SingleOrDefault(u => u.ProductId == id);
            if (removeItem != null)
            {
                shoppingCartList.Remove(removeItem);
            }

            HttpContext.Session.Set(ENV.SessionCard, shoppingCartList);
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENV.SessionCard) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENV.SessionCard).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(ENV.SessionCard);
            }

            DetailsVM vm = new DetailsVM()
            {
                Product = _db.Products.Include(u => u.Category).FirstOrDefault(i => i.Id == id),
                InCard = false,
            };

            foreach (var item in shoppingCartList)
            {
                if (item.ProductId == id)
                {
                    vm.InCard = true;
                }
            }

            return View(vm);
        }

        public IActionResult Order()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claimSession = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENV.SessionCard) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENV.SessionCard).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(ENV.SessionCard);
            }
            List<Product> products = new List<Product>();
            foreach (var item in shoppingCartList)
            {
                Product product = _db.Products.Include(u => u.Category).FirstOrDefault(i => i.Id == item.ProductId);
                products.Add(product);
            }

            ProductUserVM productUserVM = new ProductUserVM()
            {
                ProductList = products,
                AppUser = _db.AppUser.FirstOrDefault(i => i.Id == claimSession.Value),
            };
            return View(productUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Order")]
        public async Task<IActionResult> OrderPostAsync(ProductUserVM productUserVM)
        {
            // send email

            var path_template = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "teplates" + Path.DirectorySeparatorChar.ToString() + "orderproduct_template.html";

            var subject = "New order";
            string HTML_body = "";

            using (StreamReader sr = System.IO.File.OpenText(path_template))
            {
                HTML_body = sr.ReadToEnd();
            }

            StringBuilder productsSB = new StringBuilder();

            foreach (var item in productUserVM.ProductList)
            {
                productsSB.Append($"{item.Name}\t | \t{item.Price}$UAN");
            }

            string messageBody = string.Format(
                HTML_body,
                productUserVM.AppUser.Name, productUserVM.AppUser.Email, productUserVM.AppUser.PhoneNumber,
                productsSB.ToString());


            await _emailSender.SendEmailAsync(ENV.AdminEmail, subject, messageBody);

            return RedirectToAction(nameof(OrderConfirmarion));

        }

        public IActionResult OrderConfirmarion()
        {
            HttpContext.Session.Clear();
            return View();
        }
    }
}
