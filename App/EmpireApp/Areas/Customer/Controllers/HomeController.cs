using Empire.DataAccess.Repository.IRepository;
using Empire.Models;
using Empire.Models.API;
using Empire.Utilities;
using EmpireApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

            IEnumerable<Product> products = await _unitOfWork.Product.GetAllEntities(includeProperties: "ProductSizes,ProductImages");

            return View(products.ToList());
        }
        public async Task<IActionResult> Details(int productId)
		{
            ShoppingCart shoppingCart = new()
            {
                Product = await _unitOfWork.Product.GetEntity(u => u.Id == productId, includeProperties: "Category,ProductSizes,ProductImages"),
                count = 1,
                ProductId = productId
            };
            ViewBag.sizes = shoppingCart.Product.ProductSizes.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
			return View(shoppingCart);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
		{
            ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
            if (claimsIdentity == null)
                return NotFound();
            string? userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return NotFound();

            shoppingCart.ApplicationUserId = userId;

            ShoppingCart cardFromDb = await _unitOfWork.ShoppingCart.GetEntity(u => u.ApplicationUserId == userId &&
            u.ProductId == shoppingCart.ProductId);

			if (cardFromDb != null && cardFromDb.productSizeId == shoppingCart.productSizeId)
            {
                cardFromDb.count += shoppingCart.count;
                _unitOfWork.ShoppingCart.Update(cardFromDb);
                await _unitOfWork.Save();
            }
            else
            {
                await _unitOfWork.ShoppingCart.Add(shoppingCart);
                await _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAllEntities(u => u.ApplicationUserId == userId).GetAwaiter().GetResult().Count());
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