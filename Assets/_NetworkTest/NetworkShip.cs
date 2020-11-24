﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrappyPirates;

public class NetworkShip : NetworkBehaviour, ICameraFocusObject
{
    public Rigidbody body = null;
    public float acceleration = 100.0f;
    public float angularAcceleration = 1.0f;

    public GameObject projectilePrefab = null;

    public Transform[] forwardShootSpawnPoints = null;
    public Transform[] portShootSpawnPoints = null;
    public Transform[] starboardShootSpawnPoints = null;
    public Transform[] rearShootSpawnPoints = null;

    [SyncVar]
    public int team = 0;
    private int localPlayerTeam = 0;

    public float projectileVelocity = 4.0f;

    public Vector3 Position => transform.position;

    public override void OnStartClient()
    {
        GameCamera.MainCamera.GetComponent<BoundingCamera>().AddBoundingObject(this);
    }


    public void FireForward()
    {
        foreach(Transform t in forwardShootSpawnPoints) {
            NetworkProjectile p = Instantiate(projectilePrefab).GetComponent<NetworkProjectile>();
            p.transform.position = t.position;
            p.transform.rotation = t.rotation;
            p.body.velocity = body.velocity + (p.transform.forward * projectileVelocity);
            NetworkServer.Spawn(p.gameObject);
        }
    }

    public void FirePort()
    {
        foreach (Transform t in portShootSpawnPoints) {
            NetworkProjectile p = Instantiate(projectilePrefab).GetComponent<NetworkProjectile>();
            p.transform.position = t.position;
            p.transform.rotation = t.rotation;
            p.body.velocity = body.velocity + (p.transform.forward * projectileVelocity);
            NetworkServer.Spawn(p.gameObject);
        }
    }

    public void FireStarboard()
    {
        foreach (Transform t in starboardShootSpawnPoints) {
            NetworkProjectile p = Instantiate(projectilePrefab).GetComponent<NetworkProjectile>();
            p.transform.position = t.position;
            p.transform.rotation = t.rotation;
            p.body.velocity = body.velocity + (p.transform.forward * projectileVelocity);
            NetworkServer.Spawn(p.gameObject);
        }
    }

    public void FireRear()
    {
        foreach (Transform t in rearShootSpawnPoints) {
            NetworkProjectile p = Instantiate(projectilePrefab).GetComponent<NetworkProjectile>();
            p.transform.position = t.position;
            p.transform.rotation = t.rotation;
            p.body.velocity = body.velocity + (p.transform.forward * projectileVelocity);
            NetworkServer.Spawn(p.gameObject);
        }
    }

    public void Accelerate()
    {
        body.velocity += acceleration * transform.forward * Time.fixedDeltaTime;
    }

    public void Deccelerate()
    {
        body.velocity -= acceleration * transform.forward * Time.fixedDeltaTime;
    }

    public void TurnRight()
    {
        body.angularVelocity += new Vector3(0, angularAcceleration * Time.fixedDeltaTime, 0);
    }

    public void TurnLeft()
    {
        body.angularVelocity -= new Vector3(0, angularAcceleration * Time.fixedDeltaTime, 0);
    }
}
