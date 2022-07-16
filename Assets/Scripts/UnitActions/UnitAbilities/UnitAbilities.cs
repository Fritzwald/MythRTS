using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Ability", menuName = "Units/CreateNewAbililty", order = 1)]
public class UnitAbilities : ScriptableObject
{
    public string abilityName;
    public Sprite icon;
    // public List<Stance> unitStances;
}
