using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Team{
    public string name;
    public int teamID;
    public bool isPlayerTeam;
    public bool isHostileToPlayer;

    Team(string _name, int id, bool player, bool hostile) {
        name = _name;
        teamID = id;
        isPlayerTeam = player;
        isHostileToPlayer = hostile;
    }
}
