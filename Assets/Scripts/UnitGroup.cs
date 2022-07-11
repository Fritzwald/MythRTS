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

    public Vector3 unitDirection = Vector3.zero;


    public virtual void Awake()
    {
        groupContainer = new GameObject("GroupContainer");
        groupContainer.transform.position = Vector3.zero;
        this.transform.parent = groupContainer.transform;
        
    }

    public virtual void Start() {
        AdoptUnitProperties(unitProperties);
    }

    private void AdoptUnitProperties(UnitProperties props){
        unitSpacing = props.unitRadius * 2;
        maxUnitCount = props.groupMaxUnits;
        // TODO Add Health
    }

    public virtual void Update() {
        
    }

    public void CreateUnits (Vector3[] startPositions, int delay = 0){
        StartCoroutine(CreateUnitsCoroutine(startPositions, delay));
    }

    private IEnumerator CreateUnitsCoroutine(Vector3[] startPositions, int delay){
        for (int i = 0; i < startPositions.Length; i++ ){
            CreateUnit(startPositions[i]);
            yield return new WaitForSeconds(delay);
        }
    }

    private void CreateUnit(Vector3 position)
    {
        GameObject newUnit = Instantiate(unitProperties.unitPrefab, position, Quaternion.identity, groupContainer.transform);
        Unit newUnitScript = newUnit.GetComponent<Unit>();
        newUnitScript.unitGroup = this;
        groupUnits.Add(newUnitScript);
        currentUnitCount ++;
    }

    public void AssignUnitProperties(UnitProperties unitProps, Vector3 createPosition)
    {
        transform.position = createPosition;
        unitProperties = unitProps;
        
    }

    public void SelectGroup(){
        foreach(Unit unit in groupUnits){
            unit.EnableHighlight();
        }
        SelectedEntityManager.Instance.AddSelected(this);
    }

    public void ClearSelection(){
        foreach(Unit unit in groupUnits){
            unit.DisableHighlight();
        }
    }

    public void IssueMoveCommand(Vector3 destinationPos){
        List<Vector3> destPositions = CalcIntendedPositions(destinationPos);
        Vector3 newDirection = (destinationPos - transform.position).normalized;
        newDirection.y = 0;
        //this funciton needs work, something is wrong. how to find the most efficient unit pathing among the units?
        //List<Vector3> destPosWithOrder = FindPositionOrder(destPositions);
        Quaternion directionAngle = Quaternion.FromToRotation(newDirection, unitDirection);
        float directionEuler = directionAngle.eulerAngles.y;
        //print(directionEuler);
        if (directionEuler < 45 || directionEuler > 315) {
            //go straight
        } else if (directionEuler < 105) {
            //go left
        } else if (directionEuler < 255) {
            groupUnits.Reverse();
            
            //go back
            /*
            if lastrow not full:

            else:
                backwards row order
            */
        } else {
            //go right
        }

        for(int i = 0;i < groupUnits.ToArray().Length;i++){
            //print(destPosWithOrder[i]);
            groupUnits[i].gameObject.GetComponent<NavMeshAgent>().SetDestination(destPositions[i]);
        }
        unitDirection = newDirection;
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
        print(direction);
        int maxRows = (int)(groupUnits.ToArray().Length/unitProperties.groupDefaultUnitWidth);
        for(int i = 0;i < groupUnits.ToArray().Length;i++){
            float rowNumber = (float)i/((float)unitProperties.groupDefaultUnitWidth);
            float unfinishedRowNumber = (float)(i+1)/((float)unitProperties.groupDefaultUnitWidth);
            int numberOfUnitsInLastRow = groupUnits.ToArray().Length - (int)(rowNumber)*unitProperties.groupDefaultUnitWidth;
            Vector3 posSideOffset = new Vector3(-1/direction.x,0,1/direction.z).normalized*(unitSpacing*(i % unitProperties.groupDefaultUnitWidth));
            Vector3 posRowOffset = direction * unitSpacing * (int)rowNumber;
            // Centers rows
            Vector3 posCenterOffset = new Vector3(-1/direction.x,0,1/direction.z).normalized*(Math.Clamp(groupUnits.ToArray().Length-1,0,unitProperties.groupDefaultUnitWidth-1)*unitSpacing/2);
            // Position units in last uneven row
            Vector3 posUnevenOffset = new Vector3(-1/direction.x,0,1/direction.z).normalized*(unitProperties.groupDefaultUnitWidth - numberOfUnitsInLastRow)*unitSpacing/2;
            print(posUnevenOffset);
            if((direction.z >= 0 && direction.x >= 0) || (direction.z < 0 && direction.x < 0)){
                // Offset to the sides
                Vector3 pos = destPos + posSideOffset;
                // Offset by row
                pos = pos - posRowOffset;
                // To center
                pos = pos - posCenterOffset;
                if(unfinishedRowNumber > maxRows && maxRows > 0)
                    pos = pos + posUnevenOffset;
                positions.Add(pos);
            }
            else{
                // Offset to the sides
                Vector3 pos = destPos - posSideOffset;
                // Offset by row
                pos = pos - posRowOffset;;
                // To center
                pos = pos + posCenterOffset;
                if(unfinishedRowNumber > maxRows && maxRows > 0)
                    pos = pos - posUnevenOffset;
                positions.Add(pos);
            }
            
        }
        return positions;
    }

    public void UpdateCenterPosition(){
        Vector3 sumOfPositions = Vector3.zero;
        for(int i = 0;i < groupUnits.Count;i++){
            sumOfPositions += groupUnits[i].transform.position;
        }

        if (groupUnits.Count == 0) 
            transform.position = sumOfPositions;
        else
            transform.position = sumOfPositions / groupUnits.Count;

    }

    
}

class PositionToDistance{
    public Vector3 position;
    public float distance;

}
