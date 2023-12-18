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

        if(Input.GetKey(KeyCode.Joystick1Button0))
        {
            SceneManager.LoadScene("Sandbox");
            // Sonic Utopia Para Nintendo Switch Android NSP
        }      
    }

}