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
    public float groupWidth = 0f;
    public float groupDepth = 0f;
    public bool selected = false;
    public List<Unit> groupUnits = new List<Unit>();
    public List<Vector3> intendedUnitPositions = new List<Vector3>();
    public UnitProperties unitProperties;
    public GameObject groupContainer;
    public Vector3 groupDirection = Vector3.zero;


    public virtual void Awake()
    {
        groupContainer = new GameObject("GroupContainer");
        groupContainer.transform.position = Vector3.zero;
        this.transform.parent = groupContainer.transform;
        currentUnitCount = groupUnits.Count;
    }

    public virtual void Start() {
        
    }

    public void AdoptUnitProperties(UnitProperties props){
        unitSpacing = props.unitRadius * 2;
        maxUnitCount = props.groupMaxUnits;
        // TODO Add Health
    }

    public virtual void Update() {
        UpdateCenterPosition();
    }

    public void CreateUnits (Vector3[] startPositions, Vector3 pos, float delay = 0) {
        currentUnitCount += startPositions.Length;
        // Calc unit positions for rally points
        Vector3 direction = (pos - startPositions[0]).normalized;
        intendedUnitPositions = CalcIntendedPositions(pos, direction);
        StartCoroutine(CreateUnitsCoroutine(startPositions, delay));
    }

    private IEnumerator CreateUnitsCoroutine(Vector3[] startPositions, float delay) {
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
        // Why doesnt this work simon???? ===============
        if (selected) newUnitScript.EnableHighlight();
        // ==============================================
        groupUnits.Add(newUnitScript);
        IssueUnitMoveCommand(intendedUnitPositions[groupUnits.Count - 1], groupUnits.Count - 1); 
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

    public void IssueMoveCommand(Vector3 destinationPos, bool useDir = false, Vector3 dir = new Vector3()){
        Vector3 direction = useDir ? dir : (destinationPos - transform.position).normalized;
        direction.y = 0;
        intendedUnitPositions = CalcIntendedPositions(destinationPos, direction);
        // This funciton needs work, something is wrong. how to find the most efficient unit pathing among the units?
        // This might not be needed now
        //List<Vector3> destPosWithOrder = FindPositionOrder(intendedUnitPositions);
        Quaternion directionAngle = Quaternion.FromToRotation(direction, groupDirection);
        float directionEuler = directionAngle.eulerAngles.y;
        //print(directionEuler);
        if (directionEuler < 45 || directionEuler > 315) {
            //go straight
            
        } else if (directionEuler < 105) {
            //go left
        } else if (directionEuler < 255) {
            int endRowUnitCount = groupUnits.Count % unitProperties.groupDefaultUnitWidth;
            print(endRowUnitCount);
            int endRowMissingCount = unitProperties.groupDefaultUnitWidth - endRowUnitCount;
            //groupUnits.Reverse();
            List<Unit> tempGroupUnits = groupUnits;
            tempGroupUnits.Reverse();
            for (int i = 0; i < groupUnits.Count - endRowUnitCount; i++) {
              if (i % unitProperties.groupDefaultUnitWidth == 0) {
                print("================");
                print(i);
                for (int j = 0; j < endRowMissingCount; j++) {
                  int moveIndex = i + j;
                  int moveFrom = tempGroupUnits.Count - 1 - moveIndex;
                  print("moveFrom" + moveFrom);
                  int moveTo = moveFrom - endRowUnitCount;
                  print("moveTo" + moveTo);
                  ListIndexSwap(tempGroupUnits, moveFrom, moveTo);
                }
              }
            }
            groupUnits = tempGroupUnits;
            
            //go back
            /*
            if lastrow not full:

            else:
                backwards row order
            */
        } else {
            //go right
        }

        for(int i = 0;i < groupUnits.Count;i++){
            //groupUnits[i].gameObject.GetComponent<NavMeshAgent>().SetDestination(intendedUnitPositions[i]);
            IssueUnitMoveCommand(intendedUnitPositions[i], i);
        }
        groupDirection = direction;
    }

    private void IssueUnitMoveCommand(Vector3 pos, int unitIndex) {
        groupUnits[unitIndex].gameObject.GetComponent<NavMeshAgent>().SetDestination(pos);
    }

    // Not currently used in favour of reversing unit order in certain situations
    /* public List<Vector3> FindPositionOrder(List<Vector3> intendedPos){
        //List<Vector3> orderedPositions = new List<Vector3>(new Vector3[intendedPos.ToArray().Length]);
        //List<float> distances = new List<float>(new float[intendedPos.ToArray().Length]);

        List<PositionToDistance> posToDist = new List<PositionToDistance>(new PositionToDistance[intendedPos.Count]);
        for(int i = 0;i < currentUnitCount;i++){
            float smallestDist = float.MaxValue;
            Vector3 destPos = intendedPos[i];
            for(int j = 0;j < currentUnitCount;j++){
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

        List<Vector3> properOrder = new List<Vector3>(new Vector3[intendedPos.Count]);
        List<int> indexesUsed = new List<int>();
        for(int i = 0;i < currentUnitCount;i++){
            int shortestPathIndex = 0;
            float smallestDist = float.MaxValue;
            Vector3 destPos = posToDist[i].position;
            for(int j = 0;j < currentUnitCount;j++){
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
    } */

    public List<Vector3> CalcIntendedPositions(Vector3 destPos, Vector3 direction)
    {
        List<Vector3> positions = new List<Vector3>();
        direction.y = 0;
        int maxRows = (int)(currentUnitCount/unitProperties.groupDefaultUnitWidth);
        // Set group width and depth
        groupWidth = Math.Clamp(currentUnitCount, 0, unitProperties.groupDefaultUnitWidth) * unitSpacing;
        groupDepth = unitSpacing * maxRows;
        Vector3 perpendicularDirection = new Vector3((direction.x == 0 ? (direction.z > 0 ? 10000000 : -10000000) : -1/direction.x), 0, 1/direction.z).normalized;
        //possible cleaner alternative: must consider rotational direction, and unit position swapping
        //Vector3 perpendicularDirection = new Vector3(-direction.z , 0, direction.x).normalized;
        for(int i = 0;i < currentUnitCount;i++){
            float rowNumber = (float)i/((float)unitProperties.groupDefaultUnitWidth);
            float unfinishedRowNumber = (float)(i+1)/((float)unitProperties.groupDefaultUnitWidth);
            int numberOfUnitsInLastRow = currentUnitCount - (int)(rowNumber)*unitProperties.groupDefaultUnitWidth;
            Vector3 posSideOffset = perpendicularDirection*(unitSpacing*(i % unitProperties.groupDefaultUnitWidth));
            Vector3 posRowOffset = direction * unitSpacing * (int)rowNumber;
            // Centers rows
            Vector3 posCenterOffset = perpendicularDirection * ( Math.Clamp(currentUnitCount - 1, 0, unitProperties.groupDefaultUnitWidth - 1) * unitSpacing / 2);
            // Position units in last uneven row
            Vector3 posUnevenOffset = perpendicularDirection * (unitProperties.groupDefaultUnitWidth - numberOfUnitsInLastRow)*unitSpacing/2;

            if((direction.z >= 0 && direction.x >= 0) || (direction.z < 0 && direction.x < 0)){
                // Offset to the sides
                Vector3 pos = destPos + posSideOffset;
                // Offset by row
                pos = pos - posRowOffset;
                // To center
                pos = pos - posCenterOffset;
                if(unfinishedRowNumber > maxRows && maxRows > 0)
                    pos = pos + posUnevenOffset;
                //print("Position: " + pos);
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
                //print("Position: " + pos);
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

    public void ListIndexSwap<T>(IList<T> list, int indexA, int indexB)
    {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }

    
}

class PositionToDistance {
    public Vector3 position;
    public float distance;

}
