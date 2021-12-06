using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearmanGroup : UnitGroup
{
    public override void Start()
    {
        base.Start();
        CalcIntendedPositions();
        /*foreach(Vector3 pos in intendedGroupPositions)
        {
            Unit newUnit = CreateUnit<Spearman>(pos);
            groupUnits.Add(newUnit);
        }*/
        print("SpearmanGroup Created");
        
    }

    
}
