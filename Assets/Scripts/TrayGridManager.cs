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

    // Satır sayısı ve sütun sayısı alıyoruz
    public void InitializeRows(int rowCount, int colCount)
    {
        trayRows.Clear();
        columns = colCount;

        // rowCount kadar satır listesi oluştur
        for (int i = 0; i < rowCount; i++)
        {
            trayRows.Add(new List<TrayBase>());

            // Her satır için sütun sayısı kadar boş (null) yer aç
            for (int j = 0; j < colCount; j++)
            {
                trayRows[i].Add(null);
            }
        }
    }

    public void AddTrayToGrid_RowBased(TrayBase tray, int row, int col)
    {
        if (row < 0 || row >= trayRows.Count)
        {
            Debug.LogError($"Invalid row index: {row}");
            return;
        }

        if (col < 0 || col >= columns)
        {
            Debug.LogError($"Invalid column index: {col}");
            return;
        }

        tray.gridRow = row;
        tray.gridCol = col;

        trayRows[row][col] = tray;
    }

    public void RemoveTrayFromGrid(TrayBase tray)
    {
        int row = tray.gridRow;
        int col = tray.gridCol;

        if (row < 0 || row >= trayRows.Count || col < 0 || col >= columns)
        {
            Debug.LogWarning("TrayGridManager: Invalid tray position!");
            return;
        }

        float deltaY = tray.lenght + spacingY;

        // Silinen tray'i null yap
        trayRows[row][col] = null;

        // Aynı sütundaki alt satırlardaki trayleri yukarı kaydır
        for (int r = row + 1; r < trayRows.Count; r++)
        {
            TrayBase t = trayRows[r][col];
            if (t == null) continue;

            // Satır bilgisini bir yukarı çek
            t.gridRow = r - 1;

            // Grid'de yukarı taşı
            trayRows[r - 1][col] = t;
            trayRows[r][col] = null;

            // Transform pozisyonunu yukarı kaydır
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
//public class TrayGridManager : MonoBehaviour
//{
//    public static TrayGridManager Instance; 
//    public float spacingY = 0.01f;

//    private List<List<TrayBase>> trayRows = new List<List<TrayBase>>();
//    private int columns;

//    private void Awake()
//    {
//        Instance = this;
//    }

//    public void InitializeRows(int rowCount, int colCount)
//    {
//        trayRows.Clear();

//        for (int i = 0; i < colCount; i++)
//        {
//            trayRows.Add(new List<TrayBase>());
//        }
//    }

//    //public void AddTrayToGrid_ColumnBased(TrayBase tray, int row)
//    //{
//    //    if (row < 0 || row >= trayRows.Count)
//    //    {
//    //        Debug.LogError($"Invalid row index: {row}");
//    //        return;
//    //    }

//    //    int col = trayRows[row].Count;

//    //    tray.gridRow = row;
//    //    tray.gridCol = col;

//    //    trayRows[row].Add(tray);
//    //}
//    public void AddTrayToGrid_RowBased(TrayBase tray, int row, int col)
//    {
//        if (row < 0 || row >= trayRows.Count)
//        {
//            Debug.LogError($"Invalid row index: {row}");
//            return;
//        }

//        // Eğer satırda yeterli kolon yoksa ekle
//        while (trayRows[row].Count <= col)
//        {
//            trayRows[row].Add(null);
//        }

//        tray.gridRow = row;
//        tray.gridCol = col;

//        trayRows[row][col] = tray;
//    }

//    public void RemoveTrayFromGrid(TrayBase tray)
//    {
//        int row = tray.gridRow;
//        int col = tray.gridCol;

//        if (row < 0 || row >= trayRows.Count || col < 0 || col >= trayRows[row].Count)
//        {
//            Debug.LogWarning("TrayGridManager: Invalid tray position!");
//            return;
//        }

//        // 1. Silinecek tray’in yüksekliği referans alınır
//        float deltaY = tray.lenght + spacingY;

//        // 2. Listedeki yerinden çıkar
//        trayRows[row].RemoveAt(col);

//        // 3. Silinenin altındaki (col > silinen) tray'leri yukarı kaydır
//        for (int i = row; i < trayRows[row].Count; i++)
//        {
//            TrayBase t = trayRows[row][i];

//            // Sıra bilgisini güncelle
//            t.gridCol = i;

//            // Y pozisyonunu sadece sabit bir miktar kadar yükselt
//            Vector3 newPos = t.transform.position;
//            newPos.y += deltaY;

//            StartCoroutine(MoveTrayToPosition(t.transform, newPos));
//        }
//    }


//    private IEnumerator MoveTrayToPosition(Transform t, Vector3 target)
//    {
//        Vector3 start = t.position;
//        float elapsed = 0f;
//        float duration = 0.2f;

//        while (elapsed < duration)
//        {
//            t.position = Vector3.Lerp(start, target, elapsed / duration);
//            elapsed += Time.deltaTime;
//            yield return null;
//        }

//        t.position = target;
//    }
//}
