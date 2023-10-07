using Empire.DataAccess.Repository.IRepository;
using Empire.Models;
using Empire.Models.API;
using Empire.Models.ViewModels;
using Empire.Utilities;
using EmpireApp.Services;
using EmpireApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmpireApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IPayMobService _payMobService;
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM? shoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IPayMobService payMobService)
        {
            _unitOfWork = unitOfWork;
            _payMobService = payMobService;
        }
        public async Task<IActionResult> Index()
        {
            ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
            if(claimsIdentity == null )
                return NotFound();
            string? userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return NotFound();
            ShoppingCartVM shoppingCartVM = new()
            {
                ShoppingCartList = await _unitOfWork.ShoppingCart.GetAllEntities(u => u.ApplicationUserId == userId,
                includeProperties: "Product,productSize"),
                orderHeader = new()
            };

            IEnumerable<ProductImage> productImages = await _unitOfWork.ProductImage.GetAllEntities();

            foreach(var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Product.ProductImages = productImages.Where(u=>u.ProductId == cart.Product.Id).ToList();
                cart.price = cart.count * (cart.Product.ListPrice - cart.Product.ListPrice * cart.Product.Discount / 100);
                shoppingCartVM.orderHeader.OrderTotal += cart.price;
            }
            return View(shoppingCartVM);
        }

        [Authorize]
        public async Task<IActionResult> Summary()
        {
            ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
            if (claimsIdentity == null)
                return NotFound();
            string? userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return NotFound();

            ShoppingCartVM shoppingCartVM = new()
            {
                ShoppingCartList = await _unitOfWork.ShoppingCart.GetAllEntities(u => u.ApplicationUserId == userId,
                includeProperties: "Product,productSize"),
                orderHeader = new()
            };

            shoppingCartVM.orderHeader.ApplicationUser = await _unitOfWork.ApplicationUser.GetEntity(u=>u.Id == userId);

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.price = cart.Product.ListPrice;
                shoppingCartVM.orderHeader.OrderTotal += cart.count * (cart.Product.ListPrice - cart.Product.ListPrice * cart.Product.Discount / 100);
            }
            return View(shoppingCartVM);
        }


        [HttpPost]
        [ActionName(nameof(Summary))]
        public async Task<IActionResult> SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            var userId = claimsIdentity?
                         .FindFirst(ClaimTypes.NameIdentifier)?
                         .Value;

            if (userId == null)
                return BadRequest();


            if (shoppingCartVM == null)
            {
                return BadRequest();
            }

            shoppingCartVM.orderHeader.Id = await _unitOfWork.OrderHeader.CheckAvailability(userId);



            shoppingCartVM.ShoppingCartList = await _unitOfWork
                                                    .ShoppingCart
                                                    .GetAllEntities
                                                    (u => u.ApplicationUserId == userId
                                                    ,includeProperties: "Product");

            shoppingCartVM.orderHeader.OrderDate = DateTime.Now;
            shoppingCartVM.orderHeader.ApplicationUserId = userId;

            ApplicationUser applicationUser = await _unitOfWork
                                                    .ApplicationUser
                                                    .GetEntity(u=>u.Id == userId);

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.price = cart.Product.ListPrice;
                shoppingCartVM.orderHeader.OrderTotal += cart.count * (cart.Product.ListPrice - cart.Product.ListPrice * cart.Product.Discount / 100);
            }
            shoppingCartVM.orderHeader.OrderStatus = SD.StatusPending;
            shoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusPending;
            if (shoppingCartVM.orderHeader.Id != 0)
            {
                _unitOfWork.OrderHeader.Update(shoppingCartVM.orderHeader);
            }
            else
            {
                await _unitOfWork.OrderHeader.Add(shoppingCartVM.orderHeader);
                await _unitOfWork.Save();
            }

            List<Item> items = new List<Item>();
            foreach(var cart in shoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = shoppingCartVM.orderHeader.Id,
                    Price = cart.price,
                    Count = cart.count,
                    productSizeId = cart.productSizeId
                };
                items.Add(new Item {amount_cents = cart.price*100
                                    ,description = cart.Product.Description
                                    ,name = cart.Product.Description
                                    ,quantity = cart.count });
                await _unitOfWork.OrderDetail.Add(orderDetail);
            }
            await _unitOfWork.Save();

            bool success = fillDictionary(applicationUser,items);
            if (!success)
                return BadRequest();
            string token = await _payMobService.PayMobSetup(SD.FirstPayload, SD.SecondPayload);
            Response.Headers.Add("Location", $"https://accept.paymob.com/api/acceptance/iframes/769121?payment_token={token}");

            return new StatusCodeResult(303);
        }

        private bool fillDictionary(ApplicationUser applicationUser,List<Item> items)
        {
            if (shoppingCartVM == null || applicationUser.Email == null)
            {
                return false;
            }

            SD.FirstPayload["amount_cents"] = shoppingCartVM.orderHeader.OrderTotal * 100;
            SD.FirstPayload["items"] = items;
            SD.SecondPayload["amount_cents"] = shoppingCartVM.orderHeader.OrderTotal * 100;
            Dictionary<string, object> nestedDictionary = (Dictionary<string, object>)SD.SecondPayload["billing_data"];

            nestedDictionary["email"] = applicationUser.Email;
            nestedDictionary["first_name"] = shoppingCartVM.orderHeader.Name;
            nestedDictionary["street"] = shoppingCartVM.orderHeader.StreetAddress;
            nestedDictionary["phone_number"] = shoppingCartVM.orderHeader.PhoneNumber;
            nestedDictionary["city"] = shoppingCartVM.orderHeader.City;
            return true;
        }

        [Authorize]
        public async Task<IActionResult> OrderChecking()
        {
            ClaimsIdentity? claimsIdentity = (ClaimsIdentity?)User.Identity;
            if (claimsIdentity == null)
                return NotFound();
            string? userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return NotFound();


            var query = Request.Query;
            bool success_id = int.TryParse(query["id"], out int id);
            bool success_order_Id = int.TryParse(query["order"],out int order_Id);
            bool success = Convert.ToBoolean(query["success"]);
            var OrderHeader = await _unitOfWork.OrderHeader.GetEntity(u => u.ApplicationUserId == userId && u.PaymentStatus == SD.PaymentStatusPending);
            IEnumerable<ShoppingCart> shoppingCarts = await _unitOfWork.ShoppingCart.GetAllEntities(u => u.ApplicationUserId == userId);
            if (success && success_id && success_order_Id)
            {
                _unitOfWork.ShoppingCart.DeleteAll(shoppingCarts);
                await _unitOfWork.OrderHeader.UpdatePayMobPaymentID(OrderHeader.Id, order_Id, id);
                await _unitOfWork.OrderHeader.UpdateStatus(OrderHeader.Id, SD.StatusApproved, SD.PaymentStatusApproved);
                await _unitOfWork.Save();
                return RedirectToAction(nameof(OrderConfirmation), new { order_id = order_Id });
            }
            return NotFound();
        }
        public IActionResult OrderConfirmation(int order_id)
        {
            HttpContext.Session.Clear();
            return View(order_id);
        }
        public async Task<IActionResult> plus(int cartId)
        {
            ShoppingCart shoppingCart = await _unitOfWork.ShoppingCart.GetEntity(u=>u.Id == cartId);
            shoppingCart.count += 1;

            _unitOfWork.ShoppingCart.Update(shoppingCart);
            await _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> minus(int cartId)
        {
            ShoppingCart shoppingCart = await _unitOfWork.ShoppingCart.GetEntity(u => u.Id == cartId);
            if (shoppingCart.count <= 1)
            {
                _unitOfWork.ShoppingCart.Delete(shoppingCart); 
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAllEntities(u => u.ApplicationUserId == shoppingCart.ApplicationUserId)
                    .GetAwaiter().GetResult().Count() - 1);
            }
            else
            {
                shoppingCart.count -= 1;
                _unitOfWork.ShoppingCart.Update(shoppingCart);
            }
            await _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> remove(int cartId)
        {
            ShoppingCart shoppingCart = await _unitOfWork.ShoppingCart.GetEntity(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.Delete(shoppingCart);
            HttpContext.Session.SetInt32(SD.SessionCart,
                   _unitOfWork.ShoppingCart.GetAllEntities(u => u.ApplicationUserId == shoppingCart.ApplicationUserId)
                   .GetAwaiter().GetResult().Count() -1);
            await _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

    }
}
