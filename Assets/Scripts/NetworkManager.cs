using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

    public Transform myPlayer;
    public Transform myCamera;
    public Transform spawnPoint;
    public GameObject lobbyCamera;

    const string Version = "0.1";

	void Start ()
    {
        Connect();
	}
	
	void Connect()
    {
        PhotonNetwork.ConnectUsingSettings(Version);
    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    void OnJoinedLobby()
    {
        PhotonNetwork.JoinOrCreateRoom("Test", null, TypedLobby.Default);
    }

    void OnJoinedRoom()
    {
        Debug.Log("Joined!");
        SpawnPlayer();
        lobbyCamera.SetActive(false);
    }

    void SpawnPlayer()
    {
        GameObject tempPlayer = (GameObject)PhotonNetwork.Instantiate(myPlayer.name, spawnPoint.position, spawnPoint.rotation, 0); //  spawn the player and get it into a variable
        GameObject tempCamera = (GameObject)PhotonNetwork.Instantiate(myCamera.name, spawnPoint.position, spawnPoint.rotation, 0); //  spawn the camera and get it into a variable
        tempPlayer.GetComponentInChildren<CharacterController>().enabled = true;//enable the character controller from player
        ((MonoBehaviour)tempPlayer.GetComponent("PlayerMovement")).enabled = true; //enable the script "PlayerMovement" from player
        PlayerMovement movement = tempPlayer.GetComponent<PlayerMovement>(); //store it into a variable
        movement.playerCam = tempCamera.gameObject.transform.GetChild(0).gameObject.transform; // set playerCam variable from "PlayerMovement" script to the actual camera from tempCamera
        movement.centerPoint = tempCamera.transform; // set the centerPoint variable from "PlayerMovement" to the actual centerPoint

        tempCamera.GetComponentInChildren<Camera>().enabled = true; // enable the Camera
        tempCamera.GetComponentInChildren<AudioListener>().enabled = true; // enable the Audio Listener
    }
}
