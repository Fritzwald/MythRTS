using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerEntity : Entity
{
    public PlayerEnumerator.Players player;

    

    public abstract override void OnDestroyed();

    public abstract override void OnCreated();
}
