using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    public WeaponStats stats;
    public bool isProjectileWeapon;
    public ProjectileStats pstats;

    private void Awake() {
        itemRef.itemType = ItemType.Weapon;
    }



    public Damage GetDamage() {
        Damage temp;
        if (stats != null)
            temp = new Damage(Random.Range(stats.MinDamage, stats.MaxDamage), transform.position, stats.Stun, stats.WeaponForce, stats.ForceRadius, holder);
        else
            temp = new Damage(1, transform.position);

        return temp;
    }

    public void Attack(Entity attacke) {
        Damage dmg = GetDamage();

        if (!isProjectileWeapon) {
            attacke.SendMessage("TakeDamage", dmg, SendMessageOptions.DontRequireReceiver);
            if (attacke.GetComponent<Rigidbody>()) {

                attacke.GetComponent<Rigidbody>().AddForce((attacke.transform.position - dmg.DamagePos) * dmg.DamageForce, ForceMode.Impulse);
            }
            foreach(Collider c in Physics.OverlapSphere(dmg.DamagePos, dmg.ForceRadius)) {
                if (c.GetComponent<Rigidbody>()) {
                    Rigidbody rb = c.GetComponent<Rigidbody>();
                    rb.AddForce((c.transform.position - dmg.DamagePos) * dmg.DamageForce, ForceMode.Impulse);
                }
            }
        }
        else {

            GameObject proj = Instantiate(pstats.Prefab, pstats.ProjectileSpawn.position, Quaternion.identity);
            if (proj.GetComponent<Rigidbody>()) {
                Vector3 velocityOffset = Vector3.zero;
                if (proj.GetComponent<Projectile>()) {
                    proj.GetComponent<Projectile>().SetDamageAndDirection(GetDamage(), (attacke.transform.position - dmg.DamagePos));

                }

                if (attacke.GetComponent<Rigidbody>()) {
                    Vector3 temp = attacke.GetComponent<Rigidbody>().velocity;
                    temp.y = 0;
                    velocityOffset = temp;
                }
                proj.GetComponent<Rigidbody>().AddForce(((attacke.transform.position - pstats.ProjectileSpawn.position)+velocityOffset) * pstats.ProjectileSpeed, ForceMode.Impulse);
            }
        }
    }

    private void Update() {
        if (beingHeld && holder) {
            transform.LookAt(holder.transform.GetChild(0).position + holder.transform.GetChild(0).forward);
            GetComponent<Collider>().isTrigger = true;
        }
        else {
            GetComponent<Collider>().isTrigger = false;
        }
    }
}
[System.Serializable]
public class WeaponStats
{
    public int MaxDamage;
    public int MinDamage;

    public float MaxRange;
    public float MinRange;
    public float WeaponCooldown;

    public float Stun;

    public float WeaponForce;
    public float ForceRadius;

    public WeaponStats(int max, int min, float range, float minrange, float cooldown, float force, float radius) {
        MaxDamage = max;
        MinDamage = min;
        MaxRange = range;
        MinRange = minrange;
        WeaponCooldown = cooldown;
        Stun = 0;
        WeaponForce = force;
        ForceRadius = radius;
    }

    public WeaponStats(int max, int min, float range, float minrange, float cooldown, float s, float force, float radius) {
        MaxDamage = max;
        MinDamage = min;
        MaxRange = range;
        MinRange = minrange;
        WeaponCooldown = cooldown;
        Stun = s;
        WeaponForce = force;
        ForceRadius = radius;
    }
}

[System.Serializable]
public class ProjectileStats
{
    public GameObject Prefab;
    public float ProjectileSpeed;
    public Transform ProjectileSpawn;

    public ProjectileStats(GameObject prefab, float speed, Transform spawn) {
        Prefab = prefab;
        ProjectileSpeed = speed;
        ProjectileSpawn = spawn;
    }
}
