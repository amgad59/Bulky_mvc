using Empire.DataAccess.Repository.IRepository;
using Empire.Models;
using Empire.Models.API;
using Empire.Models.ViewModels;
using Empire.Utilities;
using EmpireApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;

namespace EmpireApp.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize]
	public class OrderController : Controller
    {

        private readonly IPayMobService _payMobService;
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM orderVM { get;set; }
        public OrderController(IUnitOfWork unitOfWork, IPayMobService payMobService)
        {
            _unitOfWork = unitOfWork;
            _payMobService = payMobService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(int orderId)
        {
            orderVM = new()
            {
                OrderHeader = await _unitOfWork.OrderHeader.GetEntity(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = await _unitOfWork.OrderDetail.GetAllEntities(u => u.OrderHeaderId == orderId,includeProperties:"Product")
            };
            return View(orderVM);
        }
        [HttpPost]
        [Authorize(Roles =SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> UpdateOrderDetails()
        {
            var orderHeaderFromDb = await _unitOfWork.OrderHeader.GetEntity(u => u.Id == orderVM.OrderHeader.Id);
            orderHeaderFromDb.Name = orderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = orderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = orderVM.OrderHeader.City;
            orderHeaderFromDb.ShippingDate = orderVM.OrderHeader.ShippingDate;
            if(!string.IsNullOrEmpty(orderVM.OrderHeader.Carrier))
                orderHeaderFromDb.Carrier = orderVM.OrderHeader.Carrier;
            if(!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
                orderHeaderFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;

            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            await _unitOfWork.Save();
            TempData["success"] = "order updated successfully";
            return RedirectToAction(nameof(Details),new {orderId = orderHeaderFromDb.Id});
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> StartProcessing()
        {
            await _unitOfWork.OrderHeader.UpdateStatus(orderVM.OrderHeader.Id,SD.StatusInProcess);
            await _unitOfWork.Save();
            TempData["success"] = "order status updated";
            return RedirectToAction(nameof(Details),new {orderId = orderVM.OrderHeader.Id});
        }
        
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> ShipOrder()
        {
            var orderHeader = await _unitOfWork.OrderHeader.GetEntity(u => u.Id == orderVM.OrderHeader.Id);
            orderHeader.Carrier = orderVM.OrderHeader.Carrier;
            orderHeader.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            _unitOfWork.OrderHeader.Update(orderHeader);
            await _unitOfWork.Save();
            TempData["success"] = "order shipped successfully";
            return RedirectToAction(nameof(Details),new {orderId = orderVM.OrderHeader.Id});
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> CancelOrder()
        {
            var orderHeader = await _unitOfWork.OrderHeader.GetEntity(u => u.Id == orderVM.OrderHeader.Id);
            if(orderHeader.TransactionId != null && orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                await _payMobService.Refund<APIResponse>((int)orderHeader.TransactionId, orderHeader.OrderTotal);
                await _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                await _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            await _unitOfWork.Save();
            TempData["success"] = "order cancelled successfully";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }


        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders;
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaders = await _unitOfWork.OrderHeader.GetAllEntities(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                orderHeaders = await _unitOfWork.OrderHeader.GetAllEntities(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
            }
            switch (status)
			{
				case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
					break;
				case "completed":
					orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
					break;
				case "approved":
					orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
					break;
				default:
					break;
			}

			return Json(new { data = orderHeaders.ToList() });
        }

        #endregion
    }
}
