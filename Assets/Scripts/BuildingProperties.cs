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
    public GameObject buildingPrefab;
    public Texture2D buildingIcon;
    public List<UnitProperties> trainableUnits;
}
