using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : Fighter
{
    protected override void Death()
    {
        base.Death();
        Destroy(gameObject);

    }
}

// A crate is a unit with HP and can be attacked and killed by the player.

