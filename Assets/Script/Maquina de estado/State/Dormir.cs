using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dormir : Humano
{
    public Transform camaTarget; // Asignar en el Inspector
    private void Awake()
    {
        typestate = TypeState.Dormir;
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
        // Mover hacia la cama
        _StateMachine.GetComponent<SteeringAgent>().target = camaTarget;
        _StateMachine.GetComponent<SteeringAgent>().behavior = SteeringBehavior.Arrive;

        // Recargar energía
        if (_DataAgent.Energy.value < 1f)
        {
            _DataAgent.Energy.value += Time.deltaTime * 0.3f;
        }
        else
        {
            _StateMachine.ChangeState(TypeState.Jugar); // Volver a jugar
        }

        base.Execute();
    }
    public override void Exit()
    {

    }
}
