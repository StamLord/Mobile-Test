/* 
 * Date: 15.08.2016
 * Author: Etienne Frank
 * Website: www.etiennefrank.com
 * Mail: mail@etiennefrank.com
 * Info: Feel free to use it in any project. It would be pleased to mention the author name. 
 * 	     By the way the author can be hired.
 *
*/

using UnityEngine;
using System;
using System.Collections;

public class AndroidLightSensorPluginScript : MonoBehaviour {
	private AndroidJavaObject activityContext = null;
	private AndroidJavaObject jo = null;
	AndroidJavaClass activityClass = null;

	void Start () {
		#if UNITY_ANDROID
		activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");

		jo = new AndroidJavaObject("com.etiennefrank.lightsensorlib.LightSensorLib");
		jo.Call("init", activityContext);
		#endif
	}

	public float getLux() {
		#if UNITY_ANDROID
		return jo.Call<float>("getLux");
		#endif
	}
}
