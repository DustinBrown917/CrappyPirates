using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityProvider : MonoBehaviour
{

    [SerializeField] private float maxGravitationalForce_ = 1.0f;
    public float MaxGravitationalForce { get => maxGravitationalForce_; }

    [SerializeField] private float influenceRadius_ = 1.0f;
    public float InfluenceRadius { get => influenceRadius_; }

    // Start is called before the first frame update
    void Start()
    {
        RegisterWithGravityManager();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(transform.position, InfluenceRadius);
    }

    private void OnDestroy()
    {
        DeregisterWithGravityManager();
    }

    public void RegisterWithGravityManager()
    {
        GravityManager.AddGravityProvider(this);
    }

    public void DeregisterWithGravityManager()
    {
        GravityManager.RemoveGravityProvider(this);
    }
}
