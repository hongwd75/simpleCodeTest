using UnityEngine;
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
        return land_Y + (m_Collider.bounds.size.y / 2.0f);
    }
    protected virtual float getYPositionDrag(float land_Y)
    {
        return land_Y + m_Collider.bounds.size.y;
    }
    // 터치 이벤트 
    /// <summary>
    /// 
    /// </summary>
    public virtual void OnTouchBegin(Vector3 pos)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
    }

    public virtual void OnTouchMove(Vector3 oldpos, Vector3 newpos)
    {
        OnDraging(newpos);
    }

    public virtual void OnTouchEnd(Vector3 pos)
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
    protected virtual void OnDraging(Vector3 pos)
    {
        RaycastHit hit;
        Ray _ray = Camera.main.ScreenPointToRay(pos);
        if (Physics.Raycast(_ray, out hit, 1000.0f, m_RayMaskLayer))
        {
            transform.position = new Vector3(hit.point.x, getYPositionDrag(hit.point.y), hit.point.z);
        }
    }

    protected virtual void OnDragEnd(Vector3 pos)
    {
        RaycastHit hit;
        Ray _ray = Camera.main.ScreenPointToRay(pos);

        if (Physics.Raycast(_ray, out hit, 1000.0f, m_RayMaskLayer))
        {
            //hit.transform.gameObject.layer
            transform.position = new Vector3(hit.point.x, getYPositionNormal(hit.point.y), hit.point.z);
        }
    }
}
