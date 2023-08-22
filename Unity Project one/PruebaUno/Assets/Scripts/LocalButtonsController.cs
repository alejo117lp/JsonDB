using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalButtonsController : MonoBehaviour
{
    
    public void BackToHome() {
        SceneManager.LoadScene("Home");
    }
}
