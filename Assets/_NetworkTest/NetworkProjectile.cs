using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkProjectile : NetworkBehaviour
{
    public Rigidbody body = null;
    public float lifetime = 15.0f;
    private float currentLifetime = 0;

    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    [ServerCallback]
    private void Update()
    {
        currentLifetime += Time.deltaTime;
        if(currentLifetime >= lifetime) { Destroy(gameObject); }
    }
}
