using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CrappyPirates
{
    public class PlayerLobbyUIContainer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText = null;
        [SerializeField] private TextMeshProUGUI playerModuleText = null;
        [SerializeField] private Button changeTeamButton = null;
        [SerializeField] private Button changeModuleButton = null;
        [SerializeField] private TextMeshProUGUI changeModuleButtonText = null;

        private NetworkPlayer_Lobby targetPlayer_ = null;
        public NetworkPlayer_Lobby TargetPlayuer { get => targetPlayer_; }

        public string TargetPlayerDisplayName { get => targetPlayer_.DisplayName; }

        private void Awake()
        {
            changeModuleButtonText = changeModuleButton.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetTargetPlayer(NetworkPlayer_Lobby p)
        {
            changeTeamButton.gameObject.SetActive(false);
            changeModuleButton.gameObject.SetActive(false);
            playerModuleText.gameObject.SetActive(false);

            if (targetPlayer_ != null) {
                DisconnectEvents();
            }

            targetPlayer_ = p;

            if(targetPlayer_ != null) {
                playerNameText.text = targetPlayer_.DisplayName;

                playerModuleText.text = targetPlayer_.CommandingModule.ToString();
                changeModuleButtonText.text = targetPlayer_.CommandingModule.ToString();

                ConnectEvents();
                changeTeamButton.gameObject.SetActive(targetPlayer_.hasAuthority);
                changeModuleButton.gameObject.SetActive(targetPlayer_.hasAuthority);
                playerModuleText.gameObject.SetActive(!targetPlayer_.hasAuthority);
            }
        }

        private void ConnectEvents()
        {
            targetPlayer_.DisplayNameChanged += TargetPlayer_NameChanged;
            targetPlayer_.ReadyStateChanged += TargetPlayer_ReadyStateChanged;
            targetPlayer_.CommandingModuleChanged += TargetPlayer_CommandingModuleChanged;
        }


        private void DisconnectEvents()
        {
            targetPlayer_.DisplayNameChanged -= TargetPlayer_NameChanged;
            targetPlayer_.ReadyStateChanged -= TargetPlayer_ReadyStateChanged;
            targetPlayer_.CommandingModuleChanged -= TargetPlayer_CommandingModuleChanged;
        }

        private void TargetPlayer_NameChanged(object sender, ValueChangedArgs<string> e)
        {
            playerNameText.text = e.newValue;
        }

        private void TargetPlayer_ReadyStateChanged(object sender, ValueChangedArgs<bool> e)
        {

        }

        private void TargetPlayer_CommandingModuleChanged(object sender, ValueChangedArgs<ModuleTypes> e)
        {
            changeModuleButtonText.text = playerModuleText.text = targetPlayer_.CommandingModule.ToString();
        }


        public void ChangeTeam()
        {
            if(targetPlayer_.Team == 1) {
                targetPlayer_.SetTeam(2);
            } else if(targetPlayer_.Team == 2) {
                targetPlayer_.SetTeam(0);
            } else {
                targetPlayer_.SetTeam(1);
            }
        }

        public void ChangeModule()
        {
            int maxModuleIndex = Enum.GetValues(typeof(ModuleTypes)).Length;

            int currentModule = (int)targetPlayer_.CommandingModule;
            currentModule++;
            if(currentModule >= maxModuleIndex) { currentModule = 0; }

            targetPlayer_.SetCommandingModule((ModuleTypes)currentModule);
        }
    }
}

