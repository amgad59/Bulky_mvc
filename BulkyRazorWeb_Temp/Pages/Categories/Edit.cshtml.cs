using BulkyRazorWeb_Temp.Data;
using BulkyRazorWeb_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyRazorWeb_Temp.Pages.Categories
{
    public class EditModel : PageModel
    {

		private readonly ApplicationDbContext _db;
		[BindProperty]
		public Category Category { get; set; }
		public EditModel(ApplicationDbContext db)
		{
			_db = db;
		}
		public IActionResult OnGet(int? id)
        {
			if(id != 0 && id != null)
			{
				Category = _db.Categories.FirstOrDefault(u=>u.Id == id);
			}
			if(Category == null)
			{
				return NotFound();
			}
			return Page();
        }
		public IActionResult OnPost()
		{
			if (ModelState.IsValid)
			{
				_db.Categories.Update(Category);
				_db.SaveChanges();
				TempData["success"] = "category updated";
				return RedirectToPage("Index");
			}
			return Page();
		}
    }
}
