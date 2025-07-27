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

        public IActionResult Index()
        {
            var products = _unitOfWork.Products.GetAll();
            return View(products);
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

            // Nếu có lỗi validation
            return View(product);
        }


        [HttpPost]
        public IActionResult Edit(Product product, IFormFile? Image)
        {
            var productFromDb = _unitOfWork.Products.Get(product.Id);
            if (productFromDb == null) return NotFound();

            if (ModelState.IsValid)
            {
                // Cập nhật thông tin cơ bản
                productFromDb.Name = product.Name;
                productFromDb.Price = product.Price;

                // Nếu có upload ảnh mới
                if (Image != null)
                {
                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(productFromDb.ImageUrl))
                    {
                        string oldPath = Path.Combine(_env.WebRootPath, productFromDb.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    // Upload ảnh mới
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


        public IActionResult Edit(int id)
        {
            var product = _unitOfWork.Products.Get(id);
            return View(product);
        }

        // GET: Xác nhận xóa
        public IActionResult Delete(int id)
        {
            var product = _unitOfWork.Products.Get(id);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Thực hiện xóa
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _unitOfWork.Products.Get(id);
            if (product == null) return NotFound();

            // Xóa ảnh nếu có
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

    }
}
