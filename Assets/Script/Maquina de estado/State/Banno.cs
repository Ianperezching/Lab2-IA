using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banno : Humano
{
    private void Awake()
    {
        typestate = TypeState.Banno;
        LocadComponent();
    }
    public override void LocadComponent()
    {
        base.LocadComponent();

    }
    public override void Enter()
    {

    }
    public override void Execute()
    {


        base.Execute();
    }
    public override void Exit()
    {

    }
}
