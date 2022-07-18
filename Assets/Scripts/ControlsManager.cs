using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ControlsManager : MonoBehaviour
{
    public static ControlsManager Instance = null;
    public LayerMask rightClickLayer;
    public int dragDirectionThreshold = 3;
    public float unitGroupSpacing = 1f;
    public int unitGroupsDefaultCols = 3;

    private Vector3 worldStartPos = Vector3.zero;
    private Vector3 worldEndPos = Vector3.zero;
    private SelectedEntityManager.selectionTypes selectionType = SelectedEntityManager.selectionTypes.None;

    private void Awake() {
        if(Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Mouse1)) {
          selectionType = SelectedEntityManager.Instance.GetSelectionType();
          worldStartPos = RightMouseRaycast();
          // if it is a building selection set rellay point fo each building
          if (selectionType == SelectedEntityManager.selectionTypes.Building) { 
            List<Building> buildings = SelectedEntityManager.Instance.GetSelectedBuildings();
            for (int i = 0; i < buildings.Count; i++) {
              buildings[0].rallyPoint = worldStartPos;
            }
          }
        }

        if (selectionType == SelectedEntityManager.selectionTypes.Unit) {
          // Raycast on update to show group UI
          if(Input.GetKey(KeyCode.Mouse1)) {
              worldEndPos = RightMouseRaycast();
              Vector3 dragVector = worldEndPos - worldStartPos;
              Vector3 dragDirection = dragVector.normalized;
              bool useDragDir = dragVector.magnitude > dragDirectionThreshold;
          }

          if(Input.GetKeyUp(KeyCode.Mouse1)) {
              // Raycast endpoint to get direction of drag and if over threshold use for unit group positions
              worldEndPos = RightMouseRaycast();
              Vector3 dragVector = worldEndPos - worldStartPos;
              Vector3 dragDirection = dragVector.normalized;
              bool useDragDir = dragVector.magnitude > dragDirectionThreshold;

              // Array and lists to use during calculation
              List<UnitGroup> unitGroups = SelectedEntityManager.Instance.GetSelectedUnitGroups();
              List<Vector3> groupPositions = unitGroups.Select(unitGroup => unitGroup.transform.position).ToList();

              // Set direction based on drag or center point of all 
              Vector3 direction = useDragDir ? dragDirection : (worldStartPos - (groupPositions.Count == 1 ? groupPositions[0] : CenterOfVectors(groupPositions))).normalized;
              // We may need to make this a global function to be used in multiple scripts?
              Vector3 perpendicularDirection = new Vector3((direction.x == 0 ? (direction.z > 0 ? 10000000 : -10000000) : -1/direction.x), 0, 1/direction.z).normalized;

              // Do we maybe average all group direction, rather than just first??
              Quaternion directionAngle = Quaternion.FromToRotation(direction, unitGroups[0].groupDirection);
              float directionEuler = directionAngle.eulerAngles.y;
              if (directionEuler > 105 && directionEuler < 255) unitGroups.Reverse();

              // Calc group offset from group width plus previous group offset after first unit group
              int unitGroupsCols = unitGroupsDefaultCols;
              int maxRows = (int)(unitGroups.Count / unitGroupsCols);
              float[] groupWidths = new float[unitGroups.Count];
              float[] groupColOffsets = new float[unitGroups.Count];
              float[] groupRowOffsets = new float[maxRows + 1];
              float maxGroupDepth = 0f;
              groupRowOffsets[0] = maxGroupDepth;
              for (int i = 0; i < unitGroups.Count; i++) {
                //Calc col offsets
                float rowOrder = (float)i % (float)unitGroupsCols;
                float groupColOffset = rowOrder != 0 ? unitGroups[i - 1].groupWidth + groupColOffsets[i - 1] + unitGroupSpacing : 0;
                groupColOffsets[i] = groupColOffset;

                // calc Row offsets
                maxGroupDepth = Mathf.Max(maxGroupDepth, unitGroups[i].groupDepth);
                int rowNumber = (int)((float)i/(float)unitGroupsCols);
                if (rowOrder == unitGroupsCols - 1 && rowNumber <= maxRows) {
                  groupRowOffsets[rowNumber + 1] = maxGroupDepth;
                  maxGroupDepth = 0f;
                }
              }

              // Center group offsets by minusing half last (largest) offset
              for (int i = 0; i < groupColOffsets.Length; i++) {
                int rowNumber = (int)((float)i/(float)unitGroupsCols);
                int rowCount = Mathf.Clamp(groupColOffsets.Length - (rowNumber * unitGroupsCols), 0, unitGroupsCols);
                int lastIndexOfRow = Mathf.Clamp((Mathf.Max(rowNumber, 1) * rowCount) - 1, 0, Mathf.Min(groupColOffsets.Length - 1, unitGroups.Count - 1));
                // get largest offset unless only one object in row then use width
                float centeringOffset = (lastIndexOfRow != 0 ? groupColOffsets[lastIndexOfRow] : unitGroups[i].groupWidth) / 2 ;
                groupColOffsets[i] -= centeringOffset;
              }
              if (unitGroups.Count != 1) {
                for (int i = 0; i < unitGroups.Count; i++) {
                  int rowNumber = (int)((float)i/(float)unitGroupsCols);
                  Vector3 rowOffset = direction * groupRowOffsets[rowNumber];
                  Vector3 colOffset = perpendicularDirection * groupColOffsets[i];
                  Vector3 groupPosition = worldStartPos + colOffset - rowOffset;
                  unitGroups[i].IssueMoveCommand(groupPosition, true, direction);
                }
              } else {
                unitGroups[0].IssueMoveCommand(worldStartPos, true, direction);
              }
          }
        }
    }

    private Vector3 RightMouseRaycast() {
      RaycastHit hit;

      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

      if(Physics.Raycast(ray, out hit, 100, rightClickLayer)){
        return hit.point;
      }
      else {
        return Vector3.zero;
      }
    }

    public Vector3 CenterOfVectors( List<Vector3> vectors ) {
      Vector3 sum = Vector3.zero;
      if( vectors == null || vectors.Count == 0 )
      {
          return sum;
      }
  
      foreach( Vector3 vec in vectors )
      {
          sum += vec;
      }
      return sum/vectors.Count;
    }
}
