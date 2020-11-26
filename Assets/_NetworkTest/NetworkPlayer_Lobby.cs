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
        private static NetworkPlayer_Lobby localPlayer = null;
        public static NetworkPlayer_Lobby LocalPlayer { get => localPlayer; }

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

        private NetworkWeaponUI weaponUI = null;
        private NetworkNavigationUI navUI = null;

        private CP_NetworkManager room_ = null;

        public event EventHandler<ValueChangedArgs<string>> DisplayNameChanged;
        public event EventHandler<ValueChangedArgs<bool>> ReadyStateChanged;
        public event EventHandler<ValueChangedArgs<ModuleTypes>> CommandingModuleChanged;
        public event EventHandler<ValueChangedArgs<int>> TeamChanged;


        private NetworkShip ship_ = null;
        public NetworkShip Ship { get => ship_; }

        private CP_NetworkManager Room
        {
            get {
                if(room_ == null) { room_ = NetworkManager.singleton as CP_NetworkManager; }
                return room_;
            }
        }

        public override void OnStartAuthority()
        {
            CmdSetDisplayName(PlayerPrefs.GetString(PlayerNameInput.PP_PLAYER_NAME_TAG, "New Player"));
            localPlayer = this;
            NetworkWeaponUI.InstanceUpdated += NetworkWeaponUI_InstanceUpdated;
            NetworkNavigationUI.InstanceUpdated += NetworkNavigationUI_InstanceUpdated;
        }

        private void NetworkNavigationUI_InstanceUpdated()
        {
            navUI = NetworkNavigationUI.Instance;

            if (weaponUI != null) {
                if (commandingModule_ == ModuleTypes.NAVIGATION || commandingModule_ == ModuleTypes.NONE) {
                    navUI.gameObject.SetActive(true);
                } else {
                    navUI.gameObject.SetActive(false);
                }
            }
        }

        private void NetworkWeaponUI_InstanceUpdated()
        {
            if(weaponUI != null) {
                weaponUI.gameObject.SetActive(false);
                weaponUI.FireForward -= FireForwardBattery;
                weaponUI.FirePort -= FirePortBattery;
                weaponUI.FireRear -= FireRearBattery;
                weaponUI.FireStarboard -= FireStarboardBattery;
            }

            weaponUI = NetworkWeaponUI.Instance;

            if (weaponUI != null) {
                if (commandingModule_ == ModuleTypes.WEAPONS || commandingModule_ == ModuleTypes.NONE) {
                    weaponUI.gameObject.SetActive(true);
                    weaponUI.FireForward += FireForwardBattery;
                    weaponUI.FirePort += FirePortBattery;
                    weaponUI.FireRear += FireRearBattery;
                    weaponUI.FireStarboard += FireStarboardBattery;
                } else {
                    weaponUI.gameObject.SetActive(false);
                }
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            DontDestroyOnLoad(gameObject);
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

        public void SetShip(NetworkShip ship)
        {
            this.ship_ = ship;
            ShowShipHealthbar();
        }

        public void ShowShipHealthbar()
        {

            Ship.ShowHealthBar();
            
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
            Room.StartGame();
        }

        [Command] 
        public void AccelerateShip()
        {
            if(ship_ != null) {
                ship_.Accelerate();
            }
        }

        [Command]
        public void DeccelerateShip()
        {
            if (ship_ != null) {
                ship_.Deccelerate();
            }
        }

        [Command]
        public void TurnShipRight()
        {
            if (ship_ != null) {
                ship_.TurnRight();
            }
        }

        [Command]
        public void TurnShipLeft()
        {
            if (ship_ != null) {
                ship_.TurnLeft();
            }
        }

        [Command]
        public void FireForwardBattery()
        {
            if(ship_ != null) {
                ship_.FireForward();
            }
        }

        [Command]
        public void FirePortBattery()
        {
            if (ship_ != null) {
                ship_.FirePort();
            }
        }

        [Command]
        public void FireStarboardBattery()
        {
            if (ship_ != null) {
                ship_.FireStarboard();
            }
        }

        [Command]
        public void FireRearBattery()
        {
            if (ship_ != null) {
                ship_.FireRear();
            }
        }


        [Client]
        private void FixedUpdate()
        {
            if((commandingModule_ == ModuleTypes.NAVIGATION || commandingModule_ == ModuleTypes.NONE) && navUI != null) {
                if(navUI.accelerate == 1) {
                    AccelerateShip();
                } else if(navUI.accelerate == -1) {
                    DeccelerateShip();
                }

                if(navUI.turn == 1) {
                    TurnShipRight();
                } else if(navUI.turn == -1) {
                    TurnShipLeft();
                }
            }

            if (Input.GetKey(KeyCode.W)) {
                AccelerateShip();
            } else if (Input.GetKey(KeyCode.S)) {
                DeccelerateShip();
            }

            if (Input.GetKey(KeyCode.D)) {
                TurnShipRight();
            } else if (Input.GetKey(KeyCode.A)) {
                TurnShipLeft();
            }
        }

        [Client]
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) {
                FirePortBattery();
            }
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
