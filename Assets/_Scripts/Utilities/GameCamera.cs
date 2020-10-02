using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameCamera 
{
    private static Camera mainCamera_ = null;
    public static Camera MainCamera {
        get {
            if(mainCamera_ == null) {
                mainCamera_ = Camera.main;
            }

            return mainCamera_;
        }
    }
}
