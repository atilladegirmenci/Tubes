using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tray : TrayBase
{
    
    public override void OnSelected()
    {
        Debug.Log($"tray clicked color: {trayColor}, type: {trayType}, capacity: {MaxCapacity}");

        if(!isOnTable && !TableManager.Instance.IsTableFull())
        {
            StartCoroutine(TableManager.Instance.TryPlaceTrayOnTable(this, success => {
                if (success)
                {
                    Debug.Log($"Tray {trayColor}, {trayType} placed successfully!");
                    TubeQueueManager.Instance.TryFillTrayFromTubeQueue();
                    TrayGridManager.Instance.RemoveTrayFromGrid(this);
                }
                else
                {
                    Debug.Log("tray could not place.");
                }
            }));
           
        }
        

       
    }
    public override bool CanSelect()
    {
        return true;
    }
}
