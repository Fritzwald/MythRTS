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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
		{

			SelectionBox();

			if(Input.GetKeyDown(KeyCode.Mouse0))
			{
				boxStartPos = Input.mousePosition;
			}
			else
			{
				boxEndPos = Input.mousePosition;
			}

		}
		else if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			SelectedEntityManager.Instance.ClearSelection();
            Entity mouseReleaseEntity = MouseReleaseSelect();
			if(mouseReleaseEntity != null){
				mouseReleaseEntity.OnSelect();
			}

			if(boxEndPos != Vector2.zero && boxStartPos != Vector2.zero)
			{
				
                foreach (Entity ent in SelectEntities()){
					ent.OnSelect();
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

        foreach(Collider obj in overlapColliders){
			//print("Start: " + boxStartPos);
			//print("End: " + boxEndPos);
			Vector3 objScreenLoc = Camera.main.WorldToScreenPoint(obj.transform.position);
            //print("ObjPos: " + objScreenLoc);
			if (boxStartPos.x < objScreenLoc.x && objScreenLoc.x < boxEndPos.x && boxStartPos.y < objScreenLoc.y && objScreenLoc.y < boxEndPos.y)
			{
				selectedEntites.Add(obj.gameObject.GetComponent<Entity>());
			}
			else if (boxStartPos.x < objScreenLoc.x && objScreenLoc.x < boxEndPos.x && boxStartPos.y > objScreenLoc.y && objScreenLoc.y > boxEndPos.y)
			{
				selectedEntites.Add(obj.gameObject.GetComponent<Entity>());
			}
			else if (boxStartPos.x > objScreenLoc.x && objScreenLoc.x > boxEndPos.x && boxStartPos.y > objScreenLoc.y && objScreenLoc.y > boxEndPos.y)
			{
				selectedEntites.Add(obj.gameObject.GetComponent<Entity>());
			}
			else if (boxStartPos.x > objScreenLoc.x && objScreenLoc.x > boxEndPos.x && boxStartPos.y < objScreenLoc.y && objScreenLoc.y < boxEndPos.y)
			{
				selectedEntites.Add(obj.gameObject.GetComponent<Entity>());
			}
            
		}

		foreach(Entity ent in selectedEntites){
			if(ent is PlayerEntity){
				if(ent.player == HumanPlayer.Instance.playerID){
					if(ent is Unit){

					}
					else if(ent is Building){

					}
				}
				else{

				}
			}
		}

		return selectedEntites;
		//print(selectedEntites.ToArray().Length);
	        /*foreach(WorldEntity ent in selectedEntites){
            print(ent.currentHealth);
        }*/
        
    }
}
