using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControlSystem : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 _touchPosition;
    Vector3 _startTouchPosition;
    Vector3 _ObjectToLand;
    public  float topSpeed = 10;

    [Tooltip("Defines the maximum time between two taps to make it double tap")]
    [SerializeField] private float tapThreshold = 0.25f;
    private float tapDuration = 0.0f;
    private int tapCount = 0;

    GlobalTickTimer _test = new GlobalTickTimer();
    TouchAndDragBaseObject _selectedObjectScpit = null;
    TouchAndDragBaseObject _lastselectedObjectScpit = null;
    private void Start()
    {
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
            tapDuration = Time.time;
            OnTouchHandle(0, Input.mousePosition, TouchPhase.Began);
        } else if (Input.GetMouseButtonUp(0) == true && _selectedObjectScpit != null)
        {
            Vector3 newpos = Input.mousePosition;
            //if (_touchPosition != newpos)
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
                _ObjectToLand = Vector3.zero;
                _touchPosition = touchPosition;
                _startTouchPosition = touchPosition;

                tapCount++;
                RaycastHit hit;
                Ray _ray = Camera.main.ScreenPointToRay(touchPosition);

                if (Physics.Raycast(_ray, out hit, 100, 1 << 10 | 1 << 9))
                {
                    RaycastHit hit2;
                    if (Physics.Raycast(_ray, out hit2, 100, (1 << 4) | (1 << 8)) == false)
                    {
                            return;
                    }

                    if (hit.transform.gameObject.tag.CompareTo("Player") == 0)
                    {
                        _selectedObjectScpit = hit.transform.gameObject.GetComponent<TouchAndDragBaseObject>();
                        if (_selectedObjectScpit != null)
                        {
                                Vector3 incomingVec = hit.point - _selectedObjectScpit.transform.position;
                             Debug.LogFormat("HIT:{0} - NORMAL {1} = Y:{2}", hit.point, hit.normal, incomingVec);
                            _ObjectToLand = hit.point - incomingVec - hit2.point;



                            _selectedObjectScpit.m_isDragDrop = false;
                            _selectedObjectScpit.OnTouchBegin(touchPosition, _ObjectToLand);
                            }
                    }
                    else
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
               if (_selectedObjectScpit.m_isDragDrop == false && touchPosition != _startTouchPosition)
                {
                    Debug.Log("Moved");
                    tapCount = 0;
                    _selectedObjectScpit.m_isDragDrop = true;
                } else
               if (_selectedObjectScpit.m_isDragDrop == true)
                {
                    _selectedObjectScpit.OnTouchMove(_touchPosition, touchPosition, _ObjectToLand);
                }

                
                } break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
            {
                    float timevalue = Time.time - tapDuration;
                    if (timevalue >= 0.0f && timevalue <= tapThreshold)
                    {
                        if (tapCount == 1)
                        {
                            StartCoroutine("singleOrDouble");
                        }
                    } else if(tapCount == 1 && timevalue > tapThreshold * 2)
                    {
                        Debug.Log("LongTouch");
                    }
                    float h = Input.GetAxis("Mouse X");
                    float v = Input.GetAxis("Mouse Y");

                    Debug.LogFormat("Ended.MOUSEDELTA: {0},{1}", h, v);
                    Vector3 nn = Camera.main.transform.eulerAngles.normalized;
                    Vector3 addPos = new Vector3(v, 0, h) * 4.0f;

                    _lastselectedObjectScpit = _selectedObjectScpit;
                    _selectedObjectScpit.OnTouchEnd(touchPosition, _ObjectToLand);
                    _selectedObjectScpit.m_isDragDrop = false;
                    _selectedObjectScpit.runSmooth(addPos);
                    _selectedObjectScpit = null;
                    
            } break;
        }
    }

    IEnumerator singleOrDouble()
    {
        yield return new WaitForSeconds(tapThreshold);
        if (tapCount == 1)
        {
            Debug.Log("Single");
        }
        else if (tapCount > 1)
        {
            //this coroutine has been called twice. We should stop the next one here otherwise we get two double tap
            StopCoroutine("singleOrDouble");
            Debug.Log("Double");
        }
        tapCount = 0;

    }
}
