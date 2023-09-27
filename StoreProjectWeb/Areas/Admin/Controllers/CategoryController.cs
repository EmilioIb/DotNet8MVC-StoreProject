using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreProject.DataAccess.Data;
using StoreProject.DataAccess.Repository.IRepository;
using StoreProject.Models;
using StoreProject.Utility;

namespace StoreProjectWeb.Areas.Admin.Controllers
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

        //Se puede añadir autorizacion a acciones unicas
        //[Authorize(Roles = SD.Role_Admin)]
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
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
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category created succesfully";
                return RedirectToAction("Index");
            }

            return View();

        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            //Find use the primary key
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.CategoryId == id);

            //FirstOrDefault can use any property
            //Category? categoryFromDb2 = _db.Categories.FirstOrDefault(u => u.CategoryId == id);

            // Where busca igual que FirstOrDefault pero busca varios
            //Category? categoryFromDb3 = _db.Categories.Where(u => u.CategoryId == id).FirstOrDefault();


            if (categoryFromDb == null)
            {
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
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
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

            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.CategoryId == id);

            if (categoryFromDb == null)
            {
                return NotFound();

            }

            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.CategoryId == id);

            if (categoryFromDb == null)
            {
                return NotFound();

            }

            _unitOfWork.Category.Remove(categoryFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted succesfully";
            return RedirectToAction("Index");
        }
    }
}
