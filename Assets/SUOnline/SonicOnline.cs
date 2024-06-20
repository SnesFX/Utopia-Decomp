using UnityEngine;
using UnityEngine.SceneManagement;

public class SonicOnline : Photon.MonoBehaviour
{
    public Transform playerPrefab;

    public void Awake()
    {
        // Check if playerPrefab is assigned
        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab is not assigned in the Inspector!");
            return;
        }

        // in case we started this demo with the wrong scene being active, simply load the menu scene
        if (!PhotonNetwork.connected)
        {
            Debug.Log("Photon Not Connected, Returning to Lobby!");
            SceneManager.LoadScene("SUOnlineLobby");
            return;
        }

        Debug.Log("Loading Sonic");

        // Log the name of the prefab to be instantiated
        Debug.Log("Prefab name: " + this.playerPrefab.name);

        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
        var instantiatedPlayer = PhotonNetwork.Instantiate(this.playerPrefab.name, transform.position, Quaternion.identity, 0);

        // Log whether the player was successfully instantiated
        if (instantiatedPlayer != null)
        {
            Debug.Log("Player instantiated successfully.");
        }
        else
        {
            Debug.LogError("Failed to instantiate player.");
        }
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Return to Lobby"))
        {
            Debug.Log("Leaving game!");
            PhotonNetwork.LeaveRoom();  // we will load the menu level when we successfully left the room
        }
    }

    public void OnMasterClientSwitched(PhotonPlayer player)
    {
        Debug.Log("OnMasterClientSwitched: " + player);

        string message;
        InRoomChat chatComponent = GetComponent<InRoomChat>();  // if we find a InRoomChat component, we print out a short message

        if (chatComponent != null)
        {
            // to check if this client is the new master...
            if (player.IsLocal)
            {
                Debug.Log("Masta.");
                message = "You are Master Client now.";
            }
            else
            {
                Debug.Log("no masta");
                message = player.NickName + " is Master Client now.";
            }

            chatComponent.AddLine(message); // the Chat method is a RPC. as we don't want to send an RPC and neither create a PhotonMessageInfo, lets call AddLine()
        }
    }

    public void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom (local)");

        // back to main menu
        SceneManager.LoadScene("SUOnlineLobby");
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("OnDisconnectedFromPhoton");

        // back to main menu
        SceneManager.LoadScene("SUOnlineLobby");
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("OnPhotonInstantiate " + info.sender);    // you could use this info to store this or react
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        Debug.Log("OnPhotonPlayerConnected: " + player);
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("OnPlayerDisconneced: " + player);
    }

    public void OnFailedToConnectToPhoton()
    {
        Debug.Log("OnFailedToConnectToPhoton");

        // back to main menu
        SceneManager.LoadScene("SUOnlineLobby");
    }
}
