using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TrayPrefabEntry
{
    public ColorType color;
    public TrayBase.TrayTypes type;
    public GameObject prefab;
}

[CreateAssetMenu(fileName = "TrayPrefabLibrary", menuName = "Tray System/Prefab Library")]
public class TrayPrefabLibrary : ScriptableObject
{
    public TrayPrefabEntry[] entries;

    public GameObject GetPrefab(ColorType color, TrayBase.TrayTypes type)
    {
        foreach (var e in entries)
        {
            if (e.color == color && e.type == type)
                return e.prefab;
        }

        Debug.LogWarning($"Prefab could not find: {color} - {type}");
        return null;
    }
}
