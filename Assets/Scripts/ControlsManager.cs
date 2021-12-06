using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager : MonoBehaviour
{
    public static ControlsManager Instance = null;

    public LayerMask rightClickLayer;
    private void Awake() {
        if(Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Mouse1)){
            
            foreach(UnitGroup unitGroup in SelectedEntityManager.Instance.GetSelectedUnitGroups())
                unitGroup.IssueMoveCommand(MouseReleaseRaycast());
        }
    }

    private Vector3 MouseReleaseRaycast(){
		RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit, 100, rightClickLayer)){
            return hit.point;
        }
		else
			return Vector3.zero;
	}
}
