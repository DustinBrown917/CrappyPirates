using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ship : MonoBehaviour, ICameraFocusObject
{
    private NavigationModule navigation;
    private WeaponsModule weapons;

    private Rigidbody body_ = null;
    public Rigidbody Body { get => body_; }

    [SerializeField] private Sprite shipIcon = null;
    public Sprite ShipIcon { get => shipIcon; }
    [SerializeField] private Dictionary<ThrusterGroups, List<Thruster>> thrusters = new Dictionary<ThrusterGroups, List<Thruster>>();

    public Vector3 Position => transform.position;

    protected void Awake()
    {
        body_ = GetComponent<Rigidbody>();
        navigation = GetComponent<NavigationModule>();
        weapons = GetComponent<WeaponsModule>();
    }

    protected void Start()
    {
        navigation.Initialize();
        weapons.Initialize();

        GameCamera.MainCamera.gameObject.GetComponent<BoundingCamera>().AddBoundingObject(this);
    }
}
