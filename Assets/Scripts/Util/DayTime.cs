using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayTime : MonoBehaviour
{
    public static int GetHours()
    {
        return System.DateTime.Now.Hour;
    }

    public static int GetMinutes()
    {
        return System.DateTime.Now.Minute;
    }

}
