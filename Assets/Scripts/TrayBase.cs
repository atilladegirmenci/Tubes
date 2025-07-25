using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TrayBase;

public abstract class TrayBase : MonoBehaviour
{
    //public enum TrayColors
    //{
    //    Blue,
    //    Green,
    //    Purple,
    //    Red,
    //    White,
    //    Yellow
    //}
    public enum TrayTypes
    {
        Small,
        Medium,
        Large
    }
    public ColorType trayColor;
    public TrayTypes trayType;
    public int MaxCapacity => GetCapacity(trayType);
    public int currentCapacity = 0;

    public bool isOnTable;

    public Transform[] tubePos;

    public int gridCol;
    public int gridRow;
    public float lenght => GetLenght(trayType);
    private static readonly Dictionary<TrayTypes, float> TrayLengths = new Dictionary<TrayTypes, float>()
    {
        { TrayTypes.Small, 0.83f },
        { TrayTypes.Medium, 1.14f },
        { TrayTypes.Large, 1.72f }
    };
    private static readonly Dictionary<TrayTypes, int> Capacities = new Dictionary<TrayTypes, int>()
    {
        { TrayTypes.Small, 4 },
        { TrayTypes.Medium, 6 },
        { TrayTypes.Large, 10 }
    };

    public virtual void OnSelected()
    {
        
    }
    public  bool CheckIsFull()
    {
        return currentCapacity >= MaxCapacity ? true: false; 
    }
    public static float GetLenght(TrayTypes type)
    {
        return TrayLengths.TryGetValue(type, out float length) ? length : 0;
    }
    public static int GetCapacity(TrayTypes type)
    {
        return Capacities.TryGetValue(type, out int capacity) ? capacity : 0;
    }
    public virtual bool CanSelect()
    {
        return true;
    }
}
