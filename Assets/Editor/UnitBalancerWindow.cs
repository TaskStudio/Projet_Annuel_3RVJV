using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitBalancerWindow : EditorWindow
{
    private Vector2 scrollPos;
    private Dictionary<GameObject, ObjectData> allyUnits = new Dictionary<GameObject, ObjectData>();
    private Dictionary<GameObject, ObjectData> enemyUnits = new Dictionary<GameObject, ObjectData>();

    [MenuItem("Window/Unit Balancer")]
    public static void ShowWindow()
    {
        GetWindow<UnitBalancerWindow>("Unit Balancer");
    }

    private void OnEnable()
    {
        LoadUnits();
    }

    private void LoadUnits()
    {
        string allyPath = "Assets/Prefabs/Units/Allies/";
        string enemyPath = "Assets/Prefabs/Units/Enemies/";

        LoadUnitData(allyPath, allyUnits);
        LoadUnitData(enemyPath, enemyUnits);
    }

    private void LoadUnitData(string path, Dictionary<GameObject, ObjectData> unitDict)
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { path });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab != null)
            {
                BaseObject baseObject = prefab.GetComponent<BaseObject>();
                if (baseObject != null && baseObject.Data != null)
                {
                    unitDict[prefab] = baseObject.Data;
                }
            }
        }
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.Label("Allies", EditorStyles.boldLabel);
        DisplayUnits(allyUnits, Color.green);

        GUILayout.Space(20);

        // Draw a horizontal line as a separator
        DrawHorizontalLine(Color.gray);

        GUILayout.Space(20);

        GUILayout.Label("Enemies", EditorStyles.boldLabel);
        DisplayUnits(enemyUnits, Color.red);

        EditorGUILayout.EndScrollView();
    }

    private void DrawHorizontalLine(Color color)
    {
        var rect = EditorGUILayout.GetControlRect(false, 2);
        EditorGUI.DrawRect(rect, color);
    }

    private void DisplayUnits(Dictionary<GameObject, ObjectData> units, Color backgroundColor)
    {
        float windowWidth = position.width;
        int columns = Mathf.Max(1, (int)(windowWidth / 350)); // Calculate the number of columns based on window width

        int count = 0;
        bool horizontalGroupStarted = false;

        foreach (var kvp in units)
        {
            if (kvp.Key == null || kvp.Value == null) continue;

            if (count % columns == 0)
            {
                if (horizontalGroupStarted)
                {
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.BeginHorizontal();
                horizontalGroupStarted = true;
            }

            GUI.backgroundColor = backgroundColor;
            EditorGUILayout.BeginVertical("box", GUILayout.Width(300), GUILayout.ExpandWidth(true));
            GUILayout.Space(10);

            EditorGUILayout.LabelField("Prefab Name:", kvp.Key.name);

            SerializedObject serializedData = new SerializedObject(kvp.Value);
            serializedData.Update();
            SerializedProperty property = serializedData.GetIterator();
            property.NextVisible(true);
            while (property.NextVisible(false))
            {
                EditorGUILayout.PropertyField(property, true);
            }

            serializedData.ApplyModifiedProperties();

            GUILayout.Space(10);
            if (GUILayout.Button("Save", GUILayout.Width(100), GUILayout.Height(20)))
            {
                EditorUtility.SetDirty(kvp.Value);
                AssetDatabase.SaveAssets();
            }

            GUILayout.Space(10);
            EditorGUILayout.EndVertical();

            count++;
        }

        if (horizontalGroupStarted)
        {
            EditorGUILayout.EndHorizontal();
        }
    }
}
