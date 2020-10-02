using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CrappyPirates
{
    public class PlayerNameInput : MonoBehaviour
    {
        public const string PP_PLAYER_NAME_TAG = "PlayerName";
        [SerializeField] private TMP_InputField inputField = null;
        [SerializeField] private Button confirmButton = null;

        private void Awake()
        {
            inputField.text = PlayerPrefs.GetString(PP_PLAYER_NAME_TAG, string.Empty);
            ValidateName(inputField.text);
        }

        public void ValidateName(string nm)
        {
            if (string.IsNullOrWhiteSpace(nm)) { confirmButton.interactable = false; } else { confirmButton.interactable = true; }
        }

        public void ConfirmName()
        {
            PlayerPrefs.SetString(PP_PLAYER_NAME_TAG, inputField.text);
        }
    }
}

