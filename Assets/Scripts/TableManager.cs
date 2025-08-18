using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TableSlots
{
    public bool isOccupied;
    public bool isLocked;
    public Transform transform;
    public GameObject lockedOverlay;
    
}
public class TableManager : MonoBehaviour
{
    public static TableManager Instance;

    public TableSlots[] slots;
    
    private void Awake()
    {
        Instance = this;
    }
    public void Initialize(int slotCount)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                Debug.Log("null slot");
                continue;
            }
            
            if (slots[i].transform != null)
            {
                slots[i].isLocked = i >= slotCount;
                slots[i].lockedOverlay.SetActive(slots[i].isLocked);
            }
        }
    }
    public IEnumerator TryPlaceTrayOnTable(TrayBase tray, System.Action<bool> onComplete = null)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].transform.GetComponentInChildren<TrayBase>() == null && !slots[i].isLocked)
            {
                TraySpawner.Instance.SetCanSelect(false);

                tray.transform.SetParent(null); 
                tray.gameObject.transform.position += new Vector3(0, 0, -3); //bumps the tray abowe grid

                yield return StartCoroutine(ObjectMover.Instance.MoveObject(
                    tray.gameObject,
                    slots[i].transform.position + new Vector3(0,0,-3),
                    0.4f,
                    -1f 
                ));

                tray.isOnTable = true;
                tray.SetSlot(slots[i]);

                onComplete?.Invoke(true);
                yield break;
            }
        }
        onComplete?.Invoke(false);
    }

    public TableSlots GetFirstEmptySlot()
    {
        foreach (var slot in slots)
        {
            if (slot.transform.childCount == 0)
                return slot;
        }
        return null;

    }

    public bool IsTableFull()
    {
        foreach (TableSlots slot in slots)
        {
            if (slot.transform != null && slot.transform.GetComponentInChildren<TrayBase>() == null)
                return false;
        }
        Debug.Log("No empty space on table!!!");
        return true; 
    }
}
