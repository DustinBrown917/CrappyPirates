using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrappyPirates;

public static class ProjectileManager
{
    private static ProjectileLibrary library = null;
    private static Dictionary<ProjectileTypes, Pool> pools = new Dictionary<ProjectileTypes, Pool>();

    private static NetworkBehaviour networkBehaviour = null;

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

            if (NetworkServer.active) {
                p = GameObject.Instantiate(library.projectilePrefabs[type]).GetComponent<Projectile>();
                NetworkServer.Spawn(p.gameObject);
            } else {
                p = GameObject.Instantiate(library.projectilePrefabs[type]).GetComponent<Projectile>();
            }
        }

        p.Active = true;
        return p;
    }

    public static void PoolProjectile(Projectile p)
    {
        if (!pools.ContainsKey(p.ProjectileType)) {
            pools.Add(p.ProjectileType, new Pool());
        }

        pools[p.ProjectileType].pooledProjectiles.Enqueue(p);
        p.Active = false;
    }

    private class Pool
    {
        public Queue<Projectile> pooledProjectiles = new Queue<Projectile>();
    }
}
