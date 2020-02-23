using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainSingleton : MonoBehaviour
{
    public static mainSingleton instance = null;
    public int Value1 { get; private set; }
    public int missileType = 0;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else if(instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        OnInit();
    }

    void OnInit()
    {
        Value1 = 1;
    }
}
