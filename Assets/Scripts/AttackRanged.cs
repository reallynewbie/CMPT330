using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shoots a projectiles and calculates recharge time
/// </summary>
/// 
/// Sources: https://www.youtube.com/watch?v=wkKsl1Mfp5M
///          https://forum.unity.com/threads/controlling-fire-rate-with-c.539131/#post-3554140
/// 
/// Field           Description
/// projPrefab      Projectile prefab
/// projSpawnLoc    Location to spawn the projectile
/// fireRate        Rate to fire projectiles
/// nextFire        Calculate if another shot can be fired
/// projDeathTime   How long a projectile lasts
/// 
/// Author: Chamod Welhenge
/// 
public class AttackRanged : MonoBehaviour
{

    public GameObject projPrefab;
    public Transform projSpawnLoc;
    public float projDeathTime = 2f;
    public float fireRate = 3f;
    private float nextFire = -1f;

    // Update is called once per frame
    void Update()
    {
     
        if (nextFire > 0)
        {
            //can not fire
            nextFire -= Time.deltaTime;

        }

        else
        {
            //can fire
            if (Input.GetButtonDown("Fire1"))
            {
                Attack();
            }
        }

    }

    /// <summary>
    /// Initialized projectile at given spawn location
    /// Projectile times out after a given time
    /// </summary>
    void Attack()
    {
        GameObject tmpProj;
        // Create the projectile
        tmpProj = Instantiate(projPrefab, projSpawnLoc.position, projSpawnLoc.rotation);
        // Destroy projectile after given time
        Destroy(tmpProj, projDeathTime);
        // When to fire next projectile
        nextFire = fireRate;


    }

}
