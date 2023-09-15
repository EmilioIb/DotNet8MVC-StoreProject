using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreProjectWebRazor.Data;
using StoreProjectWebRazor.Models;

namespace StoreProjectWebRazor.Pages.Categories
{

    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public Category CategoryItem { get; set; }
        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet(int? id)
        {
            if (id != 0 && id != null)
            {
                CategoryItem = _db.Categories.Find(id);
            }
        }

        public IActionResult OnPost()
        {
            

            Category? categoryFromDb = _db.Categories.Find(CategoryItem.CategoryId);

            if (categoryFromDb == null)
            {
                return NotFound();

            }

            _db.Categories.Remove(categoryFromDb);
            _db.SaveChanges();
            TempData["success"] = "Category deleted succesfully";
            return RedirectToPage("Index");
        }
    }
}
