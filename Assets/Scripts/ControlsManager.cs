using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager : MonoBehaviour
{
    public static ControlsManager Instance = null;
    public LayerMask rightClickLayer;
    public int dragDirectionThreshold = 3;
    public float unitGroupSpacing = 1f;

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
              List<UnitGroup> unitGroups = SelectedEntityManager.Instance.GetSelectedUnitGroups();
              Vector3[] groupPositions = new Vector3[unitGroups.Count];
              float[] groupWidths = new float[unitGroups.Count];
              float[] groupOffsets = new float[unitGroups.Count];

              // Calc group offset from group width plus previous group offset after first unit group
              for (int i = 0; i < unitGroups.Count; i++) {
                groupPositions[i] = unitGroups[i].transform.position;
                float groupOffset = i != 0 ? unitGroups[i - 1].groupWidth + groupOffsets[i - 1] + unitGroupSpacing : 0;
                groupOffsets[i] = groupOffset;
              }

              // Center group offsets by minusing half last (largest) offset
              float centeringOffset = groupOffsets[groupOffsets.Length - 1] / 2;
              for (int i = 0; i < groupOffsets.Length; i++) {
                groupOffsets[i] -= centeringOffset;
              }
              // Set direction based on drag or center point of all 
              Vector3 direction = useDragDir ? dragDirection : (worldStartPos - CenterOfVectors(groupPositions)).normalized;

              // Do we maybe average all group direction, rather than just first??
              Quaternion directionAngle = Quaternion.FromToRotation(direction, unitGroups[0].groupDirection);
              float directionEuler = directionAngle.eulerAngles.y;
              if (directionEuler > 105 && directionEuler < 255) unitGroups.Reverse();

              // We may need to make this a global function to be used in multiple scripts?
              Vector3 perpendicularDirection = new Vector3((direction.x == 0 ? (direction.z > 0 ? 10000000 : -10000000) : -1/direction.x), 0, 1/direction.z).normalized;
              for (int i = 0; i < unitGroups.Count; i++) {
                Vector3 groupPosition = worldStartPos + (perpendicularDirection * groupOffsets[i]);
                unitGroups[i].IssueMoveCommand(groupPosition, true, direction);
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

    public Vector3 CenterOfVectors( Vector3[] vectors ) {
      Vector3 sum = Vector3.zero;
      if( vectors == null || vectors.Length == 0 )
      {
          return sum;
      }
  
      foreach( Vector3 vec in vectors )
      {
          sum += vec;
      }
      return sum/vectors.Length;
    }
}
