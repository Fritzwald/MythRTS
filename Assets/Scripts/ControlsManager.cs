using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager : MonoBehaviour
{
    public static ControlsManager Instance = null;
    public LayerMask rightClickLayer;
    public int dragDirectionThreshold = 3;

    private Vector3 worldStartPos = Vector3.zero;
    private Vector3 worldEndPos = Vector3.zero;

    private void Awake() {
        if(Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Mouse1)){
          worldStartPos = RightMouseRaycast();
        }

        // Raycast on update to show group UI
        if(Input.GetKey(KeyCode.Mouse1)){
            worldEndPos = RightMouseRaycast();
            Vector3 distanceVector = worldEndPos - worldStartPos;
            Vector3 direction = distanceVector.normalized;
            bool useDir = distanceVector.magnitude > dragDirectionThreshold;
            print(useDir);
        }

        if(Input.GetKeyUp(KeyCode.Mouse1)){
            worldEndPos = RightMouseRaycast();
            Vector3 distanceVector = worldEndPos - worldStartPos;
            Vector3 direction = distanceVector.normalized;
            bool useDir = distanceVector.magnitude > dragDirectionThreshold;
            print(useDir);
            foreach(UnitGroup unitGroup in SelectedEntityManager.Instance.GetSelectedUnitGroups())
                unitGroup.IssueMoveCommand(worldStartPos, useDir, direction);
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
}
