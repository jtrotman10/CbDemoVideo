using Azure.Storage.Blobs;
using CbDemoVideo.Data;
using CbDemoVideo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CbDemoVideo.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(BlobServiceClient blobServiceClient, IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _blobServiceClient = blobServiceClient;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Image()
        {
            return View();
        }

        public async Task<IActionResult> Database()
        {
            var customers = await _dbContext.Customers
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .ToListAsync();

            return View(customers);
        }

        [HttpGet]
        public async Task<IActionResult> BlobImage()
        {
            var containerName = _configuration["BlobImage:ContainerName"];
            var blobName = _configuration["BlobImage:BlobName"];

            if (string.IsNullOrWhiteSpace(containerName) || string.IsNullOrWhiteSpace(blobName))
            {
                return Problem("BlobImage configuration is incomplete.");
            }

            var blobClient = _blobServiceClient
                .GetBlobContainerClient(containerName)
                .GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
            {
                return NotFound();
            }

            var download = await blobClient.DownloadStreamingAsync();
            var contentType = download.Value.Details.ContentType;

            return File(download.Value.Content, string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
