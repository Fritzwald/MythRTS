using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionControl : MonoBehaviour
{
    public Image selectImage;
    private Vector2 boxStartPos = Vector2.zero;
	private Vector2 boxEndPos = Vector2.zero;

    public float boxLength = 100;

    public LayerMask groundLayer;

    public LayerMask selectableLayer;

	private bool overUI = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
		{

			if(Input.GetKeyDown(KeyCode.Mouse0))
			{
				overUI = MouseInputUIBlocker.Instance.BlockedByUI;
				if(!overUI)
					boxStartPos = Input.mousePosition;
			}
			else
			{
				if(!overUI)
					boxEndPos = Input.mousePosition;
			}
			if(!overUI) {
				
				SelectionBox();
			}
		}
		else if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			if (!overUI) {
				SelectedEntityManager.Instance.ClearSelection();
				
				if(boxEndPos != Vector2.zero && boxStartPos != Vector2.zero)
				{
					List<Entity> selectedEntites = SelectEntities();
					for (int i = 0; i < selectedEntites.Count; i++) {
						selectedEntites[i].OnSelect();
					}
					
					// Deselect first
					//Deselect();
					// New Selection Box
					//Selection();
					// Reset UI
					ResetSelectUI();
				}
				boxEndPos = boxStartPos = Vector2.zero;
			}
		}
    }

    private void SelectionBox() {
		if(boxEndPos != Vector2.zero && boxStartPos != Vector2.zero)
		{
			
			selectImage.GetComponent<RectTransform>().anchoredPosition = new Vector2((boxStartPos.x+((boxEndPos.x-boxStartPos.x)/2)), (boxStartPos.y+((boxEndPos.y-boxStartPos.y)/2)));

			if(boxEndPos.x > boxStartPos.x && boxEndPos.y > boxStartPos.y)
			{
				selectImage.GetComponent<RectTransform>().offsetMin = new Vector2(boxStartPos.x, boxStartPos.y);
				selectImage.GetComponent<RectTransform>().offsetMax = new Vector2(boxEndPos.x, boxEndPos.y);
			}
			else if (boxEndPos.x > boxStartPos.x && boxEndPos.y < boxStartPos.y)
			{
				selectImage.GetComponent<RectTransform>().offsetMin = new Vector2(boxStartPos.x, boxEndPos.y);
				selectImage.GetComponent<RectTransform>().offsetMax = new Vector2(boxEndPos.x, boxStartPos.y);
			}
			else if (boxEndPos.x < boxStartPos.x && boxEndPos.y < boxStartPos.y)
			{
				selectImage.GetComponent<RectTransform>().offsetMin = new Vector2(boxEndPos.x, boxEndPos.y);
				selectImage.GetComponent<RectTransform>().offsetMax = new Vector2(boxStartPos.x, boxStartPos.y);
			}
			else if (boxEndPos.x < boxStartPos.x && boxEndPos.y > boxStartPos.y)
			{
				selectImage.GetComponent<RectTransform>().offsetMin = new Vector2(boxEndPos.x, boxStartPos.y);
				selectImage.GetComponent<RectTransform>().offsetMax = new Vector2(boxStartPos.x, boxEndPos.y);
			}
		}
	}

    private void ResetSelectUI() {
		selectImage.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		selectImage.GetComponent<RectTransform>().offsetMin = Vector2.zero;
		selectImage.GetComponent<RectTransform>().offsetMax = Vector2.zero;
	}

	private Entity MouseReleaseSelect(){
		RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit, 100, selectableLayer)){
			if(hit.collider.GetComponent<Entity>())
            	return hit.collider.GetComponent<Entity>();
			else
				return null;
        }
		else
			return null;
	}

    private List<Entity> SelectEntities() {
        Vector3 startWorldPos = Vector3.zero;
        Vector3 endWorldPos = Vector3.zero;

        RaycastHit startHit;
        RaycastHit endHit;

        Ray startRay = Camera.main.ScreenPointToRay(boxStartPos);
        Ray endRay = Camera.main.ScreenPointToRay(boxEndPos);

        if(Physics.Raycast(startRay, out startHit, 100, groundLayer)){
            startWorldPos = startHit.point;
        }
        if(Physics.Raycast(endRay, out endHit, 100, groundLayer)){
            endWorldPos = endHit.point;
        }
        //print(boxStartPos);
        //print(boxEndPos);

        Vector3 midWorldPos = Vector3.Lerp(startWorldPos, endWorldPos, 0.5f);

		Collider[] overlapColliders = Physics.OverlapSphere(midWorldPos, Vector3.Magnitude(endWorldPos-midWorldPos), selectableLayer);
		//print(overlapColliders.Length);
        List<Entity> selectedEntites = new List<Entity>();

		Entity mouseReleaseEntity = MouseReleaseSelect();
		if(mouseReleaseEntity != null)
			selectedEntites.Add(mouseReleaseEntity);

		for (int i = 0; i < overlapColliders.Length; i++) {
			//print("Start: " + boxStartPos);
			//print("End: " + boxEndPos);
			Vector3 objScreenLoc = Camera.main.WorldToScreenPoint(overlapColliders[i].transform.position);
            //print("ObjPos: " + objScreenLoc);
			if (boxStartPos.x < objScreenLoc.x && objScreenLoc.x < boxEndPos.x && boxStartPos.y < objScreenLoc.y && objScreenLoc.y < boxEndPos.y)
			{
				selectedEntites.Add(overlapColliders[i].gameObject.GetComponent<Entity>());
			}
			else if (boxStartPos.x < objScreenLoc.x && objScreenLoc.x < boxEndPos.x && boxStartPos.y > objScreenLoc.y && objScreenLoc.y > boxEndPos.y)
			{
				selectedEntites.Add(overlapColliders[i].gameObject.GetComponent<Entity>());
			}
			else if (boxStartPos.x > objScreenLoc.x && objScreenLoc.x > boxEndPos.x && boxStartPos.y > objScreenLoc.y && objScreenLoc.y > boxEndPos.y)
			{
				selectedEntites.Add(overlapColliders[i].gameObject.GetComponent<Entity>());
			}
			else if (boxStartPos.x > objScreenLoc.x && objScreenLoc.x > boxEndPos.x && boxStartPos.y < objScreenLoc.y && objScreenLoc.y < boxEndPos.y)
			{
				selectedEntites.Add(overlapColliders[i].gameObject.GetComponent<Entity>());
			}
            
		}

		if(selectedEntites.ToArray().Length > 0){
			bool containsPlayerUnit = false;
			bool containsPlayerBuilding = false;
			bool containsOtherPlayerEntity = false;

			for (int i = 0; i < selectedEntites.Count; i++) {
				if(selectedEntites[i] is PlayerEntity){
					if(selectedEntites[i].player == HumanPlayer.Instance.playerID){
						if(selectedEntites[i] is Unit){
							containsPlayerUnit = true;
						}
						else if(selectedEntites[i] is Building){
							containsPlayerBuilding = true;
						}
					}
					else{
						containsOtherPlayerEntity = true;
					}
				}
			}
			if(containsPlayerUnit){
				SelectedEntityManager.Instance.selectionType = SelectedEntityManager.selectionTypes.Unit;
				for(int i = selectedEntites.ToArray().Length - 1; i >= 0; i--){
					if(!(selectedEntites[i] is Unit)){
						// selectedEntites.Remove(selectedEntites[i]);
						selectedEntites.RemoveAt(i);
					}
				}
			}
			else if(containsPlayerBuilding){
				SelectedEntityManager.Instance.selectionType = SelectedEntityManager.selectionTypes.Building;
				for(int i = selectedEntites.ToArray().Length - 1; i >= 0; i--) {
					if (!(selectedEntites[i] is Building)) {
						selectedEntites.RemoveAt(i);
					}
				}
			}
			else if(containsOtherPlayerEntity){
				SelectedEntityManager.Instance.selectionType = SelectedEntityManager.selectionTypes.OtherPlayer;
				for(int i = selectedEntites.ToArray().Length - 1; i >= 0; i--) {
					if (selectedEntites[i].player != HumanPlayer.Instance.playerID) {
						selectedEntites.RemoveAt(i);
					}
				}
			}
		}
		//print(selectedEntites.ToArray().Length);
		return selectedEntites;
		//print(selectedEntites.ToArray().Length);
	        /*foreach(WorldEntity ent in selectedEntites){
            print(ent.currentHealth);
        }*/
    }
}
