using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NavigationModule : ShipModule
{
    private Rigidbody Body { get => Ship.Body; }
    [SerializeField] private float forwardAcceleration = 1.0f;
    [SerializeField] private float backwardAcceleration = 0.5f;
    [SerializeField] private float strafeAcceleration = 0.4f;
    [SerializeField] float rotationalAccel = 1.0f;
    [SerializeField] float initialVelocity = 1.0f;

    private Thruster[] thrusters = new Thruster[0];

    private Dictionary<ThrusterGroups, Observable<float>> throttles = new Dictionary<ThrusterGroups, Observable<float>>();


    public override void Initialize()
    {
        thrusters = GetComponentsInChildren<Thruster>();

        foreach(ThrusterGroups group in System.Enum.GetValues(typeof(ThrusterGroups))) {
            throttles.Add(group, new Observable<float>());
        }

        base.Initialize();
    }

    [Client]
    protected override void SetUpUI()
    {
        NavigationUI ui = Instantiate(UIModulePrefab, ShipUICanvas.Canvas.transform).GetComponent<NavigationUI>();
        if (ui) {
            ui.SetModule(this);          
        }
    }

    public void SetThrottle(ThrusterGroups group, float amount)
    {
        foreach(ThrusterGroups g in throttles.Keys) {
            if((group & g) == g) {
                throttles[g].value = amount;
            }
        }

        UpdateThrusters();
    }

    public (ThrusterGroups, Observable<float>)[] GetThrottles()
    {
        (ThrusterGroups, Observable<float>)[] throttles = new (ThrusterGroups, Observable<float>)[this.throttles.Count];

        int i = 0;
        foreach(KeyValuePair<ThrusterGroups, Observable<float>> kvp in this.throttles) {
            throttles[i] = (kvp.Key, kvp.Value);
            i++;
        }

        return throttles;
    }

    public ThrusterGroups[] GetThrottleGroups()
    {
        return throttles.Keys.ToArray();
    }

    public Observable<float> GetThrottle(ThrusterGroups thrusterGroup)
    {
        if (throttles.ContainsKey(thrusterGroup)) {
            return throttles[thrusterGroup];
        } else {
            return null;
        }
    }

    private void UpdateThrusters()
    {
        for(int i = 0; i < thrusters.Length; i++) {
            float throttle = 0;
            foreach (ThrusterGroups g in throttles.Keys) {
                if ((thrusters[i].ThrusterGroup & g) == g && throttles[g].value > throttle) {
                    throttle = throttles[g].value;                    
                }
            }
            thrusters[i].SetThrottle(throttle);
        }
    }

    protected override IEnumerator LocalPlayerFixedUpdate()
    {
        while (true) {
            Vector3 force = new Vector3();

            force += transform.forward * forwardAcceleration * throttles[ThrusterGroups.REAR].value;
            force += transform.forward * -backwardAcceleration * throttles[ThrusterGroups.FORWARD].value;
            force += transform.right * -strafeAcceleration * throttles[ThrusterGroups.STARBOARD].value;
            force += transform.right * strafeAcceleration * throttles[ThrusterGroups.PORT].value;

            Body.AddForce(force);

            Vector3 torque = new Vector3();
            torque.y += -rotationalAccel * throttles[ThrusterGroups.ROT_CCW].value;
            torque.y += rotationalAccel * throttles[ThrusterGroups.ROT_CW].value;

            Body.AddTorque(torque);
            yield return new WaitForFixedUpdate();
        }
    }

    protected override IEnumerator LocalPlayerUpdate()
    {
        while (true) {

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
                SetThrottle(ThrusterGroups.REAR, 1.0f);
            } else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W)) {
                SetThrottle(ThrusterGroups.REAR, 0.0f);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
                SetThrottle(ThrusterGroups.ROT_CCW, 1.0f);
            } else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A)) {
                SetThrottle(ThrusterGroups.ROT_CCW, 0.0f);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
                SetThrottle(ThrusterGroups.ROT_CW, 1.0f);
            } else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D)) {
                SetThrottle(ThrusterGroups.ROT_CW, 0.0f);
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
                SetThrottle(ThrusterGroups.FORWARD, 1.0f);
            } else if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S)) {
                SetThrottle(ThrusterGroups.FORWARD, 0.0f);
            }

            if (Input.GetKeyDown(KeyCode.Q)) {
                SetThrottle(ThrusterGroups.STARBOARD, 1.0f);
            } else if (Input.GetKeyUp(KeyCode.Q)) {
                SetThrottle(ThrusterGroups.STARBOARD, 0.0f);
            }

            if (Input.GetKeyDown(KeyCode.E)) {
                SetThrottle(ThrusterGroups.PORT, 1.0f);
            } else if (Input.GetKeyUp(KeyCode.E)) {
                SetThrottle(ThrusterGroups.PORT, 0.0f);
            }
            yield return null;
        }
    }
}
