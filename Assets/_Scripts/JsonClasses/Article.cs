using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class Article
{
	public string title;
	public string image;
	public string description;
	public string link;

	[JsonIgnore]
	public Sprite sprite;
}