using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class  DataService 
{
    public static bool isLoggedin;
    public static bool tryingToLogin;
    public const string HOST = "http://localhost:5000/api/";

    public static IEnumerator Login(string username, string password)
    {
        // Build JSON object and convert it to bytes
        string json = "{" + String.Format("\"username\":\"{0}\",\"password\":\"{1}\"", username, password) + "}";
        byte[] userData = System.Text.Encoding.Default.GetBytes(json);

        // Create a POST request because Unity apperantly cannot
        UnityWebRequest request = new UnityWebRequest(HOST+"login", "POST");
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(userData);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("Trying to Login...");
        tryingToLogin = true;
        yield return request.SendWebRequest();
        
        if (!request.isNetworkError)
        {
            byte[] result = request.downloadHandler.data;
            string resJson = System.Text.Encoding.Default.GetString(result);
            User user = JsonUtility.FromJson<User>(resJson);
            GameManager.instance.user = user;
            isLoggedin = true;
            
            // Remembers credentials
            PlayerPrefs.SetString("username", username);
            PlayerPrefs.SetString("password", password);

            Debug.Log("Successfully logged in");
        }
        else
        {
            Debug.Log("Error fetching from server");
        }

        tryingToLogin = false;
    }

    public static IEnumerator GetUser(string username)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(HOST + username)) {
            Debug.Log("Fetching user from server...");
            yield return request.SendWebRequest();
            
            if (!request.isNetworkError)
            {
                byte[] result = request.downloadHandler.data;
                string resJson = System.Text.Encoding.Default.GetString(result);
                Debug.Log(resJson);
                User user = JsonUtility.FromJson<User>(resJson);
                GameManager.instance.user = user;
            }
            else
            {
                Debug.Log("Error fetching from server");
            }
        }
    }

    public static IEnumerator CreatePet(ActivePet newPet, string username)
    {
        string json = JsonUtility.ToJson(newPet.GetSnapshotCopy());
        Debug.Log(json);
        byte[] data = System.Text.Encoding.Default.GetBytes(json);
        
        // Create a PUT request because Unity apperantly cannot
        UnityWebRequest request = new UnityWebRequest(HOST + username + "/pet", "PUT");
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(data);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
    
        yield return request.SendWebRequest();

        if(request.isNetworkError)
            Debug.Log("Error sending data to server");
        else
        {
            byte[] result = request.downloadHandler.data;
            string resJson = System.Text.Encoding.Default.GetString(result);
            Debug.Log(resJson);
            PetSnapshot newSnapshot = JsonUtility.FromJson<PetSnapshot>(resJson);
            newPet.SetId(newSnapshot._id);
            Debug.Log("Pet created successfully");
        }
    }

    public static IEnumerator UpdatePet(PetSnapshot snapshot)
    {
        string json = JsonUtility.ToJson(snapshot);
        Debug.Log(json);
        byte[] data = System.Text.Encoding.Default.GetBytes(json);

        // Create a POST request because Unity apperantly cannot
        UnityWebRequest request = new UnityWebRequest(HOST+"login", "POST");
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(data);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        
        if (!request.isNetworkError)
        {
            Debug.Log("Successfully Updated pet");
        }
        else
        {
            Debug.Log("Error updating pet");
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
