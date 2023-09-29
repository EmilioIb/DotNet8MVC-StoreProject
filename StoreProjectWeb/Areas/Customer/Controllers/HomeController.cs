using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreProject.DataAccess.Repository;
using StoreProject.DataAccess.Repository.IRepository;
using StoreProject.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace StoreProjectWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return View(productList);
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.ProductId == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = productId
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            if (shoppingCart.Count >= 1 && shoppingCart.Count <= 150)
            {
                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u =>
                u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);

                if (cartFromDb != null)
                {
                    cartFromDb.Count += shoppingCart.Count;
                    _unitOfWork.ShoppingCart.Update(cartFromDb);
                }
                else
                {
                    _unitOfWork.ShoppingCart.Add(shoppingCart);
                }

                TempData["success"] = "Cart updated successfully";
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }

            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.ProductId == shoppingCart.ProductId, includeProperties: "Category"),
                Count = 1,
                ProductId = shoppingCart.ProductId
            };

            return View(cart);



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