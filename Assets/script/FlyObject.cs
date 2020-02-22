using UnityEngine;
using System.Collections;

public class FlyObject : MonoBehaviour
{
    protected int m_RayMaskLayer = (1 << 4) | (1 << 8);
    // Use this for initialization
    void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down * 100, out hit, m_RayMaskLayer) == true)
        {
            this.transform.position = hit.point + new Vector3(0,5.0f,0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + Vector3.down * 4.8f, Color.red);
        Debug.DrawLine(gameObject.transform.position + Vector3.down * 4.8f, gameObject.transform.position + Vector3.down * 4.8f + Vector3.left *2f, Color.red);
        //Debug.DrawRay(gameObject.transform.position, Vector3.down * 4.8f, Color.red);
    }
}
