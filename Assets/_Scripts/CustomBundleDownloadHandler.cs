using System.IO;
using UnityEngine.Networking;

public class CustomBundleDownloadHandler : DownloadHandlerScript
{
	private string _targetFilePath;
	private Stream _fileStream;

	public CustomBundleDownloadHandler(string targetFilePath)
		: base(new byte[4096]) // use pre-allocated buffer for better performance
	{
		_targetFilePath = targetFilePath;
	}

	protected override bool ReceiveData(byte[] data, int dataLength)
	{
		// create or open target file
		if (_fileStream == null)
		{
			_fileStream = File.OpenWrite(_targetFilePath);
		}

		// write data to file
		_fileStream.Write(data, 0, dataLength);

		return true;
	}

	protected override void CompleteContent()
	{
		// close and save
		_fileStream.Close();
	}
}
