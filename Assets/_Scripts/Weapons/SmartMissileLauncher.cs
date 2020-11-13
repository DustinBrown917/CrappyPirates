using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartMissileLauncher : Weapon
{
    public override void Fire(FiringParameters parameters)
    {
        if (currentShotsInMagazine_.value <= 0 || !CanFire) { return; }
        Projectile p = ProjectileManager.RequestProjectile(projectileType);

        if (p) {
            p.transform.position = launchPosition.position;
            p.transform.rotation = transform.rotation;
            p.Arm(parameters.target, collidersToIgnore);

            float power = Mathf.Lerp(minPower, maxPower, parameters.power);

            //if (relativeBody) {
            //    p.Body.velocity = (p.transform.forward * power) + relativeBody.velocity;
            //} else {
            //    p.Body.velocity = p.transform.forward * power;
            //}
        }

        currentShotsInMagazine_.value--;

        StartCooldown();

        if (currentShotsInMagazine_.value <= 0) {
            StartReloading();
        }
    }
}

