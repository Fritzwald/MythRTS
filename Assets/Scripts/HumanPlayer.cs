using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{

    public static HumanPlayer Instance = null;

    private void Awake() {
        if(Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        playerID = PlayerEnumerator.Players.Player1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
