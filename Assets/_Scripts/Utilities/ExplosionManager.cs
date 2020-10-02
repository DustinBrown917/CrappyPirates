using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExplosionManager
{
    private static ExplosionLibrary library = null;
    private static Dictionary<ExplosionTypes, Pool> pools = new Dictionary<ExplosionTypes, Pool>();

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        library = Resources.Load<ExplosionLibrary>("Libraries/Explosions/Explosion Library");
        foreach (ExplosionTypes et in library.explosionPrefabs.Keys) {
            pools.Add(et, new Pool());
        }
    }

    public static Explosion RequestExplosion(ExplosionTypes type)
    {
        if (!pools.ContainsKey(type)) { return null; }

        Pool pool = pools[type];
        Explosion e = null;
        if (pool.pooledExplosions.Count > 0) {
            e = pool.pooledExplosions.Dequeue();
        } else {
            e = GameObject.Instantiate(library.explosionPrefabs[type]).GetComponent<Explosion>();
        }
        
        e.gameObject.SetActive(true);
        return e;
    }

    public static void PoolExplosion(Explosion e)
    {
        if (!pools.ContainsKey(e.ExplosionType)) {
            pools.Add(e.ExplosionType, new Pool());
        }

        pools[e.ExplosionType].pooledExplosions.Enqueue(e);
    }

    private class Pool
    {
        public Queue<Explosion> pooledExplosions = new Queue<Explosion>();
    }
}
