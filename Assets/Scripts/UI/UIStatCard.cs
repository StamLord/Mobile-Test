using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;
using TMPro;

public class UIStatCard : MonoBehaviour
{
    public UIImage sprite;

    public TextMeshProUGUI species;
    public TextMeshProUGUI nickname;

    public UIImage atk;
    public UIImage spd;
    public UIImage def;

    public TextMeshProUGUI atkVal;
    public TextMeshProUGUI spdVal;
    public TextMeshProUGUI defVal;

    public UIImage weightIcon;
    public TextMeshProUGUI weightVal;
    public Sprite[] weightIcons;

    public int viewingPet;


    public void UpdateCard(int index)
    {
        UpdateViewingPet(index);
        UpdateCard();
    }

    void UpdateViewingPet(int index)
    {
        if(index < 0)
            index = 0;
        viewingPet = index;
    }

    void UpdateCard()
    {
        if(viewingPet > GameManager.instance.activePets.Count - 1 || GameManager.instance.activePets[viewingPet] == null)
        {
            sprite.enabled = false;
            species.text = "?????";
            //nickname.text = "?????";

            atk.fillAmount = 0;
            spd.fillAmount = 0;
            def.fillAmount = 0;

            atkVal.text = "?";
            spdVal.text = "?";
            defVal.text = "?";

            weightVal.text = "???G";
            
            
            return;
        }
            
        ActivePet pet = GameManager.instance.activePets[viewingPet];

        sprite.sprite = GameManager.instance.FindSheet(GameManager.instance.activePets[viewingPet].species).idle[0];
        sprite.enabled = true;

        species.text = pet.species.ToUpper();
        //nickname.text = pet.nickname.ToUpper();

        atk.fillAmount = pet.atk / 255f;
        spd.fillAmount = pet.spd / 255f;
        def.fillAmount = pet.def / 255f;

        atkVal.text = pet.atk.ToString();
        spdVal.text = pet.spd.ToString();
        defVal.text = pet.def.ToString();

        weightVal.text = + pet.weight + "g";
        
        int weightStatus = pet.GetWeightStatus();
        switch(weightStatus)
        {
            case -1:
                weightIcon.sprite = weightIcons[0]; // Under
                break;
            case 0:
                weightIcon.sprite = weightIcons[1]; // Normal
                break;
            case 1:
                weightIcon.sprite = weightIcons[2]; // Over
                break;
        }
    }

    public void Share()
    {
        StartCoroutine(ShareCoroutine());
    }

    IEnumerator ShareCoroutine()
    {
        yield return new WaitForEndOfFrame();

        RectTransform rt = GetComponent<RectTransform>();
        Vector3[] worldCorners = new Vector3[4];
        rt.GetWorldCorners(worldCorners);

        Rect rect = new Rect(rt.rect.x, rt.rect.y, rt.rect.width, rt.rect.height);
        SharingManager.instance.ShareCard(rect);
    }
}
