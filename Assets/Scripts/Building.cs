using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using System;

public class Building : PlayerEntity
{
    public Vector3 unitSpawnLocation = Vector3.zero;
    public Vector3 rallyPoint;

    public BuildingProperties buildingProperties;

    // Start is called before the first frame update
    public override void Start()
    {
        rallyPoint = transform.position + buildingProperties.defaultRallyOffset;
        rallyPoint.y = 1;
        //print(new Vector3(transform.position.x, 1, transform.position.z - 7))
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

    public void TrainUnit(UnitProperties unitProps){
        print(unitProps.unitName);
        GameObject newGroup = Instantiate(unitProps.unitGroupPrefab);
        //newGroup.transform.position = new Vector3(transform.position.x, 1, transform.position.z - 3);
        UnitGroup newGroupScript = newGroup.GetComponent<UnitGroup>();
        newGroupScript.unitProperties = unitProps;
        newGroupScript.AdoptUnitProperties(unitProps);
        Vector3[] startPosArray = new Vector3[unitProps.groupMaxUnits];
        Array.Fill(startPosArray, new Vector3(transform.position.x, 1, transform.position.z - 3));
        newGroupScript.CreateUnits(startPosArray, rallyPoint, unitProps.unitCreationDelay);
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
