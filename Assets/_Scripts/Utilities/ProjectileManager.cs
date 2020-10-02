using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProjectileManager
{
    private static ProjectileLibrary library = null;
    private static Dictionary<ProjectileTypes, Pool> pools = new Dictionary<ProjectileTypes, Pool>();

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        library = Resources.Load<ProjectileLibrary>("Libraries/Projectiles/Projectile Library");
        foreach(ProjectileTypes pt in library.projectilePrefabs.Keys) {
            pools.Add(pt, new Pool());
        }
    }

    public static Projectile RequestProjectile(ProjectileTypes type)
    {
        if (!pools.ContainsKey(type)) { return null; }

        Pool pool = pools[type];
        Projectile p = null;
        if(pool.pooledProjectiles.Count > 0) {
            p = pool.pooledProjectiles.Dequeue();
        } else {
            p = GameObject.Instantiate(library.projectilePrefabs[type]).GetComponent<Projectile>();
        }

        p.gameObject.SetActive(true);
        return p;
    }

    public static void PoolProjectile(Projectile p)
    {
        if (!pools.ContainsKey(p.ProjectileType)) {
            pools.Add(p.ProjectileType, new Pool());
        }

        pools[p.ProjectileType].pooledProjectiles.Enqueue(p);
    }

    private class Pool
    {
        public Queue<Projectile> pooledProjectiles = new Queue<Projectile>();
    }
}
