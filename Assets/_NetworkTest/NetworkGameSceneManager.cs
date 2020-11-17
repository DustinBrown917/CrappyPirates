using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrappyPirates
{
    public class NetworkGameSceneManager : NetworkBehaviour
    {
        [SerializeField] private Transform team1Spawn = null;
        [SerializeField] private Transform team2Spawn = null;

        [SerializeField] private GameObject shipPrefab = null;

        private NetworkShip team1Ship = null;
        private NetworkShip team2Ship = null;

        private CP_NetworkManager networkManager;

        private void Start()
        {
            SpawnShips();
        }

        public override void OnStartServer()
        {
            SpawnShips();
        }

        private void CP_NetworkManager_OnServerReadied(NetworkConnection obj)
        {
            SpawnShips();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            networkManager = FindObjectOfType<CP_NetworkManager>();
        }


        [Server]
        private void SpawnShips()
        {
            team1Ship = Instantiate(shipPrefab, team1Spawn).GetComponent<NetworkShip>();
            team1Ship.team = 1;
            NetworkServer.Spawn(team1Ship.gameObject);

            team1Ship.transform.SetParent(null);

            team2Ship = Instantiate(shipPrefab, team2Spawn).GetComponent<NetworkShip>();
            team2Ship.team = 2;
            NetworkServer.Spawn(team2Ship.gameObject);
            team2Ship.transform.SetParent(null);

            foreach(NetworkPlayer_Lobby p in CP_NetworkManager.Instance.currentlyConnectedPlayers_) {
                if(p.Team == 1) {
                    p.ship = team1Ship;
                } else {
                    p.ship = team2Ship;
                }
            }
        }


        
    }
}

