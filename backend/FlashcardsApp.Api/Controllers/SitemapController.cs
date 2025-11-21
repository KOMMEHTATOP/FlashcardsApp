using System.Xml.Linq;
using FlashcardsApp.DAL; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Api.Controllers
{
    [ApiController]
    [Route("sitemap.xml")] 
    public class SitemapController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private const string FrontendUrl = "https://flashcardsloop.org"; 

        public SitemapController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить GetSitemap, для SEO
        /// </summary>
        [HttpGet]
        [ResponseCache(Duration = 86400)] 
        public async Task<IActionResult> GetSitemap()
        {
            var publicGroups = await _context.Groups
                .Where(g => g.IsPublished)
                .Select(g => new { g.Id, LastModified = g.CreatedAt })
                .AsNoTracking()
                .ToListAsync();

            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

            var urlset = new XElement(ns + "urlset",
                
                // Главная (Лендинг)
                new XElement(ns + "url",
                    new XElement(ns + "loc", $"{FrontendUrl}/about"),
                    new XElement(ns + "lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd")),
                    new XElement(ns + "changefreq", "monthly"),
                    new XElement(ns + "priority", "1.0")
                ),
                 // Библиотека
                new XElement(ns + "url",
                    new XElement(ns + "loc", $"{FrontendUrl}/store"),
                    new XElement(ns + "lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd")),
                    new XElement(ns + "changefreq", "daily"),
                    new XElement(ns + "priority", "0.9")
                ),
                 // Логин
                new XElement(ns + "url",
                    new XElement(ns + "loc", $"{FrontendUrl}/login"),
                    new XElement(ns + "priority", "0.5")
                )
            );

            foreach (var group in publicGroups)
            {
                urlset.Add(new XElement(ns + "url",
                    new XElement(ns + "loc", $"{FrontendUrl}/subscription/{group.Id}"),
                    new XElement(ns + "lastmod", group.LastModified.ToString("yyyy-MM-dd")),
                    new XElement(ns + "changefreq", "weekly"),
                    new XElement(ns + "priority", "0.8")
                ));
            }

            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), urlset);

            return Content(doc.Declaration + Environment.NewLine + doc, "application/xml");
        }
    }
}