using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Explosion Library", menuName = "New Explosion Library")]
public class ExplosionLibrary : SerializedScriptableObject
{
    public Dictionary<ExplosionTypes, GameObject> explosionPrefabs = new Dictionary<ExplosionTypes, GameObject>();
}
