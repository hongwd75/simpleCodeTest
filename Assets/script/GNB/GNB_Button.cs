using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GNB_Button : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("click object name : " + gameObject.name);

        GameObject title = GameObject.Find("TitleText");
        if(title != null)
        {
            title.GetComponent<Text>().text = gameObject.name;
        }
       
    }
}
