using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;

public class EM_Initializer : MonoBehaviour
{
    private void Awake()
    {
        if (RuntimeManager.IsInitialized())
        {
            RuntimeManager.Init();
        }
    }
}
