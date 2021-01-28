using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[Serializable]
public class Brand
{
	public string name;
	[JsonIgnore]
	public Color primaryColor;
	public SerializableColor serializableColor;
	public List<BrandItem> techniques;
	public List<BrandItem> products;

	public bool EqualsBrand(Brand brand)
	{
		return name.Equals(brand.name);
	}
}

[Serializable]
public class SerializableColor
{
	public float r;
	public float g;
	public float b;
	public float a;

	private SerializableColor()
	{

	}

	public SerializableColor(Color color)
	{
		r = color.r;
		g = color.g;
		b = color.b;
		a = color.a;
	}
}

[Serializable]
public class BrandItem
{
	public string path;
	public Hash128 version;
	public string name;

	[JsonIgnore][NonSerialized]
	public float downloadProgress;

	[JsonIgnore][NonSerialized]
	public bool isDownloading;


	public ItemContent content;

	private BrandItem()
	{
	}

	public BrandItem(string path, Hash128 version, string name)
	{
		this.path = path;
		this.version = version;
		this.name = name;
	}
}

//Class in charge of loading and unloading all content (bundles & images)
public class ContentManager : MonoBehaviour
{
	public static ContentManager Instance
	{
		get => instance;
	}

	private static ContentManager instance;

	public bool ARSupport;
	public Sprite userImage;
	public Content data;

	public Brand currentBrand;
	public BrandItem currentItem;

	public string language;
	public bool isTechnique;
	public bool isAR;
	public bool isMale;

	public AssetBundle loadedBundle;

	private List<Material> materials;
	private List<Texture> textures;
	private List<AnimationClip> clips;
	private List<AudioClip> audioClips;
	
	public bool techniquesFlag;
	public bool startTechniqueFlag;
	public bool productsFlag;
	public bool startProductFlag;

	

	public event Action<BrandItem> OnItemDownloadCompleted;
	public event Action<Sprite> UserImageUploaded;
	public event Action CorruptBundleEvent;

	public void UploadImageToServer(string path)
	{
		StartCoroutine(PostImage(path));
	}

