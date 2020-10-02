using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Weapon : MonoBehaviour
{
    public enum FireBlockers
    {
        COOLDOWN,
        RELOAD
    }

    protected Transform target = null;
    [SerializeField] protected ProjectileTypes projectileType;
    [SerializeField] protected Transform launchPosition = null;
    [SerializeField] protected Collider[] collidersToIgnore = new Collider[0];
    [SerializeField] protected Rigidbody relativeBody = null;
    [SerializeField] protected Sprite weaponIcon = null;
    public Sprite WeaponIcon { get => weaponIcon; }

    [Space]
    [Title("Targeting Line")]
    [SerializeField] protected LineRenderer targetingLine = null;
    [SerializeField] private Color readyToFireColor_ = Color.green;
    public Color ReadyToFireColor { get => readyToFireColor_; }
    [SerializeField] private Color cooldownColor_ = Color.yellow;
    public Color CooldownColor { get => cooldownColor_; }
    [SerializeField] private Color reloadingColor_ = Color.red;
    public Color ReloadingColor { get => reloadingColor_; }
    [SerializeField] private Color disabledColor_ = Color.grey;
    public Color DisabledColor { get => disabledColor_; }
    [SerializeField] private float disabledLineLength = 2.0f;
    [SerializeField] private float enabledLineLength = 8.0f;
    private bool targetingLineEnabled = true;
    [Space]

    [SerializeField] protected float maxPower = 1.0f;
    [SerializeField] protected float minPower = 0.0f;

    [SerializeField] protected int magazineSize = 10;
    public int MagazineSize { get => magazineSize; }

    protected Observable<int> currentShotsInMagazine_ = new Observable<int>(0);
    public int CurrentShotsInMagazine { get => currentShotsInMagazine_.value; }
    public event EventHandler<Observable<int>.ValueChangedArgs> CurrentShotInMagazinChanged
    {
        add {
            currentShotsInMagazine_.ValueChanged += value;
        }
        remove {
            currentShotsInMagazine_.ValueChanged -= value;
        }
    }

    [SerializeField] protected float reloadTime = 3.0f;
    public float ReloadTime { get => reloadTime; }
    
    protected Observable<float> reloadTimeRemaining = new Observable<float>(0.0f);
    public float ReloadTimeRemaining { get => reloadTimeRemaining.value; }
    public event EventHandler<Observable<float>.ValueChangedArgs> ReloadTimeRemainingChanged
    {
        add {
            reloadTimeRemaining.ValueChanged += value;
        }
        remove {
            reloadTimeRemaining.ValueChanged -= value;
        }
    }

    [SerializeField] protected float cooldownBetweenShots = 0.5f;
    public float CooldownBetweenShots { get => cooldownBetweenShots; }
    protected Observable<float> cooldownTimeRemaining = new Observable<float>(0.0f);
    public float CooldownTimeRemaining { get => cooldownTimeRemaining.value; }
    public event EventHandler<Observable<float>.ValueChangedArgs> CooldownTimeRemainingChanged
    {
        add {
            cooldownTimeRemaining.ValueChanged += value;
        }
        remove {
            cooldownTimeRemaining.ValueChanged -= value;
        }
    }

    protected bool CanFire
    {
        get {
            int totalBlockers = 0;
            foreach(FireBlockers b in fireBlockers.Keys) {
                totalBlockers += fireBlockers[b];
            }
            return totalBlockers == 0;
        }
    }

    [SerializeField] protected bool requiresTarget_ = false;
    public bool RequiresTarget { get => requiresTarget_; }

    protected Dictionary<FireBlockers, int> fireBlockers = new Dictionary<FireBlockers, int>();

    private Coroutine cr_Reloading = null;
    private Coroutine cr_CoolingDown = null;

    protected virtual void Awake()
    {
        currentShotsInMagazine_.value = magazineSize;
    }

    public abstract void Fire(FiringParameters parameters);

    public virtual void SetTarget(Transform target)
    {
        this.target = target;
    }

    public class FiringParameters
    {
        public Rigidbody target = null;
        public float power = 0.0f;

        public FiringParameters(Rigidbody target, float power)
        {
            this.target = target;
            this.power = power;
        }
    }

    public void AddFireBlocker(FireBlockers blocker)
    {
        if (!fireBlockers.ContainsKey(blocker)) {
            fireBlockers.Add(blocker, 0);
        }

        fireBlockers[blocker]++;
    }

    public void RemoveFireBlocker(FireBlockers blocker)
    {
        if (!fireBlockers.ContainsKey(blocker) || fireBlockers[blocker] == 0) {
            return;
        }
        fireBlockers[blocker]--;
    }

    public void ToggleTargetingLine(bool active)
    {
        if(targetingLineEnabled != active) {
            targetingLineEnabled = active;
            if (targetingLineEnabled) {
                targetingLine.SetPosition(1, new Vector3(0, 0, enabledLineLength));             
            } else {
                targetingLine.SetPosition(1, new Vector3(0, 0, disabledLineLength));
            }
            UpdateTargetingLineColor();
        }
    }

    private void UpdateTargetingLineColor()
    {
        if (!targetingLineEnabled) {
            SetTargetingLineColor(disabledColor_);
        } else if(reloadTimeRemaining.value > 0) {
            SetTargetingLineColor(reloadingColor_);
        } else if (cooldownTimeRemaining.value > 0) {
            SetTargetingLineColor(cooldownColor_);
        } else {
            SetTargetingLineColor(readyToFireColor_);
        }
    }

    private void SetTargetingLineColor(Color color)
    {
        Color startColor = new Color(color.r, color.g, color.b, targetingLine.startColor.a);
        Color endColor = new Color(color.r, color.g, color.b, targetingLine.endColor.a);
        targetingLine.startColor = startColor;
        targetingLine.endColor = endColor;
    }

    public void ManualReload()
    {
        if(reloadTimeRemaining.value == 0 && CurrentShotsInMagazine < MagazineSize) {
            StartReloading();
        }
    }

    protected void StartReloading()
    {
        StopReloading();
        cr_Reloading = StartCoroutine(Reload());
    }

    protected void StopReloading()
    {
        if(cr_Reloading != null) {
            RemoveFireBlocker(FireBlockers.RELOAD);
            StopCoroutine(cr_Reloading);
            cr_Reloading = null;
        }
    }

    private IEnumerator Reload()
    {      
        reloadTimeRemaining.value = reloadTime;
        AddFireBlocker(FireBlockers.RELOAD);
        UpdateTargetingLineColor();

        while (reloadTimeRemaining.value > 0) {
            reloadTimeRemaining.value -= Time.deltaTime;
            yield return null;
        }

        reloadTimeRemaining.value = 0;

        currentShotsInMagazine_.value = magazineSize;

        UpdateTargetingLineColor();
        RemoveFireBlocker(FireBlockers.RELOAD);
        cr_Reloading = null;
    }

    protected void StartCooldown()
    {
        StopCooldown();
        cr_CoolingDown = StartCoroutine(Cooldown());
    }

    protected void StopCooldown()
    {
        if (cr_CoolingDown != null) {
            StopCoroutine(cr_CoolingDown);
            RemoveFireBlocker(FireBlockers.COOLDOWN);
            cr_CoolingDown = null;
        }
    }

    private IEnumerator Cooldown()
    {
        cooldownTimeRemaining.value = cooldownBetweenShots;
        AddFireBlocker(FireBlockers.COOLDOWN);
        UpdateTargetingLineColor();

        while (cooldownTimeRemaining.value > 0) {
            cooldownTimeRemaining.value -= Time.deltaTime;
            yield return null;
        }

        cooldownTimeRemaining.value = 0;
        RemoveFireBlocker(FireBlockers.COOLDOWN);
        UpdateTargetingLineColor();
        cr_CoolingDown = null;
    }
}
