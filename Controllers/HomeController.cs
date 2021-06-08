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

        [HttpPost("/Steganography/encode")]
        public ActionResult Encrypt(HomeViewModel ivm, IFormFile file)
        {
            string strpath = Path.GetExtension(file.FileName);
            if (strpath != ".jpg" && strpath != ".jpeg" && file.Length !<= 10000000)
            {
                ViewData["text"] = "Not a jpg file";
                return View();
            }
            else
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

                string format = "image/png";
                stream.Seek(0, SeekOrigin.Begin);
                FileStreamResult res = base.File(stream, format); ;

                res.FileDownloadName = "hidden_image.png";
                return res;
            }
        }

        [HttpPost("/Steganography/decode")]
        public ActionResult Decrypt(HomeViewModel ivm, IFormFile file)
        {
            string strpath = Path.GetExtension(file.FileName);
            if (strpath != ".png" && file.Length <= 10000000)
            {
                ViewData["text"] = "Not a png file";
                return View();
            }
            else
            {
                Bitmap img;
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    Image tempImg = Image.FromStream(memoryStream);
                    img = (Bitmap)tempImg;
                }
                string plaintext = Steganography.extractText(img);
                string decryptedText = CipherHelper.Decrypt(plaintext, ivm.Password);

                var stream = new MemoryStream();
                string format = "image/png";
                stream.Seek(0, SeekOrigin.Begin);
                FileStreamResult res = base.File(stream, format); ;

                ViewData["text"] = decryptedText;
                return View();

            }
        }
    }
}
