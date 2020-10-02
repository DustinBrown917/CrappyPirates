using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponsUI : ModuleUI<WeaponsModule>
{
    [SerializeField] private GameObject buttonPrefab = null;
    [SerializeField] private Transform batteryButtonsTransform = null;
    [SerializeField] private RadialDial fireArchDial = null;
    [SerializeField] private Color activeButtonColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.gray;
    [SerializeField] private Transform weaponIconContainer = null;

    private List<WeaponIcon> weaponIcons = new List<WeaponIcon>();
    private WeaponsModule.Battery activeBattery = null;
    private Dictionary<Batteries, Button> batteryButtons = new Dictionary<Batteries, Button>();

    protected override void CleanOldModule()
    {
        module.ActiveBatteryChanged -= Module_ActiveBatteryChanged;
    }

    protected override void Initialize()
    {
        Batteries[] batteries = module.GetBatteries();
        Button[] buttons = new Button[batteries.Length];
        this.batteryButtons.Clear();

        for(int i = 0; i < batteries.Length; i++) {
            Batteries b = batteries[i];
            batteryButtons.Add(b, Instantiate(buttonPrefab, batteryButtonsTransform).GetComponent<Button>());
            batteryButtons[b].GetComponentInChildren<TextMeshProUGUI>().text = batteries[i].FormattedString();

            batteryButtons[b].onClick.AddListener(delegate { module.SetActiveBattery(b); });
            
            batteryButtons[b].gameObject.SetActive(true);

        }
        module.ActiveBatteryChanged += Module_ActiveBatteryChanged;
        fireArchDial.ShipImage.sprite = module.Ship.ShipIcon;
        StartCoroutine(InitAfterUI());

    }

    private void Module_ActiveBatteryChanged(object sender, WeaponsModule.ActiveBatteryChangedArgs e)
    {
        UpdateForActiveBattery();
    }

    private void UpdateForActiveBattery()
    {
        if(activeBattery != null) {
            activeBattery.currentRotation.ValueChanged -= CurrentRotation_ValueChanged;
        }
        foreach(Batteries b in batteryButtons.Keys) {
            batteryButtons[b].GetComponent<Image>().color = inactiveColor;
        }

        batteryButtons[module.ActiveBattery].GetComponent<Image>().color = activeButtonColor;

        activeBattery = module.GetBattery(module.ActiveBattery);

        fireArchDial.SetFiringArch(activeBattery.fireArch);
            
        fireArchDial.SetArchBaseRotation(activeBattery.Facing);
        fireArchDial.SetValueWithoutNotify(-activeBattery.targetRotation); //Have to invert it because cw on 3d y axis = ccw on 2d z axis
        fireArchDial.SetSecondaryHandleRotation(-activeBattery.currentRotation.value); //Have to invert it because cw on 3d y axis = ccw on 2d z axis
        fireArchDial.onValueChanged.RemoveAllListeners();
        fireArchDial.onValueChanged.AddListener(delegate { 
            RotateBattery(fireArchDial.value); 
        });

        activeBattery.TargetRotationChanged += TargetRotation_ValueChanged;
        activeBattery.currentRotation.ValueChanged += CurrentRotation_ValueChanged;
        SetUpWeaponIcons();
    }

    private void TargetRotation_ValueChanged(object sender, Observable<float>.ValueChangedArgs e)
    {
        fireArchDial.SetValueWithoutNotify(-activeBattery.targetRotation);
    }

    private void CurrentRotation_ValueChanged(object sender, Observable<float>.ValueChangedArgs e)
    {
        fireArchDial.SetSecondaryHandleRotation(-activeBattery.currentRotation.value);
    }

    public void FireActiveBattery()
    {
        module.FireBattery(module.ActiveBattery, null, 1.0f);
    }

    public void RotateBattery(float value)
    {
        if (!module) { return; }
        activeBattery.targetRotation = value;
    }

    private IEnumerator InitAfterUI()
    {
        yield return new WaitForEndOfFrame();
        UpdateForActiveBattery();
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }
    
    public void SetUpWeaponIcons()
    {
        int iconIndex = -1;
        for(int i = 0; i < activeBattery.weapons.Count; i++) {

            for(int j = 0; j < activeBattery.weapons[i].WeaponCount; j++) {
                iconIndex++;
                if (iconIndex >= weaponIcons.Count) {
                    weaponIcons.Add(Instantiate(weaponIconContainer.transform.GetChild(0).gameObject, weaponIconContainer).GetComponent<WeaponIcon>());
                }
                weaponIcons[iconIndex].SetTargetWeapon(activeBattery.weapons[i].GetWeapon(j));
                weaponIcons[iconIndex].gameObject.SetActive(true);
            }
        }

        for(int i = iconIndex + 1; i < weaponIcons.Count; i++) {
            weaponIcons[i].SetTargetWeapon(null);
            weaponIcons[i].gameObject.SetActive(false);
        }
    }
}
