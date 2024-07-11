using System;
using System.IO;
using System.Text.RegularExpressions;
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

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Create Data", EditorStyles.miniButton, GUILayout.Width(200)))
            CreateData();

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        EditorGUILayout.Space(10);

        SerializedProperty property = serializedObject.GetIterator();
        property.NextVisible(true);
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
        ObjectData dataTypeInstance = GetDataTypeInstanceFromTarget();
        if (dataTypeInstance == null)
        {
            Debug.LogError("The 'data' type could not be determined.");
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

        dataTypeInstance.objectName = ToSentenceCase(target.name);

        AssetDatabase.CreateAsset(dataTypeInstance, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        data.objectReferenceValue = dataTypeInstance;
        serializedObject.ApplyModifiedProperties();
    }

    private string ToSentenceCase(string str)
    {
        return Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1]));
    }

    private string GetCurrentObjectDirectory()
    {
        string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target);
        if (!string.IsNullOrEmpty(prefabPath)) return Path.GetDirectoryName(prefabPath);

        return "Assets/Prefabs";
    }


    private ObjectData GetDataTypeInstanceFromTarget()
    {
        Type targetType = target.GetType();
        while (targetType != null && targetType != typeof(BaseObject))
        {
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(BaseObject<>))
            {
                Type dataType = targetType.GetGenericArguments()[0];
                if (typeof(ObjectData).IsAssignableFrom(dataType))
                    return (ObjectData) CreateInstance(dataType);
                return null;
            }

            targetType = targetType.BaseType;
        }

        return null;
    }
}