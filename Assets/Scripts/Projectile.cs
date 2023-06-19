using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float checkRange;
    Damage damageToDeal;
    Vector3 direction;

    public void SetDamageAndDirection(Damage dmg, Vector3 dir) {
        damageToDeal = dmg;
        direction = dir;
    }

    // Update is called once per frame
    void Update()
    {
        if(direction != null) {
            transform.LookAt(transform.position + direction);
        }
        RaycastHit hit;
        Ray ray = new Ray(transform.position, direction); 
        if(Physics.Raycast(ray, out hit, checkRange)) {
            print(hit.transform.name);
            
            if (damageToDeal == null)
                damageToDeal = new Damage(1, transform.position);

            if (hit.transform.GetComponent<Entity>()) {
                if (damageToDeal.dealtBy != null && damageToDeal.dealtBy == hit.transform.GetComponent<Entity>())
                    return;

                    damageToDeal.DamagePos = transform.position;
                hit.transform.SendMessage("TakeDamage", damageToDeal, SendMessageOptions.DontRequireReceiver);
                Destroy(gameObject);
            }
        }
    }

    
}
