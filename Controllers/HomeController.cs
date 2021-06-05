using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using JoshuaPortelliSynoptic.Models;
using JoshuaPortelliSynoptic.ViewModel;
using SecuringSynoptic;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace JoshuaPortelliSynoptic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public ActionResult Encrypt(HomeViewModel ivm, IFormFile file)
        {
            Bitmap img;
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                Image tempImg = Image.FromStream(memoryStream);
                img = (Bitmap)tempImg;
            }
            string encryptedText = CipherHelper.Encrypt(ivm.Text, ivm.Password);
            img = Steganography.embedText(encryptedText, img);
            var stream = new MemoryStream();
            
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

            //string format = MediaTypeNames.Application.Octet.ToString(); -> doing this leads to direct download
            string format = "image/png";
            stream.Seek(0, SeekOrigin.Begin);
            FileStreamResult res = base.File(stream, format); ;

            res.FileDownloadName = "hidden_image.png";
            return res;
        }

        public ActionResult Decrypt(HomeViewModel ivm, IFormFile file)
        {
            Bitmap img;
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                Image tempImg = Image.FromStream(memoryStream);
                img = (Bitmap)tempImg;
            }
            string encryptedText = CipherHelper.Encrypt(ivm.Text, ivm.Password);
            img = Steganography.embedText(encryptedText, img);
            var stream = new MemoryStream();

            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

            //string format = MediaTypeNames.Application.Octet.ToString(); -> doing this leads to direct download
            string format = "image/png";
            stream.Seek(0, SeekOrigin.Begin);
            FileStreamResult res = base.File(stream, format); ;

            res.FileDownloadName = "hidden_image.png";
            return res;
        }

        /*[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }*/

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(HomeViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(model);

                Home home = new Home
                {
                    Text = model.Text,
                    Password = model.Password,
                    Picture = uniqueFileName,
                };
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        
        
        private string UploadedFile(HomeViewModel model)
        {
            string uniqueFileName = null;

            if (model.Picture != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }*/
    }
}
