using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ContentManager))]
public class ContentManagerInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if(GUILayout.Button("Delete Prefs & Cache"))
		{
			PlayerPrefs.DeleteAll();
			Caching.ClearCache();
			Debug.LogWarning("All player prefs & cache deleted");
		}
	}
}
