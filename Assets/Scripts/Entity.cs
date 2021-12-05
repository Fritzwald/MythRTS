using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    public Vector3 worldPosition;
    

    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    public abstract void OnCreated();

    public abstract void OnDestroyed();

    public virtual void OnMouseHover()
    {

    }

    public virtual void OnSelect()
    {

    }

}
