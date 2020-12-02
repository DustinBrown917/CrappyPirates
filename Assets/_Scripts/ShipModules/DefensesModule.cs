using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensesModule : ShipModule
{

    [SerializeField] private bool defensesViable_ = true;
    public bool DefensesViable { get => defensesViable_; }

    public override void Initialize()
    {
        base.Initialize();
    }

    protected override IEnumerator LocalPlayerFixedUpdate()
    {
        yield return null;
    }

    protected override IEnumerator LocalPlayerUpdate()
    {
        yield return null;
    }


    protected override void SetUpUI()
    {
        DefensesUI ui = Instantiate(UIModulePrefab, ShipUICanvas.Canvas.transform).GetComponent<DefensesUI>();
        if (ui) {
            ui.SetModule(this);
        }
    }
}
