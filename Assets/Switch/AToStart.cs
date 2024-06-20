using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AToStart : MonoBehaviour {

    public KeyCode reginput;
    public KeyCode arrowinput;
    
    void Start ()
    {
        // nun
    }

    void Update () {

        if(Input.GetKey(KeyCode.Joystick1Button1))
        {
            SceneManager.LoadScene("Sandbox");
            // Sonic Utopia Para Skyline Android NSP
        }      

        if(Input.GetKey(KeyCode.Joystick1Button0))
        {
            SceneManager.LoadScene("SUOnlineLobby");
            // Sonic Utopia Online Para Skyline Android NSP
        }      
    }

}