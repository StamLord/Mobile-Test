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

    public int activePet;

    void Update()
    {
        ActivePet pet = GameManager.instance.activePets[activePet];

        species.text = "SP: " + pet.species;
        nickname.text = "NICK: " + pet.nickname;

        atk.fillAmount = pet.atk / 255f;
        spd.fillAmount = pet.spd / 255f;
        def.fillAmount = pet.def / 255f;

        atkVal.text = pet.atk.ToString();
        spdVal.text = pet.spd.ToString();
        defVal.text = pet.def.ToString();

    }
}
