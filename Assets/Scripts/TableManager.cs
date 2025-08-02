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

    public IEnumerator TryPlaceTrayOnTable(TrayBase tray, System.Action<bool> onComplete = null)
    {
        for (int i = 0; i < traySlots.Length; i++)
        {
            if (traySlots[i].childCount == 0)
            {
                tray.transform.SetParent(null); 
                tray.gameObject.transform.position += new Vector3(0, 0, -3); //bumps the tray abowe grid
                yield return StartCoroutine(ObjectMover.Instance.MoveObject(
                    tray.gameObject,
                    traySlots[i].position + new Vector3(0,0,-3),
                    0.4f,
                    -1f 
                ));

                tray.transform.SetParent(traySlots[i]);
                tray.transform.localPosition = new Vector3(0, 0, -1); // secure it
                tray.isOnTable = true;

                onComplete?.Invoke(true);
                yield break;
            }
        }
        onComplete?.Invoke(false);
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
