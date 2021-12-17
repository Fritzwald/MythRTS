using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject menuArea;
    public GameObject UIButton;

    private List<GameObject> currentMenuButtons = new List<GameObject>();

    public static UIManager Instance = null;

    private void Awake() {
        if(Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);
    }

    public void RedrawMenu(){
        ClearMenu();
    }
    public void RedrawMenu (Building building){
        ClearMenu();
        List<UnitProperties> units = building.trainableUnits;
        for (int i = 0; i < units.Count; i++) {
            UnitProperties unit = units[i];
            GameObject newButton = Instantiate(UIButton, menuArea.transform);
            newButton.GetComponent<Image>().sprite = unit.unitIcon;
            currentMenuButtons.Add(newButton);
            newButton.GetComponent<Button>().onClick.AddListener(() => building.TrainUnit(unit));
        }
    }
    public void RedrawMenu (List<UnitGroup> units){

    }

    private void ClearMenu () {
        for (int i = 0; i < currentMenuButtons.Count; i++) {
            currentMenuButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
            Destroy(currentMenuButtons[i]);
        }
        currentMenuButtons.Clear();
    }
}