using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class WeaponsModule : ShipModule
{
    [SerializeField] private Rigidbody DEBUG_TARGET = null;
    [SerializeField] private Dictionary<Batteries, Battery> batteries = new Dictionary<Batteries, Battery>();

    private Batteries activeBattery_ = default;
    public Batteries ActiveBattery { get => activeBattery_; }

    public event EventHandler<ActiveBatteryChangedArgs> ActiveBatteryChanged;

    public override void Initialize()
    {
        WeaponMount[] foundWeaponMounts = GetComponentsInChildren<WeaponMount>();

        foreach (WeaponMount wm in foundWeaponMounts) {
            if (!batteries.ContainsKey(wm.Battery)) {
                batteries.Add(wm.Battery, new Battery());
                batteries[wm.Battery].wm = this;
            }

            batteries[wm.Battery].weapons.Add(wm);
        }

        foreach(Batteries battery in batteries.Keys) {
            HandleBatteryActiveStateChanged(battery, false);
        }

        base.Initialize();

        SetActiveBattery(Batteries.FORWARD);
    }

    public Batteries[] GetBatteries()
    {
        return batteries.Keys.ToArray();
    }

    public float GetCurrentRotation(Batteries battery)
    {
        if (!batteries.ContainsKey(battery)) { return 0; }

        float max = 0;
        WeaponMount wmWithHighestArch = null;
        foreach(WeaponMount wm in batteries[battery].weapons) {
            if(wm.FiringArch >= max) {
                max = wm.FiringArch;
                wmWithHighestArch = wm;
            }
        }

        return wmWithHighestArch.CurrentRotation;
    }

    public float GetTargetRotation(Batteries battery)
    {
        if (!batteries.ContainsKey(battery)) { return 0; }

        float max = 0;
        WeaponMount wmWithHighestArch = null;
        foreach (WeaponMount wm in batteries[battery].weapons) {
            if (wm.FiringArch >= max) {
                max = wm.FiringArch;
                wmWithHighestArch = wm;
            }
        }

        return wmWithHighestArch.TargetRotation;
    }

    public void SetTargetRotation(Batteries battery, float target)
    {
        if (!batteries.ContainsKey(battery)) { return; }

        batteries[battery].targetRotation = target;
    }

    public void SetActiveBattery(Batteries battery)
    {
        ActiveBatteryChangedArgs args = new ActiveBatteryChangedArgs(activeBattery_, battery);
        HandleBatteryActiveStateChanged(activeBattery_, false);

        activeBattery_ = battery;

        HandleBatteryActiveStateChanged(activeBattery_, true);
        ActiveBatteryChanged?.Invoke(this, args);
    }

    private void HandleBatteryActiveStateChanged(Batteries battery, bool active)
    {
        if (batteries.ContainsKey(battery)) {
            foreach (WeaponMount wm in batteries[battery].weapons) {
                for (int i = 0; i < wm.WeaponCount; i++) {
                    wm.GetWeapon(i).ToggleTargetingLine(active);
                }
            }
        }
    }

    public List<WeaponMount> GetWeaponMountsInBattery(Batteries battery)
    {
        if (batteries.ContainsKey(battery)) {
            return batteries[battery].weapons;
        } else { return new List<WeaponMount>(); }
    }

    public Battery GetBattery(Batteries battery)
    {
        if (batteries.ContainsKey(battery)) {
            return batteries[battery];
        } else { return null; }
    }

    public WeaponMount GetWeaponMountInBattery(Batteries battery, int index)
    {
        if (batteries.ContainsKey(battery)) {
            return batteries[battery].weapons[index];
        } else { return null; }
    }

    public void FireBattery(Batteries battery, Rigidbody target, float power)
    {
        if (batteries.ContainsKey(battery)) {
            //batteries[battery].Fire(target, power);
            batteries[battery].Fire(DEBUG_TARGET, power);
        }
    }


    public void ManualRotateBattery(Batteries battery, bool clockWise, float deltaTime)
    {
        if (batteries.ContainsKey(battery)) {
            foreach (WeaponMount wm in batteries[battery].weapons) {
                wm.Rotate(clockWise, deltaTime);
            }
        }
    }

    public float GetFiringArch(Batteries battery)
    {
        if (!batteries.ContainsKey(battery)) { return 0; }

        return batteries[battery].fireArch;
    }

    protected override void SetUpUI()
    {
        WeaponsUI ui = Instantiate(UIModulePrefab, ShipUICanvas.Canvas.transform).GetComponent<WeaponsUI>();
        if (ui) {
            ui.SetModule(this);
        }
    }

    protected override IEnumerator LocalPlayerFixedUpdate()
    {
        yield return null;
    }

    protected override IEnumerator LocalPlayerUpdate()
    {
        while (true) {
            if (Input.GetKey(KeyCode.Space)) {
                FireBattery(activeBattery_, null, 1.0f);
            }

            if (Input.GetKey(KeyCode.L)) {
                batteries[activeBattery_].targetRotation += batteries[activeBattery_].rotationSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.K)) {
                batteries[activeBattery_].targetRotation -= batteries[activeBattery_].rotationSpeed * Time.deltaTime;
            }

            yield return null;
        }
    }

    public class ActiveBatteryChangedArgs : EventArgs
    {
        public Batteries oldBattery = default;
        public Batteries newBattery = default;

        public ActiveBatteryChangedArgs(Batteries oldBattery, Batteries newBattery)
        {
            this.oldBattery = oldBattery;
            this.newBattery = newBattery;
        }
    }

    [System.Serializable]
    public class Battery
    {
        [NonSerialized] public WeaponsModule wm = null;
        public List<WeaponMount> weapons = new List<WeaponMount>();
        [NonSerialized] public Coroutine cr_AutoRotate = null;
        public float rotationSpeed = 10;
        [NonSerialized] public Observable<float> currentRotation = new Observable<float>(0.0f);
        private Observable<float> targetRotation_ = new Observable<float>(0.0f);
        public float targetRotation { get => targetRotation_.value; set => SetTargetRotation(value); }
        public float fireArch = 90.0f;
        public float Facing
        {
            get {
                float sum = 0;
                for (int i = 0; i < weapons.Count; i++) {
                    sum += weapons[i].DefaultRotation;
                }
                return sum / weapons.Count;
            }
        }

        public event EventHandler<Observable<float>.ValueChangedArgs> TargetRotationChanged
        {
            add {
                targetRotation_.ValueChanged += value;
            }
            remove {
                targetRotation_.ValueChanged -= value;
            }
        }

        public Battery()
        {
            targetRotation_.ValueChanged += TargetRotation__ValueChanged;
        }

        private void TargetRotation__ValueChanged(object sender, Observable<float>.ValueChangedArgs e)
        {
            if(cr_AutoRotate == null) {
                StartAutoRotate();
            }   
        }

        private void SetTargetRotation(float angle)
        {
            targetRotation_.value = Mathf.Clamp(angle, -fireArch * 0.5f, fireArch * 0.5f);
        }

        public void StartAutoRotate()
        {
            StopAutoRotate();
            cr_AutoRotate = wm.StartCoroutine(AutoRotate());
        }

        private void StopAutoRotate()
        {
            if (cr_AutoRotate != null) {
                wm.StopCoroutine(cr_AutoRotate);
            }
            cr_AutoRotate = null;
        }

        private IEnumerator AutoRotate()
        {
            while (Mathf.Abs(currentRotation.value - targetRotation_.value) > 0.05f) {

                float rot = rotationSpeed * Time.deltaTime * ((targetRotation_.value > currentRotation.value) ? 1 : -1); 

                Rotate(rot);
                
                yield return null;
            }

            cr_AutoRotate = null;
        }

        private void Rotate(float amount)
        {
            currentRotation.value += amount;
            foreach(WeaponMount wm in weapons) {
                wm.Rotate(amount);
            }
        }

        public void Fire(Rigidbody target, float power)
        {
            foreach (WeaponMount wm in weapons) {
                wm.Fire(target, power);
            }
        }
    }
}
