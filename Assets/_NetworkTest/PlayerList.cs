using CrappyPirates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerList : MonoBehaviour
{
    private List<PlayerLobbyUIContainer> playerUIContainers = new List<PlayerLobbyUIContainer>();

    [SerializeField] private Transform container = null;

    public List<NetworkPlayer_Lobby> Players
    {
        get {
            return GetPlayers();
        }
    }

    private int playerCount = 0;
    public int Count { get => playerCount; }

    public void AddPlayerToList(NetworkPlayer_Lobby player)
    {
        PlayerLobbyUIContainer pluc = Instantiate(container.GetChild(0), container).GetComponent<PlayerLobbyUIContainer>();
        pluc.SetTargetPlayer(player);
        pluc.gameObject.SetActive(true);
        playerUIContainers.Add(pluc);
        playerCount++;
    }

    public void RemovePlayerFromList(NetworkPlayer_Lobby player)
    {
        for (int i = 0; i < playerUIContainers.Count; i++) {
            if (playerUIContainers[i].TargetPlayuer == player) {
                Destroy(playerUIContainers[i].gameObject);
                playerUIContainers.RemoveAt(i);
                playerCount--;
                break;
            }
        }
    }

    private List<NetworkPlayer_Lobby> GetPlayers()
    {
        List<NetworkPlayer_Lobby> players = new List<NetworkPlayer_Lobby>();
        foreach(PlayerLobbyUIContainer pluc in playerUIContainers) {
            if(pluc.TargetPlayuer != null) {
                players.Add(pluc.TargetPlayuer);
            }
        }

        return players;
    }
}
