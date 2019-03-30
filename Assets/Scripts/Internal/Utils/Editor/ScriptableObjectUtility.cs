using UnityEditor;
using UnityEngine;

public static class ScriptableObjectUtility
{
	public static void CreateAsset<T>() where T : ScriptableObject
	{
		CreateAsset<T>(typeof(T).FullName);
	}

	public static void CreateAsset<T>(string specifiedName) where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T>();
		AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath($"{AssetDatabase.GetAssetPath(Selection.activeObject)}/{specifiedName}.asset"));
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}
}
