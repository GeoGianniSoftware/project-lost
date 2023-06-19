using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Damage
{
    public int DamageAmount;
    [System.NonSerialized]
    public Vector3 DamagePos;
    public float StunTime;
    public float DamageForce;
    public float ForceRadius;

    public Entity dealtBy;

    public Damage(int amt, Vector3 pos) {
        DamageAmount = amt;
        DamagePos = pos;
        StunTime = 0;
        DamageForce = 1;
        ForceRadius = .25f;
    }
    public Damage(int amt, Vector3 pos, float stun) {
        DamageAmount = amt;
        DamagePos = pos;
        StunTime = stun;
        DamageForce = 1;
        ForceRadius = .25f;
    }

    public Damage(int amt, Vector3 pos, float stun, float force, float radius, Entity dealer) {
        DamageAmount = amt;
        DamagePos = pos;
        StunTime = stun;
        DamageForce = force;
        ForceRadius = radius;
        dealtBy = dealer;
    }
}
