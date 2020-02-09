using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Trigger : MonoBehaviour
{
    public RaycastHit _Hit;
    public bool isMoveNow = false;

    void OnMouseDown()
    {
        isMoveNow = true;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _Hit))
        {
            Debug.LogFormat(" Trigger Click : {0},{1},{2}", _Hit.point.x, _Hit.point.y, _Hit.point.z);
        }
        else
        {
            isMoveNow = false;
        }
    }

    void OnMouseDrag()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _Hit))
        {
            isMoveNow = true;
            Debug.LogFormat("Click : {0},{1},{2}", _Hit.point.x, _Hit.point.y, _Hit.point.z);
        }
        else
        {
            isMoveNow = false;
        }
    }

    void OnMouseUp()
    {
        isMoveNow = false;
    }
}
