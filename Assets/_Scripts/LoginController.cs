using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nox.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
	public InputField email;
	public InputField pass;
	public Button loginButton;
	public Button forgetPasswordButton;
	private UIHandler loginHandler;

	public InputField recoveryEmail;
	public Button recoveryButton;
	public Text recoveryText;

	private class LoginData
	{
		public string email;
		public string password;

		public LoginData(string email, string password)
		{
			this.email = email;
			this.password = password;
		}
	}

	public void Initialize(UIHandler _loginHandler)
	{
		loginHandler = _loginHandler;
	}

	public void OnLoginButton()
	{
		StartCoroutine(RequestLogin());

	}

	public void OnPasswordRecoveryButton()
	{
		loginHandler.Show<PasswordRecoveryLayer>();
	}

	public void OnSendPasswordRecovery()
	{
		StartCoroutine(RecoveryRequest());
	}

	private IEnumerator RecoveryRequest()
	{
		string url = "https://merzaestheticsclick.com/api/v1/auth/reset";
		var form = new Dictionary<string, string>();
		form.Add("email", recoveryEmail.text);

		UnityWebRequest recoveryRequest = new UnityWebRequest();
		recoveryRequest.url = url;
		recoveryRequest.method = UnityWebRequest.kHttpVerbPOST;


		recoveryButton.interactable = false;
		recoveryEmail.interactable = false;
		yield return recoveryRequest.SendWebRequest();

		recoveryButton.interactable = true;
		recoveryEmail.interactable = true;

		recoveryText.text = "Solicitud enviada. Si la dirección existe en la base de datos, se le enviará un correo para reestablecer su contraseña";
	}

	private void ToggleControls(bool value)
	{
		loginButton.interactable = value;
		forgetPasswordButton.interactable = value;
		email.interactable = value;
		pass.interactable = value;
	}

	private IEnumerator RequestLogin()
	{
		ToggleControls(false);
		var form = new Dictionary<string, string>();
		form.Add("email", email.text);
		form.Add("password", pass.text);

		//Authentication
		UnityWebRequest loginRequest = new UnityWebRequest();
		loginRequest.url = "https://merzaestheticsclick.com/api/v1/auth/login";
		loginRequest.method = UnityWebRequest.kHttpVerbPOST;
		loginRequest.downloadHandler = new DownloadHandlerBuffer();
		loginRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new LoginData(email.text, pass.text))));
		loginRequest.SetRequestHeader("Content-Type", "application/json");

		yield return loginRequest.SendWebRequest();

		if (loginRequest.isNetworkError || loginRequest.isHttpError)
		{
			Debug.LogError(loginRequest.error);
			ToggleControls(true);
			yield break;
		}
			

		string response = loginRequest.downloadHandler.text;
		JObject json = JObject.Parse(response);

		var error = json.Property("error");

		if (error != null)
		{
			print(response);
			ToggleControls(true);
		}
		else
		{
			print(response);
			CredentialsManager.Instance.SetCredentials(JsonConvert.DeserializeObject<Credentials>(response));

			//Get user profile image
			var avatarPath = CredentialsManager.Instance.data.user.avatar;
			if (avatarPath != null && !avatarPath.Equals(""))
			{
				var profileImageRequest = UnityWebRequestTexture.GetTexture(avatarPath);

				yield return profileImageRequest.SendWebRequest();

				if (profileImageRequest.isNetworkError || profileImageRequest.isHttpError)
					MobileMessage.ShowAlert("Profile Image download fail", profileImageRequest.error);
				else
				{
					Texture2D texture = ((DownloadHandlerTexture)profileImageRequest.downloadHandler).texture;
					ContentManager.Instance.userImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width * 0.5f, texture.height * 0.5f));
				}
			}

			string url = "https://merzaestheticsclick.com/api/v1/permissions";

#if UNITY_IOS
			url = "https://merzaestheticsclick.com/api/v1/permissions?platform=ios";
#endif

			ContentManager.Instance.data = new Content();

			//Get permissions
			UnityWebRequest request = UnityWebRequest.Get(url);
			request.SetRequestHeader("Authorization", "Bearer " + CredentialsManager.Instance.data.token);
			yield return request.SendWebRequest();

			print(request.downloadHandler.text);
			Permissions permissions = new Permissions();
			permissions.items = JsonConvert.DeserializeObject<List<Item>>(request.downloadHandler.text);
			request.Dispose();

			//Generate brand info and get asset bundles
			foreach(var p in permissions.items)
			{
				string[] parts = p.brandName.Split('_');
				string brandName = parts[0].First().ToString().ToUpper() + parts[0].Substring(1);

				Brand brand = null;
				foreach(var b in ContentManager.Instance.data.brands)
				{
					if(b.name.Equals(brandName))
					{
						brand = b;
						break;
					}
				}

				if(brand == null)
				{
					brand = new Brand();
					brand.name = brandName;
					brand.techniques = new List<BrandItem>();
					brand.products = new List<BrandItem>();
					if(!ColorUtility.TryParseHtmlString(p.color, out brand.primaryColor))
						ColorUtility.TryParseHtmlString("#EC7405",out brand.primaryColor);
					brand.serializableColor = new SerializableColor(brand.primaryColor);
					ContentManager.Instance.data.brands.Add(brand);
				}
	
					var newItem = new BrandItem(p.bundle, Hash128.Parse(p.version), p.brandName);

					string contentAsset = p.itemContent;

					if(contentAsset != null && !contentAsset.Equals(""))
					{
						newItem.content = JsonConvert.DeserializeObject<ItemContent>(contentAsset);
						yield return StartCoroutine(ContentManager.Instance.GetItemImages(newItem,p));
					}
					else
					{
						newItem.content = new ItemContent();
					}

					if (parts[1].Equals("t"))
						brand.techniques.Add(newItem);
					else
						brand.products.Add(newItem);
				
			}

			//Get news
			request = UnityWebRequest.Get("https://merzaestheticsclick.com/api/v1/articles");
			request.SetRequestHeader("Authorization", "Bearer " + CredentialsManager.Instance.data.token);
			yield return request.SendWebRequest();

			print(request.downloadHandler.text);
			News news = new News();
			news.articles = JsonConvert.DeserializeObject<List<Article>>(request.downloadHandler.text);

			ContentManager.Instance.data.news = news;
			ContentManager.Instance.data.permissions = permissions;

			request.Dispose();

			foreach(var article in news.articles)
			{
				request = UnityWebRequestTexture.GetTexture(article.image);
				yield return request.SendWebRequest();
				if (request.isNetworkError || request.isHttpError)
					continue;

				Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
				article.sprite = Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(texture.width*0.5f,texture.height*0.5f));
			}

			//Save all offline info
			CredentialsManager.Instance.SaveCredentials();
			ContentManager.Instance.SaveData();
			PlayerPrefs.Save();

			SceneManager.LoadScene("HubScene");
		}
	}
}
