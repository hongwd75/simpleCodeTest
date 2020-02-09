using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterObject : TouchAndDragBaseObject
{
    // Start is called before the first frame update
    MoveChar _mC;


    /// <summary>
    /// 위치로 이동시킨다.
    /// </summary>

    public override void OnMoveTo(Vector3 pos)
    {
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        Vector3 xreposition = new Vector3(pos.x, getYPositionNormal(pos.y) , pos.z);
        moveCoroutine = MoveToPosition(xreposition);
        StartCoroutine(moveCoroutine);
    }

    public override IEnumerator MoveToPosition(Vector3 movepos)
    {
        while(_mC.Run(movepos) == true)
        {
            yield return new WaitForSeconds(0.03f);
        }


        //OnTouchEnd(Camera.main.WorldToScreenPoint(movepos));
        OnTouchEnd(Camera.main.ViewportToScreenPoint(movepos));
        yield return null;
    }



    /// <summary>
    /// 
    /// </summary>
    static int colorindex = 0;
    void Start()
    {
        baseInit();

        _mC = gameObject.AddComponent<MoveChar>();

        
        if(colorindex == 0) GetComponent<MeshRenderer>().material.color = Color.red;
        if (colorindex == 1) GetComponent<MeshRenderer>().material.color = Color.green;
        if (colorindex == 2) GetComponent<MeshRenderer>().material.color = Color.blue;
        colorindex++;
    }

    public override void OnTouchEnd(Vector3 pos)
    {
        RaycastHit hit;
        Ray _ray = Camera.main.ScreenPointToRay(pos);

        if (Physics.Raycast(_ray, out hit, 1000.0f, m_RayMaskLayer))
        {
            float updateYpos = 0;
            if(hit.transform.gameObject.layer == 4) // 바다면
            {
                updateYpos = m_Collider.bounds.size.y / 4.0f;
                StartCoroutine("Swimming");
            }
            else { StopCoroutine("Swimming"); }
            transform.position = new Vector3(hit.point.x, getYPositionNormal(hit.point.y)- updateYpos, hit.point.z);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");
        if (collision.gameObject.layer == 4)
        {
            StartCoroutine("Swimming");
        } else if (collision.gameObject.layer == 8)
        {
            StopCoroutine("Swimming");
        } else
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
        }


    }

    private void OnCollisionExit(Collision collision)
    {
        StopCoroutine("Swimming");
    }
    private void OnCollisionStay(Collision collision)
    {
    }

    private IEnumerator Swimming()
    {
        float limitRange = m_Collider.bounds.size.y / 4.0f;
        float translatepos = 0;
        Vector3 updown = new Vector3(0, limitRange, 0);
        while (true)
        {
            while (true)
            {
                this.transform.Translate(updown * translatepos * 0.05f);
                translatepos -= Time.deltaTime;
                yield return null;
                if (translatepos <= -0.5f)
                {
                    translatepos = -0.5f;
                    break;
                }
            }

            while (true)
            {
                this.transform.Translate(updown * translatepos * 0.05f);
                translatepos += Time.deltaTime;
                yield return null;
                if (translatepos >= 0.5f)
                {
                    translatepos = 0.5f;
                    break;
                }
            }
        }
    }
}
