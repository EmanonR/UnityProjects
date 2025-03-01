using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayerTurn, EnemyTurn, Won, Lost, End }

public class BattleController : MonoBehaviour
{
    public float enemySpawnRange, playerSpawnRange;

    public Transform targetPlayer;
    public Vector3 camOffset;
    public Vector3 camGaze;

    [Range(1, 8)]
    public int enemyCount, playerCount;

    [Header("Battle Configuration")]
    public List<GameObject> EnemyParty;
    public List<GameObject> PlayerParty;
    public List<GameObject> turnOrder;

    [Header("Battle State")]
    public int currentTurn = 0;

    BattleLayout currentBattleLayout;
    BattleState battleState;

    Transform cam;

    private void Awake()
    {
        cam = Camera.main.transform;
    }

    public void SetupBattle(BattleLayout battleLayout)
    {
        battleState = BattleState.Start;
        currentBattleLayout = battleLayout;

        SpawnEntities();

        StartBattle();

    }

    private void Update()
    {
        MoveCamToPosition();
    }

    void MoveCamToPosition()
    {
        //Move camera to first player in queue
        cam.position = CalculateSpawnPoint(1, 1, playerSpawnRange);
        cam.position += transform.right * camOffset.x;
        cam.position += transform.up * camOffset.y;
        cam.position += transform.forward * camOffset.z;

        cam.LookAt(camGaze);
    }

    private void SpawnEntities()
    {
        // Clear Existing Entites
        EnemyParty.Clear();
        PlayerParty.Clear();

        //Spawn Enemies
        foreach (GameObject enemy in currentBattleLayout.enemies)
        {
            GameObject newEnemy = Instantiate(enemy);
            EnemyParty.Add(newEnemy);
        }

        //Spawn Players
        foreach (GameObject partyMem in CombatManager.instance.playerParty)
        {
            GameObject newPartyMem = Instantiate(partyMem);
            PlayerParty.Add(newPartyMem);
        }

        enemyCount = EnemyParty.Count;
        playerCount = PlayerParty.Count;

        //Move entites into position
        for (int i = 0; i < EnemyParty.Count; i++)
        {
            Vector3 spawnPoint = CalculateSpawnPoint(i, enemyCount, enemySpawnRange);
            EnemyParty[i].transform.position = new Vector3(spawnPoint.z, 0, spawnPoint.x);
        }

        for (int i = 0; i < PlayerParty.Count; i++)
        {
            PlayerParty[i].transform.position = CalculateSpawnPoint(i, playerCount, playerSpawnRange);
        }

    }

    private void StartBattle()
    {
        currentTurn = 0;
        StartNextTurn();
    }

    private void StartNextTurn()
    {
        currentTurn++;
        // Implement turn logic here
    }

    public void PostBattle()
    {
        switch (battleState)
        {
            case BattleState.Won:
                break;
            case BattleState.Lost:
                break;
            default:
                break;
        }
    }

    public void EndBattle()
    {
        CombatManager.instance.EndBattle();
    }

    Vector3 CalculateSpawnPoint(int num, int maxNum, float dist)
    {
        var radians = 2 * Mathf.PI / maxNum * num;

        var vertical = Mathf.Sin(radians);
        var horizontal = Mathf.Cos(radians);

        var spawnDir = new Vector3(horizontal, 0, vertical);

        Vector3 spawnPos = Vector3.zero + spawnDir * dist;

        return spawnPos;
    }

    void CalculateTurnOrder(int turnsToCalculate)
    {
        turnOrder.Clear();

        List<GameObject> tempList = new();

        foreach (GameObject enemy in EnemyParty)
        {
            tempList.Add(enemy);
        }

        foreach (GameObject partyMem in PlayerParty)
        {
            tempList.Add(partyMem);
        }

        for (int t = 0; t < turnsToCalculate; t++)
        {
            for (int i = 0; i < tempList.Count; i++)
            {
                //Sort by highest speed

                //Sort by each itteration, t * s + sCount, sCount needs to 0 out if chosen
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, enemySpawnRange);

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPoint = CalculateSpawnPoint(i, enemyCount, enemySpawnRange);
            Gizmos.DrawWireSphere(new Vector3(spawnPoint.z, 0, spawnPoint.x), .2f);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Vector3.zero, playerSpawnRange);
        for (int i = 0; i < playerCount; i++)
        {
            Gizmos.DrawWireSphere(CalculateSpawnPoint(i, playerCount, playerSpawnRange), .2f);
        }



        Vector3 camPos = CalculateSpawnPoint(1, 1, playerSpawnRange) + camOffset;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(camPos, 1f);
        Gizmos.DrawWireSphere(camGaze, .3f);
    }
}
