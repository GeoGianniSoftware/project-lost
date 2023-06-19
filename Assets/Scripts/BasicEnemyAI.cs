using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BasicEnemyAI : Entity
{

    List<Entity> playerUnits = new List<Entity>();
    Vector3 movePos;
    Entity attackTarget;

    public float attackRange;
    public int attackDamage;
    public float attackCooldown;
    

    public float wanderRadius;
    float wanderReachDistance = 2f;

    public float maxHungerTime;
    public float starveDamageTime;


    float hungerAmt;
    public float wanderTime;
    float wT;
    float cooldownAmt;
    float wanderAmt;
    float starveTimer;

    public Transform rallyPoint;

    // Start is called before the first frame update
    private void Awake() {
        Initialize();
        movePos = transform.position;
        hungerAmt = maxHungerTime;

    }

    // Update is called once per frame
    void Update()
    {
        Tick();
        if (!NMA.enabled)
            return;
        playerUnits.AddRange(FindObjectsOfType<BasicCommandAI>());
        hungerAmt -= Time.deltaTime;
        wT -= Time.deltaTime;

        //Wandering
        if(hungerAmt > 0 && attackTarget == null) {
            if (hungerAmt > maxHungerTime)
                hungerAmt = maxHungerTime;
            if (rallyPoint == null && Vector3.Distance(transform.position, movePos) <= wanderReachDistance || wT <= 0) {
                Vector3 randomPos = getRandomPos(wanderRadius);
                if (NMA.enabled) {
                    NavMeshPath testPath = new NavMeshPath();
                    NMA.CalculatePath(randomPos, testPath);
                    if(testPath.status == NavMeshPathStatus.PathComplete) {
                        movePos = randomPos;
                    }
                }
                wT = wanderTime;
            }else if (rallyPoint) {
                if (NMA.enabled) {
                    NavMeshPath testPath = new NavMeshPath();
                    NMA.CalculatePath(rallyPoint.position, testPath);
                    if (testPath.status == NavMeshPathStatus.PathComplete) {
                        movePos = rallyPoint.position;
                    }

                    if(Vector3.Distance(transform.position, rallyPoint.position) <= 2f) {
                        Destroy(rallyPoint.gameObject);
                        rallyPoint = null;
                    }
                }
            }
            if (NMA.enabled)
                NMA.destination = movePos;
        }
        else {
            starveTimer -= Time.deltaTime;
            if(starveTimer <= 0 && hungerAmt < 0) {
                TakeDamage(new Damage(1, transform.position, 1));
                starveTimer = starveDamageTime;
            }
            if(playerUnits.Count > 0) {

                if (!attackTarget) {
                    int unlucky = Random.Range(0, playerUnits.Count);
                    attackTarget = playerUnits[unlucky];
                }
                else {
                    if (!NMA.enabled)
                        return;

                    Vector3 testPoint = calculateClosestPointOnCircle(transform.position, attackTarget.transform.position, attackRange);
                    if (Vector3.Distance(transform.position, attackTarget.transform.position) > Vector3.Distance(testPoint, attackTarget.transform.position)) {
                        NMA.destination = testPoint;
                    }
                        
                    if(Vector3.Distance(transform.position, attackTarget.transform.position) < attackRange && cooldownAmt <= 0) {
                        Attack(attackTarget);
                    }
                }


            }
        }
        
    }

    public void setRallyPoint(Vector3 position) {
        GameObject rally = new GameObject("Rally");
        rally.transform.position = position;
        rallyPoint = rally.transform;
    }

    void Attack(Entity attacke) {
        Damage dmg = new Damage(attackDamage, transform.position, 2f, 5, 3f, this);
        attacke.SendMessage("TakeDamage", dmg, SendMessageOptions.DontRequireReceiver);
        hungerAmt += maxHungerTime / 10;
        cooldownAmt = attackCooldown;
    }

    public void TakeDamage(Damage dmg) {
        print("Hit!");
        currentHealth -= dmg.DamageAmount;
        Vector3 dir = transform.position - dmg.DamagePos;

        DamageFX(dmg);
        dir.Normalize();
        if (dir.y < 0) {
            dir.y = 0;
        }

        float force = dmg.DamageForce;
        if (currentHealth <= 0) {
            force = force * 1.5f;
            GetComponent<BasicEnemyAI>().enabled = false;
        }

        stun = dmg.StunTime;
        NMA.enabled = false;
        RB.AddForce(dir * force, ForceMode.Impulse);

        if (currentHealth <= 0) {
            Die();
        }
        else {
            if(attackTarget == null) {
                attackTarget = dmg.dealtBy;
            }
        }
    }

    Vector3 calculateClosestPointOnCircle(Vector3 pos, Vector3 targetPos, float radius) {
        float vX = pos.x - targetPos.x;
        float vZ = pos.z - targetPos.z;
        float magV = Mathf.Sqrt(vX * vX + vZ * vZ);
        float aX = targetPos.x + vX / magV * radius;
        float aZ = targetPos.z + vZ / magV * radius;

        Vector3 calculatedPos = new Vector3(aX, targetPos.y, aZ);

        return calculatedPos;
    }

    Vector3 getRandomPos(float range) {
        Vector3 temp = new Vector3();
        temp.x = getRandomNegToPos(range);
        temp.y = transform.position.y;
        temp.z = getRandomNegToPos(range);
        return temp;
    }

    float getRandomNegToPos(float num) {
        return Random.Range(-num, num);
    }

    
}
