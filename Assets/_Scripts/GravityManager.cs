using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    #region Statics
    private static GravityManager instance_ = null;

    private static HashSet<GravityReceiver> receivers = new HashSet<GravityReceiver>();
    private static HashSet<GravityProvider> providers = new HashSet<GravityProvider>();

    private static float globalGravityFactor = 1.0f;

    public static void AddGravityReceiver(GravityReceiver receiver)
    {
        receivers.Add(receiver);
    }

    public static void RemoveGravityReceiver(GravityReceiver receiver)
    {
        receivers.Remove(receiver);
    }

    public static void AddGravityProvider(GravityProvider provider)
    {
        providers.Add(provider);
    }

    public static void RemoveGravityProvider(GravityProvider provider)
    {
        providers.Remove(provider);
    }

    public static void UpdateGravity()
    {
        foreach(GravityProvider p in providers) {
            if (!p.enabled) { continue; }
            foreach(GravityReceiver r in receivers) {
                if (!r.enabled) { continue; }

                Vector3 rawDirection = p.transform.position - r.transform.position;
                Vector3 direction = rawDirection.normalized;
                float distance = rawDirection.magnitude;

                if(distance <= 0.0001f) { continue; }

                r.Body.AddForce(p.MaxGravitationalForce * (1 - Mathf.Clamp01(distance / p.InfluenceRadius)) * direction * r.GravityInfluenceFactor * globalGravityFactor);
            }
        }
    }
    #endregion

    [SerializeField] private float globalGravityFactor_ = 1.0f;

    private void Awake()
    {
        if(instance_ == null) {
            instance_ = this;
        } else {
            Destroy(gameObject);
            return;
        }

        GravityManager.globalGravityFactor = globalGravityFactor_;
    }

    private void FixedUpdate()
    {
        UpdateGravity();
    }

    private void OnDestroy()
    {
        if(instance_ == this) {
            instance_ = null;
        }
    }
}
