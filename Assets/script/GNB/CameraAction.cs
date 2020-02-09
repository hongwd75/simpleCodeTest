using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAction : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Player;
    Vector3 CameraLocation;
    void Start()
    {
        CameraLocation = transform.position - Player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = CameraLocation+ Player.transform.position;
    }
}
