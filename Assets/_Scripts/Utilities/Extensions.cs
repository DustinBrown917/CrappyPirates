using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static string FormattedString(this Batteries battery) 
    {
        string returnString = "";
        switch (battery) {
            case Batteries.FORWARD:
                returnString = "Forward";
                break;
            case Batteries.STERN:
                returnString = "Stern";
                break;
            case Batteries.STARBOARD:
                returnString = "Starboard";
                break;
            case Batteries.PORT:
                returnString = "Port";
                break;
            default:
                returnString = "Battery";
                break;
        }
        return returnString;
    }
}
