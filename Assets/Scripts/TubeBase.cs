using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TubeBase : MonoBehaviour
{
    
    public ColorType color;
    public bool inTray;
    public bool canPickUp;

    private void Start()
    {
        canPickUp = true;
    }
    public void PickUp()
    {
        Debug.Log($"{color} tube picked up");
    }

    public bool CanPickUp()
    {
        return canPickUp;
    }
}
