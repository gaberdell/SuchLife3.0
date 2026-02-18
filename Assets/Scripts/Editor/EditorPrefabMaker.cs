using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveablePrefabManager))]
public class EditorPrefabMaker : Editor {
    void MakePrefab(object prefab) {
        SaveablePrefabManager.CreatePrefab((string)prefab);
    }

    void AddMenuItemMenu(GenericMenu menu, string prefabName) {
        menu.AddItem(new GUIContent(prefabName), false, MakePrefab, prefabName);
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        GUIContent guiContent = new GUIContent("My Name jeff?");

        if (EditorGUILayout.DropdownButton(guiContent, FocusType.Passive)) {
            GenericMenu menu = new GenericMenu();

            SaveablePrefabManager saveObjectManager = (SaveablePrefabManager)target;
            saveObjectManager.EDITOR_SETUP();

            foreach (KeyValuePair<string, GameObject> pair in SaveablePrefabManager.StringToPrefabKey) {
                AddMenuItemMenu(menu, pair.Key);
            }

            menu.ShowAsContext();
        }
    }
}
