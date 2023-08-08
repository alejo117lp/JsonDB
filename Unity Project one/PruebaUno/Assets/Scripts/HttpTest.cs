using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HttpTest : MonoBehaviour
{
    public int userId = 2;
    public string url = "https://my-json-server.typicode.com/alejo117lp/JsonDB";
    public string apiRickAndMorty = "https://rickandmortyapi.com/api/character";
    //public  RawImage yourRawImage;
    private User myUser;

    [SerializeField] private TextMeshProUGUI userNameLabel;
    [SerializeField] private TextMeshProUGUI[] myDeckLabels;
    [SerializeField] private RawImage[] myDeck;
    
    

    public void SendRequest() {
        StartCoroutine(GetUser());
    }


    IEnumerator GetUser() {

        UnityWebRequest request = UnityWebRequest.Get(url + "/users/" + userId);
        yield return request.SendWebRequest();

        if (request.isNetworkError) {
            Debug.Log("NETWORK ERROR: " + request.error);
        }
        else {
            Debug.Log(request.downloadHandler.text);
            if (request.responseCode == 200) {
                //Transformación de la data
                myUser = JsonUtility.FromJson<User>(request.downloadHandler.text);

                userNameLabel.text = myUser.username;

                for (int i = 0; i < myUser.deck.Length; i++) {
                    StartCoroutine(GetCharacter(i));
                }

                /*foreach (int card in myUser.deck) {
                    Debug.Log(card);
                }*/

                /*foreach(User user in userList.users) {
                    Debug.Log("ID: " + user.id + ", username " + user.username);
                }*/

            }
            else {
                Debug.Log(request.error);
            }
        }
    }

    IEnumerator GetCharacter(int index) { //(int [] ids o int id)

        int characterId = myUser.deck[index];
        UnityWebRequest request = UnityWebRequest.Get(apiRickAndMorty+"/"+characterId);
        yield return request.SendWebRequest();

        if (request.isNetworkError) {
            Debug.Log(request.error);
        }

        else {
            Debug.Log(request.downloadHandler.text);
            if(request.responseCode == 200) {
                //Debug.Log(request.GetResponseHeader("Content-Type"));//opcional para demostración
                Character character = JsonUtility.FromJson<Character>(request.downloadHandler.text);
                StartCoroutine(DownloadImage(character.image, myDeck[index]));
                myDeckLabels[index].text = character.name;
            }
            else {
                Debug.Log(request.error);
            }
        }


    }

   
    IEnumerator DownloadImage(string mediaUrl, RawImage yourRawImage) {
     
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
        yield return request.SendWebRequest();

        if (request.isNetworkError) {
            Debug.Log(request.error);
        }

        else if (!request.isHttpError) {
            yourRawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }
}

[System.Serializable]
public class UserList {
    public List<User> users;
}

public class Worlds {
    public List<int> worlds;
}

[System.Serializable]
public class User {
    public int id;
    public string username;
    public int[] deck;
    //public bool state;
}

public class Character {
    public int id;
    public string name;
    public string image;
}
