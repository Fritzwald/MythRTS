﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : PlayerEntity
{
    public UnitGroup unitGroup;

    public void AssignUnitState(PlayerEnumerator.Players playerID, Vector3 startPosition, int startHealth, int startMaxHealth, UnitGroup group)
    {
        maxHealth = startMaxHealth;
        currentHealth = startHealth;
        worldPosition = startPosition;
        player = playerID;
        unitGroup = group;
    }


    public override void OnDestroyed()
    {

    }

    public override void OnCreated()
    {

    }
}