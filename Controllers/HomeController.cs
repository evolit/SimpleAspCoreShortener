using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

using SimpleUrlShortenerSPA.Models;

namespace SimpleUrlShortenerSPA.Controllers
{
    public class HomeController : Controller, IDisposable
    {
        private IShortUrlsRepository repo;

        public HomeController(IShortUrlsRepository repository)
        {
            repo = repository;
        }

        ~HomeController()
        {
            repo.Dispose();
        }
    
        static List<string> masks = new List<string> { "http://", "https://" };

        #region Web Api
        public class UrlShorterRequest
        {
            public string url { get; set; }
        }

        [HttpPost("/api/shorten")]
        public IActionResult ShortenUrl([FromBody]UrlShorterRequest request)
        {
            if (request.url == null || request.url == string.Empty) 
                return new BadRequestResult();
            
            //TODO: check URL with regexp /http(s)?://[A-Za-z0-9\.]+\.[A-Za-z0-9]+.*/
            if (!masks.Any(s => request.url.StartsWith(s, StringComparison.OrdinalIgnoreCase)))
                return new BadRequestResult();
            
            ShortedUrlEntity shortenUrl = new ShortedUrlEntity() {
                Url = request.url,
                ShortUrlSuffix = RandomString(),
                CreateDate = DateTime.Now
            };
            repo.Create(shortenUrl);
            repo.Save();
            System.Console.WriteLine($">>>Short Url successfuly created { request.url }");
            return Json(shortenUrl);
        }

        [HttpGet("/api/history")]
        public IEnumerable<ShortedUrlEntity> GetHistoryPage(int page = 1, int itemsPerPage = 10)
        {
            System.Console.WriteLine(">>>Return history");
            return repo.getAll();
        }

        private static Random random = new Random();
        public static string RandomString(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        #endregion //end Web Api

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
