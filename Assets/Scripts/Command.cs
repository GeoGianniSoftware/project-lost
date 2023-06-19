using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Command
{
    public CommandType commandType;
    public int teamID;
    public Entity entityToCommand;

    public Command() {
        commandType = CommandType.Move;
    }
}

public enum CommandType
{
    Move,
    Attack,
    Build,
    Pickkup,
    Guard
}

public class MoveCommand : Command
{
    public Vector3 posToMove;
    public MoveCommand(int t, Entity entity, Vector3 pos) {
        commandType = CommandType.Move;
        teamID = t;
        entityToCommand = entity;

        posToMove = pos;
    }
}

public class GuardCommand : Command
{
    public Vector3 posToGuard;
    public Entity entityToAttack;
    public GuardCommand(int t, Entity entity, Vector3 pos) {
        commandType = CommandType.Guard;
        teamID = t;
        entityToCommand = entity;
        entityToAttack = null;
        posToGuard = pos;
    }
}

public class AttackCommand : Command
{
    public Entity entityToAttack;
    public AttackCommand(int source, Entity entity, Entity attack) {

        commandType = CommandType.Attack;
        teamID = source;
        entityToCommand = entity;

        entityToAttack = attack;
    }
}

public class BuildCommand : Command
{
    public Entity entityToBuild;
    public Vector3 positionToBuild;
    public BuildCommand(int source, Entity entity, Entity buildentity, Vector3 posToBuild) {

        commandType = CommandType.Build;
        teamID = source;
        entityToCommand = entity;

        entityToBuild = buildentity;
        positionToBuild = posToBuild;
    }
}

public class PickupCommand : Command
{
    public Item itemToPickup;
    public PickupCommand(int source, Entity entity, Item item) {

        commandType = CommandType.Build;
        teamID = source;
        entityToCommand = entity;

        itemToPickup = item;
    }
}


