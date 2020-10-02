using Mirror;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CrappyPirates
{
    public class CP_NetworkManager : NetworkManager
    {
        [SerializeField] private int minPlayers = 2;
        [Scene] [SerializeField] private string menuScene = string.Empty;
        [SerializeField] private NetworkPlayer_Lobby lobbyPlayerPrefab = null;

        public static event EventHandler<ClientConnectionArgs> OnClientConnected;
        public static event EventHandler<ClientConnectionArgs> OnClientDisconnected;

        public static event EventHandler<PlayerConnectionArgs> OnPlayerConnected;
        public static event EventHandler<PlayerConnectionArgs> OnPlayerDisconnected;
        
        [SerializeField, ReadOnly] private List<NetworkPlayer_Lobby> currentlyConnectedPlayers_ = new List<NetworkPlayer_Lobby>();
        public int CurrentlyConnectedPlayerCount { get => currentlyConnectedPlayers_.Count; }

        public override void OnStartServer()
        {
            base.OnStartServer();
            spawnPrefabs = Resources.LoadAll<GameObject>("NetworkedPrefabs").ToList();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            List<GameObject> spawnablePrefabs = Resources.LoadAll<GameObject>("NetworkedPrefabs").ToList();

            foreach(GameObject go in spawnablePrefabs) {
                ClientScene.RegisterPrefab(go);
            }
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            OnClientConnected?.Invoke(this, new ClientConnectionArgs());
            Debug.Log("New COnnection");
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            Debug.Log("Disconnected");
            OnClientDisconnected?.Invoke(this, new ClientConnectionArgs());
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
  
            if(numPlayers >= maxConnections) {
                conn.Disconnect();
                Debug.Log("Too many connections - disconnecting");
                return;
            }

            if(SceneManager.GetActiveScene().path != menuScene) {
                Debug.Log("Wrong scene");
                conn.Disconnect();
                return;
            }
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        { 
            if(SceneManager.GetActiveScene().path == menuScene) {
                bool isLeader = currentlyConnectedPlayers_.Count == 0;

                NetworkPlayer_Lobby lobbyPlayer = Instantiate(lobbyPlayerPrefab);

                lobbyPlayer.IsLeader = isLeader;

                NetworkServer.AddPlayerForConnection(conn, lobbyPlayer.gameObject);
            }
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            if(conn.identity != null) {
                NetworkPlayer_Lobby player = conn.identity.GetComponent<NetworkPlayer_Lobby>();

                currentlyConnectedPlayers_.Remove(player);

                NotifyPlayersOfReadyState();
            }

            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            currentlyConnectedPlayers_.Clear();
        }

        public void NotifyPlayersOfReadyState()
        {
            foreach (NetworkPlayer_Lobby player in currentlyConnectedPlayers_) {
                player.HandleIsReadyToStart(IsReadyToStart());
            }
        }

        public NetworkPlayer_Lobby GetPlayer(int index)
        {
            return currentlyConnectedPlayers_[index];
        }

        public void RegisterPlayer(NetworkPlayer_Lobby player)
        {
            currentlyConnectedPlayers_.Add(player);
            OnPlayerConnected?.Invoke(this, new PlayerConnectionArgs(player));
        }

        public void DeregisterPlayer(NetworkPlayer_Lobby player)
        {
            currentlyConnectedPlayers_.Remove(player);
            OnPlayerDisconnected?.Invoke(this, new PlayerConnectionArgs(player));
        }

        private bool IsReadyToStart()
        {
            if(numPlayers < minPlayers) { return false; }

            foreach(NetworkPlayer_Lobby player in currentlyConnectedPlayers_) {
                if (!player.IsReady) { return false; }
            }

            return true;
        }

        public class ClientConnectionArgs : EventArgs
        {

        }

        public class PlayerConnectionArgs : EventArgs
        {
            public NetworkPlayer_Lobby player = null;

            public PlayerConnectionArgs(NetworkPlayer_Lobby player)
            {
                this.player = player;
            }
        }
    } 
}
