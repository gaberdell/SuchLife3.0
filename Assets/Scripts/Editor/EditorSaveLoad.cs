using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveObjectsManager))]
public class EditorSaveLoad : Editor
{
    static string path = "Save/Load Path";

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        SaveObjectsManager saveObjectManager = (SaveObjectsManager)target;

        GUILayout.BeginHorizontal();

        path = EditorGUILayout.TextField(path);

        if (GUILayout.Button("Save Scene")) {
            Debug.Log(path);
        }

        if (GUILayout.Button("Load Scene")) {
            Debug.Log(path);
        }

        GUILayout.EndHorizontal();
    }
}
