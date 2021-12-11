using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Building", menuName = "Building/CreateNewBuilding", order = 1)]
public class BuildingProperties : ScriptableObject
{
    public string unitName;
    public int maxHealth;
    public int attackDamage;
    public int range;
    public float movementSpeed;
    public float attackSpeed;
    public GameObject buildingPrefab;
    public GameObject buildingIcon;
    public List<Unit> units;
}
