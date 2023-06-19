using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(NavMeshAgent))]
public class Entity : MonoBehaviour
{
    public bool isSelected;

    public int owner;

    [System.NonSerialized]
    public GameManager GM;
    [System.NonSerialized]
    public Rigidbody RB;
    [System.NonSerialized]
    public Collider COL;
    [System.NonSerialized]
    public NavMeshAgent NMA;
    



    public int maxHealth;
    public int currentHealth;
    public Vector3 healthbarOffset;
    public float healthbarSize;
    public bool staticEntity;
    public bool beserk;

    GameObject healthbarRef;
    GameObject selectioncircleRef;

    Vector3 lastMeshPostion;
    [Min(1)]
    public float entityRadius;
    public Vector3 bottomCenter;
    

    public float stun;

    private void Awake() {
        Initialize();
        if (staticEntity) {
            NMA.enabled = false;
        }
    }

    public void Initialize() {
        GM = FindObjectOfType<GameManager>();
        RB = GetComponent<Rigidbody>();
        COL = GetComponent<Collider>();
        NMA = GetComponent<NavMeshAgent>();
        RB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        currentHealth = maxHealth;
        GameObject bar = Instantiate((GameObject)Resources.Load("FX/Healthbar"));
        bar.transform.SetParent(this.transform);
        bar.GetComponent<Healthbar>().Attach(this);
        healthbarRef = bar;

        GameObject selection = Instantiate((GameObject)Resources.Load("FX/Selectioncircle"), transform.position + bottomCenter, Quaternion.identity, this.transform);
        selectioncircleRef = selection;
        if(owner == 1) {

            selection.GetComponent<OutlneFX>().color = Color.red;
        }

    }

    float onMeshThreshold = 3;

    public bool IsAgentOnNavMesh(GameObject agentObject) {
        Vector3 agentPosition = agentObject.transform.position;
        NavMeshHit hit;

        // Check for nearest point on navmesh to agent, within onMeshThreshold
        if (NavMesh.SamplePosition(agentPosition, out hit, onMeshThreshold, NavMesh.AllAreas)) {
            // Check if the positions are vertically aligned
            if (Mathf.Approximately(agentPosition.x, hit.position.x)
                && Mathf.Approximately(agentPosition.z, hit.position.z)) {
                // Lastly, check if object is below navmesh
                return agentPosition.y >= hit.position.y;
            }
        }

        return false;
    }

    public void Tick() {
        NavMeshHit hit;
        if(IsAgentOnNavMesh(this.gameObject)) {
            
            lastMeshPostion = transform.position;
        }
        else {
            transform.position = lastMeshPostion;
        }

        stun -= Time.deltaTime;
        if(stun <= 0) {
            NMA.enabled = true;
        }
        else {
            NMA.enabled = false;
        }

        if (currentHealth < 0) {
            Destroy(this);
        }

        if (selectioncircleRef)
            selectioncircleRef.SetActive(isSelected);
    }

    public void SelectUnit() {
        isSelected = true;
    }

    public void DeselectUnit() {
        isSelected = false;
    }



    public void DamageFX(Damage dmg) {
        //Healthbar Refresh
        healthbarRef.GetComponent<Healthbar>().Refresh();

        //Spawn Damage Text
        GameObject fxPrefab = ((GameObject)Resources.Load("FX/DamagePopup"));
        GameObject damageText = Instantiate(fxPrefab, healthbarRef.transform.position, Quaternion.identity);
        damageText.GetComponent<DamagePopup>().SetDamageText(dmg);

    }

    public void TakeDamage(Damage dmg) {
        print("Hit!");
        currentHealth -= dmg.DamageAmount;
        Vector3 dir = transform.position - dmg.DamagePos;

        DamageFX(dmg);
        dir.Normalize();
        
            
       

        float force = dmg.DamageForce;
        if(currentHealth <= 0) {
            force = force*1.5f;
        }

        stun = dmg.StunTime;
        NMA.enabled = false;

        Vector3 finForce = dir * force;
        finForce.y = 5f;
        RB.AddForce(finForce, ForceMode.Impulse);
        
        if (currentHealth <= 0) {
            Die();
        }
    }

    

    public void Die() {
        GetComponent<Entity>().enabled = false;
        NMA.enabled = false;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireCube(transform.position + bottomCenter, new Vector3(entityRadius*1.5f, 0.1f, entityRadius * 1.5f));
    }

}

[System.Serializable]
public enum AIState
{
    Idle,
    Walking,
    Pickingup,
    Building,
    Attacking,
    Guarding
}
