using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Unit", menuName = "Units/CreateNewUnit", order = 1)]
public class UnitProperties : ScriptableObject
{
    public string unitName;
    public int maxHealth;
    public int attackDamage;
    public int range;
    public float movementSpeed;
    public float attackSpeed;
    public float unitRadius;
    public int groupDefaultUnitWidth;
    public float unitCreationDelay;
    public int groupMaxUnits;
    public GameObject unitGroupPrefab;
    public GameObject unitPrefab;
    public Sprite unitIcon;
    public BuildingProperties[] buildings;
    public UnitAbilities[] abilities;
    public UnitFormations[] formations;
    // public List<Stance> unitStances;
}
