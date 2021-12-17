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
    public int defaultUnitFile;
    public GameObject unitGroupPrefab;
    public GameObject unitPrefab;
    public Sprite unitIcon;
    // public List<Stance> unitStances;
}
