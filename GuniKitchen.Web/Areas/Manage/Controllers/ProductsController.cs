using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GuniKitchen.Web.Data;
using GuniKitchen.Web.Models;
using GuniKitchen.Web.Areas.Manage.ViewModels;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace GuniKitchen.Web.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            ApplicationDbContext context,
            IConfiguration config,
            ILogger<ProductsController> logger)
        {
            _context = context;
            _config = config;
            _logger = logger;
        }

        // GET: Manage/Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Manage/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Manage/Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Manage/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ProductId,ProductName,ProductDescription,Price,UnitOfMeasure,Size,CategoryId,Photo")] ProductViewModel productViewModel)
        {

            if (ModelState.IsValid)
            {
                Product newProduct = new Product
                {
                    ProductId = productViewModel.ProductId,
                    ProductName = productViewModel.ProductName,
                    ProductDescription = productViewModel.ProductDescription,
                    Price = productViewModel.Price,
                    UnitOfMeasure = productViewModel.UnitOfMeasure,
                    Size = productViewModel.Size,
                    CategoryId = productViewModel.CategoryId
                };
                _context.Add(newProduct);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName", productViewModel.CategoryId);
            return View(productViewModel);
        }

        // GET: Manage/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // POST: Manage/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductDescription,Price,UnitOfMeasure,Size,CategoryId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Manage/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Manage/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        private async Task<string> SavePhotoToBlobAsync(IFormFile productImage)
        {
            //if (this.Request.Form.Files.Count >= 0)
            //{
            //    IFormFile file = this.Request.Form.Files.FirstOrDefault();
            //    using (var dataStream = new MemoryStream())
            //    {
            //        await file.CopyToAsync(dataStream);
            //        productViewModel.Photo = dataStream.ToArray();
            //    }
            //}
            //else
            //{
            //    ModelState.AddModelError("Photo", "Please select an image file to upload.");
            //}

            string photoUrl = string.Empty;
            string storageConnection1 = _config.GetValue<string>("MyAzureSettings:StorageAccountKey1");
            string storageConnection2 = _config.GetValue<string>("MyAzureSettings:StorageAccountKey2");
            string blobContainerName = "productimages";

            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnection1);
            BlobContainerClient blobContainerClient = new BlobContainerClient(storageConnection1, blobContainerName);
            await blobContainerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);

            string fileName = productImage.FileName;
            string fileType = productImage.ContentType;
            byte[] file;
            using (var dataStream = new MemoryStream())
            {
                await productImage.CopyToAsync(dataStream);
                file = dataStream.ToArray();
            }

            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);
            // await blobClient.UploadAsync();
            photoUrl = blobClient.Uri.ToString();
            return photoUrl;
        }
    }
}
