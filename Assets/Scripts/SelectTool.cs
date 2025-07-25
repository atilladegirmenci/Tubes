using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTool : MonoBehaviour
{
    private Camera _camera;
    private bool _active = true;
    private TrayBase selectableItem;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (!_active)
            return;

        if (Input.GetMouseButtonDown(0))
            CreateObjectSelectRay();
    }



    private void CreateObjectSelectRay()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider == null) return;

        selectableItem = CheckIsComponent(hit.collider);

        selectableItem?.OnSelected();

    }

    private static TrayBase CheckIsComponent(Component hit)
    {
        if (hit.TryGetComponent(out TrayBase tray) && tray.CanSelect())
            return tray;

        Debug.Log("no selectable item clicked");
        return null;

        //if (!hit.TryGetComponent(out TrayBase newDragAndDropObject))
        //    return null;

        //if (!newDragAndDropObject.CanSelect())
        //    newDragAndDropObject = null;

        //if (newDragAndDropObject != null)
        //    return newDragAndDropObject;
        //else return null;
    }

    private Ray GenerateMouseRay()
    {
        var mousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _camera.farClipPlane);
        var mousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _camera.nearClipPlane);
        var mousePosF = _camera.ScreenToWorldPoint(mousePosFar);
        var mousePosN = _camera.ScreenToWorldPoint(mousePosNear);

        Debug.DrawRay(mousePosN, mousePosF - mousePosN, Color.green);

        var mr = new Ray(mousePosN, mousePosF - mousePosN);
        return mr;
    }
}
