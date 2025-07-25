using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class TraySpawner : MonoBehaviour
{
    public TrayPrefabLibrary trayLibrary;
    public Transform spawnParent;

    [SerializeField] public int rows;
    [SerializeField] public int columns;
    [SerializeField] public Transform startPos;
    [SerializeField] public float spacingX;
    [SerializeField] public float spacingY;
    [SerializeField] public float trayWidth;
    [SerializeField] public List<ColorType> spawnableTrayColors = new List<ColorType>();

    private void Start()
    {
        TrayGridManager.Instance.InitializeRows(columns);
        SpawnTrayGrid();
    }
    public GameObject SpawnTray(ColorType color, TrayBase.TrayTypes type, Vector2 position)
    {
        GameObject prefab = trayLibrary.GetPrefab(color, type);
        if (prefab == null) return null;

        GameObject trayGO = Instantiate(prefab, position, Quaternion.identity, spawnParent);
        return trayGO;
    }
   
    public void SpawnTrayGrid()
    {
        Vector2 currentOrigin = startPos.position;

        for(int row = 0 ; row < rows; row++)
        {
            float currentX = currentOrigin.x;
            currentOrigin.y = startPos.position.y;

            for (int col = 0; col < columns; col++)
            {
                
                var color = spawnableTrayColors[UnityEngine.Random.Range(0, spawnableTrayColors.Count)];
                var type = GetRandomEnum<TrayBase.TrayTypes>();

                GameObject prefab = trayLibrary.GetPrefab(color,type);
                TrayBase trayData = prefab.GetComponent<TrayBase>();

                if(trayData == null)
                {
                    Debug.LogWarning($"null prefab or traybase: color= {color}, type= {type}");
                }

                float length = trayData.lenght;

                Vector3 position = new Vector2(currentOrigin.x + trayWidth / 2f,  currentOrigin.y - length / 2f);

                GameObject trayGO = SpawnTray(color, type, position);
                TrayBase tray = trayGO.GetComponent<TrayBase>();
                TrayGridManager.Instance.AddTrayToGrid_ColumnBased(tray, row);

                currentOrigin.y -= (length + spacingY);
            }

            currentOrigin.x += (trayWidth + spacingX);
        }
    }
   
    
    public void ClearSpawned()
    {
        for (int i = spawnParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(spawnParent.GetChild(i).gameObject);
        }
    }
    private T GetRandomEnum<T>()
    {
        System.Array values = System.Enum.GetValues(typeof(T));
        return (T)values.GetValue(Random.Range(0, values.Length));
    }
}
