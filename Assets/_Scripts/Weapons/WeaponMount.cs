using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponMount : MonoBehaviour
{
    [SerializeField] private Batteries battery_ = default;
    public Batteries Battery { get => battery_; }
    [SerializeField] private Weapon[] weapons = new Weapon[0];
    public int WeaponCount { get => weapons.Length; }
    [SerializeField] private float firingArch = 30.0f;
    public float FiringArch { get => firingArch; }
    [SerializeField] private float rotationSpeed = 1.0f;
    private Ship ownerShip = null;

    [SerializeField] private bool invertRotation_ = false;
    public bool InvertRotation { get => invertRotation_; }

    [SerializeField] private float currentRot = 0;
    public float CurrentRotation { get => currentRot; }
    public float DefaultRotation { get => transform.localRotation.eulerAngles.y - currentRot; }

    private float targetRotation_ = 0;
    public float TargetRotation { get => targetRotation_; }

    private Coroutine cr_AutoRotate = null;

    private void Awake()
    {
        weapons = GetComponentsInChildren<Weapon>();

        Transform t = transform;
        while(t != null && ownerShip == null) {
            ownerShip = t.GetComponent<Ship>();
            t = t.transform.parent;
        }

        if(ownerShip == null) {
            Debug.LogError($"Owner ship not found for WeaponMount {GetInstanceID()}.");
        }      
    }

    public void SetTargetRotation(float target)
    {
        targetRotation_ = Mathf.Clamp(target, -firingArch * 0.5f, firingArch * 0.5f);
    }

    public Weapon GetWeapon(int index)
    {
        return weapons[index];
    }

    public void Rotate(bool clockwise, float deltaTime)
    {
        float newRot = 0;
        if (clockwise && currentRot < firingArch * 0.5f) {
            newRot = rotationSpeed * deltaTime;           
        } else if(currentRot > -firingArch * 0.5f) {
            newRot = -rotationSpeed * deltaTime;
        }

        if(newRot != 0) {
            currentRot += newRot;
            transform.Rotate(Vector3.up, newRot, Space.Self);
        }

    }

    public void Rotate(float amount)
    {
        if (amount != 0) {
            currentRot += amount;
            transform.Rotate(Vector3.up, amount, Space.Self);
        }
    }

    public void Fire(GameObject target, float power)
    {
        foreach(Weapon w in weapons) {
            if (w.RequiresTarget && target == null) { return; }
            w.Fire(new Weapon.FiringParameters(target, power));           
        }
    }

    public void StartAutoRotate()
    {
        StopAutoRotate();
        cr_AutoRotate = StartCoroutine(AutoRotate());
    }

    private void StopAutoRotate()
    {
        if(cr_AutoRotate != null) {
            StopCoroutine(cr_AutoRotate);
        }
        cr_AutoRotate = null;
    }

    private IEnumerator AutoRotate()
    {
        while(Mathf.Abs(currentRot - targetRotation_) > 0.1f) {
            bool clockWise = (invertRotation_? -1 : 1) * targetRotation_ > currentRot;

            Rotate(clockWise, Time.deltaTime);
            yield return null;
        }

        cr_AutoRotate = null;

    }
}

public enum Batteries
{
    FORWARD,
    STERN,
    STARBOARD,
    PORT
}