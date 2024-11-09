using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : AIMove
{
    private void Update()
    {
        Move();
    }
    protected void Move()
    {
        CanUseUpdate();
    }
}
