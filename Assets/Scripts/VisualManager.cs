using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VisualManager : MonoBehaviour
{
    [SerializeField] private Transform visualParent;
    [SerializeField] private float horizontalRange = 5f;

    [SerializeField] private GameObject brainPrefab;
    [SerializeField] private GameObject visualPrefab;

    [SerializeField] private GameObject selection;
    [SerializeField] private Vector3 selectionOffset;

    [SerializeField] private ClickListener background;

    [SerializeField] private List<AIBrain> visualPets = new List<AIBrain>();

    [SerializeField] private GameObject evolutionParent;
    [SerializeField] private SpriteRenderer oldEvo;
    [SerializeField] private SpriteRenderer newEvo;

    void Awake()
    {
        GameManager.onActivePetsChange += UpdateSpriteSheets;
        GameManager.onSelectedPetUpdate += UpdateSelection;
        GameManager.onMisbehavePet += PetMisbehave;
        GameManager.onSleepChange += PetSleepChange;
        GameManager.onEvolutionEvent += StartEvoAnimation;

        background.onMouseDown += CancelSelection;
    }

    void CancelSelection()
    {
        if (EventSystem.current.IsPointerOverGameObject() == false)
            GameManager.instance.SetSelection(-1);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-horizontalRange, -1, 0), new Vector3(horizontalRange, -1, 0));
    }

    void UpdateSpriteSheets(List<ActivePet> pets)
    {
        // Find out how much space to dedicate each pet
        float horizontalSegmentPerPet = (horizontalRange * 2) / pets.Count;

        while(visualPets.Count < pets.Count)
        {
            CreatePrefab();
        }
        
        for (int i = 0; i < visualPets.Count; i++)
        {   
            if(i < pets.Count)
            {
                visualPets[i].SetIndex(i);
                ActivePet pet = GameManager.instance.activePets[i];
                Spritesheet sheet = GameManager.instance.FindSheet(pet.species);
                
                if(sheet != null)
                    visualPets[i].SetSpriteSheet(sheet);
                else
                    visualPets[i].SetSpriteSheet(null);

                if(pet.isDead)
                    visualPets[i].state = AIBrain.AIState.DEAD;

                visualPets[i].SetSleep(pet.IsSleeping());
                Debug.Log(pet.IsSleeping());

                visualPets[i].transform.localPosition = new Vector3(-horizontalRange + (i * horizontalSegmentPerPet) + horizontalSegmentPerPet / 2, 0, 0);
            }
            else
            {
                visualPets[i].SetSpriteSheet(null);
                visualPets[i].SetIndex(-1);
            }
        }
    }

    void CreatePrefab()
    {
        GameObject brain = Instantiate(brainPrefab, visualParent);
        GameObject visual = Instantiate(visualPrefab, brain.transform);
        
        // Brain Component
        AIBrain aiBrain = brain.GetComponent<AIBrain>();

        if(aiBrain == null)
            aiBrain = brain.AddComponent<AIBrain>();

        visualPets.Add(aiBrain);
    }

    void UpdateSelection(int index)
    {
        if(index < 0)
        {
            selection.SetActive(false);
            selection.transform.SetParent(transform);
        }
        else
        {
            selection.SetActive(true);
            selection.transform.SetParent(visualPets[index].transform);
        }

        selection.transform.localPosition = Vector3.zero;
        selection.transform.localPosition += selectionOffset;
    }

    void PetMisbehave(int index)
    {
        if(index < 0)
            return;
        
        visualPets[index].Animate("No", false);
    }

    void PetSleepChange(int index, bool sleeping)
    {
        if(index < 0)
            return;

        visualPets[index].SetSleep(sleeping);
    }

    void OnDestroy()
    {
        GameManager.onActivePetsChange -= UpdateSpriteSheets;
        GameManager.onSelectedPetUpdate -= UpdateSelection;
        GameManager.onMisbehavePet -= PetMisbehave;
        GameManager.onSleepChange -= PetSleepChange;
        GameManager.onEvolutionEvent -= StartEvoAnimation;
    }

    public void StartEvoAnimation(Sprite oldSprite, Sprite newSprite)
    {
        //Debug.Log("StartEvoAnimation");
        oldEvo.sprite = oldSprite;
        newEvo.sprite = newSprite;
        evolutionParent.SetActive(true);
    }

    public void EndEvoAnimation()
    {
        evolutionParent.SetActive(false);
    }
}
