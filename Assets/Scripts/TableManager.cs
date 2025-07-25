using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    public static TableManager Instance;

    public Transform[] traySlots; // Assign 5 Transforms in Inspector
   

    private void Awake()
    {
        Instance = this;
    }


    public bool TryPlaceTrayOnTable(TrayBase tray)
    {
        for (int i = 0; i < traySlots.Length; i++)
        {
            if (traySlots[i].childCount == 0)
            {
                tray.transform.SetParent(traySlots[i]);
                tray.transform.localPosition = new Vector3(0,0,-1);
                tray.isOnTable = true;
                return true;
            }
        }

        Debug.Log("Table is full. Cannot place tray.");
        return false;
    }

    public bool IsTableFull()
    {
        foreach (Transform slot in traySlots)
        {
            if (slot.childCount == 0)
                return false; 
        }
        Debug.Log("No empty space on table!!!");
        return true; 
    }
}
