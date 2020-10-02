using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationUI : ModuleUI<NavigationModule>
{
    [SerializeField] private GameObject throttleSliderPrefab = null;
    [SerializeField] private Transform throttleArea = null;

    private List<ThrottleSlider> throttleSliders = new List<ThrottleSlider>();

    protected override void CleanOldModule()
    {
        foreach(ThrottleSlider ts in throttleSliders) {
            ts.SetTarget(null, default);
        }
    }

    protected override void Initialize()
    {
        ThrusterGroups[] throttles = module.GetThrottleGroups();

        for(int i = 0; i < throttles.Length; i++) {
            if(i >= throttleSliders.Count) {
                throttleSliders.Add(Instantiate(throttleSliderPrefab, throttleArea).GetComponentInChildren<ThrottleSlider>());
            }

            throttleSliders[i].SetTarget(module, throttles[i]);
            throttleSliders[i].ThrottleImage.sprite = module.Ship.ShipIcon;                     
        }
    }


}
