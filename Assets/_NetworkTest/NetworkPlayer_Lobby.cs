using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CrappyPirates
{
    public class NetworkPlayer_Lobby : NetworkBehaviour
    {
        public bool IsLeader { get; set; }

        [SyncVar(hook = nameof(HandleDisplayNameChanged))]
        private string displayName_ = "Loading";
        public string DisplayName { get => displayName_; }

        [SyncVar(hook = nameof(HandleReadyStateChanged))]
        private bool isReady_ = false;
        public bool IsReady { get => isReady_; set => isReady_ = value; }

        [SyncVar(hook = nameof(HandleCommandingModuleChanged))]
        private ModuleTypes commandingModule_ = ModuleTypes.NONE;
        public ModuleTypes CommandingModule { get => commandingModule_; }

        [SyncVar(hook = nameof(HandleTeamChanged))]
        private int team_ = 0;
        public int Team { get => team_; }

        private CP_NetworkManager room_ = null;

        public event EventHandler<ValueChangedArgs<string>> DisplayNameChanged;
        public event EventHandler<ValueChangedArgs<bool>> ReadyStateChanged;
        public event EventHandler<ValueChangedArgs<ModuleTypes>> CommandingModuleChanged;
        public event EventHandler<ValueChangedArgs<int>> TeamChanged;

        private CP_NetworkManager Room
        {
            get {
                if(room_ == null) { room_ = NetworkManager.singleton as CP_NetworkManager; }
                return room_;
            }
        }

        public override void OnStartAuthority()
        {
            CmdSetDisplayName(PlayerPrefs.GetString(PlayerNameInput.PP_PLAYER_NAME_TAG, "Butts 2.0"));
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            Room.RegisterPlayer(this);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            Room.DeregisterPlayer(this);
        }

        public void HandleIsReadyToStart(bool ready)
        {
            if (!IsLeader) { return; }
        }

        [Command]
        private void CmdSetDisplayName(string displayName)
        {
            displayName_ = displayName;
        }

        [Command]
        private void CmdSetCommandingModule(ModuleTypes moduleType)
        {
            commandingModule_ = moduleType;
        }

        public void SetCommandingModule(ModuleTypes moduleType)
        {
            CmdSetCommandingModule(moduleType);
        }

        [Command]
        private void CmdSetTeam(int team)
        {
            team_ = team;
        }

        public void SetTeam(int team)
        {
            CmdSetTeam(team);
        }

        [Command]
        private void CmdToggleReady()
        {
            isReady_ = !isReady_;
            Room.NotifyPlayersOfReadyState();
        }

        [Command]
        public void CmdStartGame()
        {
            if(Room.GetPlayer(0).connectionToClient != connectionToClient) { return; } //Making sure we are the MVP (first player to join lobby)
        }

        public void HandleReadyStateChanged(bool oldValue, bool newValue)
        {
            ReadyStateChanged?.Invoke(this, new ValueChangedArgs<bool>(oldValue, newValue));
        }

        public void HandleDisplayNameChanged(string oldValue, string newValue)
        {
            DisplayNameChanged?.Invoke(this, new ValueChangedArgs<string>(oldValue, newValue));
        }

        public void HandleCommandingModuleChanged(ModuleTypes oldValue, ModuleTypes newValue)
        {
            CommandingModuleChanged?.Invoke(this, new ValueChangedArgs<ModuleTypes>(oldValue, newValue));
        }

        public void HandleTeamChanged(int oldValue, int newValue)
        {
            TeamChanged?.Invoke(this, new ValueChangedArgs<int>(oldValue, newValue));
        }
    } 
}
