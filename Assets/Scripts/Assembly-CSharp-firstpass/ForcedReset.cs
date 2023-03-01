using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Image))]
public class ForcedReset : MonoBehaviour
{
	private void Update()
	{
		if (CrossPlatformInputManager.GetButtonDown("ResetObject"))
		{
			SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
		}
	}
}
