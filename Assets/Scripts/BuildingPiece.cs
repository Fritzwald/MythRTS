using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPiece : Entity
{

    public Building parentBuilding;

    public override void Start(){
        parentBuilding = transform.parent.GetComponent<Building>();
    }

    public override void OnCreated(){

    }

    public override void OnDestroyed(){
        
    }

    public override void OnSelect()
    {
        parentBuilding.OnSelect();
    }
}
