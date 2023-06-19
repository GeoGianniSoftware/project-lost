using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public List<Team> Teams = new List<Team>();
    public GameObject testPoint;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public Vector3 getMousePosInGameWorld() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.y = 25;


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach(RaycastHit hit in hits) {
            if (hit.transform.tag == "Map")
                return hit.point;
        }
        return mousePos;
    }

    public List<Entity> getActiveEntities() {
        List<Entity> entities = new List<Entity>();
        entities.AddRange(FindObjectsOfType<Entity>());
        List<Entity> active = new List<Entity>();

        foreach(Entity e in entities) {
            if (e.currentHealth > 0)
                active.Add(e);
        }

        return active;
    }

    public List<Entity> getActiveEntitiesOnTeam(int teamID) {
        List<Entity> entities = new List<Entity>();
        entities.AddRange(FindObjectsOfType<Entity>());

        List<Entity> active = new List<Entity>();

        foreach (Entity e in entities) {
            if (e.currentHealth > 0 && e.owner == teamID)
                active.Add(e);
        }

        return active;
    }

    public List<Entity> getActiveEnemies() {
        List<Entity> entities = new List<Entity>();
        entities.AddRange(FindObjectsOfType<Entity>());

        List<Entity> active = new List<Entity>();

        foreach (Entity e in entities) {
            Team team = getTeamFromID(e.owner);
            if (e.currentHealth > 0 && team.isHostileToPlayer)
                active.Add(e);
        }

        return active;
    }

    public List<Entity> getEnemiesInRange(Vector3 pos, float range) {
        List<Entity> inRange = new List<Entity>();
        foreach (Entity e in getActiveEnemies()) {
            if (Vector3.Distance(pos, e.transform.position) <= range)
                inRange.Add(e);
        }
        return inRange;
    }

    public Entity getClosestEnemy(Vector3 pos) {
        Entity closest = null;
        float dist = float.MaxValue;
        foreach (Entity e in getActiveEnemies()) {
            float checkDist = Vector3.Distance(pos, e.transform.position);
            if (checkDist < dist) {
                
                closest = e;
                dist = checkDist;
            }
        }
        return closest;
    }

    public Team getTeamFromID(int teamId) {
        if (Teams[teamId] != null)
            return Teams[teamId];
        return null;
    }
}
