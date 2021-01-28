using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalizeTextMesh : MonoBehaviour
{
	public string key;
	public TextMeshPro text;

	public void Localize(Dictionary<string,string> table)
	{
		if (table.ContainsKey(key))
			text.text = table[key];
		else
			text.text = "Key not found: " + key;
	}
}
