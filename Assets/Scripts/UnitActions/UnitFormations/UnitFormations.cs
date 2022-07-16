using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Formation", menuName = "Units/CreateNewFormation", order = 1)]
public class UnitFormations : ScriptableObject
{
    public string formationName;
    public Sprite icon;
}
