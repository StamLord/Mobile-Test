using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public BattleUIManager uiManager;

    PetSnapshot player;
    PetSnapshot enemy;

    int playerHealth;
    int enemyHealth;

    Modifiers playerBoost;
    Modifiers enemyBoost;

    bool usedItem;

    [SerializeField] bool playerDefending;
    [SerializeField] bool playerCounterAttack;
    [SerializeField] bool enemyDefending;
    [SerializeField] bool enemyCounterAttack;

    public float fightPhaseDuration = 15;
    public int turnsNumber = 3;

    [SerializeField] int turn = 0;

    public enum Phase {Orders, Fight, Ended};
    public Phase phase;

    public enum Order {Balanced, Aggressive, Defensive, Command};

    public int orderSize = 10;
    float orderTime;
    [SerializeField] List<Order> playerOrders = new List<Order>();
    [SerializeField] List<Order> enemyOrders = new List<Order>();

    int playerActionSize = 0;
    [SerializeField] float playerActionTime = 0;

    int enemyActionSize = 0;
    [SerializeField] float enemyActionTime = 0;

    [SerializeField] float fightTimer;
    [SerializeField] float lastPlayerAction;
    [SerializeField] float lastEnemyAction;

    #region Visual

    public SpritesheetAnimator playerSprite;
    public SpritesheetAnimator enemySprite;
    public Animator playerAnimator;
    public Animator enemyAnimator;
    
    #endregion

    struct Modifiers 
    {
        public int atk;
        public int spd;
        public int def;
        public int strengthLose;
    }

    void Start()
    {
        PetSnapshot player = new PetSnapshot();
        player.species = "Chamiri";
        player.Strength = 5;
        player.g_atk = 255;
        player.g_def = 255;
        player.g_spd = 255;

        PetSnapshot enemy = new PetSnapshot();
        enemy.species = "Nanasu";
        enemy.Strength = 5;
        enemy.g_atk = 255;
        enemy.g_def = 255;
        enemy.g_spd = 255;

        playerHealth = enemyHealth = 100;
        
        StartBattle(player, enemy);
    }

    void StartBattle(PetSnapshot player, PetSnapshot opponent)
    {
        this.player = player;
        this.enemy = opponent;

        orderTime = fightPhaseDuration / orderSize;

        turn = 0;
        

        InitializeVisualPets();

        // Initialize UI
        uiManager.UpdatePlayerStrength(player.Strength);
        uiManager.UpdateEnemyStrength(enemy.Strength);
        uiManager.SetOrders(orderSize);

        StartOrderPhase();
    }

    void InitializeVisualPets()
    {
        playerSprite.spritesheet = GameManager.instance.FindSheet(player.species);
        enemySprite.spritesheet = GameManager.instance.FindSheet(enemy.species);
    }

    void StartOrderPhase()
    {
        playerOrders.Clear();
        enemyOrders.Clear();

        uiManager.UpdateOrders(playerOrders);
        uiManager.UpdateFightIndicator(-0.5f);

        usedItem = false;

        phase = Phase.Orders;

        // Activate UI for order placement
        uiManager.ShowOrderPhaseWindow(true);
        uiManager.ShowItemsWindow(turn > 0);

        for(int i = 0; i < orderSize; i++)
            enemyOrders.Add((Order)Random.Range(1,2));
    }

    public void AddOrder(int order)
    {
        if(phase != Phase.Orders)
            return;

        if(playerOrders.Count >= orderSize)
            return;

        playerOrders.Add((Order)order);

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
        while(playerOrders.Count < orderSize)
        {
            playerOrders.Add(Order.Balanced);
        }

        uiManager.UpdateOrders(playerOrders);

        StartFightPhase();
    }

    public void UseItem(string item)
    {
        if(usedItem)
            return;

        switch(item)
        {
            case "Potion":
                playerHealth += 35;
                break;
            case "Energy Drink":
                player.Strength += 3;
                break;
            case "ATK Boost":
                playerBoost.atk = (int)(player.atk * 0.1);
                break;
            case "SPD Boost":
                playerBoost.spd = (int)(player.spd * 0.1);
                break;
            case "DEF Boost":
                playerBoost.def = (int)(player.def * 0.1);
                break;
        }

        usedItem = true;

        uiManager.UpdateHealth(playerHealth, enemyHealth);
    }

    void StartFightPhase()
    {
        // Hide ui
        uiManager.ShowOrderPhaseWindow(false);

        playerActionSize = CalculateActionSize(player.spd + playerBoost.spd);
        playerActionTime = fightPhaseDuration / playerActionSize;

        enemyActionSize = CalculateActionSize(enemy.spd + enemyBoost.spd);
        enemyActionTime = fightPhaseDuration / enemyActionSize;

        fightTimer = 0f;
        lastPlayerAction = 0f;
        lastEnemyAction = 0f;

        phase = Phase.Fight;
    }

    void Update()
    {
        if(phase == Phase.Fight)
            FightUpdate();
    }

    void FightUpdate()
    {
        fightTimer += Time.deltaTime;
    
        if(fightTimer >= lastPlayerAction + playerActionTime)
            ExecutePlayerAction();

        if(fightTimer >= lastEnemyAction + enemyActionTime)
            ExecuteEnemyOrder();

        if(playerHealth < 0 || enemyHealth < 0)
            EndBattle();

        if(fightTimer >= fightPhaseDuration)
            EndFightPhase();

        uiManager.UpdateFightIndicator(fightTimer/fightPhaseDuration);
    }

    void ExecutePlayerAction()
    {
        Order currentOrder = playerOrders[Mathf.FloorToInt(fightTimer / orderTime)];
        Debug.Log("Player acting under order: " + currentOrder.ToString());

        playerDefending = false;
        playerCounterAttack = false;
        
        float chance = Random.Range(0f, 1f);
        switch(currentOrder)
        {
            case Order.Balanced:
                if(chance > 0.1f)
                {
                    if(chance> 0.55f)
                        PlayerAttack();
                    else
                        PlayerDefend();
                }
                break;
            case Order.Aggressive:
                if(chance > 0.1)
                    PlayerAttack();
                break;
            case Order.Defensive:
                if(chance > 0.1)
                    PlayerDefend();
                break;
        }

        playerAnimator.SetBool("Defending", playerDefending);

        lastPlayerAction = fightTimer;
    }

    void ExecuteEnemyOrder()
    {
        Order currentOrder = enemyOrders[Mathf.FloorToInt(fightTimer / orderTime)];
        Debug.Log("Enemy acting under order: " + currentOrder.ToString());

        enemyDefending = false;
        enemyCounterAttack = false;

        float chance = Random.Range(0f, 1f);
        switch(currentOrder)
        {
            case Order.Balanced:
                if(chance > 0.1f)
                {
                    if(chance> 0.55f)
                        EnemyAttack();
                    else
                        EnemyDefend();
                }
                break;
            case Order.Aggressive:
                if(chance > 0.1)
                    EnemyAttack();
                break;
            case Order.Defensive:
                if(chance > 0.1)
                    EnemyDefend();
                break;
        }

        enemyAnimator.SetBool("Defending", enemyDefending);

        lastEnemyAction = fightTimer;
    }

    void PlayerAttack()
    {
        int damage = CalculateDamage(player.atk, enemy.def, 10, player.Strength, enemy.Strength, playerBoost, enemyBoost);

        playerBoost.strengthLose += 1;
        if(playerBoost.strengthLose >= 5)
        {
                player.Strength--;
                playerBoost.strengthLose -= 5;
                uiManager.UpdatePlayerStrength(player.Strength);
        }

        playerAnimator.Play("Attack");
        
        if(enemyDefending)
        {
            if(enemyCounterAttack)
            {   
                enemyAnimator.Play("Attack");
                damage *= 2;
                playerHealth -= damage;
                uiManager.DisplayDamageToPlayer(damage);
            }
            else
            {
                damage /= 2;
                if(damage < 1) damage = 1;
                enemyHealth -= damage;
                uiManager.DisplayDamageToEnemy(damage);
            }
        }
        else
        {
            enemyHealth -= damage;
            uiManager.DisplayDamageToEnemy(damage);
        }

        uiManager.UpdateHealth(playerHealth, enemyHealth);
    }

    void EnemyAttack()
    {
        int damage = CalculateDamage(enemy.atk, player.def, 10, enemy.Strength, player.Strength, enemyBoost, playerBoost);

        enemyBoost.strengthLose += 1;
        if(enemyBoost.strengthLose >= 5)
        {
                enemy.Strength--;
                enemyBoost.strengthLose -= 5;
                uiManager.UpdateEnemyStrength(enemy.Strength);
        }

        enemyAnimator.Play("Attack");
                    
        if(playerDefending)
        {
            if(playerCounterAttack)
            {    
                playerAnimator.Play("Attack");
                damage *= 2;
                enemyHealth -= damage;
                uiManager.DisplayDamageToEnemy(damage);
            }
            else
            {
                damage /= 2;
                if(damage < 1) damage = 1;
                playerHealth -= damage;
                uiManager.DisplayDamageToPlayer(damage);
            }
        }
        else
        {
            if(damage < 1) damage = 1;
            playerHealth -= damage;
            uiManager.DisplayDamageToPlayer(damage);
        }

        uiManager.UpdateHealth(playerHealth, enemyHealth);
    }

    void PlayerDefend()
    {
        playerBoost.strengthLose -= 1;
        if(playerBoost.strengthLose <= -5)
        {
            player.Strength++;
            playerBoost.strengthLose += 5;
            uiManager.UpdatePlayerStrength(player.Strength);
        }

        playerDefending = true;
        playerCounterAttack = (Random.Range(0f, 1f) < 0.25f) ? true : false; 
    }   

    void EnemyDefend()
    {
        enemyBoost.strengthLose -= 1;
        if(enemyBoost.strengthLose <= -5)
        {
            enemy.Strength++;
            enemyBoost.strengthLose += 5;
            uiManager.UpdateEnemyStrength(enemy.Strength);
        }

        enemyDefending = true;
        enemyCounterAttack = (Random.Range(0f, 1f) < 0.25f) ? true : false; 
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

    int CalculateActionSize(int speed)
    {
        int actionSize = 5 + Mathf.FloorToInt(speed / 10);
        return actionSize;
    }

    int CalculateDamage(int attackA, int defenseB, int attackPower, int strengthA, int strengthB, Modifiers boostA, Modifiers boostB)
    {
        float modAttack = attackA * (0.2f + (float)strengthA / 5) + boostA.atk;
        float modDefense = defenseB * (0.2f + (float)strengthB / 5) + boostB.def;
        Debug.Log(string.Format("AttacK: {0} StrengthA: {1} ModAttack: {4} Defense: {2} StrengthB: {3} ModDefense: {5} ", attackA, strengthA, defenseB, strengthB, modAttack, modDefense));

        // Linear
        int damage = (int)(modAttack / modDefense * attackPower / 10 + 1);
        Debug.Log("Damage: " + damage);

        // nth Root
        // return (int)((Mathf.Sqrt(attackPower * 10) * attackA / defenseB) / 20) + 1;

        return damage;
    }
}
