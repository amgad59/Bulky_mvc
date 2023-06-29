using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM shoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM shoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                orderHeader = new()
            };
            foreach(var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.price = cart.count * (cart.Product.ListPrice - cart.Product.ListPrice * cart.Product.Discount / 100);
                shoppingCartVM.orderHeader.OrderTotal += cart.price;
            }
            return View(shoppingCartVM);
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM shoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                orderHeader = new()
            };

            shoppingCartVM.orderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u=>u.Id == userId);

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.price = cart.Product.ListPrice;
                shoppingCartVM.orderHeader.OrderTotal += cart.count * (cart.Product.ListPrice - cart.Product.ListPrice * cart.Product.Discount / 100);
            }
            return View(shoppingCartVM);
        }


        [HttpPost]
        [ActionName(nameof(Summary))]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId
            ,includeProperties: "Product");

            shoppingCartVM.orderHeader.OrderDate = DateTime.Now;
            shoppingCartVM.orderHeader.ApplicationUserId = userId;

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u=>u.Id == userId);

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.price = cart.Product.ListPrice;
                shoppingCartVM.orderHeader.OrderTotal += cart.count * (cart.Product.ListPrice - cart.Product.ListPrice * cart.Product.Discount / 100);
            }
            shoppingCartVM.orderHeader.OrderStatus = SD.StatusPending;
            shoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusPending;

            _unitOfWork.OrderHeader.Add(shoppingCartVM.orderHeader);
            _unitOfWork.save();

            foreach(var cart in shoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = shoppingCartVM.orderHeader.Id,
                    Price = cart.price,
                    Count = cart.count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.save();
            }

            return View(shoppingCartVM);
        }


        public IActionResult plus(int cartId)
        {
            ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(u=>u.Id == cartId);
            shoppingCart.count += 1;
            _unitOfWork.ShoppingCart.update(shoppingCart);
            _unitOfWork.save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult minus(int cartId)
        {
            ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(u=>u.Id == cartId);
            if (shoppingCart.count <= 1)
            {
                _unitOfWork.ShoppingCart.Delete(shoppingCart);
            }
            else
            {
                shoppingCart.count -= 1;
                _unitOfWork.ShoppingCart.update(shoppingCart);
            }
            _unitOfWork.save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult remove(int cartId)
        {
            ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(u=>u.Id == cartId);
            _unitOfWork.ShoppingCart.Delete(shoppingCart);
            _unitOfWork.save();
            return RedirectToAction(nameof(Index));
        }

    }
}
