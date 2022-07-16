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

    public void RedrawMenu (List<Building> buildings){
        ClearMenu();
        UnitProperties[] units = buildings[0].buildingProperties.trainableUnits;
        for (int i = 0; i < units.Length; i++) {
            UnitProperties unit = units[i];
            GameObject unitButton = CreateMenuButton(unit.unitIcon);
            unitButton.GetComponent<Button>().onClick.AddListener(() => buildings[0].TrainUnit(unit));
        }
    }

    public void RedrawMenu (List<UnitGroup> units){
        ClearMenu();
        if(units.Count > 0){
            if(units[0].unitProperties.buildings.Length > 0){
                BuildingProperties[] buildings = units[0].unitProperties.buildings;
                for (int i = 0; i < buildings.Length; i++) {
                    BuildingProperties building = buildings[i];
                    GameObject buildingButton = CreateMenuButton(building.buildingIcon);
                    //buildingButton.GetComponent<Button>().onClick.AddListener(() => unit.Build(building));
                }
            }
        }
    }

    private GameObject CreateMenuButton (Sprite icon) {
        GameObject newButton = Instantiate(UIButton, menuArea.transform);
        newButton.GetComponent<Image>().sprite = icon;
        currentMenuButtons.Add(newButton);
        return newButton;
    }

    private void ClearMenu () {
        for (int i = 0; i < currentMenuButtons.Count; i++) {
            currentMenuButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
            Destroy(currentMenuButtons[i]);
        }
        currentMenuButtons.Clear();
    }
}