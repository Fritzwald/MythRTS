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
        List<Vector3> destPositions = CalcIntendedPositions(destinationPos);

        //this funciton needs work, something is wrong. how to find the most efficient unit pathing among the units?
        //List<Vector3> destPosWithOrder = FindPositionOrder(destPositions);

        for(int i = 0;i < groupUnits.ToArray().Length;i++){
            //print(destPosWithOrder[i]);
            groupUnits[i].gameObject.GetComponent<NavMeshAgent>().SetDestination(destPositions[i]);
        }
    }

    public List<Vector3> FindPositionOrder(List<Vector3> intendedPos){
        //List<Vector3> orderedPositions = new List<Vector3>(new Vector3[intendedPos.ToArray().Length]);
        //List<float> distances = new List<float>(new float[intendedPos.ToArray().Length]);

        List<PositionToDistance> posToDist = new List<PositionToDistance>(new PositionToDistance[intendedPos.ToArray().Length]);
        for(int i = 0;i < groupUnits.ToArray().Length;i++){
            float smallestDist = float.MaxValue;
            Vector3 destPos = intendedPos[i];
            for(int j = 0;j < groupUnits.ToArray().Length;j++){
                Vector3 unitPos = groupUnits[j].transform.position;
                float distance = Vector3.Distance(unitPos,destPos);
                if(distance < smallestDist){
                        smallestDist = distance;
                }
            }
            posToDist[i] = new PositionToDistance(){position = intendedPos[i], distance = smallestDist};
        }

        posToDist.Sort((x,y) => x.distance.CompareTo(y.distance));
        posToDist.Reverse();

        List<Vector3> properOrder = new List<Vector3>(new Vector3[intendedPos.ToArray().Length]);
        List<int> indexesUsed = new List<int>();
        for(int i = 0;i < groupUnits.ToArray().Length;i++){
            int shortestPathIndex = 0;
            float smallestDist = float.MaxValue;
            Vector3 destPos = posToDist[i].position;
            for(int j = 0;j < groupUnits.ToArray().Length;j++){
                if(!indexesUsed.Contains(j)){
                    Vector3 unitPos = groupUnits[j].transform.position;
                    float distance = Vector3.Distance(unitPos,destPos);
                    if(distance < smallestDist){
                        smallestDist = distance;
                        shortestPathIndex = j;
                    }
                }
            }
            indexesUsed.Add(shortestPathIndex);
            properOrder[i] = posToDist[shortestPathIndex].position;
        }
        return properOrder;
    }

    public List<Vector3> CalcIntendedPositions(Vector3 destPos)
    {
        List<Vector3> positions = new List<Vector3>();
        Vector3 direction = (destPos - transform.position).normalized;
        direction.y = 0;
        int maxRows = (int)(groupUnits.ToArray().Length/unitProperties.defaultUnitFile);
        for(int i = 0;i < groupUnits.ToArray().Length;i++){
            float rowNumber = (float)i/((float)unitProperties.defaultUnitFile);
            float unfinishedRowNumber = (float)(i+1)/((float)unitProperties.defaultUnitFile);
            int numberOfUnitsInLastRow =groupUnits.ToArray().Length - (int)(rowNumber)*unitProperties.defaultUnitFile;
            Vector3 pos = destPos + new Vector3(-1/direction.x,0,1/direction.z).normalized*(unitSpacing*(i % unitProperties.defaultUnitFile));
            pos = pos - direction * unitSpacing * (int)rowNumber;
            pos = pos - new Vector3(-1/direction.x,0,1/direction.z).normalized* (Math.Clamp(groupUnits.ToArray().Length -1,0,unitProperties.defaultUnitFile)*unitSpacing/2);
            if(unfinishedRowNumber > maxRows)
                pos = pos + new Vector3(-1/direction.x,0,1/direction.z).normalized* (unitProperties.defaultUnitFile - numberOfUnitsInLastRow)*unitSpacing/2;
            positions.Add(pos);
        }
        return positions;
    }

    public void UpdateCenterPosition(){
        Vector3 sumOfPositions = Vector3.zero;
        for(int i = 0;i < groupUnits.ToArray().Length;i++){
            sumOfPositions += groupUnits[i].transform.position;
        }
        transform.position = sumOfPositions / groupUnits.ToArray().Length;
    }

    
}

class PositionToDistance{
    public Vector3 position;
    public float distance;

}
