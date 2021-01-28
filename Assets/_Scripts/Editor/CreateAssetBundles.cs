using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{
	[MenuItem("Assets/AssetBundles/Build Android Bundles")]
	private static void BuildAllAndroidAssetBundles()
	{
		string assetBundleDirectory = "Assets/BundleFiles/Android";
		if(!Directory.Exists(assetBundleDirectory))
			Directory.CreateDirectory(assetBundleDirectory);
		BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None,BuildTarget.Android);
		Debug.Log("Android Bundles Created");
	}

	[MenuItem("Assets/AssetBundles/Build iOS Bundles")]
	private static void BuillAlliOSAssetBundles()
    {
		string assetBundleDirectory = "Assets/BundleFiles/iOS";
		if (!Directory.Exists(assetBundleDirectory))
			Directory.CreateDirectory(assetBundleDirectory);
		BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.iOS);
		Debug.Log("iOS Bundles Created");
	}
}
