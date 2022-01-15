using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;

public class UIIndicators : MonoBehaviour
{
    [SerializeField] private UIView view;
    [SerializeField] private Sprite[] moodIcons;
    [SerializeField] private Sprite[] disciplineIcons;

    [SerializeField] private UIImage mood;
    [SerializeField] private UIImage injury;
    [SerializeField] private UIImage sickness;
    [SerializeField] private UIImage discipline;

    [SerializeField] private int viewingPet = 0;

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

        // Update Mood Icon
        int m_icon = Mathf.FloorToInt(pet.happiness / 100f * (moodIcons.Length - 1));
        if(m_icon < 0)
            mood.enabled = false;
        else
        {
            mood.enabled = true;
            mood.sprite = moodIcons[m_icon];    
        }

        // Update Discipline Icon
        int d_icon = Mathf.FloorToInt(pet.discipline / 100f * (disciplineIcons.Length - 1));
        Debug.Log(pet.discipline);
        if(d_icon < 0)
            discipline.enabled = false;
        else
        {
            discipline.enabled = true;
            discipline.sprite = disciplineIcons[d_icon];    
        }

        injury.enabled = pet.isInjured;
        sickness.enabled = false;
    }

    void OnDestroy()
    {
        GameManager.onSelectedPetUpdate -= UpdateViewingPet;
    }
}
