using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile Library", menuName = "New Projectile Library")]
public class ProjectileLibrary : SerializedScriptableObject
{
    public Dictionary<ProjectileTypes, GameObject> projectilePrefabs = new Dictionary<ProjectileTypes, GameObject>();
}
