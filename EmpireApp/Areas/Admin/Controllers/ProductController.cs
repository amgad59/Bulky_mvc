using Empire.DataAccess.Repository;
using Empire.DataAccess.Repository.IRepository;
using Empire.Models;
using Empire.Models.ViewModels;
using Empire.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace EmpireApp.Areas.Admin.Controllers
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
        public async Task<IActionResult> Upsert(int? id)
        {
            ProductVM productVM = new ProductVM();
            IEnumerable<Category> categories = await _unitOfWork.Category.GetAllEntities();
            productVM.CategoryList = categories
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            IEnumerable<ProductSize> productSizes = await _unitOfWork.ProductSize.GetAllEntities();
            productVM.ProductSizeList = productSizes.ToList();
            if (id == null || id == 0)
            {
                productVM.product = new Product();
                productVM.product.ProductSizes = new List<ProductSize>();
                return View(productVM);
            }
            productVM.product = await _unitOfWork.Product.GetEntity(u => u.Id == id, includeProperties: "ProductSizes,ProductImages");

            if (productVM.product == null)
            {
                return NotFound();
            }
            if(productVM.product.Id != 0)
            {
                foreach (var size in productVM.product.ProductSizes)
                {
                    productVM.ProductSizeList.FirstOrDefault(u => u.Id == size.Id).isSelected = true;
                }
            }
            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(ProductVM productVM, List<IFormFile> files)
        {

            if (ModelState.IsValid)
            {
                if (productVM.product.Id == 0)
                    await _unitOfWork.Product.Add(productVM.product);
                else
                {
                    _unitOfWork.Product.Update(productVM.product);
                }
                await _unitOfWork.Save();

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\product\product-" + productVM.product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);
                        if (!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }
                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productVM.product.Id,
                        };
                        if(productVM.product.ProductImages == null)
                            productVM.product.ProductImages = new List<ProductImage>();

                        productVM.product.ProductImages.Add(productImage);
                    }
                    _unitOfWork.Product.Update(productVM.product);
                    await _unitOfWork.Save();
                }



                int size = productVM.ProductSizeList.Count;
                productVM.product = await _unitOfWork.Product.GetEntity(u => u.Id == productVM.product.Id, includeProperties: "ProductSizes",isTracked:true);
                for (int i = 0; i < size; i++)
                {
                    if (!productVM.ProductSizeList[i].isSelected)
                    {
                        _unitOfWork.Product.DeleteSize(productVM.product, productVM.ProductSizeList[i].Id);

                    }
                    else
                    {
                        await _unitOfWork.Product.AddSize(productVM.product, productVM.ProductSizeList[i].Id);
                    }
                }
                await _unitOfWork.Save();
                TempData["success"] = "product created/updated";
                return RedirectToAction(nameof(Index));
            }
            IEnumerable<Category> categories = await _unitOfWork.Category.GetAllEntities();
            productVM.CategoryList = categories
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            IEnumerable<ProductSize> productSizes = await _unitOfWork.ProductSize.GetAllEntities();
            productVM.ProductSizeList = productSizes.ToList();
            return View(productVM);
        }

        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var imageToBeDeleted = await _unitOfWork.ProductImage.GetEntity(u => u.Id == imageId);
            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                        imageToBeDeleted.ImageUrl.TrimStart('\\'));
                    if(System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.ProductImage.Delete(imageToBeDeleted);
                await _unitOfWork.Save();
                TempData["success"] = "Deleted Successfully";
            }
            return RedirectToAction(nameof(Upsert), new { id = imageToBeDeleted.ProductId});
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> productList = await _unitOfWork.Product.GetAllEntities(includeProperties: "Category,ProductSizes");
            return View(productList.ToList());
        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Product> products = await _unitOfWork.Product.GetAllEntities(includeProperties: "Category,ProductSizes");
            return Json(new { data = products.ToList() });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            Product product = await _unitOfWork.Product.GetEntity(u => u.Id == id);
            if (product == null) { return Json(new { success = false, message = "error deleting" }); }


            string productPath = @"images\product\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);
            if (Directory.Exists(finalPath))
            {
                Directory.Delete(finalPath,true);
            }
            _unitOfWork.Product.Delete(product);
            await _unitOfWork.Save();
            return Json(new { success = true, message = "Delete success" });
        }
        #endregion
    }
}
