using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TubePrefabEntry
{
    public ColorType color;
   // public TrayBase.TrayTypes type;
    public GameObject prefab;
}

[CreateAssetMenu(fileName = "TubePrefabLibrary", menuName = "Tube System/Prefab Library")]
public class TubePrefabLibrary : ScriptableObject
{
    public TubePrefabEntry[] entries;

    public GameObject GetPrefab(ColorType color /*TrayBase.TrayTypes type*/)
    {
        foreach (var e in entries)
        {
            if (e.color == color/* && e.type == type*/)
                return e.prefab;
        }

        Debug.LogWarning($"Prefab could not find: {color}");
        return null;
    }
}