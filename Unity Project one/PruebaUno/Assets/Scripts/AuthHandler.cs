using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class AuthHandler : MonoBehaviour
{
    [SerializeField] GameObject leaderboard;
    [SerializeField] TextMeshProUGUI currentUsernameUI;
    [SerializeField] TextMeshProUGUI currentScoreUI;
    [SerializeField] TextMeshProUGUI[] firtsUsers; 
    public string ApiUrl = "https://sid-restapi.onrender.com/api/";
    TMP_InputField userNameInputfield;
    TMP_InputField passwordInputfield;
    AuthData currentUser;
    UserD[] usuarios;
    private string token;
    private string username;
    public bool restoreData = true;
    void Start()
    {
        if(restoreData) {
            PlayerPrefs.SetString("username", "");
            PlayerPrefs.SetString("token", "");
        }
        token = PlayerPrefs.GetString("token");
        if (string.IsNullOrEmpty(token)) {
            Debug.Log("No hay token almacenado");
        }
        else {
            username = PlayerPrefs.GetString("username");
            StartCoroutine(GetPerfil(username));
            StartCoroutine(GetUserList());
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
        int rnd = Random.Range (0, 901);
        currentUser.usuario.data.score = rnd;
        UserD user = new UserD();
        user.data = new DataUser();
        user.username = currentUser.usuario.username;
        user.data.score = currentUser.usuario.data.score;
        Debug.Log(currentUser.usuario.username + " SCORE: " + currentUser.usuario.data.score);
        currentUsernameUI.text = currentUser.usuario.username;
        currentScoreUI.text = (currentUser.usuario.data.score).ToString();
        string json = JsonUtility.ToJson(user);
        StartCoroutine(SendPATCH(json));
        StartCoroutine(GetUserList());
    }

    public  void SortUsers() {

        for(int i= 0; i < firtsUsers.Length; i++) {
            firtsUsers[i].text = 
                ( i+1 +". "+ usuarios[i].username + " " + usuarios[i].data.score.ToString());
        }
    }

    IEnumerator GetUserList() {
        UnityWebRequest request = UnityWebRequest.Get(ApiUrl + "usuarios?limit5");
        request.SetRequestHeader("x-token", token);
        yield return request.SendWebRequest();

        if (request.isNetworkError) {
            Debug.Log("NETWORK ERROR" + request.error);
        }

        else {
            if (request.responseCode == 200) {
                AuthData authData = JsonUtility.FromJson<AuthData>(request.downloadHandler.text);
                usuarios = authData.usuarios;
                usuarios = usuarios.OrderByDescending(user => user.data.score).ToArray();
                SortUsers();
            }
            else {
                Debug.Log(request.error);
            }
        }

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
                leaderboard.SetActive(true);
                Debug.Log("Sesión Activa de: " + data.usuario.username);
                Debug.Log("Su scores es: " + data.usuario.data.score);
                StartCoroutine(GetUserList());
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
                PlayerPrefs.SetString("token", data.token);
                PlayerPrefs.SetString("username", data.usuario.username);
                Debug.Log("Inició Sesión el usuario: " + data.usuario.username);
                currentUser = data;
                token = data.token;
                username = data.username;
                leaderboard.SetActive(true);
                currentUsernameUI.text = data.usuario.username;
                currentScoreUI.text = (currentUser.usuario.data.score).ToString();
                StartCoroutine(GetUserList());
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
                StartCoroutine(GetUserList());
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
