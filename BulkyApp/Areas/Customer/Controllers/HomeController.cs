using Empire.DataAccess.Repository.IRepository;
using Empire.Models;
using Empire.Models.API;
using Empire.Utilities;
using EmpireApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NuGet.Common;
using System.Diagnostics;
using System.Security.Claims;
using static EmpireApp.Services.PayMobService;

namespace EmpireApp.Areas.Customer.Controllers
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

        public async Task<IActionResult> Index()
		{

            IEnumerable<Product> products = await _unitOfWork.Product.GetAll(includeProperties: "ProductSizes");

            return View(products.ToList());
        }
        public async Task<IActionResult> Details(int productId)
		{
            ShoppingCart shoppingCart = new()
            {
                Product = await _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category"),
                count = 1,
                ProductId = productId
            }; 
			return View(shoppingCart);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
		{
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = UserId;

            ShoppingCart cardFromDb = await _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == UserId &&
            u.ProductId == shoppingCart.ProductId);

            if (cardFromDb != null)
            {
                cardFromDb.count += shoppingCart.count;
                _unitOfWork.ShoppingCart.update(cardFromDb);
                await _unitOfWork.save();
            }
            else
            {
                await _unitOfWork.ShoppingCart.Add(shoppingCart);
                await _unitOfWork.save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == UserId).GetAwaiter().GetResult().Count());
            }

            TempData["success"] = "Added to Cart";
			return RedirectToAction(nameof(Index));
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