using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseObject), true)]
[CanEditMultipleObjects]
public class CustomBaseObjectEditor : Editor
{
    private SerializedProperty data;

    private void OnEnable()
    {
        data = serializedObject.FindProperty("data");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Object Data", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(data);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Create Data", EditorStyles.miniButton, GUILayout.Width(200)))
            CreateData();

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        SerializedProperty property = serializedObject.GetIterator();
        property.NextVisible(true); // Skip the script reference
        do
        {
            if (property.name == "data")
                continue;

            EditorGUILayout.PropertyField(property, true);
        } while (property.NextVisible(false));

        serializedObject.ApplyModifiedProperties();
    }

    private void CreateData()
    {
        Type dataType = GetDataTypeFromTarget();
        if (dataType == null)
        {
            Debug.LogError("The 'data' type could not be determined.");
            return;
        }

        var newData = CreateInstance(dataType);
        if (newData == null)
        {
            Debug.LogError($"Could not create an instance of {dataType}.");
            return;
        }

        string directoryPath = GetCurrentObjectDirectory();
        if (string.IsNullOrEmpty(directoryPath))
        {
            Debug.LogError("The directory path could not be determined.");
            return;
        }

        string assetPath = Path.Combine(directoryPath, target.name + "Data.asset");
        assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

        // Save the ScriptableObject as an asset
        AssetDatabase.CreateAsset(newData, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Assign the created ScriptableObject to the data field
        data.objectReferenceValue = newData;
        serializedObject.ApplyModifiedProperties();
    }

    private string GetCurrentObjectDirectory()
    {
        string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target);
        if (!string.IsNullOrEmpty(prefabPath)) return Path.GetDirectoryName(prefabPath);

        return "Assets/Prefabs";
    }


    private Type GetDataTypeFromTarget()
    {
        Type targetType = target.GetType();
        while (targetType != null && targetType != typeof(BaseObject))
        {
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(BaseObject<>))
                return targetType.GetGenericArguments()[0];
            targetType = targetType.BaseType;
        }

        return null;
    }
}