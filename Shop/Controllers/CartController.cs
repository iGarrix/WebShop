using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Models.ViewModels;
using Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CartController(ApplicationDbContext db)
        {
            _db = db;
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
    }
}
