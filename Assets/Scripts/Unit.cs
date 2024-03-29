﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;


public class Unit : PlayerEntity
{
    public UnitGroup unitGroup;

    public override void Awake() {
        base.Awake();
        
    }

    public override void Start(){
        base.Start();
        if(!unitGroup.selected)
            gameObject.GetComponent<Outline>().DisableOutline();
    }


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

    public override void OnSelect()
    {
        base.OnSelect();
        unitGroup.SelectGroup();
    }

}
