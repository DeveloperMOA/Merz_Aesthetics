using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class SplashScreenController : MonoBehaviour
{
	private IEnumerator Start()
	{
		if(ARSession.state == ARSessionState.None || ARSession.state == ARSessionState.CheckingAvailability)
			yield return ARSession.CheckAvailability();

		if (ARSession.state == ARSessionState.Unsupported)
			ContentManager.Instance.ARSupport = false;
		else
			ContentManager.Instance.ARSupport = true;


		yield return new WaitForSeconds(1f);


		if (PlayerPrefs.HasKey("token"))
		{
			string expire = PlayerPrefs.GetString("expire");
			DateTime expireDate = DateTime.Parse(expire);

			if (expireDate > DateTime.Now)
			{
				var data = new Credentials();
				data.expires_at = expire;
				data.token = PlayerPrefs.GetString("token");
				data.user = JsonConvert.DeserializeObject<User>(PlayerPrefs.GetString("user"));

				CredentialsManager.Instance.data = data;

				//Get user profile picture
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


				ContentManager.Instance.data = new Content();
				Content offlineContent = null;
				if(PlayerPrefs.HasKey("content"))
				{
					try
					{
						offlineContent = JsonConvert.DeserializeObject<Content>(PlayerPrefs.GetString("content"));
					}
					catch(Exception)
					{
						PlayerPrefs.DeleteAll();
						PlayerPrefs.Save();
						SceneManager.LoadScene(0);
					}
				}


				//Get permissions
				string url = "https://merzaestheticsclick.com/api/v1/permissions";
				UnityWebRequest request = UnityWebRequest.Get(url);
#if UNITY_IOS || UNITY_EDITOR
				request.SetRequestHeader("platform", "ios");
#endif
				request.SetRequestHeader("Authorization", "Bearer " + CredentialsManager.Instance.data.token);
				yield return request.SendWebRequest();

				if(request.isNetworkError || request.isHttpError)
				{
					MobileMessage.ShowAlert("Network Error","Using local content");
					//Try to load offline information
					if(offlineContent != null)
					{
						ContentManager.Instance.data.permissions = offlineContent.permissions;
						ContentManager.Instance.data.brands = offlineContent.brands;

						foreach (var b in ContentManager.Instance.data.brands)
							b.primaryColor = new Color(b.serializableColor.r,b.serializableColor.g,b.serializableColor.b,b.serializableColor.a);

						foreach(var brand in ContentManager.Instance.data.brands)
						{
							//Try to load from storage if there is no internet connection
							foreach (var item in brand.products)
								ContentManager.Instance.LoadImagesFromStorage(item);
							foreach (var item in brand.techniques)
								ContentManager.Instance.LoadImagesFromStorage(item);
						}
					}
				}
				else
				{
					print(request.downloadHandler.text);
					Permissions permissions = new Permissions();
					permissions.items = JsonConvert.DeserializeObject<List<Item>>(request.downloadHandler.text);
					request.Dispose();

					foreach (var p in permissions.items)
					{
						string[] parts = p.brandName.Split('_');
						string brandName = parts[0].First().ToString().ToUpper() + parts[0].Substring(1);

						Brand brand = null;
						foreach (var b in ContentManager.Instance.data.brands)
						{
							if (b.name.Equals(brandName))
							{
								brand = b;
								break;
							}
						}

						//Generate brands from items
						if (brand == null)
						{
							brand = new Brand();
							brand.name = brandName;
							brand.techniques = new List<BrandItem>();
							brand.products = new List<BrandItem>();
							if (!ColorUtility.TryParseHtmlString(p.color, out brand.primaryColor))
								ColorUtility.TryParseHtmlString("#EC7405", out brand.primaryColor);
							brand.serializableColor = new SerializableColor(brand.primaryColor);

							ContentManager.Instance.data.brands.Add(brand);
						}

						var newItem = new BrandItem(p.bundle, Hash128.Parse(p.version), p.brandName);

						string contentAsset = p.itemContent;

						if (contentAsset != null)
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
					//Assign downloaded permissions
					ContentManager.Instance.data.permissions = permissions;
				}

				//Get news
				request = UnityWebRequest.Get("https://merzaestheticsclick.com/api/v1/articles");
				request.SetRequestHeader("Authorization", "Bearer " + CredentialsManager.Instance.data.token);
				yield return request.SendWebRequest();

				if(request.isNetworkError || request.isHttpError)
				{
					if (offlineContent != null)
					{
						ContentManager.Instance.data.news = offlineContent.news;
					}
				}
				else
				{
					print(request.downloadHandler.text);
					News news = new News();
					news.articles = JsonConvert.DeserializeObject<List<Article>>(request.downloadHandler.text);

					request.Dispose();

					foreach (var article in news.articles)
					{
						request = UnityWebRequestTexture.GetTexture(article.image);
						yield return request.SendWebRequest();
						if (request.isNetworkError || request.isHttpError)
							continue;

						Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
						article.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width * 0.5f, texture.height * 0.5f));
					}

					ContentManager.Instance.data.news = news;
				}

				//Save new downloaded content
				ContentManager.Instance.SaveData();
				PlayerPrefs.Save();

				SceneManager.LoadScene("HubScene", LoadSceneMode.Single);
			}
			else
			{
				//Erase all data since token is no longer valid
				PlayerPrefs.DeleteAll();
				Caching.ClearCache();
				SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
			}
		}
		else
		{
			SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
		}

	}
}
