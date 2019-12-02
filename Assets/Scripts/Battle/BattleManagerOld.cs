using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class BattleManagerOld : MonoBehaviour
{
    public BattleUIManager uiManager;

    PetSnapshot player;
    PetSnapshot enemy;

    int playerHealth;
    int enemyHealth;

    public float fightPhaseDuration = 15;
    public int turnsNumber = 3;

    [SerializeField] int turn = 0;

    public enum Phase {Orders, Fight, Ended};
    public Phase phase;

    public enum Order {Balanced, Aggressive, Defensive, Command};

    [SerializeField] List<Order> playerOrders = new List<Order>();
    int playerOrderSize = 0;
    [SerializeField] float playerOrderTime = 0;

    [SerializeField] List<Order> enemyOrders = new List<Order>();
    int enemyOrderSize = 0;
    [SerializeField] float enemyOrderTime = 0;

    [SerializeField] float fightTimer;
    [SerializeField] int currentPlayerOrder;
    [SerializeField] int currentEnemyOrder;

    public SpritesheetAnimator playerAnimator;
    public SpritesheetAnimator enemyAnimator;

    void Start()
    {
        PetSnapshot player = new PetSnapshot();
        player.species = "Chamiri";
        player.g_atk = 50;
        player.g_def = 150;
        player.g_spd = 200;

        PetSnapshot enemy = new PetSnapshot();
        enemy.species = "Nanasu";
        enemy.g_atk = 200;
        enemy.g_def = 200;
        enemy.g_spd = 100;

        playerHealth = enemyHealth = 100;
        
        StartBattle(player, enemy);
    }

    void StartBattle(PetSnapshot player, PetSnapshot opponent)
    {
        this.player = player;
        this.enemy = opponent;

        turn = 0;
        playerOrderSize = CalculateOrderSize(player.spd);
        playerOrderTime = fightPhaseDuration / playerOrderSize;

        enemyOrderSize = CalculateOrderSize(enemy.spd);
        enemyOrderTime = fightPhaseDuration / enemyOrderSize;

        InitializeVisualPets();

        // Initialize UI
        uiManager.SetOrders(playerOrderSize);

        StartOrderPhase();
    }

    void InitializeVisualPets()
    {
        playerAnimator.spritesheet = GameManager.instance.FindSheet(player.species);
        enemyAnimator.spritesheet = GameManager.instance.FindSheet(enemy.species);
    }

    void StartOrderPhase()
    {
        playerOrders.Clear();
        enemyOrders.Clear();
        uiManager.UpdateOrders(playerOrders);

        phase = Phase.Orders;
        // Activate UI for order placement
        uiManager.ShowOrderPhaseWindow(true);

        for(int i = 0; i < enemyOrderSize; i++)
            enemyOrders.Add((Order)Random.Range(0,3));
    }

    public void AddOrder(int order)
    {
        if(phase != Phase.Orders)
            return;

        if(playerOrders.Count >= playerOrderSize)
            return;

        switch(order)
        {
            case 0:
                playerOrders.Add(Order.Balanced);
                break;
            case 1:
                playerOrders.Add(Order.Aggressive);
                break;
            case 2:
                playerOrders.Add(Order.Defensive);
                break;
            case 3:
                playerOrders.Add(Order.Command);
                break;
        }

        // Update UI
        uiManager.UpdateOrders(playerOrders);
    }

    public void ReplaceOrder(int index, Order order)
    {
        playerOrders[index] = order;
    }

    public void Ready()
    {
        // Autofill if not enough orders
        while(playerOrders.Count < playerOrderSize)
        {
            playerOrders.Add(Order.Balanced);
        }

        StartFightPhase();
    }

    void StartFightPhase()
    {
        // Hide ui
        uiManager.ShowOrderPhaseWindow(false);

        fightTimer = 0f;
        currentPlayerOrder = 0;
        currentEnemyOrder = 0;
        
        phase = Phase.Fight;
        
        // Update UI
    }

    void Update()
    {
        if(phase == Phase.Fight)
            FightUpdate();
    }

    void FightUpdate()
    {
        fightTimer += Time.deltaTime;

        if(fightTimer >= currentPlayerOrder * playerOrderTime && currentPlayerOrder < playerOrderSize-1)
            ExecutePlayerOrder();

        if(fightTimer >= currentEnemyOrder * enemyOrderTime && currentEnemyOrder < enemyOrderSize-1)
            ExecuteEnemyOrder();

        if(playerHealth < 0 || enemyHealth < 0)
            EndBattle();

        if(fightTimer >= fightPhaseDuration)
            EndFightPhase();
    }

    void ExecutePlayerOrder()
    {
        if(currentPlayerOrder >= playerOrders.Count)
            return;
        
        int damage = CalculateDamage(player.atk, enemy.def, 10);
        
        if(enemyOrders[currentEnemyOrder] == Order.Defensive)
            damage /= 2;

        switch(playerOrders[currentPlayerOrder])
        {
            case Order.Balanced:
                Debug.Log("Balanced");
                float attackChance = Random.Range(0f, 1f);
                if(attackChance > 0.5f)
                {
                    enemyHealth -= damage;
                    Debug.Log("Damage: " + damage);
                    uiManager.UpdateHealth(playerHealth, enemyHealth);
                }
                break;
            case Order.Aggressive:
                Debug.Log("Aggressive");
                enemyHealth -= damage;
                Debug.Log("Damage: " + damage);
                uiManager.UpdateHealth(playerHealth, enemyHealth);
                break;
            case Order.Defensive:
                Debug.Log("Defensive");
                break;
            case Order.Command:
                Debug.Log("Command");
                break;
        }

        currentPlayerOrder++;
        currentPlayerOrder = Mathf.Clamp(currentPlayerOrder, 0, playerOrders.Count-1);
    }

    void ExecuteEnemyOrder()
    {
        if(currentEnemyOrder >= enemyOrders.Count)
            return;
        
        int damage = CalculateDamage(enemy.atk, player.def, 10);

        if(playerOrders[currentPlayerOrder] == Order.Defensive)
            damage /= 2;

        switch(enemyOrders[currentEnemyOrder])
        {
            case Order.Balanced:
                Debug.Log("Balanced");
                float attackChance = Random.Range(0f, 1f);
                if(attackChance > 0.5f)
                {
                    playerHealth -= damage;
                    Debug.Log("Damage: " + damage);
                    uiManager.UpdateHealth(playerHealth, enemyHealth);
                }
                break;
            case Order.Aggressive:
                Debug.Log("Aggressive");
                playerHealth -= damage;
                Debug.Log("Damage: " + damage);
                uiManager.UpdateHealth(playerHealth, enemyHealth);
                break;
            case Order.Defensive:
                Debug.Log("Defensive");
                break;
            case Order.Command:
                Debug.Log("Command");
                break;
        }

        currentEnemyOrder++;
        currentEnemyOrder = Mathf.Clamp(currentEnemyOrder, 0, enemyOrders.Count-1);
    }

    void EndFightPhase()
    {
        turn++;
        if(turn < turnsNumber)
        {
            StartOrderPhase();
        }
        else
            EndBattle();
    }

    void EndBattle()
    {
        phase = Phase.Ended;

        // Check who's winner

    }

    int CalculateOrderSize(int speed)
    {
        int orderSize = 5 + Mathf.FloorToInt(speed / 10);
        return orderSize;
    }

    int CalculateDamage(int attackA, int defenseB, int attackPower)
    {
        // Linear
        // return attackA / defenseB * attackPower / 2 + 1;

        // nth Root
        return (int)(Mathf.Sqrt(attackPower * 10) * attackA / defenseB) + 1;
    }
}
*/