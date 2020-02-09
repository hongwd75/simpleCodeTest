using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControlSystem : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 _touchPosition;
    Vector3 _startTouchPosition;

    public  float topSpeed = 10;

    GlobalTickTimer _test = new GlobalTickTimer();
    TouchAndDragBaseObject _selectedObjectScpit = null;

    private void OnMouseDrag()
    {
        Debug.Log("OnMouseDrag");
    }
    protected bool _FindEmptyTile(int _x, int _y, int count)
    {
        int maxy = _y + (count + 1);
        int maxx = _x + (count + 1);

        for (int y = _y - count; y < maxy; y++)
        {
            int xplus = 1;
            if (y == _y - count || y == maxy - 1)
            {
                xplus = 1;
            }
            else
            {
                xplus = count+count;
            }

            for (int x = _x - count; x < maxx; x += xplus)
            {
                Debug.LogFormat("C:{0}, XX : {1},{2}", count, x, y);
            }
        }

        Debug.Log("***");
        return false;
    }

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            _FindEmptyTile(10, 10, i);
        }

        for (int a = 0; a < 10; a++)
        {
            _MYOBJECT_ pl = new _MYOBJECT_();
            _test.Add((a / 5) * 1.0f, pl, true, true);
         }
    }
    // Update is called once per frame
    void Update()
    {
        _test.UpdateList();
        if (Input.GetMouseButtonDown(0) == true)
        {
            OnTouchHandle(0, Input.mousePosition, TouchPhase.Began);
        } else if (Input.GetMouseButtonUp(0) == true && _selectedObjectScpit != null)
        {
            Vector3 newpos = Input.mousePosition;
            if (_touchPosition != newpos)
            {
                OnTouchHandle(0, Input.mousePosition, TouchPhase.Ended);
                _touchPosition = newpos;
            }
        } else if(Input.GetMouseButton(0) == true && _selectedObjectScpit != null)  /* check Drag */
        {
            OnTouchHandle(0, Input.mousePosition, TouchPhase.Moved);
        }
    }

    /// <summary>
    /// 터치 처리 함수
    /// </summary>
    /// <param name="fingerID"></param>
    /// <param name="touchPosition"></param>
    /// <param name="touchPhase"></param>
    private void OnTouchHandle(int fingerID,Vector3 touchPosition, TouchPhase touchPhase)
    {
        switch(touchPhase)
        {
            case TouchPhase.Began:
            {
                _touchPosition = touchPosition;
                _startTouchPosition = touchPosition;

                RaycastHit hit;
                Ray _ray = Camera.main.ScreenPointToRay(touchPosition);

                if (Physics.Raycast(_ray, out hit))
                {
                    if (hit.transform.gameObject.tag.CompareTo("Player") == 0)
                    {
                        _selectedObjectScpit = hit.transform.gameObject.GetComponent<TouchAndDragBaseObject>();
                        if (_selectedObjectScpit != null)
                        {
                            _selectedObjectScpit.m_isDragDrop = false;
                            _selectedObjectScpit.OnTouchBegin(touchPosition);
                        }
                    } else
                    {
                            
                            GameObject[] charlist = GameObject.FindGameObjectsWithTag("Player");

                            foreach (GameObject charc in charlist)
                            {
                                TouchAndDragBaseObject component = charc.GetComponent<TouchAndDragBaseObject>();
                                component.OnMoveTo(hit.point);
                                break;
                            }
                    }
                }
            } break;
            case TouchPhase.Moved:
            {
                _selectedObjectScpit.m_isDragDrop = true;
                _selectedObjectScpit.OnTouchMove(_touchPosition, touchPosition);
                    float h = Input.GetAxis("Mouse X");
                    float v = Input.GetAxis("Mouse Y");

                    Debug.LogFormat("Moved.MOUSEDELTA: {0},{1}", h, v);
                } break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
            {
                    float h = Input.GetAxis("Mouse X");
                    float v = Input.GetAxis("Mouse Y");

                    Debug.LogFormat("Ended.MOUSEDELTA: {0},{1}", h, v);

                    _selectedObjectScpit.OnTouchEnd(touchPosition);
                    _selectedObjectScpit.m_isDragDrop = false;
                    _selectedObjectScpit = null;
            } break;
        }
    }
}
