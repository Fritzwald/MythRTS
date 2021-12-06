using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

[System.Serializable]
public class UnitGroup : MonoBehaviour
{
    public int currentUnitCount;
    public int maxUnitCount;
    public int maxTotalHealth;
    public int currentTotalHealth;
    public float unitSpacing;
    public List<Unit> groupUnits = new List<Unit>();
    public List<Vector3> intendedGroupPositions = new List<Vector3>();
    public UnitProperties unitProperties;
    public GameObject groupContainer;

    public virtual void Start()
    {
        groupContainer = new GameObject("GroupContainer");
        groupContainer.transform.position = Vector3.zero;
        this.transform.parent = groupContainer.transform;
    }

    public void AssignUnitProperties(UnitProperties unitProps, Vector3 createPosition)
    {
        transform.position = createPosition;
        unitProperties = unitProps;
        
    }

    public void CalcIntendedPositions()
    {
        
    }

    public Unit CreateUnit<type>(Vector3 position) where type : Unit
    {
        GameObject newUnit = Instantiate(unitProperties.unitPrefab, position, Quaternion.identity, groupContainer.transform);
        Unit unitScript = newUnit.AddComponent<type>();
        return unitScript;
    }

    public void SelectGroup(){
        foreach(Unit unit in groupUnits){
            unit.EnableHighlight();
        }
        SelectedEntityManager.Instance.AddSelectedEntity(this);
    }

    public void ClearSelection(){
        foreach(Unit unit in groupUnits){
            unit.DisableHighlight();
        }
    }

    public void IssueMoveCommand(Vector3 destinationPos){
        print("Destination: " + destinationPos);
        foreach(Unit unit in groupUnits){
            unit.gameObject.GetComponent<NavMeshAgent>().SetDestination(destinationPos);
        }
    }

    public virtual void Update() {
        
    }
}
