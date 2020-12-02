using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public abstract class ShipModule : MonoBehaviour
{
    [SerializeField] protected GameObject UIModulePrefab = null;

    protected Ship ship_ = null;
    public Ship Ship { get => ship_; }

    [SerializeField] protected bool isLocalPlayerControlled_ = true;
    public bool IsLocalPlayerControlled { get => isLocalPlayerControlled_; set => SetIsLocalPlayerControlled(value); }

    private bool initialized_ = false;

    private Coroutine cr_LocalPlayerFixedUpdate = null;
    private Coroutine cr_LocalPlayerUpdate = null;

    protected virtual void Awake()
    {
        ship_ = GetComponent<Ship>();
    }

    protected virtual void Start() { }

    public virtual void Initialize()
    {
        if (isLocalPlayerControlled_) {
            StartLocalPlayerFixedUpdate();
            StartLocalPlayerUpdate();
            SetUpUI();
        }

        initialized_ = true;
    }

    protected abstract void SetUpUI();

    protected virtual void SetIsLocalPlayerControlled(bool b)
    {
        if(b == isLocalPlayerControlled_) { return; }
        isLocalPlayerControlled_ = b;

        if (initialized_) {
            if (b) {
                StartLocalPlayerFixedUpdate();
                StartLocalPlayerUpdate();
            } else {
                StopLocalPlayerFixedUpdate();
                StopLocalPlayerUpdate();
            }
        }
    }

    protected void StartLocalPlayerFixedUpdate()
    {
        StopLocalPlayerFixedUpdate();
        cr_LocalPlayerFixedUpdate = StartCoroutine(LocalPlayerFixedUpdate());
    }

    protected void StopLocalPlayerFixedUpdate()
    {
        if(cr_LocalPlayerFixedUpdate != null) {
            StopCoroutine(cr_LocalPlayerFixedUpdate);
        }
        cr_LocalPlayerFixedUpdate = null;
    }

    protected void StartLocalPlayerUpdate()
    {
        StopLocalPlayerUpdate();
        cr_LocalPlayerUpdate = StartCoroutine(LocalPlayerUpdate());
    }

    protected void StopLocalPlayerUpdate()
    {
        if (cr_LocalPlayerUpdate != null) {
            StopCoroutine(cr_LocalPlayerUpdate);
        }
        cr_LocalPlayerUpdate = null;
    }

    protected abstract IEnumerator LocalPlayerFixedUpdate();
    protected abstract IEnumerator LocalPlayerUpdate();

}
