using Empire.DataAccess.Data;
using Empire.DataAccess.Repository;
using Empire.DataAccess.Repository.IRepository;
using Empire.Models;
using Empire.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EmpireApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categories = await _unitOfWork.Category.GetAllEntities();
            return View(categories.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category c)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Category.Add(c);
                await _unitOfWork.Save();
                TempData["success"] = "category created";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = await _unitOfWork.Category.GetEntity(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category c)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(c);
                await _unitOfWork.Save();
                TempData["success"] = "category updated";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = await _unitOfWork.Category.GetEntity(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName(nameof(Delete))]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            Category? category = await _unitOfWork.Category.GetEntity(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Delete(category);
            await _unitOfWork.Save();
            TempData["success"] = "category deleted";
            return RedirectToAction(nameof(Index));
        }
    }
}
