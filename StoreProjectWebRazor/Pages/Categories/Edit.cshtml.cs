using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StoreProjectWebRazor.Data;
using StoreProjectWebRazor.Models;

namespace StoreProjectWebRazor.Pages.Categories
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public Category CategoryItem { get; set; }
        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int? id)
        {
            if(id != 0 && id != null)
            {
                CategoryItem = _db.Categories.Find(id);
            }
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Update(CategoryItem);
                _db.SaveChanges();
                TempData["success"] = "Category updated succesfully";
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
