using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LocalButtonsController : MonoBehaviour
{
    [SerializeField] GameObject learderboard;
    public void BackToHome() {
        learderboard.SetActive(false);
    }
}
