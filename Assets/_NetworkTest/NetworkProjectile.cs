using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkProjectile : NetworkBehaviour
{
    public Rigidbody body = null;
    public float lifetime = 15.0f;
    private float currentLifetime = 0;
    float damage = 5.0f;

    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        Health h = collision.gameObject.GetComponent<Health>();
        if (h) {
            h.Damage(damage);
        }

        Destroy(gameObject);
    }

    [ServerCallback]
    private void Update()
    {
        currentLifetime += Time.deltaTime;
        if(currentLifetime >= lifetime) { Destroy(gameObject); }
    }
}
