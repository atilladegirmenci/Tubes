using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TubeQueueManager : MonoBehaviour
{
    public event System.Action OnTubeQueueUpdateComplete;
    
    public static TubeQueueManager Instance;
    public TubePrefabLibrary tubeLibrary;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private int queueLength;
    public Transform[] tubePositions;
    [SerializeField] private List<ColorType> spawnableTubeColors = new List<ColorType>();
    public Dictionary<ColorType,int> colorCounts = new Dictionary<ColorType,int>();

    [Header("TUBE SPAWN BIAS SETTINGS")]
    [SerializeField] private float sameColorBias; // bias weight for back to back same colors (chance to get same color)
    [SerializeField] private float streakDecay; // gets decresed on every same color (chance decreses to get back to b same color)

    [Header("TUBES ON SCENE")]
    [SerializeField] private List<ColorType> tubeQueue = new List<ColorType>();
    [SerializeField] private List<GameObject> visibleTubeObjects = new List<GameObject>();

    [SerializeField] private TextMeshProUGUI tubeCountText;

    private bool isFillingTube = false;
    private int maxVisibleTubes;
    

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        spawnableTubeColors = LevelConfig.Instance.GetColors();
        queueLength = LevelConfig.Instance.GetTubeCount();
        sameColorBias = LevelConfig.Instance.GetBias();
        maxVisibleTubes = tubePositions.Length;
        

        for (int i = 0; i < queueLength; i++)
        {
            ColorType color = GenerateNextTubeColor(tubeQueue);

            tubeQueue.Add(color); // only colors get added to the logic queue

            if (!colorCounts.ContainsKey(color)) //count colors to spawn trays
                colorCounts[color] = 0;

            colorCounts[color]++;
        }
        tubeCountText.text = tubeQueue.Count.ToString();

        TraySpawner.Instance.Initialize(colorCounts); //spawn trays, sent color counts

        OnTubeQueueUpdateComplete += HandleTubeQueueUpdated;
        HandleTubeQueueUpdated();
    }
   
  
    private void HandleTubeQueueUpdated()
    {
        RefreshVisibleTubes();

        if (!isFillingTube && tubeQueue.Count > 0)
        {
            ColorType color = tubeQueue[0];
            if (FindAvailableTray(color) != null)
            {
                TryFillTrayFromTubeQueue(); // Uygun tray varsa sıradaki tüpü gönder
            }
        }
    }
    private void RefreshVisibleTubes()
    {
        int visibleCount = Mathf.Min(maxVisibleTubes, tubeQueue.Count);

        // 1. Var olan tube'leri yeniden konumlandır
        for (int i = 0; i < visibleTubeObjects.Count && i < visibleCount; i++)
        {
            Transform targetPos = tubePositions[i];
            StartCoroutine(ObjectMover.Instance.MoveObject(visibleTubeObjects[i], targetPos.position, 0.25f, -1f));
        }

        // 2. Eğer yeni tüp gerekiyorsa (örneğin bir tane eksilmişse)
        if (visibleTubeObjects.Count < visibleCount)
        {
            for (int i = visibleTubeObjects.Count; i < visibleCount; i++)
            {
                SpawnTubeVisual(i, tubeQueue[i]);
            }
        }

        // 3. Fazla kalanları sil (örn: tray'e eklendikten sonra sırada kalanlar)
        while (visibleTubeObjects.Count > visibleCount)
        {
            GameObject objToRemove = visibleTubeObjects[visibleTubeObjects.Count - 1];
            visibleTubeObjects.RemoveAt(visibleTubeObjects.Count - 1);
            Destroy(objToRemove);
        }

    }

    private ColorType GenerateNextTubeColor(List<ColorType> queueSoFar)
    {
        Dictionary<ColorType, float> weights = new Dictionary<ColorType, float>();

        // if no tubes full random
        if (queueSoFar.Count == 0)
            return spawnableTubeColors[UnityEngine.Random.Range(0, spawnableTubeColors.Count)];

        ColorType lastColor = queueSoFar[queueSoFar.Count - 1];
        int streak = 1;

        for (int i = queueSoFar.Count - 2; i >= 0; i--)
        {
            if (queueSoFar[i] == lastColor)
                streak++;
            else
                break;
        }

        foreach (ColorType color in spawnableTubeColors)
        {
            if (color == lastColor)
            {
                float biasWeight = sameColorBias - (streak - 1) * streakDecay;
                biasWeight = Mathf.Clamp(biasWeight, 0.1f, 10f); // negatif olmasın
                weights[color] = 1f + biasWeight;
            }
            else
            {
                weights[color] = 1.0f;
            }
        }

        // Weighted random selection
        float totalWeight = 0;
        foreach (var w in weights.Values) totalWeight += w;

        float rand = UnityEngine.Random.Range(0, totalWeight);
        float cumulative = 0f;

        foreach (var kvp in weights)
        {
            cumulative += kvp.Value;
            if (rand <= cumulative)
            {
                return kvp.Key;
            }
        }

        return spawnableTubeColors[0]; // fallback 
    }
    private void SpawnTubeVisual(int index, ColorType color)
    {
        GameObject prefab = tubeLibrary.GetPrefab(color);
        if (prefab == null) return;

        Transform pos = tubePositions[index];
        GameObject tubeGO = Instantiate(prefab, pos.position + new Vector3(0, 0, -1), Quaternion.identity, spawnParent);
        visibleTubeObjects.Add(tubeGO);
        
    }
    

    public void TryFillTrayFromTubeQueue()
    {
        if (isFillingTube || tubeQueue.Count == 0) return;

        StartCoroutine(FillTrayCoroutine());
    }
    private IEnumerator FillTrayCoroutine()
    {
        isFillingTube = true;

        ColorType color = tubeQueue[0];
        GameObject tubeGO = visibleTubeObjects[0];
        Tube tube = tubeGO.GetComponent<Tube>();

        TrayBase targetTray = FindAvailableTray(color);

        if (targetTray != null)
        {
            float zOffset = -1.9f + targetTray.currentCapacity * 0.1f;
            Transform pos = targetTray.tubePos[targetTray.currentCapacity];

            yield return StartCoroutine(ObjectMover.Instance.MoveObject(tubeGO, pos.position + new Vector3(0,0.15f,0), 0.25f, zOffset));

            tube.transform.SetParent(pos);
            tube.inTray = true;

            targetTray.currentCapacity++;
            if (targetTray.CheckIsFull())
            {
                StartCoroutine(DestroyTrayAfterDelay(targetTray.gameObject, 1.5f));
                //move blanket down
            }

            Debug.Log($"Tube of color {color} added to tray: {targetTray.name}");

            // Mantıksal kuyruk güncelle
            tubeQueue.RemoveAt(0);
            visibleTubeObjects.RemoveAt(0);

            tubeCountText.text = tubeQueue.Count.ToString();
        }
        else
        {
            Debug.Log($"No available tray for color {color}");
        }

        isFillingTube = false;
        OnTubeQueueUpdateComplete?.Invoke();
    }

    private TrayBase FindAvailableTray(ColorType color)
    {
        foreach (var slot in TableManager.Instance.slots)
        {
            if (slot.transform.childCount == 0) continue;

            TrayBase tray = slot.transform.GetChild(0).GetComponent<TrayBase>();
            if (tray != null && tray.trayColor == color && tray.currentCapacity < tray.MaxCapacity)
            {
                return tray;
            }
        }

        return null; // no matching tray found
    }
    private IEnumerator DestroyTrayAfterDelay(GameObject tray, float delay)
    {
        yield return new WaitForSeconds(delay);

        TrayBase trayBase = tray.GetComponent<TrayBase>();
        if (trayBase != null)
        {
            trayBase.RemoveFromSlot();
        }

        foreach (Transform child in tray.transform)
        {
            Destroy(child.gameObject);
        }
        
        Destroy(tray);
    }
    private void OnDestroy()
    {
        OnTubeQueueUpdateComplete -= HandleTubeQueueUpdated;
    }

}
