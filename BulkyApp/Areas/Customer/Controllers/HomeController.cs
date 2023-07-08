using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.API;
using BulkyApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyApp.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class HomeController : Controller
    {
        IPayMobService _payMobService;
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IPayMobService payMobService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _payMobService = payMobService;
        }

        public async Task<IActionResult> Index()
		{
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "ProductSizes").ToList();
            var x = await _payMobService.FirstStep<APIResponse>();
            var y = await _payMobService.SecondStep<APIResponse>(x.token);
            var z = await _payMobService.ThirdStep<APIResponse>(x.token,y.id);
			return View(products);
        }
        public IActionResult Details(int productId)
		{
            ShoppingCart shoppingCart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category"),
                count = 1,
                ProductId = productId
            }; 
			return View(shoppingCart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
		{
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = UserId;

            ShoppingCart cardFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == UserId &&
            u.ProductId == shoppingCart.ProductId);

            if (cardFromDb != null)
            {
                cardFromDb.count += shoppingCart.count;
                _unitOfWork.ShoppingCart.update(cardFromDb);    
            }
            else
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }

            _unitOfWork.save();
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