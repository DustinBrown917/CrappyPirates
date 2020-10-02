using CrappyPirates;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private CP_NetworkManager networkManager = null;

    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private TMP_InputField ipAddressInputField;
    [SerializeField] private Button joinButton = null;


    private void OnEnable()
    {
        CP_NetworkManager.OnClientConnected += CP_NetworkManager_OnClientConnected;
        CP_NetworkManager.OnClientDisconnected += CP_NetworkManager_OnClientDisconnected;
    }


    private void OnDisable()
    {
        CP_NetworkManager.OnClientConnected -= CP_NetworkManager_OnClientConnected;
        CP_NetworkManager.OnClientDisconnected -= CP_NetworkManager_OnClientDisconnected;
    }

    public void JoinLobby()
    {
        string ipAddress = ipAddressInputField.text;

        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();

        joinButton.interactable = false;
    }

    private void CP_NetworkManager_OnClientDisconnected(object sender, CP_NetworkManager.ClientConnectionArgs e)
    {
        joinButton.interactable = true;
    }

    private void CP_NetworkManager_OnClientConnected(object sender, CP_NetworkManager.ClientConnectionArgs e)
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }
}
