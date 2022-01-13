using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManagerAuto : MonoBehaviour
{
    public BattleUIManager uiManager;

    private PetSnapshot player; // Side 1
    private PetSnapshot enemy;  // Side 2

    private int playerHealth;
    private int enemyHealth;

    private int playerStartHealth;
    private int enemyStartHealth;

    private Modifiers playerMods;
    private Modifiers enemyMods;

    private List<Boost> playerBoosts = new List<Boost>();
    private List<Boost> enemyBoosts = new List<Boost>();

    [SerializeField] float fightTimer;
    [SerializeField] bool fightStarted;

    private float playerNextAttack = -1;
    private float enemyNextAttack = -1;

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
        public int critical;
        public int strengthLose;
    }

    struct Boost
    {
        public float startTime;
        public float duration;
        public Modifiers modifiers;
    }

    void Start()
    {
        PetSnapshot player = new PetSnapshot();
        player.species = "Chamiri";
        player.Strength = 5;
        player.g_atk = 10;
        player.g_def = 10;
        player.g_spd = 10;

        PetSnapshot enemy = new PetSnapshot();
        enemy.species = "Nanasu";
        enemy.Strength = 5;
        enemy.g_atk = 10;
        enemy.g_def = 10;
        enemy.g_spd = 10;

        playerHealth = player.g_atk + player.g_spd + player.g_def * 50; 
        enemyHealth = enemy.g_atk + enemy.g_spd + enemy.g_def * 50;
        
        StartBattle(player, enemy);
    }

    void StartBattle(PetSnapshot player, PetSnapshot opponent)
    {
        this.player = player;
        this.enemy = opponent;

        playerStartHealth = playerHealth;
        enemyStartHealth = enemyHealth;

        InitializeVisualPets();

        // Initialize UI
        uiManager.UpdatePlayerStrength(player.Strength);
        uiManager.UpdateEnemyStrength(enemy.Strength);

        fightTimer = 0f;
        fightStarted = true;

        CalculateNextAttack(player);
        CalculateNextAttack(enemy);
    }

    void InitializeVisualPets()
    {
        playerSprite.spritesheet = GameManager.instance.FindSheet(player.species);
        enemySprite.spritesheet = GameManager.instance.FindSheet(enemy.species);
    }

    private void Update() 
    {
        if(fightStarted)
            FightUpdate();
    }

    private void FightUpdate()
    {
        fightTimer += Time.deltaTime;
        
        if(fightTimer >= playerNextAttack)
        {
            AgentAttack(1);
            playerNextAttack = CalculateNextAttack(player);
        }

        if(fightTimer >= enemyNextAttack)
        {
            AgentAttack(2);
            enemyNextAttack = CalculateNextAttack(enemy);
        }

        EvaluateBoosts(1);
        EvaluateBoosts(2);
    }

    private void AgentAttack(int side, int power = 10)
    {
        float rand = Random.Range(0, 101);
        int criticalBuff = (side == 1)? playerMods.critical : enemyMods.critical;
        
        int damage = 0;
        switch (side)
        {
            case 1:
                // Base Damage
                damage = CalculateDamage(player.atk, enemy.def, power, player.Strength, enemy.Strength, playerMods, enemyMods);
                
                // Critical Check
                if(rand <= 5f + criticalBuff)
                    damage *= 2;
                    
                playerMods.strengthLose += 1;
                if(playerMods.strengthLose >= 10)
                {
                    player.Strength--;
                    playerMods.strengthLose -= 10;
                    uiManager.UpdatePlayerStrength(player.Strength);
                }
                playerAnimator.Play("Attack");
                enemyHealth -= damage;
                uiManager.DisplayDamageToEnemy(damage);
                break;
            case 2:
                // Base Damage
                damage = CalculateDamage(enemy.atk, player.def, power, enemy.Strength, player.Strength, enemyMods, playerMods);
                
                // Critical Check
                if(rand <= 5f + criticalBuff)
                    damage *= 2;

                enemyMods.strengthLose += 1;
                if(enemyMods.strengthLose >= 10)
                {
                    enemy.Strength--;
                    enemyMods.strengthLose -= 10;
                    uiManager.UpdateEnemyStrength(enemy.Strength);
                }
                enemyAnimator.Play("Attack");
                playerHealth -= damage;
                uiManager.DisplayDamageToPlayer(damage);
                break;
        }
        Debug.Log("P: " + playerHealth + " E: " + enemyHealth);
        uiManager.UpdateHealth((float)playerHealth / playerStartHealth, (float)enemyHealth / enemyStartHealth);
    }

    float CalculateNextAttack(PetSnapshot agent)
    {
        float modifiedSpeed = agent.spd * (0.5f + 0.5f * (float)agent.Strength / 5);
        float nextAttack = 1 / (modifiedSpeed / 50);
        nextAttack += fightTimer;

        return nextAttack;
    }

    int CalculateDamage(int attackA, int defenseB, int attackPower, int strengthA, int strengthB, Modifiers boostA, Modifiers boostB)
    {
        float modAttack = attackA * (0.5f + 0.5f * (float)strengthA / 5) + boostA.atk;
        float modDefense = defenseB * (0.5f + 0.5f * (float)strengthB / 5) + boostB.def;
        Debug.Log(modDefense);
        //Debug.Log(string.Format("AttacK: {0} StrengthA: {1} ModAttack: {4} Defense: {2} StrengthB: {3} ModDefense: {5} ", attackA, strengthA, defenseB, strengthB, modAttack, modDefense));

        // Linear
        //int damage = (int)(modAttack - modDefense + attackPower + 1);
        int damage = (int)(modAttack + attackPower + 1) * 2 - (int)modDefense;
        Debug.Log("Damage: " + damage);

        return damage;
    }

    public void Defense()
    {
        Boost boost = new Boost();
        boost.startTime = fightTimer;
        boost.duration = 10f;
        boost.modifiers = new Modifiers();
        boost.modifiers.def = 20;

        AddBoost(1, boost);
    }

    public void HyperBeam()
    {
        AgentAttack(1, 100);
    }

    public void Focus()
    {
        Boost boost = new Boost();
        boost.startTime = fightTimer;
        boost.duration = 10f;
        boost.modifiers = new Modifiers();
        boost.modifiers.critical = 20;

        AddBoost(1, boost);
    }

    private void AddBoost(int side, Boost boost)
    {
        switch (side)
        {
            case 1:
                playerBoosts.Add(boost);
                playerMods.atk += boost.modifiers.atk;
                playerMods.def += boost.modifiers.def;
                playerMods.spd += boost.modifiers.spd;
                playerMods.critical += boost.modifiers.critical;
                Debug.Log(playerMods.def);
                break;
            case 2:
                enemyBoosts.Add(boost);
                enemyMods.atk += boost.modifiers.atk;
                enemyMods.def += boost.modifiers.def;
                enemyMods.spd += boost.modifiers.spd;
                enemyMods.critical += boost.modifiers.critical;
                break;
        }
    }

    private void EvaluateBoosts(int side)
    {
        List<Boost> toRemove = new List<Boost>();
        List<Boost> boosts = (side == 1)? playerBoosts : enemyBoosts;

        foreach (Boost boost in boosts)
        {
            if(boost.startTime + boost.duration >= fightTimer)
                toRemove.Add(boost);
        }

        foreach (Boost boost in toRemove)
        {
            boosts.Remove(boost);
            switch (side)
            {
                case 1:
                    playerMods.atk -= boost.modifiers.atk;
                    playerMods.def -= boost.modifiers.def;
                    playerMods.spd -= boost.modifiers.spd;
                    playerMods.critical -= boost.modifiers.critical;
                    break;
                case 2:
                    enemyMods.atk -= boost.modifiers.atk;
                    enemyMods.def -= boost.modifiers.def;
                    enemyMods.spd -= boost.modifiers.spd;
                    enemyMods.critical -= boost.modifiers.critical;
                    break;
            }
        }
    }
}
