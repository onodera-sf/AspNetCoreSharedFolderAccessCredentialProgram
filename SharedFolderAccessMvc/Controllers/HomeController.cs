using Microsoft.AspNetCore.Mvc;
using SharedFolderAccessMvc.Models;
using System.Diagnostics;

namespace SharedFolderAccessMvc.Controllers
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

    // ここから追加
    [HttpPost]
    public IActionResult Index(string dummy)
		{
      try
      {
        ViewData["Message"] = Util.ReadAndWrite($"プログラムからの書き込み ({DateTime.Now})");
      }
      catch (Exception ex)
      {
        ViewData["Message"] = ex.ToString();
      }
      return View();
    }
    // ここまで追加

    public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
