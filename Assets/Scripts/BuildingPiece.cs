using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPiece : Building
{

    public Building parentBuilding;
    

    public override void OnSelect()
    {
        parentBuilding.OnSelect();
    }
}
