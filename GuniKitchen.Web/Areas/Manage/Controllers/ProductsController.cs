using Azure.Storage.Blobs;
using GuniKitchen.Web.Areas.Manage.ViewModels;
using GuniKitchen.Web.Data;
using GuniKitchen.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace GuniKitchen.Web.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductsController : Controller
    {
        private const string BlobContainerNAME = "productimages";

        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            ApplicationDbContext context,
            IConfiguration config,
            IWebHostEnvironment environment,
            ILogger<ProductsController> logger)
        {
            _context = context;
            _config = config;
            _environment = environment;
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
                string photoUrl = null;
                if (productViewModel.Photo != null)
                {
                    // Upload the product image to the Blob Storage Account.
                    photoUrl = await SavePhotoToBlobAsync(productViewModel.Photo);
                }

                Product newProduct = new Product
                {
                    ProductId = productViewModel.ProductId,
                    CategoryId = productViewModel.CategoryId,
                    ProductName = productViewModel.ProductName,
                    ProductDescription = productViewModel.ProductDescription,
                    Price = productViewModel.Price,
                    UnitOfMeasure = productViewModel.UnitOfMeasure,
                    Size = productViewModel.Size,
                    ProductImageFileUrl = photoUrl,
                    ProductImageContentType = photoUrl == null ? null : productViewModel.Photo.ContentType
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
            var productViewModel = new ProductViewModel
            {
                ProductId = product.ProductId,
                CategoryId = product.CategoryId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                Price = product.Price,
                UnitOfMeasure = product.UnitOfMeasure,
                Size = product.Size
            };
            ViewBag.ProductImageFileUrl = product.ProductImageFileUrl;
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName", productViewModel.CategoryId);
            return View(productViewModel);
        }

        // POST: Manage/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, 
            [Bind("ProductId,ProductName,ProductDescription,Price,UnitOfMeasure,Size,CategoryId,Photo")] ProductViewModel productViewModel)
        {
            if (id != productViewModel.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string photoUrl = null;
                if (productViewModel.Photo != null)
                {
                    // Upload the product image to the Blob Storage Account.
                    photoUrl = await SavePhotoToBlobAsync(productViewModel.Photo);
                }

                var product = _context.Products.FirstOrDefault(p => p.ProductId == productViewModel.ProductId);
                product.CategoryId = productViewModel.CategoryId;
                product.ProductName = productViewModel.ProductName;
                product.ProductDescription = productViewModel.ProductDescription;
                product.Price = productViewModel.Price;
                product.UnitOfMeasure = productViewModel.UnitOfMeasure;
                product.Size = productViewModel.Size;
                product.ProductImageFileUrl = photoUrl;
                product.ProductImageContentType = photoUrl == null ? null : productViewModel.Photo.ContentType;

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
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName", productViewModel.CategoryId);
            return View(productViewModel);
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
            string storageConnection1 = _config.GetValue<string>("MyAzureSettings:StorageAccountKey1");
            string storageConnection2 = _config.GetValue<string>("MyAzureSettings:StorageAccountKey2");
            string fileName = productImage.FileName;
            string fileType = productImage.ContentType;
            string filepath = string.Empty;
            string photoUrl = string.Empty;

            if (productImage != null && productImage.Length > 0)
            {
                // Save the uploaded file on to a TEMP file.
                filepath = Path.GetTempFileName();
                using (var stream = System.IO.File.Create(filepath))
                {
                    productImage.CopyToAsync(stream).Wait();
                }
            }

            // Get a reference to a container 
            BlobContainerClient blobContainerClient = new BlobContainerClient(storageConnection1, BlobContainerNAME);

            // Create the container if it does not exist - granting PUBLIC access.
            await blobContainerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);

            // Create the client to the Blob Item
            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

            // Open the file and upload its data
            using (FileStream uploadFileStream = System.IO.File.OpenRead(filepath))
            {
                await blobClient.UploadAsync(uploadFileStream, overwrite: true);
                uploadFileStream.Close();
            }

            // Delete the TEMP file since it is no longer needed.
            System.IO.File.Delete(filepath);

            // Return the URI of the item in the Blob Storage
            photoUrl = blobClient.Uri.ToString();
            return photoUrl;
        }
    }
}
