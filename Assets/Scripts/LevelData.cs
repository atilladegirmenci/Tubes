
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;

    [Header("TUBE SPAWN SETTINGS")]
    public int tubeCount;
    public int sameColorSpawnBias;
    public List<ColorType> AllowedColors;

    [Header("TRAY SPAWN WEIGHTS")]
    public int smallTraySpawnWeight;
    public int mediumTraySpawnWeight;
    public int largeTraySpawnWeight;

    [Header("UNLOCKED TABLE SLOT COUNT")]
    public int unlockedSlotCount;
}