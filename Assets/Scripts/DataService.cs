using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class  DataService 
{
    
    public const string HOST = "http://localhost:5000";

    public static IEnumerator GetRequest()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(HOST)) {
            Debug.Log("Fetching from server...");
            yield return request.SendWebRequest();
            
            if (!request.isNetworkError)
            {
                byte[] result = request.downloadHandler.data;
                string resJson = System.Text.Encoding.Default.GetString(result);
                User user = JsonUtility.FromJson<User>(resJson);
                GameManager.user = user;
            }
            else
            {
                Debug.Log("Error fetching from server");
            }
        }
    }

    public static IEnumerator PutRequest(byte[] data) {
        using(UnityWebRequest request =UnityWebRequest.Put(HOST, data)){
            yield return request.SendWebRequest();

            if(request.isNetworkError)
                Debug.Log("Error sending data to server");
            else
                Debug.Log("Data sent successfully");
            
        }
    }
}
