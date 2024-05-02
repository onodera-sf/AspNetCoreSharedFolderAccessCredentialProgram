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

    // ��������ǉ�
    [HttpPost]
    public IActionResult Index(string dummy)
		{
      try
      {
        ViewData["Message"] = Util.ReadAndWrite($"�v���O��������̏������� ({DateTime.Now})");
      }
      catch (Exception ex)
      {
        ViewData["Message"] = ex.ToString();
      }
      return View();
    }
    // �����܂Œǉ�

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
