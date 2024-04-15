using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackNav : MonoBehaviour
{
    //将屏幕大小内的区域bake为可通过
    public void RefreshBackAccessible()
    {
        transform.position = new(Camera.main.transform.position.x, Camera.main.transform.position.y,0f);
        Vector3 tempScreen = Camera.main.ScreenToWorldPoint(new(Screen.width, Screen.height, Camera.main.transform.position.z)) - Camera.main.transform.position;
        //print("Screen " + Screen.width + " , " + Screen.height);
        //print(tempScreen);
        transform.localScale = new(tempScreen.x*2, tempScreen.y*2, 1f);
    }
}
