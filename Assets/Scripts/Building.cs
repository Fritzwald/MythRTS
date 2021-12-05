using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : PlayerEntity
{
    // Start is called before the first frame update
    public override void Start()
    {
        
    }

    public virtual void AssignBuildingState(PlayerEnumerator.Players playerID, Vector3 startPosition, int startHealth, int startMaxHealth)
    {
        maxHealth = startMaxHealth;
        currentHealth = startHealth;
        worldPosition = startPosition;
        player = playerID;
    }

    public override void OnDestroyed()
    {

    }

    public override void OnCreated()
    {

    }
}
