using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemContent
{
	public string prettyName;
	public string slogan;
	public List<string> languages;

	[JsonIgnore]
	public Sprite techCard;
	[JsonIgnore]
	public Sprite productBig;
	[JsonIgnore]
	public Sprite productSmall;
	[JsonIgnore]
	public Sprite logo;

	[JsonIgnore]
	public Sprite productsBackground;

	[JsonIgnore]
	public Sprite techniquesBackground;

	public ItemContent()
	{
		languages = new List<string>();
	}
}
