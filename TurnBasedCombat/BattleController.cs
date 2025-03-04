using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState { Start, PlayerTurn, EnemyTurn, Won, Lost, End }
public enum TurnState { PickingOption, ChooseTarget, End}

public class BattleController : MonoBehaviour
{
    #region variables

    [Header("Inputs")]
    public KeyCode attackButton = KeyCode.Z;
    public KeyCode cancelButton = KeyCode.X;

    [Header("UI")]
    public GameObject attackButtonPrefab;
    public GameObject attackButtonParent;
    public GameObject targetGraphic;


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

    [HideInInspector] public List<GameObject> attackButtons;

    BattleLayout currentBattleLayout;
    BattleState battleState;
    TurnState turnState;
    BattleEntity turnEntity;
    Attack entityChosenAttack;
    Transform cam;
    bool playerTurn;
    int targetIndex;
    #endregion


    #region Awake Update
    private void Awake()
    {
        cam = Camera.main.transform;
    }

    private void Update()
    {
        MoveCamToPosition();

        if (playerTurn)
            switch (turnState)
            {
                case TurnState.PickingOption:
                    break;
                case TurnState.ChooseTarget:
                    targetGraphic.SetActive(true);
                    PlayerTargetingLogic();
                    DisplayTargetGraphic();
                    break;
                case TurnState.End:
                    targetGraphic.SetActive(false);
                    AttackEntity(playerTurn ? EnemyParty[targetIndex].GetComponent<BattleEntity>() : PlayerParty[targetIndex].GetComponent<BattleEntity>(),
                        turnEntity, entityChosenAttack);
                    break;
                default:
                    break;
            }
    }
    #endregion


    #region setup
    public void SetupBattle(BattleLayout battleLayout)
    {
        battleState = BattleState.Start;
        currentBattleLayout = battleLayout;

        targetGraphic.gameObject.SetActive(false);

        SpawnEntities();

        StartBattle();

    }
    private void StartBattle()
    {
        currentTurn = 0;
        StartNextTurn();
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

            BattleEntity entityData = newEnemy.GetComponent<BattleEntity>();
            entityData.currentHp = entityData.maxHp;
        }

        //Spawn Players
        foreach (GameObject partyMem in CombatManager.instance.playerParty)
        {
            GameObject newPartyMem = Instantiate(partyMem);
            PlayerParty.Add(newPartyMem);

            BattleEntity entityData = newPartyMem.GetComponent<BattleEntity>();
            entityData.currentHp = entityData.maxHp;
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

    #endregion


    #region Battle handling
    private void StartNextTurn()
    {
        attackButtonParent.SetActive(false);
        targetIndex = 0;

        if (battleState == BattleState.Lost || battleState == BattleState.Won)
        {
            PostBattle();
            return;
        }

        currentTurn++;
        // Implement turn logic here

        //Update all speed
        foreach (GameObject player in PlayerParty)
        {
            player.GetComponent<BattleEntity>().UpdateSpeed();
        }
        foreach (GameObject enemy in EnemyParty)
        {
            enemy.GetComponent<BattleEntity>().UpdateSpeed();
        }

        GameObject turnObj = CalculateTurn();
        turnEntity = turnObj.GetComponent<BattleEntity>();
        playerTurn = PlayerParty.Contains(turnObj);

        string attackInfo = turnObj.name + " can use attacks: ";
        for (int i = 0; i < turnEntity.attacks.Length; i++)
        {
            if (i != 0)
                attackInfo += ", ";

            attackInfo += turnEntity.attacks[i].name;
        }

        print("Current turn is " + turnObj.name);
        print(attackInfo);

        //Turn decides order

        if (playerTurn)
            PlayerTurn();
        else
            EnemyTurn();
    }

    public void PlayerTurn()
    {
        turnState = TurnState.PickingOption;

        //Display choices
        InstantiateAttackButtons();
    }

    public void EnemyTurn()
    {
        print("Enemy attacks");

        //Select random attack
        int ran = Random.Range(0, turnEntity.attacks.Length);
        BattleEntity target = PlayerParty[Random.Range(0, PlayerParty.Count)].GetComponent<BattleEntity>();

        //Perform attack on target
        AttackEntity(target, turnEntity, turnEntity.attacks[ran]);
    }

    public void PostBattle()
    {
        switch (battleState)
        {
            case BattleState.Won:
                print("Player has won");
                EndBattle();
                break;
            case BattleState.Lost:
                print("Player has Lost");
                EndBattle();
                break;
            default:
                break;
        }
    }

    public void EndBattle()
    {
        CombatManager.instance.EndBattle();
    }


    #endregion

    #region Battle Logic
    void PlayerTargetingLogic()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Left
            if (targetIndex == 0)
                targetIndex = EnemyParty.Count - 1;

            else
                targetIndex--;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Right
            if (targetIndex == EnemyParty.Count - 1)
                targetIndex = 0;

            else
                targetIndex++;
        }

