using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeQueueManager : MonoBehaviour
{
    public static TubeQueueManager Instance;
    public TubePrefabLibrary tubeLibrary;
    [SerializeField] private Transform spawnParent;
    [SerializeField] public int queueLength;
    public Transform[] tubePositions;
    [SerializeField] public List<ColorType> spawnableTubeColors = new List<ColorType>();
    [Header("TUBES ON SCENE")]
    [SerializeField] private List<ColorType> tubeQueue = new List<ColorType>();
    [SerializeField] private List<GameObject> visibleTubeObjects = new List<GameObject>();
   
    
    private int maxVisibleTubes;
    

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        maxVisibleTubes = tubePositions.Length;

        for (int i = 0; i < queueLength; i++)
        {
            var color = spawnableTubeColors[UnityEngine.Random.Range(0, spawnableTubeColors.Count)];
            
            tubeQueue.Add(color); // only colors get added to the logic queue
        }

        // initial spawn
        RefreshVisibleTubes();
    }
    private void RefreshVisibleTubes()
    {
        // Clear all previous visible tubes
        foreach (GameObject tubeGO in visibleTubeObjects)
        {
            if(tubeGO.GetComponent<Tube>().inTray == false)
            {
                Destroy(tubeGO);
            }
        }
        visibleTubeObjects.Clear();

        // Show only up to maxVisibleTubes
        int visibleCount = Mathf.Min(maxVisibleTubes, tubeQueue.Count);
        for (int i = 0; i < visibleCount; i++)
        {
            SpawnTubeVisual(i, tubeQueue[i]);
        }

        TryFillTrayFromTubeQueue();
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
        if (tubeQueue.Count == 0) return;

        ColorType color = tubeQueue[0]; 
        GameObject tubeGO = visibleTubeObjects[0];
        Tube tube = tubeGO.GetComponent<Tube>();

        TrayBase targetTray = FindAvailableTray(color);

        if (targetTray != null)
        {
            float zOffset = -1.9f + targetTray.currentCapacity * 0.1f;
            Transform pos = targetTray.tubePos[targetTray.currentCapacity];
            tube.transform.position = pos.position + new Vector3(0,0.1f,zOffset);
            tube.transform.SetParent(pos);
            targetTray.currentCapacity++;

            if (targetTray.CheckIsFull())
            {
                StartCoroutine(DestroyTrayAfterDelay(targetTray.gameObject, 1.5f));
            }

            tube.inTray = true;
            Debug.Log($"Tube of color {color} added to tray: {targetTray.name}");

            // Optionally update tray visuals here (e.g. fill bar)

            tubeQueue.RemoveAt(0); // move queue forward
            RefreshVisibleTubes();
        }
        else
        {
            Debug.Log($"No available tray for color {color}");
        }
    }
   
    private TrayBase FindAvailableTray(ColorType color)
    {
        foreach (Transform slot in TableManager.Instance.traySlots)
        {
            if (slot.childCount == 0) continue;

            TrayBase tray = slot.GetChild(0).GetComponent<TrayBase>();
            if (tray != null && tray.trayColor == color && tray.currentCapacity < tray.MaxCapacity)
            {
                return tray;
            }
        }

        return null; // no matching tray found
    }

    private T GetRandomEnum<T>()
    {
        System.Array values = System.Enum.GetValues(typeof(T));
        return (T)values.GetValue(Random.Range(0, values.Length));
    }
    private IEnumerator DestroyTrayAfterDelay(GameObject tray, float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (Transform child in tray.transform)
        {
            Destroy(child.gameObject);
        }

        Destroy(tray);
    }

}
