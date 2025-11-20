using System.Xml.Linq;
using FlashcardsApp.DAL; // Твой контекст БД
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Api.Controllers
{
    [ApiController]
    [Route("sitemap.xml")] // Важно: маршрут выглядит как файл
    public class SitemapController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        // реальный домен фронтенда
        private const string FrontendUrl = "https://flashcardsloop.org"; 

        public SitemapController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ResponseCache(Duration = 86400)] // Кешируем на 24 часа (чтобы не грузить БД каждым запросом робота)
        public async Task<IActionResult> GetSitemap()
        {
            // 1. Получаем все публичные группы (только ID и дату обновления)
            var publicGroups = await _context.Groups
                .Where(g => g.IsPublished)
                .Select(g => new { g.Id, LastModified = g.CreatedAt }) // Если есть поле UpdatedAt - лучше использовать его
                .AsNoTracking()
                .ToListAsync();

            // 2. Создаем XML пространство имен
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

            // 3. Собираем структуру
            var urlset = new XElement(ns + "urlset",

                // --- СТАТИЧЕСКИЕ СТРАНИЦЫ ---
                
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

            // --- ДИНАМИЧЕСКИЕ СТРАНИЦЫ (ГРУППЫ) ---
            foreach (var group in publicGroups)
            {
                urlset.Add(new XElement(ns + "url",
                    // Ссылка ведет на Фронтенд!
                    new XElement(ns + "loc", $"{FrontendUrl}/subscription/{group.Id}"),
                    // Дата последнего изменения важна для SEO
                    new XElement(ns + "lastmod", group.LastModified.ToString("yyyy-MM-dd")),
                    new XElement(ns + "changefreq", "weekly"),
                    new XElement(ns + "priority", "0.8")
                ));
            }

            // 4. Формируем итоговый документ
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), urlset);

            // Возвращаем как XML файл
            return Content(doc.Declaration + Environment.NewLine + doc, "application/xml");
        }
    }
}