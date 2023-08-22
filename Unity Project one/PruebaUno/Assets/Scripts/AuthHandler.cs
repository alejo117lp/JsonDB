using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AuthHandler : MonoBehaviour
{
    public string ApiUrl = "https://sid-restapi.onrender.com/api/";
    TMP_InputField userNameInputfield;
    TMP_InputField passwordInputfield;
    AuthData currentUser;
    private string token;
    private string username;
    void Start()
    {
        token = PlayerPrefs.GetString("token");
        if (string.IsNullOrEmpty(token)) {
            Debug.Log("No hay token almacenado");
        }
        else {
            username = PlayerPrefs.GetString("username");
            StartCoroutine(GetPerfil(username));
        }

        userNameInputfield = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>();
        passwordInputfield= GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>();
    }

    public void Register() {
        AuthData authData = new AuthData(); 
        authData.username = userNameInputfield.text;
        authData.password = passwordInputfield.text;

        string json = JsonUtility.ToJson(authData);
        StartCoroutine(SendRegister(json));
    }

    public void Login() {
        AuthData authData = new AuthData();
        authData.username = userNameInputfield.text;
        authData.password = passwordInputfield.text;

        string json = JsonUtility.ToJson(authData);
        StartCoroutine(SendLogin(json));
    }

    public void AssignScore() {
        int rnd = Random.Range (0, 201);
        currentUser.usuario.data.score = rnd;
        UserD user = new UserD();
        user.data = new DataUser();
        user.username = currentUser.usuario.username;
        user.data.score = currentUser.usuario.data.score;
        Debug.Log(currentUser.usuario.username + " SCORE: " + currentUser.usuario.data.score);
        string json = JsonUtility.ToJson(user);
        StartCoroutine(SendPATCH(json));
    }

    IEnumerator GetPerfil(string username) {
        UnityWebRequest request = UnityWebRequest.Get(ApiUrl + "usuarios/" + username);
        request.SetRequestHeader("x-token", token);
        yield return request.SendWebRequest();

        if (request.isNetworkError) {
            Debug.Log("NETWORK ERROR" + request.error);
        }
        else {
            Debug.Log(request.downloadHandler.text);
            if(request.responseCode == 200) {
                AuthData data = JsonUtility.FromJson<AuthData>(request.downloadHandler.text);
                currentUser = data;
                currentUser.usuario.username = username;
                currentUser.token = token;
                PlayerPrefs.SetString("user_name", currentUser.usuario.username);
                
                Debug.Log("Sesión Activa de: " + data.usuario.username);
                Debug.Log("Su scores es: " + data.usuario.data.score);
                //SceneManager.LoadScene("Level 1");
            }
            else {
                Debug.Log(request.error);
            }
        }
    }

    IEnumerator SendRegister(string json) {

        UnityWebRequest request = UnityWebRequest.Put(ApiUrl + "usuarios", json);
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

    IEnumerator SendLogin(string json) {

        UnityWebRequest request = UnityWebRequest.Put(ApiUrl + "auth/login", json);
        request.SetRequestHeader("Content-Type", "application/json"); //nombre del header, valor
        request.method = "POST";
        yield return request.SendWebRequest();

        if (request.isNetworkError) {
            Debug.Log("NETWORK ERROR: " + request.error);
        }
        else {
            Debug.Log(request.downloadHandler.text);

            if (request.responseCode == 200) {

                AuthData data = JsonUtility.FromJson<AuthData>(request.downloadHandler.text);
                Debug.Log("Inició Sesión el usuario: " + data.usuario.username);
                //SceneManager.LoadScene("Level 1");
                Debug.Log(data.token);
            }
            else {
                Debug.Log(request.error);
            }
        }
    }

    IEnumerator SendPATCH(string json) {
        UnityWebRequest request = UnityWebRequest.Put(ApiUrl + "usuarios", json);
        request.SetRequestHeader("x-token", token);
        request.SetRequestHeader("Content-Type", "application/json"); //nombre del header, valor
        request.method = "PATCH";
        yield return request.SendWebRequest();

        if (request.isNetworkError) {
            Debug.Log("NETWORK ERROR: " + request.error);
        }
        else {
            Debug.Log(request.downloadHandler.text);

            if (request.responseCode == 200) {

                Debug.Log("NUEVO SCORE");
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
    public UserD usuario;
    public UserD[] usuarios;
    public string token;
}

[System.Serializable]
public class UserD {
    public string _id;
    public string username;
    public bool estado;
    public DataUser data;
}

[System.Serializable]
public class DataUser {
    public int score;
    public UserD[] friends;
}