	private IEnumerator PostImage(string path)
	{
		byte[] imageBytes = File.ReadAllBytes(path);

		var requestData = new List<IMultipartFormSection>();
		requestData.Add(new MultipartFormFileSection("image", imageBytes, path, "image/jpeg"));

		var postRequest = UnityWebRequest.Post("https://merzaestheticsclick.com/api/v1/profile", requestData);
		postRequest.SetRequestHeader("Authorization", "Bearer " + CredentialsManager.Instance.data.token);

		yield return postRequest.SendWebRequest();

		if (postRequest.isNetworkError || postRequest.isHttpError)
			MobileMessage.ShowAlert("Profile Image upload fail", postRequest.error);
		else
		{
			Texture2D texture = new Texture2D(2, 2);
			texture.LoadImage(imageBytes);
			userImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width * 0.5f, texture.height * 0.5f));
			UserImageUploaded?.Invoke(userImage);
		}
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
			if (data.brands == null || data.brands.Count == 0)
				data.brands = new List<Brand>();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void DownloadBundle(BrandItem item)
	{
		StartCoroutine(DownloadRoutine(item));
	}

	
	private IEnumerator DownloadRoutine(BrandItem item)
	{
		item.isDownloading = true;
		var bundleRequest = UnityWebRequestAssetBundle.GetAssetBundle(item.path, item.version);
		bundleRequest.SendWebRequest();

		float progressTimer = 0;
		float lastProgress = 0;
		bool progressTimeout = false;

		while (!bundleRequest.isDone)
		{
			item.downloadProgress = bundleRequest.downloadProgress;

			if(item.downloadProgress != lastProgress)
			{
				progressTimer = 0;
			} 
			else
			{
				progressTimer += Time.deltaTime;
				if(progressTimer > 20f)
				{
					progressTimeout = true;
					break;
				}
			}

			lastProgress = item.downloadProgress;
			yield return null;
		}

		if (!bundleRequest.isHttpError && !bundleRequest.isNetworkError && !progressTimeout)
			item.downloadProgress = 1f;

		yield return new WaitForSeconds(0.5f);

		item.isDownloading = false;
		if (bundleRequest.isHttpError || bundleRequest.isNetworkError || progressTimeout)
		{
			if (!progressTimeout)
				MobileMessage.ShowAlert("Asset Bundle download failed", bundleRequest.error);
			else
				MobileMessage.ShowAlert("Asset Bundle download failed", "connection timeout");
			OnItemDownloadCompleted?.Invoke(item);
		}
		else
		{
			var downloader = (DownloadHandlerAssetBundle)bundleRequest.downloadHandler;

			if(downloader.assetBundle == null)
			{
				MobileMessage.ShowAlert("Asset Bundle Corrupted", "");
				OnItemDownloadCompleted?.Invoke(item);
				CorruptBundleEvent?.Invoke();
			}
			else
			{
				downloader.assetBundle.Unload(true);
				OnItemDownloadCompleted?.Invoke(item);
			}
		}
	}

	public void UnloadCurrentBundleAndDependencies()
	{
		StartCoroutine(UnloadProcess());
	}

	private IEnumerator UnloadProcess()
	{
		if (loadedBundle == null)
			yield break;

		yield return new WaitForSeconds(1f);
		loadedBundle.Unload(true);
		materials.Clear();
		textures.Clear();
		clips.Clear();
	}

	public IEnumerator LoadAssetBundle()
	{
		Hash128 version = currentItem.version; //Might have changed in the server

		bool isCached = Caching.IsVersionCached(currentItem.path, version);//IsBundleCached(currentItem.name, version);
		if (!isCached)
		{
			MobileMessage.ShowAlert("Bundle not found", "Retry download ");
			yield break;
		}

		var filePath = currentItem.path;
		var bundleRequest = UnityWebRequestAssetBundle.GetAssetBundle(filePath, version, 0);

		yield return bundleRequest.SendWebRequest();

		if (!bundleRequest.isNetworkError && !bundleRequest.isHttpError)
		{
			loadedBundle = ((DownloadHandlerAssetBundle)bundleRequest.downloadHandler).assetBundle;
			if (loadedBundle == null)
			{
				Debug.LogWarning("Null bundle");
				yield break;
			}
			materials = loadedBundle.LoadAllAssets<Material>().ToList();
			textures = loadedBundle.LoadAllAssets<Texture>().ToList();
			clips = loadedBundle.LoadAllAssets<AnimationClip>().ToList();
			audioClips = loadedBundle.LoadAllAssets<AudioClip>().ToList();
		}
		else
			MobileMessage.ShowAlert("Bundle load fail", bundleRequest.error);
	}

	public TextAsset LoadLocalizationAsset()
	{
		if (loadedBundle == null)
			return null;
		return loadedBundle.LoadAsset<TextAsset>("loc_" + language);
	}

	public GameObject LoadNavigationBar()
	{
		if (loadedBundle == null)
			return null;
		return loadedBundle.LoadAsset<GameObject>("NavigationBar");
	}

	public GameObject InstantiateItemShowcase()
	{
		if (loadedBundle == null)
			return null;

		string prefabName = "ItemShowcase";

		if (isTechnique)
			prefabName += (isMale ? "_M" : "_F");

		prefabName += (isAR ? "_AR" : "_3D");

		var prefab = loadedBundle.LoadAsset<GameObject>(prefabName);
		return Instantiate(prefab, Vector3.zero, Quaternion.identity);
	}

	public void SaveData()
	{
		PlayerPrefs.SetString("content", JsonConvert.SerializeObject(data));
	}

	//Saving sprites is done asynchronously to avoid blocking the main thread
	public void SaveImagesToStorage(BrandItem brandItem)
	{
		if (brandItem == null || brandItem.content == null)
			return;
		string savePath = Application.persistentDataPath + "/";

		SaveSprite(brandItem.content.logo, savePath, brandItem.name + "_logo.png");
		SaveSprite(brandItem.content.productBig, savePath, brandItem.name + "_productBig.png");
		SaveSprite(brandItem.content.productSmall, savePath, brandItem.name + "_productSmall.png");
		SaveSprite(brandItem.content.techCard, savePath, brandItem.name + "_techCard.png");
		SaveSprite(brandItem.content.productsBackground, savePath, brandItem.name + "_productsBackground.png");
		SaveSprite(brandItem.content.techniquesBackground, savePath, brandItem.name + "_techniquesBackground.png");
	}

	//This would be done during splash screen so we don't care if we block the main thread
	public void LoadImagesFromStorage(BrandItem brandItem)
	{
		if (brandItem == null || brandItem.content == null)
			return;
		string savePath = Application.persistentDataPath + "/";

		brandItem.content.logo = LoadSprite(savePath + brandItem.name + "_logo.png");
		brandItem.content.productBig = LoadSprite(savePath + brandItem.name + "_productBig.png");
		brandItem.content.productSmall = LoadSprite(savePath + brandItem.name + "_productSmall.png");
		brandItem.content.techCard = LoadSprite(savePath + brandItem.name + "_techCard.png");
		brandItem.content.productsBackground = LoadSprite(savePath + brandItem.name + "_productsBackground.png");
		brandItem.content.techniquesBackground = LoadSprite(savePath + brandItem.name + "_techniquesBackground.png");

	}

	private Sprite LoadSprite(string spritePath)
	{
		Sprite sprite = null;

		if (File.Exists(spritePath))
		{
			byte[] data = File.ReadAllBytes(spritePath);
			Texture2D texture = new Texture2D(2, 2);
			texture.LoadImage(data);
			sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width * 0.5f, texture.height * 0.5f));
		}

		return sprite;
	}

	private async void SaveSprite(Sprite sprite, string path, string name)
	{
		if (sprite == null || name == null || name.Equals(""))
			return;

		using (FileStream fs = File.Open(path + name,FileMode.OpenOrCreate))
		{
			Texture2D texture = sprite.texture;
			byte[] bytes = texture.EncodeToPNG();
			await fs.WriteAsync(bytes, 0, bytes.Length);
		}
	}

	public IEnumerator GetItemImages(BrandItem brandItem, Item permission)
	{
		if(permission.logo != null && !permission.logo.Equals(""))
		{
			var imageRequest = UnityWebRequestTexture.GetTexture(permission.logo);
			yield return imageRequest.SendWebRequest();

			if (!imageRequest.isNetworkError && !imageRequest.isHttpError)
			{
				Texture2D texture = ((DownloadHandlerTexture)imageRequest.downloadHandler).texture;
				brandItem.content.logo = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width * 0.5f, texture.height * 0.5f));
			}
			else
				MobileMessage.ShowAlert("Image logo "+ brandItem.name +" download fail", imageRequest.error);

			imageRequest.Dispose();
		}


		if (permission.productBig != null && !permission.productBig.Equals(""))
		{
			var imageRequest = UnityWebRequestTexture.GetTexture(permission.productBig);
			yield return imageRequest.SendWebRequest();

			if (!imageRequest.isNetworkError && !imageRequest.isHttpError)
			{
				Texture2D texture = ((DownloadHandlerTexture)imageRequest.downloadHandler).texture;
				brandItem.content.productBig = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width * 0.5f, texture.height * 0.5f));
			}
			else
				MobileMessage.ShowAlert("Image big " + brandItem.name + " download fail", imageRequest.error);

			imageRequest.Dispose();
		}

		if (permission.productSmall != null && !permission.productSmall.Equals(""))
		{
			var imageRequest = UnityWebRequestTexture.GetTexture(permission.productSmall);
			yield return imageRequest.SendWebRequest();

			if (!imageRequest.isNetworkError && !imageRequest.isHttpError)
			{
				Texture2D texture = ((DownloadHandlerTexture)imageRequest.downloadHandler).texture;
				brandItem.content.productSmall = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width * 0.5f, texture.height * 0.5f));
			}
			else
				MobileMessage.ShowAlert("Image small " + brandItem.name + " download fail", imageRequest.error);

			imageRequest.Dispose();
		}


		if (permission.techCard != null && !permission.techCard.Equals(""))
		{
			var imageRequest = UnityWebRequestTexture.GetTexture(permission.techCard);
			yield return imageRequest.SendWebRequest();

			if (!imageRequest.isNetworkError && !imageRequest.isHttpError)
			{
				Texture2D texture = ((DownloadHandlerTexture)imageRequest.downloadHandler).texture;
				brandItem.content.techCard = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width * 0.5f, texture.height * 0.5f));
			}
			else
				MobileMessage.ShowAlert("Image card " + brandItem.name + " download fail", imageRequest.error);

			imageRequest.Dispose();
		}

		if (permission.productsBackground != null && !permission.productsBackground.Equals(""))
		{
			var imageRequest = UnityWebRequestTexture.GetTexture(permission.productsBackground);
			yield return imageRequest.SendWebRequest();

			if (!imageRequest.isNetworkError && !imageRequest.isHttpError)
			{
				Texture2D texture = ((DownloadHandlerTexture)imageRequest.downloadHandler).texture;
				brandItem.content.productsBackground = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width * 0.5f, texture.height * 0.5f));
			}
			else
				MobileMessage.ShowAlert("Image prBack " + brandItem.name + " download fail", imageRequest.error);

			imageRequest.Dispose();
		}


		if (permission.techniquesBackground != null && !permission.techniquesBackground.Equals(""))
		{
			var imageRequest = UnityWebRequestTexture.GetTexture(permission.techniquesBackground);
			yield return imageRequest.SendWebRequest();

			if (!imageRequest.isNetworkError && !imageRequest.isHttpError)
			{
				Texture2D texture = ((DownloadHandlerTexture)imageRequest.downloadHandler).texture;
				brandItem.content.techniquesBackground = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width * 0.5f, texture.height * 0.5f));
			}
			else
				MobileMessage.ShowAlert("Image techBack " + brandItem.name + " download fail", imageRequest.error);

			imageRequest.Dispose();
		}

		SaveImagesToStorage(brandItem);
	}
}
