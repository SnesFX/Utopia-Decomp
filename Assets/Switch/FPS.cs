using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour {

    public Text FPSText;

    public float RefreshRate = 1f;

    private float Timer;

    void Update()
    {
        if (Time.fixedUnscaledTime > Timer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            FPSText.text = "FPS: " + fps;
            Timer = Time.unscaledTime + RefreshRate;
        }
    }
}