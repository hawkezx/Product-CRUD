using Microsoft.AspNetCore.Mvc;
using MyApp.Data;
using MyApp.Models;

namespace MyApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product, IFormFile? Image)
        {
            if (ModelState.IsValid)
            {
                if (Image != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                    string imagePath = Path.Combine(_env.WebRootPath, "images", "products");

                    if (!Directory.Exists(imagePath))
                        Directory.CreateDirectory(imagePath);

                    using (var stream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    {
                        Image.CopyTo(stream);
                    }

                    product.ImageUrl = "/images/products/" + fileName;
                }

                _unitOfWork.Products.Add(product);
                _unitOfWork.Complete();

                return RedirectToAction("Index");
            }

            return View(product);
        }

        public IActionResult Edit(int id)
        {
            var product = _unitOfWork.Products.Get(id);
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product, IFormFile? Image)
        {
            var productFromDb = _unitOfWork.Products.Get(product.Id);
            if (productFromDb == null) return NotFound();

            if (ModelState.IsValid)
            {
                productFromDb.Name = product.Name;
                productFromDb.Price = product.Price;

                if (Image != null)
                {
                    if (!string.IsNullOrEmpty(productFromDb.ImageUrl))
                    {
                        string oldPath = Path.Combine(_env.WebRootPath, productFromDb.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                    string imagePath = Path.Combine(_env.WebRootPath, "images", "products");

                    if (!Directory.Exists(imagePath))
                        Directory.CreateDirectory(imagePath);

                    using (var stream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                    {
                        Image.CopyTo(stream);
                    }

                    productFromDb.ImageUrl = "/images/products/" + fileName;
                }

                _unitOfWork.Complete();
                return RedirectToAction("Index");
            }

            return View(productFromDb);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _unitOfWork.Products.Get(id);
            if (product == null) return NotFound();

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                string oldPath = Path.Combine(_env.WebRootPath, product.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            _unitOfWork.Products.Remove(product);
            _unitOfWork.Complete();

            return RedirectToAction("Index");
        }

        public IActionResult Index(int page = 1)
        {
            int pageSize = 5;
            var products = _unitOfWork.Products.GetPaged(page, pageSize, out int totalItems);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(products);
        }
    }
}
