using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timestamp {

    // Returns total seconds since epoch
    public static double GetTimeStamp(){
        TimeSpan span = DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0));
        return span.TotalSeconds;
    }

    // Returns the time passed (in seconds) since given timestamp (in seconds since epoch)
    public static double GetSecondsSince(double timestamp){

        TimeSpan stamp = TimeSpan.FromSeconds(timestamp);
        DateTime sinceEpoch = new DateTime(1970,1,1,0,0,0) + stamp;
        TimeSpan span = DateTime.Now.Subtract(sinceEpoch);

        return span.TotalSeconds;
    }
}