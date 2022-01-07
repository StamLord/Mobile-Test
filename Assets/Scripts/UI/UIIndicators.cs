using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;

public class UIIndicators : MonoBehaviour
{
    [SerializeField] private UIView view;
    public Sprite[] moodIcons;

    public UIImage mood;
    public UIImage injury;
    public UIImage sickness;

    public int viewingPet = 0;

    void Awake()
    {
        view = GetComponent<UIView>();
    }

    void Start()
    {
        GameManager.onSelectedPetUpdate += UpdateViewingPet;
    }
    
    void Update()
    {
        UpdateIndicators();
    }

    public void UpdateViewingPet(int index)
    {
        if(index == -1)
            view.Hide();
        else
        {
            viewingPet = index;
            view.Show();
        }
    }

    void UpdateIndicators()
    {   
        if(GameManager.instance.activePets.Count == 0 || GameManager.instance.activePets[viewingPet] == null)
            return;

        ActivePet pet = GameManager.instance.activePets[viewingPet];

        int m_icon = Mathf.FloorToInt(pet.happiness / 100f * moodIcons.Length - 1 );
        Debug.Log(m_icon);
        if(m_icon < 0)
            mood.enabled = false;
        else
        {
            mood.enabled = true;
            mood.sprite = moodIcons[m_icon];    
        }
        

        injury.enabled = pet.isInjured;
        sickness.enabled = false;
    }

    void OnDestroy()
    {
        GameManager.onSelectedPetUpdate -= UpdateViewingPet;
    }
}
