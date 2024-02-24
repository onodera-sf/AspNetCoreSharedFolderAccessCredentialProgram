using System.Net;

public static class Util
{
	public static string ReadAndWrite(string text)
	{
		var serverName = "ServerName";
		var folderName = "SharedFolder";
		var inputFileName = "Input.txt";
		var outputFileName = "Output.txt";
		var username = "SharedUser";
		var password = "password";

		var credentials = new NetworkCredential(username, password);
		using (new SharedFolderAccessor($@"\\{serverName}", credentials))
		{
			// ファイルの書き出し
			System.IO.File.WriteAllText(Path.Combine($@"\\{serverName}\{folderName}", outputFileName), text);

      // ファイルの読み込み
      return System.IO.File.ReadAllText(Path.Combine($@"\\{serverName}\{folderName}", inputFileName));
		}
	}
}
