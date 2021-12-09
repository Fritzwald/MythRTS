using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerEntity : Entity
{
    

    

    public abstract override void OnDestroyed();

    public abstract override void OnCreated();
}
