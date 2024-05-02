using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SharedFolderAccessRazorPages.Pages
{
	public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;

		public string Message { get; set; } = "";


		public IndexModel(ILogger<IndexModel> logger)
		{
			_logger = logger;
		}

		public void OnPost()
		{
      try
      {
        Message = Util.ReadAndWrite($"�v���O��������̏������� ({DateTime.Now})");
      }
      catch (Exception ex)
      {
        Message = ex.ToString();
      }
		}
	}
}
