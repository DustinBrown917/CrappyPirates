

using UnityEngine;

public static class Directions
{
    public static D4 Opposite(D4 direction)
    {
        switch (direction) {
            case D4.N:
                return D4.S;
            case D4.E:
                return D4.W;
            case D4.S:
                return D4.N;
            case D4.W:
                return D4.E;
        }

        return direction;
    }

    public static D8 Opposite(D8 direction)
    {
        switch (direction) {
            case D8.N:
                return D8.S;
            case D8.NE:
                return D8.SW;
            case D8.E:
                return D8.W;
            case D8.SE:
                return D8.NW;
            case D8.S:
                return D8.N;
            case D8.SW:
                return D8.NE;
            case D8.W:
                return D8.E;
            case D8.NW:
                return D8.SE;
        }

        return direction;
    }

    public static D8 ToD8(D4 direction)
    {
        return (D8)(((byte)direction) * 2);
    }

    public static D8 AngleToDirection(float dir)
    {
        dir = Clamp360(dir);

        if (dir >= 337.5f) {
            return D8.W;
        }
        else if (dir >= 292.5f) {
            return D8.NW;
        }
        else if (dir >= 247.5f) {
            return D8.N;
        }
        else if (dir >= 202.5f) {
            return D8.NE;
        }
        else if (dir >= 157.5f) {
            return D8.E;
        }
        else if (dir >= 112.5f) {
            return D8.SE;
        }
        else if (dir >= 67.5f) {
            return D8.S;
        }
        else if (dir >= 22.5f) {
            return D8.SW;
        }
        else {
            return D8.W;
        }
    }

    public static float Angle(Vector2 from, Vector2 to)
    {
        return Mathf.Atan2(to.y - from.y, to.x - from.x) * (180 / Mathf.PI) + 180;
    }

    public static float DirectionToAngle(D8 dir)
    {
        switch (dir) {
            case D8.S:
                return 90.0f;
            case D8.SE:
                return 135.0f;
            case D8.E:
                return 180.0f;
            case D8.NE:
                return 225.0f;
            case D8.N:
                return 270.0f;
            case D8.NW:
                return 315.0f;
            case D8.W:
                return 0.0f;
            case D8.SW:
                return 45.0f;
        }

        return 0.0f;
    }

    public static bool IsInRangeOfDirection(float angle, D8 direction, float angleRange)
    {
        if(angleRange >= 360) { return true; }
        if(angleRange <= 0) { return false; }

        float directionAngle = DirectionToAngle(direction);
        float min = directionAngle - (angleRange * 0.5f);
        float max = directionAngle + (angleRange * 0.5f);

        if(min < 0) {
            min = 360.0f + min;
        }

        if(max > 360.0f) {
            max -= 360.0f;
        }

        angle = Clamp360(angle);

        if (min > max) {
            return angle <= max || angle >= min;
        } else {
            return angle <= max && angle >= min;
        }      
    }

    public static float Clamp360(float angle)
    {
        while (angle >= 360.0f) {
            angle -= 360.0f;
        }

        while (angle < 0) {
            angle += 360.0f;
        }

        return angle;
    }
}

public enum D4 : byte
{
    S, E, N, W
}

public enum D8 : byte
{
    S, SE, E, NE, N, NW, W, SW
}

