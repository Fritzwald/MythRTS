using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public abstract class PlayerEntity : Entity
{
    
    public abstract override void OnDestroyed();

    public abstract override void OnCreated();

    public void EnableHighlight(){
        if(gameObject.GetComponent<Outline>() != null)
            gameObject.GetComponent<Outline>().EnableOutline();
    }

    public void DisableHighlight(){
        if(gameObject.GetComponent<Outline>() != null)
            gameObject.GetComponent<Outline>().DisableOutline();
    }
}
