using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Humano : State
{
    public DataAgent _DataAgent;

     
    public override void LocadComponent()
    {
        base.LocadComponent();
        _DataAgent = GetComponent<DataAgent>();
    }

}
