using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreProjectWebRazor.Data;
using StoreProjectWebRazor.Models;

namespace StoreProjectWebRazor.Pages.Categories
{
    [BindProperties]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public Category CategoryItem { get; set; }
        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            _db.Categories.Add(CategoryItem);
            _db.SaveChanges();
            TempData["success"] = "Category created succesfully";
            return RedirectToPage("Index");
        }
    }
}
