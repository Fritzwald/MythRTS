using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SelectedEntityManager : MonoBehaviour
{
    public enum selectionTypes { None, Unit, Building, OtherPlayer }; 
    public selectionTypes selectionType = selectionTypes.None;
    private List<Entity> selectedEntities = new List<Entity>();
    private List<UnitGroup> selectedUnitGroups = new List<UnitGroup>();
    public static SelectedEntityManager Instance = null;

    private void Awake() {
        if(Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);
    }
    public void ClearSelection(){
        foreach(UnitGroup group in selectedUnitGroups){
            group.ClearSelection();
        }
        foreach(Entity ent in selectedEntities){
            ent.Deselect();
        }
        selectedEntities.Clear();
        selectedUnitGroups.Clear();
        selectionType = selectionTypes.None;
        UIManager.Instance.RedrawMenu();
    }

    public void AddSelected(Entity ent){
        if(!selectedEntities.Contains(ent)) {
            selectedEntities.Add(ent);
            SelectionChange();
        }
    }
    
    public void AddSelected(UnitGroup group){
        if(!selectedUnitGroups.Contains(group)){
            group.selected = true;
            selectedUnitGroups.Add(group);
            SelectionChange();
        }
        //print("Selected Groups: " + selectedUnitGroups.ToArray().Length);
    }

    private void SelectionChange() {
        switch(selectionType){
            case selectionTypes.Unit:
                UIManager.Instance.RedrawMenu(selectedUnitGroups);
            break;
            case selectionTypes.Building:
                UIManager.Instance.RedrawMenu(GetSelectedBuildings());
            break;
            case selectionTypes.OtherPlayer:

            break;
        }
        
    }

    public selectionTypes GetSelectionType() {
        return selectionType;
    }

    public List<UnitGroup> GetSelectedUnitGroups() {
        return selectedUnitGroups;
    }

    public List<Building> GetSelectedBuildings() {
        return selectedEntities.Cast<Building>().ToList();
    }
}
