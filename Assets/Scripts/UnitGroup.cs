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

    public virtual void Update() {
        
    }

    public void AssignUnitProperties(UnitProperties unitProps, Vector3 createPosition)
    {
        transform.position = createPosition;
        unitProperties = unitProps;
        
    }

    public List<Vector3> CalcIntendedPositions(Vector3 destPos)
    {
        List<Vector3> positions = new List<Vector3>();
        Vector3 direction = (destPos - transform.position).normalized;
        direction.y = 0;
        for(int i = 0;i < groupUnits.ToArray().Length;i++){
            int rowNumber = (int)(i/unitProperties.defaultUnitFile);
            Vector3 pos = destPos + new Vector3(-1/direction.x,0,1/direction.z)*(unitSpacing*(i % unitProperties.defaultUnitFile));
            pos = pos - direction * unitSpacing * rowNumber;
            pos = pos - new Vector3(-1/direction.x,0,1/direction.z)* (Math.Clamp(groupUnits.ToArray().Length -1,0,unitProperties.defaultUnitFile)*unitSpacing/2);
            positions.Add(pos);
        }
        return positions;
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
        /*foreach(Unit unit in groupUnits){
            unit.gameObject.GetComponent<NavMeshAgent>().SetDestination(destinationPos);
        }*/
        List<Vector3> destPositions = CalcIntendedPositions(destinationPos);
        for(int i = 0;i < groupUnits.ToArray().Length;i++){
            print(destPositions[i]);
            groupUnits[i].gameObject.GetComponent<NavMeshAgent>().SetDestination(destPositions[i]);
        }
    }

    public void UpdateCenterPosition(){
        Vector3 sumOfPositions = Vector3.zero;
        for(int i = 0;i < groupUnits.ToArray().Length;i++){
            sumOfPositions += groupUnits[i].transform.position;
        }
        transform.position = sumOfPositions / groupUnits.ToArray().Length;
    }

    
}
