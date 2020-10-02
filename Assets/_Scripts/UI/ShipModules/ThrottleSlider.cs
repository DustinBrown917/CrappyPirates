using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ThrottleSlider : SerializedMonoBehaviour
{
    [SerializeField] private TextMeshProUGUI throttleText = null;
    [SerializeField] private ThrottleImage image_ = null;
    [SerializeField] private Dictionary<ThrusterGroups, Vector3> imageOffsets = new Dictionary<ThrusterGroups, Vector3>();
    public ThrottleImage ThrottleImage { get => image_; }
    private Observable<float> targetObservable;
    private Slider targetSlider = null;
    private NavigationModule navigation = null;
    private ThrusterGroups thrusterGroup = 0;

    private void Awake()
    {
        targetSlider = GetComponent<Slider>();
        targetSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void SetTarget(NavigationModule navigation, ThrusterGroups thrusterGroup)
    {
        if(targetObservable != null) {
            DisconnectEvents();
        }

        this.navigation = navigation;
        this.thrusterGroup = thrusterGroup;
        targetObservable = navigation?.GetThrottle(thrusterGroup);
        throttleText.text = targetSlider.value.ToString("0.0%");
        if (imageOffsets.ContainsKey(thrusterGroup)) {
            ThrottleImage.offset = imageOffsets[thrusterGroup];
        }

        if (targetObservable != null) {
            ConnectEvents();
        }
    }

    private void TargetObservable_ValueChanged(object sender, Observable<float>.ValueChangedArgs e)
    {
        targetSlider.SetValueWithoutNotify(((Observable<float>)sender).value);
        throttleText.text = targetSlider.value.ToString("0.0%");
    }

    private void OnSliderValueChanged(float value)
    {
        navigation.SetThrottle(thrusterGroup, value);
        throttleText.text = targetSlider.value.ToString("0.0%");
    }

    private void ConnectEvents()
    {
        targetObservable.ValueChanged += TargetObservable_ValueChanged;
    }

    private void DisconnectEvents()
    {
        targetObservable.ValueChanged -= TargetObservable_ValueChanged;
    }
}
