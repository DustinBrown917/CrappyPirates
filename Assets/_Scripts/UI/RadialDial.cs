using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RadialDial : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TextMeshProUGUI label = null;
    [SerializeField] private Image shipImage = null;
    [SerializeField] private Image archImage = null;
    [SerializeField] private Transform handleAnchor = null;
    [SerializeField] private Transform secondaryHandleAnchor = null;
    [SerializeField] private Transform archAnchor = null;
    [SerializeField] private float archBaseRotation = 0.0f;

    [SerializeField] private float value_;
    public float value { get => -value_; set => SetValue(value); }

    public Image ShipImage { get => shipImage; }

    private float firingArch = 0.0f;
    private float minFiringAngle = 0.0f;
    private float maxFiringAngle = 0.0f;

    private Coroutine cr_MouseSetValue = null;

    public UnityEvent<float> onValueChanged;



    public void SetValue(float value)
    {
        SetValueWithoutNotify(value);
        onValueChanged?.Invoke(value);
    }

    public void SetValueWithoutNotify(float angle)
    {
        value_ = angle;
        handleAnchor.localRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Clamp(angle, minFiringAngle, maxFiringAngle));
        label.text = value_.ToString("0.00");
    }

    public void SetSecondaryHandleRotation(float angle)
    {
        secondaryHandleAnchor.localRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Clamp(angle, minFiringAngle, maxFiringAngle));
    }

    public void SetArchBaseRotation(float rotation)
    {
        archAnchor.localRotation = Quaternion.Euler(new Vector3(0, 0, -rotation));
        UpdateArchRotation();
    }

    public void UpdateArchRotation()
    {
        archImage.transform.localRotation = Quaternion.Euler(0, 0, (archImage.fillAmount * 0.5f * 360.0f));
    }

    public void SetFiringArch(float amount)
    {
        if (amount < 0) { amount = 0; }
        firingArch = amount;
        minFiringAngle = -firingArch * 0.5f;
        maxFiringAngle = firingArch * 0.5f;
        archImage.fillAmount = amount/360.0f;
        UpdateArchRotation();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.GetMouseButtonDown(0)) {
            StartMouseSetValue();
        }
    }

    private void StartMouseSetValue()
    {
        StopMouseSetValue();
        cr_MouseSetValue = StartCoroutine(MouseSetValue());
    }

    private void StopMouseSetValue()
    {
        if(cr_MouseSetValue != null) {
            StopCoroutine(cr_MouseSetValue);
        }
        cr_MouseSetValue = null;
    }

    //HandleAnchor's rotation will always be between 0 and -firingArch
    //The archImages rotation.euler.z is the most ccw it can be. This is handleAnchor 0.
    private IEnumerator MouseSetValue()
    {
        while (true) {
            Vector2 mousePosRelativeToHandle = Input.mousePosition - handleAnchor.position;
            float angle = Mathf.Clamp(Vector2.SignedAngle(archAnchor.up, mousePosRelativeToHandle), minFiringAngle, maxFiringAngle);

            SetValue(angle);

            if (Input.GetMouseButtonUp(0)) {
                StopMouseSetValue();
            }

            yield return null;
        }
    }
}
