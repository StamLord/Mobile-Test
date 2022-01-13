using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    public UIImage playerHealth;
    public UIImage enemyHealth;

    public TextMeshProUGUI playerDamage;
    public Animator playerDamageAnim;
    public TextMeshProUGUI enemyDamage;
    public Animator enemyerDamageAnim;

    public GameObject[] playerStrBars;
    public GameObject[] enemyStrBars;

    public GameObject orderphaseWindow;
    public GameObject itemsWindow;

    public Transform ordersParent;
    public GameObject orderPrefab;
    public BattleUIOrder[] spawnedOrders;

    public UIImage fightIndicator;

    private new Camera camera;
    private float screenStart;
    private float screenEnd;

    void Start()
    {
        camera = Camera.main;
        screenStart = camera.ViewportToScreenPoint(new Vector3(0,0,0)).x;
        screenEnd = camera.ViewportToScreenPoint(new Vector3(1,0,0)).x;
    }

    public void ShowOrderPhaseWindow(bool visible)
    {
        orderphaseWindow.SetActive(visible);
    }

    public void ShowItemsWindow(bool visible)
    {
        itemsWindow.SetActive(visible);
    }

    public void SetOrders(int amount)
    {
        spawnedOrders = new BattleUIOrder[amount];
        for(int i = 0; i < spawnedOrders.Length; i++)
        {
            spawnedOrders[i] = Instantiate(orderPrefab, ordersParent).GetComponent<BattleUIOrder>();
        }
    }

    public void UpdateOrders(List<BattleManagerOld2.Order> orders)
    {
        if(orders.Count > spawnedOrders.Length)
            Debug.LogWarning("Mismatch between spawnedOrders and orders!");

        for(int i =0; i < spawnedOrders.Length; i++)
        {
            // Update visually
            if(i < orders.Count)
                spawnedOrders[i].UpdateText(orders[i].ToString());
            else
                spawnedOrders[i].UpdateText("");
        }
    }

    public void UpdateHealth(float playerHealth, float enemyHealth)
    {
        this.playerHealth.fillAmount = playerHealth;
        this.enemyHealth.fillAmount = enemyHealth;
    }

    public void UpdatePlayerStrength(int strength)
    {
        for(int i = 0; i<playerStrBars.Length; i++)
        {
            playerStrBars[i].SetActive(i < strength);
        }
    }

    public void UpdateEnemyStrength(int strength)
    {
        for(int i = 0; i<enemyStrBars.Length; i++)
        {
            enemyStrBars[i].SetActive(i < strength);
        }
    }

    public void UpdateFightIndicator(float precentage)
    {
        float halfWidth = fightIndicator.rectTransform.rect.width / 2;
        float posX = (screenEnd - halfWidth) - (screenStart + halfWidth);
        posX *= precentage;
        fightIndicator.rectTransform.position = new Vector2(posX, fightIndicator.rectTransform.position.y);
    }

    public void DisplayDamageToPlayer(int damage)
    {
        playerDamage.text = damage.ToString();
        playerDamageAnim.Play("Popup", 0, 0);
    }

    public void DisplayDamageToEnemy(int damage)
    {
        enemyDamage.text = damage.ToString();
        enemyerDamageAnim.Play("Popup", 0, 0);
    }
}
