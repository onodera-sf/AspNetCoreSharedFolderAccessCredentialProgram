using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;

/// <summary>
/// ���L�t�H���_�Ƀ��[�U�[���ƃp�X���[�h�ŃA�N�Z�X���邽�߂̃N���X�ł��B
/// using ���g�p����΂��̃X�R�[�v�̊ԁA���L�t�H���_�ɃA�N�Z�X�ł��܂��B
/// </summary>
public class SharedFolderAccessor : IDisposable
{
	private readonly string _networkName;

	/// <summary>
	/// �R���X�g���N�^�ł��B
	/// </summary>
	/// <param name="networkName">���L�t�H���_�̂���T�[�o�[���u\\&lt;�T�[�o�[��&gt;�v�`���Ŏw�肵�܂��B</param>
	/// <param name="credentials">���L�t�H���_�ɃA�N�Z�X���邽�߂̎��i���ł��B</param>
	/// <exception cref="Win32Exception"></exception>
	public SharedFolderAccessor(string networkName, NetworkCredential credentials)
	{
		_networkName = networkName;

		// �ڑ�����l�b�g���[�N�̏���ݒ�
		var netResource = new NetResource
		{
			Scope = ResourceScope.GlobalNetwork,
			ResourceType = ResourceType.Disk,
			DisplayType = ResourceDisplaytype.Share,
			RemoteName = networkName,
		};

		// �h���C��������ꍇ�̓h���C�������w��A�Ȃ��ꍇ�̓��[�U�[���̂�
		var userName = string.IsNullOrEmpty(credentials.Domain)
				? credentials.UserName
				: $@"{credentials.Domain}\{credentials.UserName}";

		// ���L�t�H���_�Ƀ��[�U�[���ƃp�X���[�h�Őڑ�
		var result = WNetAddConnection2(netResource, credentials.Password, userName, 0);

		if (result != 0)
		{
			throw new Win32Exception(result, $"���L�t�H���_�ɐڑ��ł��܂���ł����B(�G���[�R�[�h:{result})");
		}

		// ����ɐڑ��ł���� WNetCancelConnection2 ���Ăяo���܂ł̓v���O�����ŋ��L�t�H���_�ɃA�N�Z�X�\
	}

	~SharedFolderAccessor()
	{
    // Dispose ���ĂіY�ꂽ�Ƃ��̕ی�
    WNetCancelConnection2(_networkName, 0, true);
	}

	public void Dispose()
	{
		WNetCancelConnection2(_networkName, 0, true);
		GC.SuppressFinalize(this);  // Dispose �𖾎��I�ɌĂ񂾏ꍇ�̓f�X�g���N�^�̏����͕s�v
	}

	/// <summary>
	/// �l�b�g���[�N ���\�[�X�ւ̐ڑ����m�����A���[�J�� �f�o�C�X���l�b�g���[�N ���\�[�X�Ƀ��_�C���N�g�ł��܂��B
	/// </summary>
	/// <param name="netResource">�l�b�g���[�N ���\�[�X�A���[�J�� �f�o�C�X�A�l�b�g���[�N ���\�[�X �v���o�C�_�[�Ɋւ�����ȂǁB</param>
	/// <param name="password">�l�b�g���[�N�ڑ��̍쐬�Ɏg�p����p�X���[�h�B</param>
	/// <param name="username">�ڑ����m�����邽�߂̃��[�U�[���B</param>
	/// <param name="flags">�ڑ��I�v�V�����̃Z�b�g�B</param>
	/// <returns></returns>
	[DllImport("mpr.dll")]
	private static extern int WNetAddConnection2(NetResource netResource, string password, string username, int flags);

	/// <summary>
	/// �����̃l�b�g���[�N�ڑ����������܂��B
	/// </summary>
	/// <param name="name">���_�C���N�g���ꂽ���[�J�� �f�o�C�X�܂��͐ؒf���郊���[�g �l�b�g���[�N ���\�[�X�̖��O�B</param>
	/// <param name="flags">�ڑ��̎�ށB</param>
	/// <param name="force">�ڑ��ɊJ���Ă���t�@�C���܂��̓W���u������ꍇ�ɐؒf���s���K�v�����邩�ǂ����B</param>
	/// <returns></returns>
	[DllImport("mpr.dll")]
	private static extern int WNetCancelConnection2(string name, int flags, bool force);

	/// <summary>
	/// NETRESOURCE �\���̂��`���Ă��܂��B
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
	/// �l�b�g���[�N���\�[�X�̃X�R�[�v�B
	/// </summary>
	private enum ResourceScope : int
	{
		/// <summary>�l�b�g���[�N���\�[�X�ւ̌��݂̐ڑ��B</summary>
		Connected = 1,
		/// <summary>���ׂẴl�b�g���[�N���\�[�X�B</summary>
		GlobalNetwork = 2,
		Remembered = 3,
		Recent = 4,
		/// <summary>���[�U�[�̌��݂���ъ���̃l�b�g���[�N�R���e�L�X�g�Ɋ֘A�t�����Ă���l�b�g���[�N���\�[�X�B</summary>
		Context = 5,
	};

	/// <summary>
	/// ���\�[�X�̎�ށB
	/// </summary>
	private enum ResourceType : int
	{
		/// <summary>������\�[�X�ƃf�B�X�N���\�[�X�̗����̃R���e�i�[�A�܂��͈���܂��̓f�B�X�N�ȊO�̃��\�[�X�ȂǁB</summary>
		Any = 0,
		/// <summary>���L�f�B�X�N�{�����[���B</summary>
		Disk = 1,
		/// <summary>���L�v�����^�[�B</summary>
		Print = 2,
		Reserved = 8,
	}

	/// <summary>
	/// ���[�U�[�C���^�[�t�F�C�X�Ŏg�p����K�v������\���̎�ށB
	/// </summary>
	private enum ResourceDisplaytype : int
	{
		/// <summary>���\�[�X�̎�ނ��w�肵�Ȃ��l�b�g���[�N�v���o�C�_�[�ɂ���Ďg�p����܂��B</summary>
		Generic = 0x0,
		/// <summary>�T�[�o�[�̃R���N�V�����B</summary>
		Domain = 0x01,
		/// <summary>�T�[�o�[�B</summary>
		Server = 0x02,
		/// <summary>���L�|�C���g�B</summary>
		Share = 0x03,
		File = 0x04,
		Group = 0x05,
		/// <summary>�l�b�g���[�N�v���o�C�_�[�B</summary>
		Network = 0x06,
		Root = 0x07,
		Shareadmin = 0x08,
		/// <summary>�f�B���N�g���B</summary>
		Directory = 0x09,
		Tree = 0x0a,
		Ndscontainer = 0x0b,
	}
}
