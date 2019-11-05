using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;
using TMPro;

public class UIAttributes : MonoBehaviour
{
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

    public int viewingPet;

    void Start()
    {
        GameManager.onSelectedPetUpdate += UpdateViewingPet;
    }

    public void UpdateViewingPet(int index)
    {
        if(index < 0)
            index = 0;
        viewingPet = index;
    }

    void Update()
    {
        if(GameManager.instance.activePets[viewingPet] == null)
            return;
            
        ActivePet pet = GameManager.instance.activePets[viewingPet];

        species.text = "SP: " + pet.species.ToUpper();
        nickname.text = "NICK: " + pet.nickname.ToUpper();

        atk.fillAmount = pet.atk / 255f;
        spd.fillAmount = pet.spd / 255f;
        def.fillAmount = pet.def / 255f;

        atkVal.text = pet.atk.ToString();
        spdVal.text = pet.spd.ToString();
        defVal.text = pet.def.ToString();

        weightVal.text = "WT: " + pet.weight;
    }
}
