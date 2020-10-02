using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModuleUI<T> : SerializedMonoBehaviour where T : ShipModule
{
    protected T module = null;

    public void SetModule(T module)
    {
        if(this.module) {
            CleanOldModule();
        }
        this.module = module;
        Initialize();
    }

    protected abstract void Initialize();

    protected abstract void CleanOldModule();
}
