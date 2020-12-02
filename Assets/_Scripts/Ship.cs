using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrappyPirates;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class Ship : MonoBehaviour, ICameraFocusObject
{
    private NavigationModule navigation = null;
    private WeaponsModule weapons = null;
    private DefensesModule defenses = null;

    private Rigidbody body_ = null;
    public Rigidbody Body { get => body_; }

    [SerializeField] private Sprite shipIcon = null;
    public Sprite ShipIcon { get => shipIcon; }
    [SerializeField] private Dictionary<ThrusterGroups, List<Thruster>> thrusters = new Dictionary<ThrusterGroups, List<Thruster>>();

    private int team = 0;
    public int Team { get => team; }

    public Vector3 Position => transform.position;

    protected void Awake()
    {
        body_ = GetComponent<Rigidbody>();
        navigation = GetComponent<NavigationModule>();
        weapons = GetComponent<WeaponsModule>();
        defenses = GetComponent<DefensesModule>();
    }

    protected void Start()
    {
        if (NetworkPlayer_Lobby.LocalPlayer == null || NetworkPlayer_Lobby.LocalPlayer.Team == team) {
            navigation.Initialize();
            weapons.Initialize();
            defenses.Initialize();
        }


        GameCamera.MainCamera.gameObject.GetComponent<BoundingCamera>().AddBoundingObject(this);
    }

    private void HandleTeamChanged(int oldVal, int newVal)
    {
        
    }

    public void SetTeam(int team)
    {
        this.team = team;
    }
}
