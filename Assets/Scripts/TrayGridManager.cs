using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayGridManager : MonoBehaviour
{
    public static TrayGridManager Instance; 
    public float spacingY = 0.01f;

    private List<List<TrayBase>> trayRows = new List<List<TrayBase>>();
    private int columns;

    private void Awake()
    {
        Instance = this;
    }

    public void InitializeRows(int rowCount)
    {
        trayRows.Clear();

        for (int i = 0; i < rowCount; i++)
        {
            trayRows.Add(new List<TrayBase>());
        }
    }

    public void AddTrayToGrid(TrayBase tray, int row, int col)
    {
        tray.gridRow = row;  // yukarıdan aşağı
        tray.gridCol = col;  // soldan sağa

        trayRows[row].Insert(col, tray);
    }
    public void AddTrayToGrid_ColumnBased(TrayBase tray, int row)
    {
        if (row < 0 || row >= trayRows.Count)
        {
            Debug.LogError($"Invalid row index: {row}");
            return;
        }

        int col = trayRows[row].Count;

        tray.gridRow = row;
        tray.gridCol = col;

        trayRows[row].Add(tray);
    }


    public void RemoveTrayFromGrid(TrayBase tray)
    {
        int row = tray.gridRow;
        int col = tray.gridCol;

        if (row < 0 || row >= trayRows.Count || col < 0 || col >= trayRows[row].Count)
        {
            Debug.LogWarning("TrayGridManager: Invalid tray position!");
            return;
        }

        // 1. Silinecek tray’in yüksekliği referans alınır
        float deltaY = tray.lenght + spacingY;

        // 2. Listedeki yerinden çıkar
        trayRows[row].RemoveAt(col);

        // 3. Silinenin altındaki (col > silinen) tray'leri yukarı kaydır
        for (int i = col; i < trayRows[row].Count; i++)
        {
            TrayBase t = trayRows[row][i];

            // Sıra bilgisini güncelle
            t.gridCol = i;

            // Y pozisyonunu sadece sabit bir miktar kadar yükselt
            Vector3 newPos = t.transform.position;
            newPos.y += deltaY;

            StartCoroutine(MoveTrayToPosition(t.transform, newPos));
        }
    }


    private IEnumerator MoveTrayToPosition(Transform t, Vector3 target)
    {
        Vector3 start = t.position;
        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            t.position = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        t.position = target;
    }
}
