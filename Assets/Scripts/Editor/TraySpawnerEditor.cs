#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TraySpawner))]
public class TraySpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TraySpawner spawner = (TraySpawner)target;

        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Spawn Tray Grid"))
        {
            //spawner.SpawnTrayGrid();
            EditorUtility.SetDirty(spawner);
        }

        if (GUILayout.Button("Clear Spawned Trays"))
        {
            spawner.ClearSpawned();
            EditorUtility.SetDirty(spawner);
        }
    }
}
#endif