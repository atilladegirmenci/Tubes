using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tray : TrayBase
{
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnSelected()
    {
        Debug.Log($"tray clicked color: {trayColor}, type: {trayType}, capacity: {MaxCapacity}");

        if(!isOnTable && !TableManager.Instance.IsTableFull())
        {
            bool placed = TableManager.Instance.TryPlaceTrayOnTable(this);
            if (placed) Debug.Log($"{trayColor} , {trayType} placed on table ");
            TrayGridManager.Instance.RemoveTrayFromGrid(this);
            TubeQueueManager.Instance.TryFillTrayFromTubeQueue();
        }
        

       
    }
    public override bool CanSelect()
    {
        return true;
    }
}
