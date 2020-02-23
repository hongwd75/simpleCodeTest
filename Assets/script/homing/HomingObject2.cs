using UnityEngine;
using System.Collections;

public class HomingObject2 : MonoBehaviour
{
    protected Transform Fire = null;
    internal Transform From = null;
    internal Transform target;
    private float rocketTurnSpeed;
    private float rocketSpeed;
    private float randomOffset;

    private float timerSinceLaunch_Contor;
    private float objectLifeTimerValue;

    // Use this for initialization
    void Start()
    {
        rocketTurnSpeed = 90.0f;
        rocketSpeed = 18.0f;
        randomOffset = 0.0f;

        timerSinceLaunch_Contor = 0;
        objectLifeTimerValue = 10;

        transform.rotation = Quaternion.LookRotation(transform.position - From.position);
        Fire = gameObject.transform.Find("fire");
    }

    // Update is called once per frame
    void Update()
    {
        timerSinceLaunch_Contor += Time.deltaTime;

        if (target != null)
        {
            if (timerSinceLaunch_Contor > 1)
            {
                float checkNearObject = (target.position - transform.position).magnitude;
                if (checkNearObject >= 15)
                {
                    randomOffset = 8.0f;
                    rocketTurnSpeed = 270.0f;
                }
                else
                {
                    randomOffset = 0.0f;
                    //if close to target
                    if (checkNearObject < 15)
                    {
                        rocketTurnSpeed = 360.0f;
                    }

                    if(checkNearObject < 1.0f)
                    {
                        Destroy(transform.gameObject, 1);
                        return;
                    }
                }
            }

            Vector3 direction = target.position - transform.position + Random.insideUnitSphere * randomOffset;
            //if(direction.y > 0) direction.y =0;
            direction.Normalize();

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), rocketTurnSpeed * Time.deltaTime);
            transform.Translate(Vector3.forward * rocketSpeed * Time.deltaTime);
            
            Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + gameObject.transform.forward * 2.8f, Color.red);
        }

        if (timerSinceLaunch_Contor > objectLifeTimerValue)
        {
            Destroy(transform.gameObject, 1);
        }
    }
}
