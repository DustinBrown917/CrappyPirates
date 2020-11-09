using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoronoiLib;
using VoronoiLib.Structures;

public class ForuneSiteMono : MonoBehaviour
{
    public FortuneSite GetSite()
    {
        return new FortuneSite(transform.position.x, transform.position.y);
    } 
}