        if (Input.GetKeyDown(attackButton))
        {
            // Choose and attack
            turnState = TurnState.End;
        }

        if (Input.GetKeyDown(cancelButton))
        {
            // Cancel and go back
            turnState = TurnState.PickingOption;
        }
    }

    void AttackEntity(BattleEntity target, BattleEntity blame, Attack attack)
    {
        print($"{blame.name} selected attack: {attack.name} and attacked {target.name}");

        // Perform the attack
        bool targetDeathState = target.TakeDamage(attack.baseDamage);

        if (targetDeathState) OnEntityDeath(target);


        // Clean up UI if player turn
        if (playerTurn)
        {
            // Clean up attack buttons
            foreach (GameObject button in attackButtons)
            {
                Destroy(button);
            }

            attackButtons.Clear();
        }

        StartNextTurn();
    }

    void OnEntityDeath(BattleEntity entity)
    {
        print($"{entity.name} has died");


        if (playerTurn)
        {
            // Killed enemy
            EnemyParty.Remove(entity.gameObject);
            Destroy(entity.gameObject);


            if (EnemyParty.Count == 0)
            {
                battleState = BattleState.Won;
            }
        }
        else
        {
            // Killed Player
            // Player cant attack / downed

            if (PlayerParty.Count == 0)
            {
                battleState = BattleState.Lost;
            }
        }
    }
    #endregion


    #region UI
    void DisplayTargetGraphic()
    {
        Transform target = EnemyParty[targetIndex].transform;
        Vector3 targetPosition = cam.GetComponent<Camera>().WorldToScreenPoint(target.position);
        targetGraphic.transform.position = targetPosition;
    }

    void InstantiateAttackButtons()
    {
        attackButtonParent.SetActive(true);

        for (int i = 0; i < turnEntity.attacks.Length; i++)
        {
            // Store the current attack index to pass to the listener
            int attackIndex = i;

            GameObject attackButton = Instantiate(attackButtonPrefab, attackButtonParent.transform);
            attackButtons.Add(attackButton);

            TMP_Text attackText = attackButton.GetComponentInChildren<TMP_Text>();
            attackText.text = turnEntity.attacks[i].name;

            // Use a lambda to capture the specific attack index
            attackButton.GetComponent<Button>().onClick.AddListener(() => AttackButton(attackIndex));
        }
    }

    public void AttackButton(int attackIndex)
    {
        // Find Attack in entity list
        entityChosenAttack = turnEntity.attacks[attackIndex];

        turnState = TurnState.ChooseTarget;
    }

    #endregion


    #region Helpers
    void MoveCamToPosition()
    {
        //Move camera to first player in queue
        cam.position = CalculateSpawnPoint(1, 1, playerSpawnRange);
        cam.position += transform.right * camOffset.x;
        cam.position += transform.up * camOffset.y;
        cam.position += transform.forward * camOffset.z;

        cam.LookAt(camGaze);
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

    GameObject CalculateTurn()
    {
        turnOrder.Clear();

        int highestSpeed = PlayerParty[0].GetComponent<BattleEntity>().speedCount;
        GameObject highestSpeedEntity = PlayerParty[0];

        for (int i = 0; i < PlayerParty.Count; i++)
        {
            if (PlayerParty[i].GetComponent<BattleEntity>().speedCount > highestSpeed)
            {
                highestSpeed = PlayerParty[i].GetComponent<BattleEntity>().speedCount;
                highestSpeedEntity = PlayerParty[i];
            }

        }

        for (int i = 0; i < EnemyParty.Count; i++)
        {
            if (EnemyParty[i].GetComponent<BattleEntity>().speedCount > highestSpeed)
            {
                highestSpeed = EnemyParty[i].GetComponent<BattleEntity>().speedCount;
                highestSpeedEntity = EnemyParty[i];
            }
        }
        highestSpeedEntity.GetComponent<BattleEntity>().ResetSpeed();
        return highestSpeedEntity;
    }

    void CalculateTurnOrder(int turnsToCalculate)
    {
        //Not finished
        turnOrder.Clear();

        int highestSpeed = PlayerParty[0].GetComponent<BattleEntity>().speedCount;
        GameObject highestSpeedEntity = PlayerParty[0];

        for (int i = 0; i < PlayerParty.Count; i++)
        {
            if (PlayerParty[i].GetComponent<BattleEntity>().speedCount > highestSpeed)
            {
                highestSpeed = PlayerParty[i].GetComponent<BattleEntity>().speedCount;
                highestSpeedEntity = PlayerParty[i];
            }

        }

        for (int i = 0; i < EnemyParty.Count; i++)
        {
            if (EnemyParty[i].GetComponent<BattleEntity>().speedCount > highestSpeed)
            {
                highestSpeed = EnemyParty[i].GetComponent<BattleEntity>().speedCount;
                highestSpeedEntity = EnemyParty[i];
            }
        }
    }
    #endregion

    #region Gizmos
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
    #endregion
}
