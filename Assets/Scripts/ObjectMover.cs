using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    public static ObjectMover Instance;

    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator MoveObject(GameObject tubeGO, Vector3 targetPos, float duration = 0.25f, float zOffset = 0f)
    {
        Vector3 finalPos = targetPos + new Vector3(0, 0, zOffset);
        Vector3 startPos = tubeGO.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            tubeGO.transform.position = Vector3.Lerp(startPos, finalPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        tubeGO.transform.position = finalPos;
    }
}
