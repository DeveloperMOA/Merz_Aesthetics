using Nox.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class ProfileLayer : UILayer
{
	public Color headerColor;
	public TextMeshProUGUI headerTitle;
	public Image headerImage;
	public Button backButton;
	public Text userName;
	public Text userCity;
	public Text userSpecialty;
	public Image icon;
	public Image photo;

	public override void OnScreenStart()
	{
		var user = CredentialsManager.Instance.data.user;

		userName.text = user.name + " " + user.apellido_paterno;
		userCity.text = user.ciudad.ToString();
		userSpecialty.text = user.especialidad;
	}

	public override void OnScreenUp()
	{
		headerTitle.text = "Perfil";
		headerImage.color = headerColor;
		backButton.gameObject.SetActive(true);
		backButton.onClick.RemoveAllListeners();
		backButton.onClick.AddListener(Back);

		if(ContentManager.Instance.userImage != null)
		{
			icon.enabled = false;
			photo.enabled = true;
			photo.sprite = ContentManager.Instance.userImage;
			photo.rectTransform.sizeDelta = new Vector2(photo.sprite.texture.width,photo.sprite.texture.height);
		}
		else
		{
			icon.enabled = true;
			photo.enabled = false;
		}

		ContentManager.Instance.UserImageUploaded += OnUserImageUploaded;
	}

	public override void OnScreenDown()
	{
		ContentManager.Instance.UserImageUploaded -= OnUserImageUploaded;
	}

	private void OnUserImageUploaded(Sprite sprite)
	{
		icon.enabled = false;
		photo.enabled = true;
		photo.sprite = ContentManager.Instance.userImage;
		photo.rectTransform.sizeDelta = new Vector2(photo.sprite.texture.width, photo.sprite.texture.height);
	}

	public void OnEditPhotoButton()
	{
#if UNITY_EDITOR
		ImageSelectedCallback("C:\\Users\\A01112128\\Pictures\\camera.jpg");
#elif UNITY_ANDROID || UNITY_IOS
		NativeGallery.GetImageFromGallery(ImageSelectedCallback,"","image/jpeg");
#endif

	}

	private void ImageSelectedCallback(string path)
	{
		if (path == null || path.Equals("") || !File.Exists(path))
			return;

		ContentManager.Instance.UploadImageToServer(path);
	}


}
