using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponIcon : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image baseIcon = null;
    [SerializeField] private Image reloadIcon = null;
    [SerializeField] private Image cooldownIcon = null;
    [SerializeField] private Slider ammoIndicatorSlider = null;

    private Weapon targetWeapon = null;

    public void SetTargetWeapon(Weapon w)
    {
        if(targetWeapon != null) {
            DisconnectEvents();
        }

        targetWeapon = w;
        baseIcon.sprite = cooldownIcon.sprite = reloadIcon.sprite = targetWeapon?.WeaponIcon;

        if(targetWeapon != null) {
            ConnectEvents();
            cooldownIcon.fillAmount = targetWeapon.CooldownTimeRemaining / targetWeapon.CooldownBetweenShots;
            reloadIcon.fillAmount = targetWeapon.ReloadTimeRemaining / targetWeapon.ReloadTime;
            ammoIndicatorSlider.maxValue = targetWeapon.MagazineSize;
            ammoIndicatorSlider.value = targetWeapon.CurrentShotsInMagazine;
        } else {
            cooldownIcon.fillAmount = reloadIcon.fillAmount = 0.0f;
        }
    }

    private void ConnectEvents()
    {
        targetWeapon.CooldownTimeRemainingChanged += TargetWeapon_CooldownTimeRemainingChanged;
        targetWeapon.ReloadTimeRemainingChanged += TargetWeapon_ReloadTimeRemainingChanged;
        targetWeapon.CurrentShotInMagazinChanged += TargetWeapon_CurrentShotInMagazinChanged;
    }

    private void TargetWeapon_CurrentShotInMagazinChanged(object sender, Observable<int>.ValueChangedArgs e)
    {
        ammoIndicatorSlider.value = targetWeapon.CurrentShotsInMagazine;
    }

    private void TargetWeapon_ReloadTimeRemainingChanged(object sender, Observable<float>.ValueChangedArgs e)
    {
        reloadIcon.fillAmount = targetWeapon.ReloadTimeRemaining / targetWeapon.ReloadTime;
    }

    private void TargetWeapon_CooldownTimeRemainingChanged(object sender, Observable<float>.ValueChangedArgs e)
    {
        cooldownIcon.fillAmount = targetWeapon.CooldownTimeRemaining / targetWeapon.CooldownBetweenShots;
    }

    private void DisconnectEvents()
    {
        targetWeapon.CooldownTimeRemainingChanged -= TargetWeapon_CooldownTimeRemainingChanged;
        targetWeapon.ReloadTimeRemainingChanged -= TargetWeapon_ReloadTimeRemainingChanged;
        targetWeapon.CurrentShotInMagazinChanged -= TargetWeapon_CurrentShotInMagazinChanged;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(targetWeapon != null) {
            targetWeapon.ManualReload();
        }
    }
}
