using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class DataService 
{
    public static bool offline_mode = true;
    public static bool isLoggedin;
    public static bool tryingToLogin;

    public const string HOST = "https://hidden-gorge-63443.herokuapp.com/api/"; //"http://localhost:5000/api/";

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
            //Debug.Log(resJson);

            User user = null;
            bool convertedJson = true;
            try 
            {
                user = JsonUtility.FromJson<User>(resJson);
            }
            catch (ArgumentException e)
            {
                Debug.Log(e);
                convertedJson = false;
            }

            if (convertedJson == false)
            {   
                Debug.Log("Failed to convert API result");
                yield break;
            }

            //GameManager.instance.user = user;
            //GameManager.instance.SetUser();
            isLoggedin = true;
            Debug.Log("Successfully logged in");
            yield return user;
        }
        else //Network error
        {
            if(offline_mode)
            {
                Debug.Log("[OFFLINE MODE] Creating offline user");
                User offline_user = new User();
                offline_user.username = "Offline";
                offline_user.active = new string[0];
                yield return offline_user;
            }
            else
                Debug.Log("Error fetching from server");
        }

        tryingToLogin = false;
    }
    public static IEnumerator Register(string username, string password, string email)
    {
        // Build JSON object and convert it to bytes
        string json = "{" + String.Format("\"username\":\"{0}\",\"password\":\"{1}\",\"email\":\"{2}\"", username, password, email) + "}";
        byte[] userData = System.Text.Encoding.Default.GetBytes(json);

        // Create a POST request because Unity apperantly cannot
        UnityWebRequest request = new UnityWebRequest(HOST+"register", "POST");
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(userData);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("Trying to Register...");
        tryingToLogin = true;
        yield return request.SendWebRequest();
        
        if (!request.isNetworkError)
        {
            byte[] result = request.downloadHandler.data;
            string resJson = System.Text.Encoding.Default.GetString(result);
            Debug.Log(resJson);
            
            User user = null;
            bool convertedJson = true;
            try 
            {
                user = JsonUtility.FromJson<User>(resJson);
            }
            catch (ArgumentException e)
            {
                Debug.Log(e);
                convertedJson = false;
            }

            if (convertedJson == false)
            {   
                Debug.Log("Failed to convert API result");
                yield break;
            }

            //GameManager.instance.user = user;
            //GameManager.instance.SetUser();
            isLoggedin = true;
            Debug.Log("Successfully registerd");
            yield return user;
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

    public static IEnumerator UpdateActive(string username, string[] active)
    {
        string json = "{ \"active\": [";

        for(int i = 0; i < active.Length; i++)
        {
            json += "\"" + active[i] + "\"";
            if(i+1 < active.Length)
                json +=",";
        }

        json += "]}";

        Debug.Log(json);
        byte[] data = System.Text.Encoding.Default.GetBytes(json);

        // Create a POST request because Unity apperantly cannot
        UnityWebRequest request = new UnityWebRequest(HOST+username+"/active", "PUT");
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(data);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        
        if (!request.isNetworkError)
        {
            Debug.Log("Successfully Updated active pets");
            yield return true;
        }
        else
        {
            Debug.Log("Error updating active pets");
            yield return false;
        }
    }

    public static IEnumerator UpdateGraveyard(string username, List<string> graveyard)
    {
        string json = "{ \"graveyard\": [";

        for(int i = 0; i < graveyard.Count; i++)
        {
            json += "\"" + graveyard[i] + "\"";
            if(i + 1 < graveyard.Count)
                json +=",";
        }

        json += "]}";

        Debug.Log(json);
        byte[] data = System.Text.Encoding.Default.GetBytes(json);

        // Create a POST request because Unity apperantly cannot
        UnityWebRequest request = new UnityWebRequest(HOST+username+"/graveyard", "PUT");
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(data);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        
        if (!request.isNetworkError)
        {
            Debug.Log("Successfully Updated Graveyard");
            yield return true;
        }
        else
        {
            Debug.Log("Error updating Graveyard");
            yield return false;
        }
    }

    public static IEnumerator CreatePet(ActivePet newPet, string username)
    {
        string json = JsonUtility.ToJson(newPet.GetSnapshotCopy());
        byte[] data = System.Text.Encoding.Default.GetBytes(json);
        
        // Create a PUT request because Unity apperantly cannot
        UnityWebRequest request = new UnityWebRequest(HOST + username + "/pet", "PUT");
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(data);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
    
        yield return request.SendWebRequest();

        if(request.isNetworkError)
        {
            if(offline_mode)
                yield return "0000-0000-0000";
            else
                Debug.Log("Error sending data to server");
        }
        else
        {
            byte[] result = request.downloadHandler.data;
            string resJson = System.Text.Encoding.Default.GetString(result);
            PetSnapshot newSnapshot = JsonUtility.FromJson<PetSnapshot>(resJson);
            yield return newSnapshot._id;
        }
    }

    public static IEnumerator UpdatePet(PetSnapshot snapshot)
    {
        string json = JsonUtility.ToJson(snapshot);
        //Debug.Log(json);
        byte[] data = System.Text.Encoding.Default.GetBytes(json);

        // Create a POST request because Unity apperantly cannot
        UnityWebRequest request = new UnityWebRequest(HOST+"pet", "POST");
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(data);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        
        if (!request.isNetworkError)
        {
            Debug.Log("Successfully Updated pet");
            yield return true;
        }
        else
        {
            if(offline_mode)
                yield return true;
            else
            {   
                Debug.Log("Error updating pet");
                yield return false;
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
