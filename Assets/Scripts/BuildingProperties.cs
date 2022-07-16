using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Building", menuName = "Building/CreateNewBuilding", order = 1)]
public class BuildingProperties : ScriptableObject
{
    public string buildingName;
    public int maxHealth;
    public int attackDamage;
    public int range;
    public float attackSpeed;
    public Vector3 defaultRallyOffset;
    public GameObject buildingPrefab;
    public Sprite buildingIcon;
    public UnitProperties[] trainableUnits;
}
