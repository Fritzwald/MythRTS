using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedEntityManager : MonoBehaviour
{
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
        selectedEntities.Clear();
        selectedUnitGroups.Clear();

    }

    public void AddSelectedEntity(Entity ent){
        if(!selectedEntities.Contains(ent))
            selectedEntities.Add(ent);
        print("Selected Entities: " + selectedEntities.ToArray().Length);
    }
    
    public void AddSelectedEntity(UnitGroup group){
        if(!selectedUnitGroups.Contains(group))
            selectedUnitGroups.Add(group);
        print("Selected Groups: " + selectedUnitGroups.ToArray().Length);
    }

    public List<UnitGroup> GetSelectedUnitGroups(){
        return selectedUnitGroups;
    }
}
