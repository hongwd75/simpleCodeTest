using UnityEngine;
using System.Collections;
// 열도 - 섬이 모여 있는 곳
public class Archipelago : MonoBehaviour
{

    protected Island[] _ISLANDS = null; // 섬들 모음

    public Island Island
    {
        get => default;
        set
        {
        }
    }

    // Use this for initialization
    void Start()
    {
        // 열도 데이터 로딩
    }

}
