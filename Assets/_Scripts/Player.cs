using Mirror;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrappyPirates
{
    public class Player : NetworkBehaviour
    {
        [SyncVar(hook = nameof(SyncVarSetName))]
        [SerializeField, ReadOnly] private string name_;
        public string Name { get => name_; }

        [SyncVar(hook = nameof(SyncVarSetId))]
        [SerializeField, ReadOnly] private int id_ = 0;
        public int Id { get => id_; }

        public bool registeredWithPlayerManager = false;

        public event EventHandler<ValueChangedArgs<string>> NameChanged;
        public event EventHandler<ValueChangedArgs<int>> IdChanged;


        public override void OnStartServer()
        {
            base.OnStartServer();
            name_ = "Player " + PlayerManager.PlayerCount.ToString(); //Just until we can get the player to pick their name
            id_ = PlayerManager.CreatePlayerId();
            PlayerManager.RegisterPlayer(this);
            RPC_RegisterWithLocalPlayerManager(name_, id_);
        }

        public override void OnStopServer()
        {
            PlayerManager.DeregisterPlayer(this);
            RPC_DeregisterWithLocalPlayerManager();
            base.OnStopServer();

        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (id_ != 0) {
                PlayerManager.RegisterPlayer(this);
                Debug.Log($"Non local player registered with id of {id_}.");
            }
        }

        public override void OnStopClient()
        {
            if (isLocalPlayer) { Debug.Log("Local player has stopped client."); }

            base.OnStopClient();
            //This is only effective for handling other players disconnecting.
            PlayerManager.DeregisterPlayer(this);            
        }


        

        private void SyncVarSetName(string oldVal, string newVal)
        {
            SetName(newVal);
        }

        public void SetName(string name)
        {
            ValueChangedArgs<string> args = new ValueChangedArgs<string>(name_, name);
            name_ = name;
            NameChanged?.Invoke(this, args);
        }

        private void SyncVarSetId(int oldVal, int newVal)
        {
            SetId(newVal);
        }

        public void SetId(int id)
        {
            ValueChangedArgs<int> args = new ValueChangedArgs<int>(id_, id);
            id_ = id;
            IdChanged?.Invoke(this, args);
        }

        [ClientRpc]
        public void RPC_RegisterWithLocalPlayerManager(string name, int id)
        {
            SetName(name);
            SetId(id);
            PlayerManager.RegisterPlayer(this);
        }

        [ClientRpc]
        public void RPC_DeregisterWithLocalPlayerManager()
        {
            Debug.Log("RPC deregister");
            PlayerManager.DeregisterPlayer(this);
        }

        public class ValueChangedArgs<T>
        {
            public T oldValue = default;
            public T newValue = default;

            public ValueChangedArgs(T oldValue, T newValue)
            {
                this.newValue = newValue;
                this.oldValue = oldValue;
            }
        }
    }
}

