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
			// �t�@�C���̏����o��
			System.IO.File.WriteAllText(Path.Combine($@"\\{serverName}\{folderName}", outputFileName), text);

      // �t�@�C���̓ǂݍ���
      return System.IO.File.ReadAllText(Path.Combine($@"\\{serverName}\{folderName}", inputFileName));
		}
	}
}
