using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

#if !NETFX_CORE
namespace Core.Data
{
	public class EncryptedJsonSaver<T> : JsonSaver<T> where T : IDataStore
	{
		/// <summary>
		/// IV for Rijndael defaults to 16 if using its default block size
		/// </summary>
		private const int k_InitializationVectorLength = 16;

		/// <summary>
		/// Longest supported key length for Rijndael is 256 bits (32 bytes)
		/// Other supported values are 128 or 192 bits
		/// </summary>
		private const int k_KeyLength = 32;

		/// <summary>
		/// Salt for encryption
		/// </summary>
		private static readonly byte[] s_Salt =
		{
			0x6b, 0xb0, 0xa1, 0x65, 0x08, 0xf8, 0xe6, 0xe8, 0x4d, 0x9e, 0x2f, 0x19, 0x97, 0xec, 0x0d, 0x6e,
			0xe7, 0xec, 0xe2, 0x0a, 0xd9, 0x47, 0xa7, 0x8d, 0xff, 0x3d, 0xe1, 0x65, 0x4f, 0x46, 0x00, 0x22
		};

		public EncryptedJsonSaver(string filename)
			: base(filename)
		{
		}

		/// <summary>
		/// Get device bytes to prevent copying save file to different device
		/// </summary>
		private static byte[] GetUniqueDeviceBytes()
		{
			var deviceIdentifier = Encoding.ASCII.GetBytes(SystemInfo.deviceUniqueIdentifier);

			return deviceIdentifier;
		}

		/// <summary>
		/// Gets encrypted write stream
		/// </summary>
		/// <returns>The write stream.</returns>
		protected override StreamWriter GetWriteStream()
		{
			var underlyingStream = new FileStream(MFilename, FileMode.Create);

			var byteGenerator = new Rfc2898DeriveBytes(GetUniqueDeviceBytes(), s_Salt, 1000);
			var random = new RNGCryptoServiceProvider();
			var key = byteGenerator.GetBytes(k_KeyLength);
			var iv = new byte[k_InitializationVectorLength];
			random.GetBytes(iv);

			var rijndael = Rijndael.Create();
			rijndael.Key = key;
			rijndael.IV = iv;

			underlyingStream.Write(iv, 0, k_InitializationVectorLength);
			var encryptedStream = new CryptoStream(underlyingStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write);

			return new StreamWriter(encryptedStream);
		}

		/// <summary>
		/// Gets decrypted read stream
		/// </summary>
		/// <returns>The read stream.</returns>
		protected override StreamReader GetReadStream()
		{
			var underlyingStream = new FileStream(MFilename, FileMode.Open);

			var byteGenerator = new Rfc2898DeriveBytes(GetUniqueDeviceBytes(), s_Salt, 1000);
			var key = byteGenerator.GetBytes(k_KeyLength);
			var iv = new byte[k_InitializationVectorLength];

			underlyingStream.Read(iv, 0, k_InitializationVectorLength);

			var rijndael = Rijndael.Create();
			rijndael.Key = key;
			rijndael.IV = iv;

			var encryptedStream = new CryptoStream(underlyingStream, rijndael.CreateDecryptor(), CryptoStreamMode.Read);

			return new StreamReader(encryptedStream);
		}
	}
}
#endif