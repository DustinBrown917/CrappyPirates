using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrappyPirates
{
    public static class PlayerManager
    {
        private static Player localPlayer_ = null;
        public static Player LocalPlayer {
            get {
                if(localPlayer_ == null) {
                    localPlayer_ = ClientScene.localPlayer.GetComponent<Player>();
                }

                return localPlayer_;
            }
        }
        private static Dictionary<int, Player> players = new Dictionary<int, Player>();
        public static int PlayerCount { get => players.Count; }

        public static event EventHandler<PlayerAddedArgs> PlayerAdded;
        public static event EventHandler<PlayerRemovedArgs> PlayerRemoved;

        public static void RegisterPlayer(Player p)
        {
            if (!players.ContainsKey(p.Id)) {
                Debug.Log("Registering " + p.Name);
                players.Add(p.Id, p);
                PlayerAdded?.Invoke(typeof(PlayerManager), new PlayerAddedArgs(p));
            }
        }

        public static void DeregisterPlayer(Player p)
        {
            if (players.Remove(p.Id)) {
                Debug.Log("Deregistering " + p.name);
                PlayerRemoved?.Invoke(typeof(PlayerManager), new PlayerRemovedArgs(p));
            }
        }

        public static int CreatePlayerId()
        {
            int id = System.DateTime.Now.GetHashCode();
            while (players.ContainsKey(id)) {
                id++;
            }

            return id;
        }

        public static Player GetPlayer(int id)
        {
            if(players.ContainsKey(id)) {
                return players[id];
            }

            return null;
        }

        public static void RemovePlayer(Player p)
        {
            RemovePlayer(p.Id);
        }

        public static void RemovePlayer(int id)
        {
            players.Remove(id);
        }

        public class PlayerAddedArgs : EventArgs
        {
            public Player player = null;

            public PlayerAddedArgs(Player p)
            {
                this.player = p;
            }
        }

        public class PlayerRemovedArgs : EventArgs
        {
            public Player player = null;

            public PlayerRemovedArgs(Player p)
            {
                this.player = p;
            }
        }
    }
}

