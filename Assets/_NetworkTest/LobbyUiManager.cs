using CrappyPirates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CrappyPirates
{
    public class LobbyUiManager : MonoBehaviour
    {
        [SerializeField] private Button startGameButton = null;

        [SerializeField] private PlayerList teamlessList = null;
        [SerializeField] private PlayerList team1List = null;
        [SerializeField] private PlayerList team2List = null;

        [SerializeField] private int team1Max = 3;
        [SerializeField] private int team2Max = 3;


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
            e.player.TeamChanged -= Player_TeamChanged;
            RemoveFromTeamList(e.player.Team, e.player);
        }

        private void CP_NetworkManager_OnPlayerConnected(object sender, CP_NetworkManager.PlayerConnectionArgs e)
        {
            e.player.TeamChanged += Player_TeamChanged;
            e.player.CommandingModuleChanged += Player_CommandingModuleChanged;
            if (e.player.IsLeader) { startGameButton.gameObject.SetActive(true); }
            SortIntoTeam(e.player);
        }

        private void Player_CommandingModuleChanged(object sender, ValueChangedArgs<ModuleTypes> e)
        {
            if (startGameButton.gameObject.activeSelf) {
                ValidateStartGameButton();
            }
        }

        private void Player_TeamChanged(object sender, ValueChangedArgs<int> e)
        {
            NetworkPlayer_Lobby p = (NetworkPlayer_Lobby)sender;
            RemoveFromTeamList(e.oldValue, p);
            SortIntoTeam(p);
            if (startGameButton.gameObject.activeSelf) {
                ValidateStartGameButton();
            }
        }

        private void SortIntoTeam(NetworkPlayer_Lobby player)
        {
            if (player.Team == 1) {
                if(team1List.Count >= team1Max) {
                    player.SetTeam(2);
                } else {
                    team1List.AddPlayerToList(player);
                }
            } else if (player.Team == 2) {
                if (team2List.Count >= team1Max) {
                    player.SetTeam(0);
                } else {
                    team2List.AddPlayerToList(player);
                }
            } else {
                teamlessList.AddPlayerToList(player);
            }
        }

        private void RemoveFromTeamList(int team, NetworkPlayer_Lobby player)
        {
            if (team == 1) {
                team1List.RemovePlayerFromList(player);
            } else if (team == 2) {
                team2List.RemovePlayerFromList(player);
            } else {
                teamlessList.RemovePlayerFromList(player);
            }
        }

        private void ValidateStartGameButton()
        {
            startGameButton.interactable = GameCanStart();
        }

        private bool GameCanStart()
        {
            //Both teams have at least one player

            if(team1List.Count < 1 || team2List.Count < 1) { return false; }

            //Each module is only represented once

            HashSet<ModuleTypes> representedModules = new HashSet<ModuleTypes>();
            foreach(NetworkPlayer_Lobby p in team1List.Players) {
                if(p.CommandingModule != ModuleTypes.NONE && !representedModules.Add(p.CommandingModule)) {
                    return false;
                }
            }

            representedModules.Clear();

            foreach (NetworkPlayer_Lobby p in team2List.Players) {
                if (p.CommandingModule != ModuleTypes.NONE && !representedModules.Add(p.CommandingModule)) {
                    return false;
                }
            }

            return true;
        }
    } 
}
