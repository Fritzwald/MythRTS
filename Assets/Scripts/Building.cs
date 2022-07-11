using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using System;

public class Building : PlayerEntity
{
    public List<UnitProperties> trainableUnits = new List<UnitProperties>();
    public Vector3 unitSpawnLocation = Vector3.zero;

    public BuildingProperties buildingProperties;

    // Start is called before the first frame update
    public override void Start()
    {
        
    }

    public virtual void AssignBuildingState(PlayerEnumerator.Players playerID, BuildingProperties properties, Vector3 startPosition, int startHealth, int startMaxHealth)
    {
        player = playerID;
        buildingProperties = properties;
        worldPosition = startPosition;
        currentHealth = startHealth;
        maxHealth = startMaxHealth;
    }

    public override void OnDestroyed()
    {

    }

    public override void OnCreated()
    {

    }

    public void TrainUnit(UnitProperties unitprops){
        print(unitprops.unitName);
        GameObject newGroup = Instantiate(unitprops.unitGroupPrefab);
        UnitGroup newGroupScript = newGroup.GetComponent<UnitGroup>();
        newGroupScript.unitProperties = unitprops;
        Vector3[] startPosArray = new Vector3[4];
        Array.Fill(startPosArray, new Vector3(transform.position.x, 1, transform.position.z - 3));
        newGroupScript.CreateUnits(startPosArray, unitprops.unitCreationDelay);
    }

    public override void OnSelect()
    {
        base.OnSelect();
        EnableHighlight();
    }

    public override void Deselect()
    {
        base.Deselect();
        DisableHighlight();
    }



}
