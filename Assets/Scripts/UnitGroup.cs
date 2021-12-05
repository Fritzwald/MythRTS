using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class UnitGroup : MonoBehaviour
{
    public int currentUnitCount;
    public int maxUnitCount;
    public int maxTotalHealth;
    public int currentTotalHealth;
    public float unitSpacing;
    public List<Unit> groupUnits = new List<Unit>();
    public List<Vector3> intendedGroupPositions = new List<Vector3>();
    public UnitProperties unitProperties;
    public GameObject groupContainer;

    public void Start()
    {
        groupContainer = new GameObject();
        groupContainer.transform.position = Vector3.zero;
        this.transform.parent = groupContainer.transform;
    }

    public void AssignUnitProperties(UnitProperties unitProps, Vector3 createPosition)
    {
        transform.position = createPosition;
        unitProperties = unitProps;
        
    }

    public void CalcIntendedPositions()
    {

    }

    public Unit CreateUnit<type>(Vector3 position) where type : Unit
    {
        GameObject newUnit = Instantiate(unitProperties.unitPrefab, position, Quaternion.identity, groupContainer.transform);
        Unit unitScript = newUnit.AddComponent<type>();
        return unitScript;
    }
}
