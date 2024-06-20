using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnMenu : MonoBehaviour
{
    public Button rtmb;

    void Start()
    {
        if (rtmb == null)
        {
            Debug.LogError("Button is not assigned!");
            return;
        }

        rtmb.onClick.AddListener(LoadMainMenuScene);
    }

    void LoadMainMenuScene()
    {
        Debug.Log("Loading MainMenu Scene");
        SceneManager.LoadScene("Main Menu");
    }
}
