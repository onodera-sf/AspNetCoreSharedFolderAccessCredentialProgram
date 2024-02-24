using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;

/// <summary>
/// 共有フォルダにユーザー名とパスワードでアクセスするためのクラスです。
/// using を使用すればそのスコープの間、共有フォルダにアクセスできます。
/// </summary>
public class SharedFolderAccessor : IDisposable
{
	private readonly string _networkName;

	/// <summary>
	/// コンストラクタです。
	/// </summary>
	/// <param name="networkName">共有フォルダのあるサーバーを「\\&lt;サーバー名&gt;」形式で指定します。</param>
	/// <param name="credentials">共有フォルダにアクセスするための資格情報です。</param>
	/// <exception cref="Win32Exception"></exception>
	public SharedFolderAccessor(string networkName, NetworkCredential credentials)
	{
		_networkName = networkName;

		// 接続するネットワークの情報を設定
		var netResource = new NetResource
		{
			Scope = ResourceScope.GlobalNetwork,
			ResourceType = ResourceType.Disk,
			DisplayType = ResourceDisplaytype.Share,
			RemoteName = networkName,
		};

		// ドメインがある場合はドメイン名も指定、ない場合はユーザー名のみ
		var userName = string.IsNullOrEmpty(credentials.Domain)
				? credentials.UserName
				: $@"{credentials.Domain}\{credentials.UserName}";

		// 共有フォルダにユーザー名とパスワードで接続
		var result = WNetAddConnection2(netResource, credentials.Password, userName, 0);

		if (result != 0)
		{
			throw new Win32Exception(result, $"共有フォルダに接続できませんでした。(エラーコード:{result})");
		}

		// 正常に接続できれば WNetCancelConnection2 を呼び出すまではプログラムで共有フォルダにアクセス可能
	}

	~SharedFolderAccessor()
	{
    // Dispose を呼び忘れたときの保険
    WNetCancelConnection2(_networkName, 0, true);
	}

	public void Dispose()
	{
		WNetCancelConnection2(_networkName, 0, true);
		GC.SuppressFinalize(this);  // Dispose を明示的に呼んだ場合はデストラクタの処理は不要
	}

	/// <summary>
	/// ネットワーク リソースへの接続を確立し、ローカル デバイスをネットワーク リソースにリダイレクトできます。
	/// </summary>
	/// <param name="netResource">ネットワーク リソース、ローカル デバイス、ネットワーク リソース プロバイダーに関する情報など。</param>
	/// <param name="password">ネットワーク接続の作成に使用するパスワード。</param>
	/// <param name="username">接続を確立するためのユーザー名。</param>
	/// <param name="flags">接続オプションのセット。</param>
	/// <returns></returns>
	[DllImport("mpr.dll")]
	private static extern int WNetAddConnection2(NetResource netResource, string password, string username, int flags);

	/// <summary>
	/// 既存のネットワーク接続を取り消します。
	/// </summary>
	/// <param name="name">リダイレクトされたローカル デバイスまたは切断するリモート ネットワーク リソースの名前。</param>
	/// <param name="flags">接続の種類。</param>
	/// <param name="force">接続に開いているファイルまたはジョブがある場合に切断を行う必要があるかどうか。</param>
	/// <returns></returns>
	[DllImport("mpr.dll")]
	private static extern int WNetCancelConnection2(string name, int flags, bool force);

	/// <summary>
	/// NETRESOURCE 構造体を定義しています。
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	private class NetResource
	{
		public ResourceScope Scope;
		public ResourceType ResourceType;
		public ResourceDisplaytype DisplayType;
		public int Usage;
		public string LocalName = "";
		public string RemoteName = "";
		public string Comment = "";
		public string Provider = "";
	}

	/// <summary>
	/// ネットワークリソースのスコープ。
	/// </summary>
	private enum ResourceScope : int
	{
		/// <summary>ネットワークリソースへの現在の接続。</summary>
		Connected = 1,
		/// <summary>すべてのネットワークリソース。</summary>
		GlobalNetwork = 2,
		Remembered = 3,
		Recent = 4,
		/// <summary>ユーザーの現在および既定のネットワークコンテキストに関連付けられているネットワークリソース。</summary>
		Context = 5,
	};

	/// <summary>
	/// リソースの種類。
	/// </summary>
	private enum ResourceType : int
	{
		/// <summary>印刷リソースとディスクリソースの両方のコンテナー、または印刷またはディスク以外のリソースなど。</summary>
		Any = 0,
		/// <summary>共有ディスクボリューム。</summary>
		Disk = 1,
		/// <summary>共有プリンター。</summary>
		Print = 2,
		Reserved = 8,
	}

	/// <summary>
	/// ユーザーインターフェイスで使用する必要がある表示の種類。
	/// </summary>
	private enum ResourceDisplaytype : int
	{
		/// <summary>リソースの種類を指定しないネットワークプロバイダーによって使用されます。</summary>
		Generic = 0x0,
		/// <summary>サーバーのコレクション。</summary>
		Domain = 0x01,
		/// <summary>サーバー。</summary>
		Server = 0x02,
		/// <summary>共有ポイント。</summary>
		Share = 0x03,
		File = 0x04,
		Group = 0x05,
		/// <summary>ネットワークプロバイダー。</summary>
		Network = 0x06,
		Root = 0x07,
		Shareadmin = 0x08,
		/// <summary>ディレクトリ。</summary>
		Directory = 0x09,
		Tree = 0x0a,
		Ndscontainer = 0x0b,
	}
}
