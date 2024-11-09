using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : AIMove
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
