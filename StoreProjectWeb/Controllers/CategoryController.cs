using Microsoft.AspNetCore.Mvc;
using StoreProjectWeb.Data;
using StoreProjectWeb.Models;

namespace StoreProjectWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db )
        {
            _db = db;
        }

        public IActionResult Index()
        {
           List<Category> objCategoryList = _db.Categories.ToList();
           return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View(); 
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if(obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Name and Display Order must be different.");
            }

            if (obj.Name == "Test")
            {
                ModelState.AddModelError("", "Test is an invalid value for Category Name");
            }

            if (ModelState.IsValid)
            {
                _db.Categories.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Category created succesfully";
                return RedirectToAction("Index");
            }

            return View();

        }

        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound(); 
            }

            //Find use the primary key
            Category? categoryFromDb = _db.Categories.Find(id);

            //FirstOrDefault can use any property
            //Category? categoryFromDb2 = _db.Categories.FirstOrDefault(u => u.CategoryId == id);

            // Where busca igual que FirstOrDefault pero busca varios
            //Category? categoryFromDb3 = _db.Categories.Where(u => u.CategoryId == id).FirstOrDefault();


            if (categoryFromDb == null){
                return NotFound();

            }

            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Name and Display Order must be different.");
            }

            if (obj.Name == "Test")
            {
                ModelState.AddModelError("", "Test is an invalid value for Category Name");
            }

            if (ModelState.IsValid)
            {
                _db.Categories.Update(obj);
                _db.SaveChanges();
                TempData["success"] = "Category updated succesfully";
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? categoryFromDb = _db.Categories.Find(id);

            if (categoryFromDb == null)
            {
                return NotFound();

            }

            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            Category? categoryFromDb = _db.Categories.Find(id);

            if (categoryFromDb == null)
            {
                return NotFound();

            }

            _db.Categories.Remove(categoryFromDb);
            _db.SaveChanges();
            TempData["success"] = "Category deleted succesfully";
            return RedirectToAction("Index");
        }
    }
}
