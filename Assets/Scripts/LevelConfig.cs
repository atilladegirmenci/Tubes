using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TrayBase;

public class LevelConfig : MonoBehaviour
{
    public static LevelConfig Instance;

    [Header("All Levels")]
    public List<LevelData> allLevels;

    [Header("Current Level Index")]
    public int currentLevelIndex;

    private LevelData currentLevelData;

    public LevelData CurrentLevelData => currentLevelData;

    public List<ColorType> AllowedColors => currentLevelData.AllowedColors;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        LoadLevel(currentLevelIndex);
        TubeQueueManager.Instance.Initialize(); //spawn tubes
        TableManager.Instance.Initialize(currentLevelData.unlockedSlotCount);
    }

    public List<ColorType> GetColors()
    {
        return AllowedColors; 
    }
    public float GetBias()
    {
        return currentLevelData.sameColorSpawnBias;
    }
    public int GetTubeCount()
    {
        return currentLevelData.tubeCount;
    }
    public int GetUnlockedSlotCount()
    {
        return CurrentLevelData.unlockedSlotCount;
    }
    public Dictionary<TrayBase.TrayTypes, int> GetTrayWeights()
    {
        return new Dictionary<TrayTypes, int>()
        {
            { TrayTypes.Small,  currentLevelData.smallTraySpawnWeight },
            { TrayTypes.Medium, currentLevelData.mediumTraySpawnWeight },
            { TrayTypes.Large,  currentLevelData.largeTraySpawnWeight }
        };
    }
    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= allLevels.Count)
        {
            Debug.LogError($"Invalid level index: {levelIndex}");
            return;
        }

        currentLevelIndex = levelIndex;
        currentLevelData = allLevels[levelIndex];

        Debug.Log($"Level {levelIndex + 1} loaded: {currentLevelData.name}");
    }

}
