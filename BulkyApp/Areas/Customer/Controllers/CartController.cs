﻿using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.API;
using Bulky.Models.ViewModels;
using Bulky.Utilities;
using BulkyApp.Services;
using BulkyApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IPayMobService _payMobService;
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM shoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IPayMobService payMobService)
        {
            _unitOfWork = unitOfWork;
            _payMobService = payMobService;
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
        public async Task<IActionResult> SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.ApplicationUserId == userId && u.PaymentStatus == SD.PaymentStatusPending);

            if (orderHeader != null)
            {
                shoppingCartVM.orderHeader.Id = orderHeader.Id;
                IEnumerable<OrderDetail> orderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderHeader.Id);
                _unitOfWork.OrderDetail.DeleteAll(orderDetails);
                _unitOfWork.save();
            }

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
            if (orderHeader != null )
            {
                _unitOfWork.OrderHeader.update(shoppingCartVM.orderHeader);
            }
            else
                _unitOfWork.OrderHeader.Add(shoppingCartVM.orderHeader);
            _unitOfWork.save();

            List<Item> items = new List<Item>();
            foreach(var cart in shoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = shoppingCartVM.orderHeader.Id,
                    Price = cart.price,
                    Count = cart.count
                };
                items.Add(new Item {amount_cents = cart.price*100,description = cart.Product.Description,name=cart.Product.Description,quantity=cart.count });
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.save();
            }


            Dictionary<string, object> FirstPayload = new Dictionary<string, object> {
                    {
                    "auth_token", ""
                    },
                    {"delivery_needed", "false"},
                    { "amount_cents", shoppingCartVM.orderHeader.OrderTotal*100 },
                    { "currency", "EGP" },
                    { "items", items}
            };
            Dictionary<string, object> SecondPayload = new Dictionary<string, object> {

                    { "auth_token","" },
                    { "amount_cents", shoppingCartVM.orderHeader.OrderTotal*100 },
                    { "expiration", 3600 },
                    { "order_id", "" },
                    { "billing_data",new Dictionary<string, object>{
                        {"apartment", "NA" },
                        {"email", applicationUser.Email },
                        {"floor", "NA"},
                        {"first_name", shoppingCartVM.orderHeader.Name},
                        {"street", shoppingCartVM.orderHeader.StreetAddress},
                        {"building", "NA"},
                        {"phone_number", shoppingCartVM.orderHeader.PhoneNumber},
                        {"shipping_method", "NA"},
                        {"postal_code", "NA"},
                        {"city", shoppingCartVM.orderHeader.City},
                        {"country", "NA"},
                        {"last_name", "Nicolas"},
                        { "state", "NA"} }
                    },
                    { "currency", "EGP"},
                    { "integration_id", 3951279},
                    { "lock_order_when_paid", "true"}
                };
            string t = await _payMobService.PayMobSetup(FirstPayload, SecondPayload);
            Response.Headers.Add("Location", $"https://accept.paymob.com/api/acceptance/iframes/769121?payment_token={t}");

            return new StatusCodeResult(303);
            //return View(shoppingCartVM);
        }



        public IActionResult OrderChecking()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var query = Request.Query;
            var id = int.Parse(query["id"]);
            int order_Id = int.Parse(query["order"]);
            bool success = Convert.ToBoolean(query["success"]);
            var OrderHeader = _unitOfWork.OrderHeader.Get(u=>u.ApplicationUserId == userId && u.PaymentStatus == SD.PaymentStatusPending);

            if (success)
            {
                _unitOfWork.OrderHeader.UpdatePayMobPaymentID(OrderHeader.Id, order_Id, id);
                _unitOfWork.OrderHeader.UpdateStatus(OrderHeader.Id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.save();
                return RedirectToAction(nameof(OrderConfirmation), new { order_id = order_Id });
            }
            return NotFound();
        }        
        public IActionResult OrderConfirmation(int order_id)
        {
            return View(order_id);
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
