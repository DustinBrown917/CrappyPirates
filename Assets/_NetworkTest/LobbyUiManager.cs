using CrappyPirates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrappyPirates
{
    public class LobbyUiManager : MonoBehaviour
    {
        [SerializeField] private Transform allPlayersContainer = null;
        private List<PlayerLobbyUIContainer> playerUiContainers = new List<PlayerLobbyUIContainer>();

        private void Awake()
        {
            CP_NetworkManager.OnPlayerConnected += CP_NetworkManager_OnPlayerConnected;
            CP_NetworkManager.OnPlayerDisconnected += CP_NetworkManager_OnPlayerDisconnected;
        }

        private void OnDestroy()
        {
            CP_NetworkManager.OnPlayerConnected -= CP_NetworkManager_OnPlayerConnected;
            CP_NetworkManager.OnPlayerDisconnected -= CP_NetworkManager_OnPlayerDisconnected;
        }

        private void CP_NetworkManager_OnPlayerDisconnected(object sender, CP_NetworkManager.PlayerConnectionArgs e)
        {
            for (int i = 0; i < playerUiContainers.Count; i++) {
                if (playerUiContainers[i].TargetPlayuer == e.player) {
                    Destroy(playerUiContainers[i].gameObject);
                    playerUiContainers.RemoveAt(i);
                    break;
                }
            }
        }

        private void CP_NetworkManager_OnPlayerConnected(object sender, CP_NetworkManager.PlayerConnectionArgs e)
        {
            PlayerLobbyUIContainer pluc = Instantiate(allPlayersContainer.GetChild(0), allPlayersContainer).GetComponent<PlayerLobbyUIContainer>();
            pluc.SetTargetPlayer(e.player);
            pluc.gameObject.SetActive(true);
            playerUiContainers.Add(pluc);
        }

    } 
}
