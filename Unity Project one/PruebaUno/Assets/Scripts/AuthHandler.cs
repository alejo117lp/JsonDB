using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class AuthHandler : MonoBehaviour
{
    public string ApiUrl = "https://sid-restapi.onrender.com/api";
    TMP_InputField userNameInputfield;
    TMP_InputField passwordInputfield;
    void Start()
    {
        userNameInputfield = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>();
        passwordInputfield= GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>();
    }

    public void Registrar() {
        AuthData authData = new AuthData(); 
        authData.username = userNameInputfield.text;
        authData.password = passwordInputfield.text;

        string json = JsonUtility.ToJson(authData);
        StartCoroutine(SendRegister(json));
    }

    IEnumerator SendRegister(string json) {

        UnityWebRequest request = UnityWebRequest.Put(ApiUrl + "/usuarios", json);
        request.SetRequestHeader("Content-Type", "application/json"); //nombre del header, valor
        request.method = "POST";
        yield return request.SendWebRequest();

        if(request.isNetworkError) {
            Debug.Log("NETWORK ERROR: " + request.error);
        }
        else {
            Debug.Log(request.downloadHandler.text);
            if(request.responseCode == 200) {
                AuthData data = JsonUtility.FromJson<AuthData>(request.downloadHandler.text);
                Debug.Log("Se registró el usuario con ID: " + data.usuario._id);
            }
            else {
                Debug.Log(request.error);
            }
        }
    }
}

[System.Serializable]
public class AuthData {
    public string username;
    public string password;
    public UserData usuario;
}

[System.Serializable]
public class UserData {
    public string _id;
    public string username;
    public bool estado;
}
