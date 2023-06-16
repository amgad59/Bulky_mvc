using BulkyApp.Data;
using BulkyApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> categories = _db.Categories.ToList();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category c)
        {/*
            if(c.Name == "hey")
            {
                ModelState.AddModelError("", "can't have numbers bruv");
            }*/
            if (ModelState.IsValid)
            {
                _db.Categories.Add(c);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}
