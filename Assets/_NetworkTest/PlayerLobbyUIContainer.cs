using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CrappyPirates
{
    public class PlayerLobbyUIContainer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText = null;
        [SerializeField] private TextMeshProUGUI playerReadyText = null;

        private NetworkPlayer_Lobby targetPlayer_ = null;
        public NetworkPlayer_Lobby TargetPlayuer { get => targetPlayer_; }

        public string TargetPlayerDisplayName { get => targetPlayer_.DisplayName; }

        public void SetTargetPlayer(NetworkPlayer_Lobby p)
        {
            if(targetPlayer_ != null) {
                DisconnectEvents();
            }

            targetPlayer_ = p;

            if(targetPlayer_ != null) {
                playerNameText.text = p.DisplayName;
                if (p.IsReady) {
                    playerReadyText.text = "Ready!";
                } else {
                    playerReadyText.text = "Not ready.";
                }
                ConnectEvents();
            }

            Debug.Log("Target player set");
        }

        private void ConnectEvents()
        {
            targetPlayer_.DisplayNameChanged += TargetPlayer_NameChanged;
            targetPlayer_.ReadyStateChanged += TargetPlayer_ReadyStateChanged;
        }

        private void DisconnectEvents()
        {
            targetPlayer_.DisplayNameChanged -= TargetPlayer_NameChanged;
            targetPlayer_.ReadyStateChanged -= TargetPlayer_ReadyStateChanged;
        }

        private void TargetPlayer_NameChanged(object sender, ValueChangedArgs<string> e)
        {
            playerNameText.text = e.newValue;
        }

        private void TargetPlayer_ReadyStateChanged(object sender, ValueChangedArgs<bool> e)
        {
            if (e.newValue) {
                playerReadyText.text = "Ready!";
            } else {
                playerReadyText.text = "Not ready.";
            }
        }
    }
}

