using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;

public class SharingManager : MonoBehaviour
{

    #region Singleton

    public static SharingManager instance;

    void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("More than 1 instance of SharingManager exists!");
            Destroy(this.gameObject);
        }

        instance = this;
    }

    #endregion

    public void ShareCard(Rect rect)
    {
        Debug.Log(rect.position);
        Sharing.ShareScreenshot(
            rect.x, 
            rect.y, 
            rect.width, 
            rect.height, 
            "Monster Card", 
            "", 
            "");
    }

}
