using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private ExplosionTypes explosionType_ = default;
    public ExplosionTypes ExplosionType { get => explosionType_; }

    private ParticleSystem[] systems = null;

    private void Awake()
    {
        systems = GetComponentsInChildren<ParticleSystem>();
    }

    public void Explode()
    {
        foreach(ParticleSystem p in systems) {
            p.Play();
        }
    }

    private void Update()
    {
        if (!systems[0].IsAlive(true)) {
            foreach(ParticleSystem ps in systems) {
                ps.Stop();
            }
            ExplosionManager.PoolExplosion(this);
            gameObject.SetActive(false);
        }
    }
}
