using UnityEngine;
using System.Collections;

public class FlyObject : MonoBehaviour
{
    float time = 0;
    protected int m_RayMaskLayer = (1 << 4) | (1 << 8);
    public GameObject missile = null;
    public GameObject target = null;

    void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down * 100, out hit, m_RayMaskLayer) == true)
        {
            this.transform.position = hit.point + new Vector3(0,5.0f,0);
        }

        //
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (mainSingleton.instance.missileType > 0)
        {
            if (time > 0.2f)
            {
                time = 0;



                if (mainSingleton.instance.missileType == 1)
                {
                    Vector3 pos = new Vector3(Random.Range(-1.0f, 1.0f), 1.0f, Random.Range(-1.0f, 1.0f));
                    var moveObject = Instantiate(missile, gameObject.transform.position + pos, Quaternion.identity);
                    var script = moveObject.AddComponent<HomingObject>();
                    script.From = this.gameObject;
                    script.Target = target;
                }
                else
                {

                    Vector3 pos = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));
                    var moveObject = Instantiate(missile, gameObject.transform.position + pos, Quaternion.identity);
                    var script = moveObject.AddComponent<HomingObject2>();
                    script.From = this.gameObject.transform;
                    script.target = target.transform;
                }

            }
        }
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + Vector3.down * 4.8f, Color.red);
        Debug.DrawLine(gameObject.transform.position + Vector3.down * 4.8f, gameObject.transform.position + Vector3.down * 4.8f + Vector3.left *2f, Color.red);
    }
}
