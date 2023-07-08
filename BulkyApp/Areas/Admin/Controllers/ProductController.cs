using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace BulkyApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM();
            productVM.CategoryList = _unitOfWork.Category.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            productVM.ProductSizeList = _unitOfWork.ProductSize.GetAll().ToList();
            if (id == null || id == 0)
            {
                productVM.product = new Product();
                productVM.product.ProductSizes = new List<ProductSize>();
                return View(productVM);
            }
            productVM.product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "ProductSizes");

            if (productVM.product == null)
            {
                return NotFound();
            }
            return View(productVM);
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {

            if (ModelState.IsValid)
            {

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");
                    if (!string.IsNullOrEmpty(productVM.product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.product.ImageUrl = @"\images\product\" + fileName;
                }



                if (productVM.product.Id == 0)
                    _unitOfWork.Product.Add(productVM.product);
                else
                {
                    _unitOfWork.Product.update(productVM.product);
                }
                _unitOfWork.save();
                int size = productVM.ProductSizeList.Count;
                productVM.product = _unitOfWork.Product.Get(u => u.Id == productVM.product.Id, includeProperties: "ProductSizes",isTracked:true);
                for (int i = 0; i < size; i++)
                {
                    if (!productVM.ProductSizeList[i].isSelected)
                    {
                        _unitOfWork.Product.deleteSize(productVM.product, productVM.ProductSizeList[i].Id);

                    }
                    else
                    {
                        _unitOfWork.Product.addSize(productVM.product, productVM.ProductSizeList[i].Id);
                    }
                }
                _unitOfWork.save();
                TempData["success"] = "product created";
                return RedirectToAction(nameof(Index));
            }
            productVM.CategoryList = _unitOfWork.Category.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            productVM.ProductSizeList = _unitOfWork.ProductSize.GetAll().ToList();
            return View(productVM);
        }
        public IActionResult Index()
        {
            List<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductSizes").ToList();
            return View(productList);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductSizes").ToList();
            return Json(new { data = products });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Product product = _unitOfWork.Product.Get(u => u.Id == id);
            if (product == null) { return Json(new { success = false, message = "error deleting" }); }
            var oldImagePath = Path.Combine(_webHostEnvironment.ContentRootPath, product.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Delete(product);
            _unitOfWork.save();
            return Json(new { success = true, message = "Delete success" });
        }
        #endregion
    }
}
