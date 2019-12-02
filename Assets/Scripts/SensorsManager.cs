using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorsManager : MonoBehaviour
{
    public AndroidLightSensorPluginScript androidLight;
    
    private float _lux = -1;
    public float lux 
    {
        get {return _lux;}
        private set 
        {
             if(value != _lux)
             {
                _lux = value;
                if(onLuxChange != null)
                    onLuxChange(_lux);
             }
        }
    }

    public delegate void luxChangeDelegate(float lux);
    public static event luxChangeDelegate onLuxChange;

    void FixedUpdate()
    {
        lux = GetLux();
    }

    private float GetLux()
    {
        float lux = 0;

        #if UNITY_ANDROID
            lux = androidLight.getLux();
        #endif

        #if UNITY_IPHONE
        
        #endif

        return lux;
    }

}
