using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UIElements;
using static TrayBase;
using static UnityEngine.UI.Image;

public class TraySpawner : MonoBehaviour
{
    public TrayPrefabLibrary trayLibrary;
    public Transform spawnParent;
    public static  TraySpawner Instance;
    private Dictionary<ColorType, int> colorCount = new Dictionary<ColorType, int>();
    [SerializeField] public int rows;
    [SerializeField] public int columns;
    [SerializeField] public Transform startPos;
    [SerializeField] public float spacingX;
    [SerializeField] public float spacingY;
    [SerializeField] public float trayWidth;
    [SerializeField] public List<ColorType> spawnableTrayColors = new List<ColorType>();
    private Dictionary<int, float> rowHeights = new Dictionary<int, float>();

    private Dictionary<TrayTypes, int> traySpawnWeights;
    private void Awake()
    {
        Instance = this;
    }
    public void Initialize(Dictionary<ColorType, int> cCounts)
    {
        colorCount = cCounts;
        spawnableTrayColors = LevelConfig.Instance.GetColors();

        traySpawnWeights = LevelConfig.Instance.GetTrayWeights();

        TrayGridManager.Instance.InitializeRows(rows,columns);
        SpawnTrayGrid_RowBased();
    }
   
    public GameObject SpawnTray(ColorType color, TrayBase.TrayTypes type, Vector2 position)
    {
        GameObject prefab = trayLibrary.GetPrefab(color, type);
        if (prefab == null) return null;

        GameObject trayGO = Instantiate(prefab, position, Quaternion.identity, spawnParent);
        return trayGO;
    }

    public void SpawnTrayGrid_RowBased()
    {
        Vector2 origin = startPos.position;
        Dictionary<ColorType, int> remainingCapacity = new Dictionary<ColorType, int>(colorCount);

        int currentRow = 0;
        int currentCol = 0;


        while (true)
        {
            // Kalan renklerden spawn yapılabilir olanlar
            List<ColorType> availableColors = new List<ColorType>();
            foreach (var kvp in remainingCapacity)
            {
                if (kvp.Value > 0)
                    availableColors.Add(kvp.Key);
            }

            // Eğer renk kalmadıysa spawn tamamdır
            if (availableColors.Count == 0)
                break;

            // Rastgele renk ve weighted tip seçimi
            var color = availableColors[Random.Range(0, availableColors.Count)];
            var type = GetWeightedRandomTrayType();
            int trayCapacity = TrayBase.GetCapacity(type);

            // Kalan kapasiteyi güncelle
            remainingCapacity[color] -= trayCapacity;
            if (remainingCapacity[color] < 0)
                remainingCapacity[color] = 0;

            // Prefab ve length alma
            GameObject prefab = trayLibrary.GetPrefab(color, type);
            TrayBase trayData = prefab.GetComponent<TrayBase>();
            float length = trayData.lenght;

            // Satırın mevcut yüksekliği yoksa 0 olarak başlat
            if (!rowHeights.ContainsKey(currentCol))
                rowHeights[currentCol] = 0f;

            float posX = origin.x + currentCol * (trayWidth + spacingX) + trayWidth / 2f;
            float posY = origin.y - rowHeights[currentCol] - length / 2f;
            // Tray spawn et
            GameObject trayGO = SpawnTray(color, type, new Vector2(posX, posY));
            TrayBase tray = trayGO.GetComponent<TrayBase>();

            // TrayGridManager’a ekle (row-based ekleme fonksiyonu)
            TrayGridManager.Instance.AddTrayToGrid_RowBased(tray, currentRow, currentCol);

            rowHeights[currentCol] += length + spacingY;

            currentCol++;
            if (currentCol >= columns)
            {
                currentRow++;
                currentCol=0;
            }
        }
    }
    private int GetSpawnWeight(TrayBase.TrayTypes type)
    {
        return traySpawnWeights.TryGetValue(type, out int weight) ? weight : 0;
    }
    private TrayBase.TrayTypes GetWeightedRandomTrayType()
    {
        List<TrayBase.TrayTypes> pool = new List<TrayBase.TrayTypes>();

        foreach (TrayBase.TrayTypes type in System.Enum.GetValues(typeof(TrayBase.TrayTypes)))
        {
            int weight = GetSpawnWeight(type);
            for (int i = 0; i < weight; i++)
            {
                pool.Add(type);
            }
        }

        return pool[Random.Range(0, pool.Count)];
    }
    public void SetCanSelect(bool set)
    {
        foreach(Transform child in spawnParent)
        {
            TrayBase comp = child.GetComponent<TrayBase>();
            if(comp != null)
            {
                comp.canSelect = set;
            }
           
        }
    }
    
    public void ClearSpawned()
    {
        for (int i = spawnParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(spawnParent.GetChild(i).gameObject);
        }
    }
   
}
