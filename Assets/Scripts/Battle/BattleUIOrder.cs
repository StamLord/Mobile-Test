using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BattleUIOrder : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void UpdateText(string text)
    {
        this.text.text = text;
    }
}
