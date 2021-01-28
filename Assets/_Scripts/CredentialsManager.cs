using Newtonsoft.Json;
using UnityEngine;

public class CredentialsManager : MonoBehaviour
{
	public static CredentialsManager Instance
	{
		get => instance;
	}

	private static CredentialsManager instance;

	public Credentials data;
	

	private void Awake()
	{
		if(instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
	
	public void SetCredentials(Credentials credentials)
	{
		data = credentials;
	}

	public void SaveCredentials()
	{
		PlayerPrefs.SetString("token", data.token);
		PlayerPrefs.SetString("expire", data.expires_at);
		PlayerPrefs.SetString("user", JsonConvert.SerializeObject(data.user));
	}
}
