using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

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
