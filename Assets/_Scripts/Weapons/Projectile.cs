using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;


[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Projectile : NetworkBehaviour
{
    [SerializeField] private ProjectileTypes projectileType_ = default;
    [SerializeField] private ExplosionTypes explosionType = default;
    public ProjectileTypes ProjectileType { get => projectileType_; }
    [SerializeField] private float ignoreLauncherColliderDuration = 1.0f;
    private Coroutine cr_IgnoreLauncherCollider = null;
    private Collider[] launcherColliders = new Collider[0];
    private Collider myCollider = null;

    private Rigidbody body_ = null;
    public Rigidbody Body { get => body_; }

    [SyncVar(hook = nameof(HandleEnabledChanged))]
    private bool active_ = false;
    public bool Active { get => active_; set => active_ = value; }

    protected virtual void Awake()
    {
        myCollider = GetComponent<Collider>();
        body_ = GetComponent<Rigidbody>();
    }

    public void HandleEnabledChanged(bool oldValue, bool newValue)
    {
        Debug.Log(newValue);
        gameObject.SetActive(newValue);
    }

    public virtual void Arm(Rigidbody target, params Collider[] launcherColliders)
    {
        this.launcherColliders = launcherColliders;
        StartIgnoringLaunchCollider(ignoreLauncherColliderDuration);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        HandleImpact();
    }

    protected virtual void HandleImpact()
    {
        Explosion e = ExplosionManager.RequestExplosion(explosionType);
        e.transform.position = transform.position;
        e.Explode();
        gameObject.SetActive(false);
        ResetClean();
    }

    protected virtual void ResetClean()
    {
        Body.velocity = new Vector3();
        Body.angularVelocity = new Vector3();
        ProjectileManager.PoolProjectile(this);
        StopIgnoringLaunchColliders();
    }

    private void StartIgnoringLaunchCollider(float duration)
    {
        gameObject.SetActive(true);
        StopIgnoringLaunchColliders();

        cr_IgnoreLauncherCollider = StartCoroutine(IgnoreLauncherCollider(duration));
    }

    private void StopIgnoringLaunchColliders()
    {
        if(cr_IgnoreLauncherCollider != null) {
            StopCoroutine(cr_IgnoreLauncherCollider);
            cr_IgnoreLauncherCollider = null;
        }

        for (int i = 0; i < launcherColliders.Length; i++) {
            Physics.IgnoreCollision(myCollider, launcherColliders[i], false);
        }
    }

    private IEnumerator IgnoreLauncherCollider(float ignoreLauncherColliderDuration)
    {
        for (int i = 0; i < launcherColliders.Length; i++) {
            Physics.IgnoreCollision(myCollider, launcherColliders[i], true);
        }

        yield return new WaitForSeconds(ignoreLauncherColliderDuration);

        for (int i = 0; i < launcherColliders.Length; i++) {
            Physics.IgnoreCollision(myCollider, launcherColliders[i], false);
        }

        cr_IgnoreLauncherCollider = null;
    }
}
