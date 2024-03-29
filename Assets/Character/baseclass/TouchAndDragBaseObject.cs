﻿using UnityEngine;
using System.Collections;

public class TouchAndDragBaseObject : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    protected int      m_RayMaskLayer = (1 << 4) | (1 << 8);
    protected Collider m_Collider;
    protected IEnumerator moveCoroutine = null;
    public bool m_isDragDrop = false;

    /// <summary>
    /// 위치 초기화
    /// </summary>
    protected virtual void InitPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100.0f, m_RayMaskLayer) == true)
        {
            transform.position = new Vector3(hit.point.x, getYPositionNormal(hit.point.y), hit.point.z);
        }
    }       // 초기 위치값 설정

    // 자동 이동을 위한 함수
    public virtual void OnMoveTo(Vector3 pos) { }
    public virtual IEnumerator MoveToPosition(Vector3 movepos) { yield return null; }

    public virtual IEnumerator MoveToAccel(Vector3 _speed)
    {
        float speed = 1.2f;
        Vector3 targetPos = transform.position + _speed;
        float dis = 0.0f;
        do
        {
            speed += 0.4f;
            dis = Vector3.Distance(transform.position, targetPos);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * (Time.deltaTime));
            yield return null;
        } while (dis >= 0.01f);
    }

    public void runSmooth(Vector3 _speed)
    {
        StopCoroutine("MoveToAccel");
        StartCoroutine(MoveToAccel(_speed));
    }


    /// <summary>
    ///  void start() 최 상단에 호출 되도록 한다.
    /// </summary>
    protected void baseInit()
    {
        m_Collider = GetComponent<Collider>();
        InitPosition();
    }

    protected virtual float getYPositionNormal(float land_Y)
    {
        return land_Y;// + (m_Collider.bounds.size.y / 2.0f);
    }
    protected virtual float getYPositionDrag(float land_Y)
    {
        return land_Y + m_Collider.bounds.size.y;
    }
    // 터치 이벤트 
    /// <summary>
    /// 
    /// </summary>
    public virtual void OnTouchBegin(Vector3 pos,Vector3 ObejctRealPosition)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
    }

    public virtual void OnTouchMove(Vector3 oldpos, Vector3 newpos, Vector3 ObejctRealPosition)
    {
        OnDraging(newpos, ObejctRealPosition);
    }

    public virtual void OnTouchEnd(Vector3 pos,Vector3 ObejctRealPosition)
    {
    }

    public virtual void OnTouchCancel(Vector3 pos)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void OnDragStart(Vector3 pos)
    {

    }
    protected virtual void OnDraging(Vector3 pos, Vector3 addPos)
    {
        RaycastHit hit;
        Ray _ray = Camera.main.ScreenPointToRay(pos);
        if (Physics.Raycast(_ray, out hit, 1000.0f, m_RayMaskLayer))
        {
            transform.position = hit.point + addPos;
        }
    }

    protected virtual void OnDragEnd(Vector3 pos, Vector3 addPos)
    {
        RaycastHit hit;
        Ray _ray = Camera.main.ScreenPointToRay(pos);

        if (Physics.Raycast(_ray, out hit, 1000.0f, m_RayMaskLayer))
        {
            transform.position = hit.point + addPos;
        }
    }
}
