using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Doozy.Engine.UI;

public class DeadPopup : MonoBehaviour
{
    public TextMeshProUGUI petName;
    public TextMeshProUGUI petAge;
    public UIPopup window;

    public void Popup(ActivePet pet)
    {
        petName.text = pet.nickname;
        petAge.text = "AGE " + pet.age;
        window.Show();
    }

    public void Hide()
    {
        window.Hide();
    }

    public bool IsVisible()
    {
        return window.IsVisible;
    }
}
