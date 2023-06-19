using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BasicCommandAI : Entity
{
    Animator ANIM;

    public List<Command> unitCommands = new List<Command>();
    public Command currentCommand;

    [System.NonSerialized]
    public Vector3 targetPosition;


    public AIState currentState;

    public Item holding;
    Weapon weapon;
    Item targetItem;

    Transform movePos;
    Vector3 buildPos;
    GameObject lastStructurePrefab;
    GameObject lastEntityBuilt;
    bool buildMode;

    public GameObject wallPrefab;

    //How close AI has to get to their destination before stopping
    float checkDistance = 2f;

    Entity attackTarget;
    float cooldown;

    // Start is called before the first frame update
    void Awake()
    {
        Initialize();
        NMA = GetComponent<NavMeshAgent>();
        ANIM = GetComponent<Animator>();
        currentState = AIState.Idle;

        targetPosition = transform.position;
    }

    private void Start() {
        if(holding != null) {
            GameObject tempItem = Instantiate(holding.gameObject, transform.position, Quaternion.identity);
            PickupItem(tempItem.GetComponent<Item>());
            
        }
    }

    void BasicCommandTick() {
        cooldown -= Time.deltaTime;
        ANIM.SetFloat("Speed", NMA.velocity.magnitude);
        Tick();
    }

    

    // Update is called once per frame
    void Update()
    {
        BasicCommandTick();
        Path(targetPosition);

        Build();
        Attack();


        Pickup();
        UpdateHeldItem();

        if (Input.GetMouseButton(0)) {
            if(Input.GetKey(KeyCode.LeftAlt))
            SetCommand(new MoveCommand(owner, this, GM.getMousePosInGameWorld()));
            else
            SetCommand(new GuardCommand(owner, this, transform.position));

        }

        if (Input.GetMouseButton(1)) {
            int max = getActiveEnemies().Count;

            int random = Random.Range(0, max);

            Entity enemy = getActiveEnemies()[random];
            SetCommand(new AttackCommand(owner, this, enemy));

        }
        if (holding.GetComponent<Weapon>())
            weapon = holding.GetComponent<Weapon>();


    }

    bool CanAttack() {
        if(attackTarget && holding != null && holding.GetComponent<Weapon>()) {


            if (Vector3.Distance(transform.position, attackTarget.transform.position) < weapon.stats.MaxRange && cooldown <= 0) {
                return true;
            }
        }
        return false;
        
    }

    bool CanSee() {
        Ray ray = new Ray(transform.position, transform.position - attackTarget.transform.position);
        RaycastHit hit = new RaycastHit();
        if(Physics.Raycast(ray, out hit, weapon.stats.MaxRange + 1f)) {
            if(hit.collider != null && hit.collider.gameObject == attackTarget) {
                return true;
            }
        }
        return false;
    }

    bool isWithinPoint(Vector3 checkPoint, float range) {
        Collider[] cols = Physics.OverlapSphere(checkPoint, range);
        foreach(Collider c in cols) {
            if (c.gameObject == this.gameObject)
                return true;
                
        }

        return false;
    }

    Vector3 CalculateAttackPoint() {
        float range = weapon.stats.MaxRange - 1f;

        Vector3 firstPoint = calculateClosestPointOnCircle(transform.position, attackTarget.transform.position, range);
        if (isWithinPoint(attackTarget.transform.position, range))
            return transform.position;
        return firstPoint;

    }

    void Pickup() {
        //Grab Item
        if (targetItem != null && Vector3.Distance(targetItem.transform.position, transform.position) < 1f) {
            targetItem.transform.position = transform.GetChild(0).transform.position;

            PickupItem(targetItem);
        }
    }

    void Attack() {
        if(CanAttack()) {
            weapon.Attack(attackTarget);
            cooldown = weapon.stats.WeaponCooldown;
            ANIM.SetTrigger("Attack");
        }
    }

    void Build() {
        if (holding && Vector3.Distance(transform.position, buildPos) < 1f) {
            GameObject built = Instantiate(lastStructurePrefab, buildPos, Quaternion.identity);
            lastEntityBuilt = built;
            Destroy(holding);
        }
    }

    void PickupItem(Item itemToPickup) {
        if(itemToPickup != null) {
            if (holding)
                DropItem();

            holding = itemToPickup;
            holding.holder = this;
            holding.beingHeld = true;
            targetItem = null;
        }
        
    }
    void DropItem() {
        holding.GetComponent<Item>().beingHeld = false;
        holding.holder = null;
        holding = null;

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

    
    void StateMachine() {
        //Walking
        if(currentCommand != null) {
            Command command = currentCommand;

            if (isCurrentCommandComplete()) {
                if(unitCommands.Count > 0) {
                    currentCommand = unitCommands[0];
                    unitCommands.RemoveAt(0);
                }
                else {
                    currentCommand = null;

                    return;
                }
            }

            switch (currentCommand.commandType) {
                case CommandType.Move:
                    MoveCommand move = (MoveCommand)command;
                    targetPosition = move.posToMove;
                    currentState = AIState.Walking;
                    return;

                case CommandType.Attack:
                    AttackCommand attack = (AttackCommand)command;
                    if (weapon && isWithinPoint(attackTarget.transform.position, weapon.stats.MaxRange-1f)) {
                        targetPosition = transform.position;
                        currentState = AIState.Attacking;

                        transform.LookAt(attackTarget.transform.position);
                    }
                    else if(weapon && !isWithinPoint(attackTarget.transform.position, weapon.stats.MaxRange-1f)) {
                        targetPosition = CalculateAttackPoint();
                        currentState = AIState.Walking;

                        transform.LookAt(attackTarget.transform.position);
                    }
                    else if(!weapon) {
                        ClearCommands();
                    }
                    return;


                case CommandType.Guard:
                    GuardCommand guard = (GuardCommand)command;

                    if (!weapon) {
                        ClearCommands();
                    }
                   

                    if (isWithinPoint(guard.posToGuard, checkDistance)) {
                        float enemiesInRange = 0;

                        float guardRange = weapon.stats.MaxRange * 3f;

                        if (GM != null)
                        enemiesInRange = GM.getEnemiesInRange(transform.position,guardRange).Count;

                        if (!guard.entityToAttack) {
                            if (enemiesInRange == 0) {
                                targetPosition = transform.position;
                                currentState = AIState.Guarding;

                                transform.LookAt(transform.forward);
                            }
                            else if (enemiesInRange > 0) {
                                guard.entityToAttack = GM.getClosestEnemy(transform.position);
                                currentState = AIState.Guarding;
                                transform.LookAt(guard.entityToAttack.transform.position);
                            }
                        }
                        else {
                            if (guard.entityToAttack && guard.entityToAttack.currentHealth > 0)
                                attackTarget = guard.entityToAttack;
                            else {
                                guard.entityToAttack = null;
                                attackTarget = null;
                            }
                                

                            if (!attackTarget)
                                return;

                            if (isWithinPoint(attackTarget.transform.position, weapon.stats.MaxRange - 1f)) {
                                targetPosition = transform.position;
                                currentState = AIState.Attacking;

                                transform.LookAt(attackTarget.transform.position);
                            }
                            else if (weapon && !isWithinPoint(attackTarget.transform.position, weapon.stats.MaxRange - 1f)) {
                                targetPosition = CalculateAttackPoint();
                                currentState = AIState.Walking;

                                transform.LookAt(attackTarget.transform.position);
                            }
                        }
                        
                    }
                    else {
                        targetPosition = guard.posToGuard;
                        guard.entityToAttack = null;
                        currentState = AIState.Walking;
                    }
                    return;


                case CommandType.Build:
                    BuildCommand build = (BuildCommand)command;
                    if (buildMode) {
                        targetPosition = build.positionToBuild;
                        currentState = AIState.Building;
                    }
                    else {
                        ClearCommands();
                    }
                    return;

                case CommandType.Pickkup:
                    PickupCommand pickup = (PickupCommand)command;
                    targetPosition = pickup.itemToPickup.transform.position;
                    return;
            }
        }
        else {
            NMA.isStopped = true;
            targetPosition = transform.position;
            currentState = AIState.Idle;
        }
        /*
        if((!attackTarget && !buildMode && movePos != null && !targetItem)) {
            
            targetPosition = movePos.position;
            currentState = AIState.Walking;

            if (NMA.remainingDistance <= checkDistance) {
                movePos = null;
                Destroy(lastPosCreated.gameObject);
                return;
            }

            return;
        }
        //Attacking
        else if (attackTarget && holding) {
            if (attackTarget.currentHealth <= 0) {
                attackTarget = null;
                return;
            }

            targetPosition = CalculateAttackPoint();
            currentState = AIState.Attacking;
            return;

        }
        //Building
        else if (holding && buildMode) {
            targetPosition = buildPos;
            currentState = AIState.Building;
            return;
        }
        //Pickup
        else if (targetItem != null && attackTarget == null) {
            
            targetPosition = targetItem.transform.position;
            currentState = AIState.Pickingup;
            return;
        }
        //Idle
        else if (!attackTarget && targetItem == null && !movePos) {

            
            currentState = AIState.Idle;
            return;
        }
        */
        
    }
    void Path(Vector3 targetPos) {
        if (NMA.enabled) {
            NavMeshPath path = new NavMeshPath();

            StateMachine();

            
            NMA.CalculatePath(targetPosition, path);
            if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathComplete) {

                NMA.SetDestination(targetPos);
            }

            if (NMA.destination != null) {
                NMA.isStopped = false;
            }
            else {
                NMA.isStopped = true;
            }

        }
        
    }
    bool isCurrentCommandComplete() {
        if(currentCommand != null) {
            switch (currentCommand.commandType) {
                case CommandType.Move:
                    MoveCommand move = (MoveCommand)currentCommand;
                    return Vector3.Distance(move.posToMove, transform.position) <= checkDistance;
                    
                case CommandType.Attack:

                    return attackTarget.currentHealth <= 0;
                    
                case CommandType.Build:
                    BuildCommand build = (BuildCommand)currentCommand;
                    return (lastEntityBuilt == build.entityToBuild && lastEntityBuilt.transform.position == build.positionToBuild);
                    
                case CommandType.Pickkup:
                    PickupCommand pickup = (PickupCommand)currentCommand;
                    return holding == pickup.itemToPickup;

                case CommandType.Guard:
                    return false;
            }
        }
        return false;
    }
    bool isPathAcceptable(Vector3 pos) {
        NavMeshPath path = new NavMeshPath();
        if (!NMA.enabled)
            return false;
        NMA.CalculatePath(pos, path);

        if (path.status == NavMeshPathStatus.PathComplete || path.status == NavMeshPathStatus.PathPartial)
            return true;

        return false;
    }
    void ClearCommands() {
        attackTarget = null;
        movePos = null;
        targetItem = null;
        buildMode = false;
        unitCommands = new List<Command>();
        currentCommand = null;
    }
    void SetCommand(Command c) {
        ClearCommands();

        Vector3 pathPoint = Vector3.zero;
        switch (c.commandType) {
            case CommandType.Move:
                MoveCommand move = (MoveCommand)c;
                if(move.posToMove != null) {
                    pathPoint = move.posToMove;
                    if (isPathAcceptable(pathPoint)) {
                        movePos = createMovePosition(move.posToMove);
                        currentCommand = move;
                    }
                        
                    
                }
                break;

            case CommandType.Guard:
                GuardCommand guard = (GuardCommand)c;
                if (guard.posToGuard != null) {
                    pathPoint = guard.posToGuard;
                    if (isPathAcceptable(pathPoint)) {
                        movePos = createMovePosition(guard.posToGuard);
                        currentCommand = guard;
                    }


                }
                break;

            case CommandType.Attack:
                AttackCommand attack = (AttackCommand)c;
                if(holding.itemRef.itemType == ItemType.Weapon && attack.entityToAttack != null) {

                    pathPoint = attack.entityToAttack.transform.position;
                    if (isPathAcceptable(pathPoint)) {
                        attackTarget = attack.entityToAttack;

                        currentCommand = attack;
                    }
                        
                }
                break;
            case CommandType.Build:
                BuildCommand build = (BuildCommand)c;
                if(build.entityToBuild != null) {

                    pathPoint = build.positionToBuild;
                    if (isPathAcceptable(pathPoint)) {

                        buildPos = build.positionToBuild;
                        buildMode = true;
                        lastStructurePrefab = build.entityToBuild.gameObject;
                        currentCommand = build;
                    }
                }
                break;
            case CommandType.Pickkup:
                PickupCommand pickup = (PickupCommand)c;
                if(pickup.itemToPickup != null) {
                    pathPoint = pickup.itemToPickup.transform.position;
                    if (isPathAcceptable(pathPoint)) {
                        targetItem = pickup.itemToPickup;
                        currentCommand = pickup;
                    }
                }
                break;
        }


        
    }
    List<Entity> getActiveEnemies() {
        return GM.getActiveEnemies();
    }

    Transform lastPosCreated;
    Transform createMovePosition(Vector3 position) {
        if (lastPosCreated != null)
            Destroy(lastPosCreated.gameObject);
        GameObject temp = new GameObject("MoveDestination");
        temp.transform.position = position;
        lastPosCreated = temp.transform;
        return temp.transform;
    }
    void UpdateHeldItem() {
        if (holding != null) {
            holding.transform.position = transform.GetChild(0).transform.position;
        }
    }

    
}
