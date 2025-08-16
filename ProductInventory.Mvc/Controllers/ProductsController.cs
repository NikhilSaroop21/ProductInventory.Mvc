using Microsoft.AspNetCore.Mvc;
using ProductInventory.Mvc.Models;
using ProductInventory.Mvc.Services;

namespace ProductInventory.Mvc.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IInventoryService _inv;
        public ProductsController(IInventoryService inv) => _inv = inv;

        // List + Search
        [HttpGet]
        public IActionResult Index(string? q)
        {
            var items = string.IsNullOrWhiteSpace(q) ? _inv.GetAll() : _inv.Search(q);
            ViewBag.Query = q ?? "";
            ViewBag.Count = _inv.Count;
            return View(items);
        }

        // Details
        [HttpGet]
        public IActionResult Details(int id)
        {
            if (!_inv.TryGet(id, out var p) || p == null)
            {
                TempData["Msg"] = $"Product {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        // Create
        [HttpGet]
        public IActionResult Create() => View(new Product());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product model)
        {
            model.Name = model.Name?.Trim() ?? string.Empty;

            if (!ModelState.IsValid) return View(model);
            if (!_inv.Add(model))
            {
                ModelState.AddModelError(nameof(model.ProductID), "ProductID already exists.");
                return View(model);
            }
            TempData["Msg"] = "Product added.";
            return RedirectToAction(nameof(Index));
        }

        // Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!_inv.TryGet(id, out var p) || p == null)
            {
                TempData["Msg"] = $"Product {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        // Guard route id vs posted id to prevent tampering
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product model)
        {
            model.Name = model.Name?.Trim() ?? string.Empty;

            if (id != model.ProductID)
                ModelState.AddModelError(nameof(model.ProductID), "ProductID cannot be changed.");

            if (!ModelState.IsValid) return View(model);

            if (!_inv.Update(model))
            {
                ModelState.AddModelError("", "Cannot update: ProductID not found.");
                return View(model);
            }
            TempData["Msg"] = "Product updated.";
            return RedirectToAction(nameof(Index));
        }

        // Delete
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!_inv.TryGet(id, out var p) || p == null)
            {
                TempData["Msg"] = $"Product {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!_inv.Delete(id))
            {
                TempData["Msg"] = $"Product {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Msg"] = "Product deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
