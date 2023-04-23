using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{
    public void GreenHill()
    {
        SceneManager.LoadScene("Sandbox");
    }

    public void SA2()
    {
        SceneManager.LoadScene("SA2B Test Stage");
    }

    public void Cross()
    {
        SceneManager.LoadScene("Roblox");
    }
}
