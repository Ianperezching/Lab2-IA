using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banno : Humano
{
    public Transform bannoTarget; // Asignar en el Inspector
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
        // Mover hacia el baño
        _StateMachine.GetComponent<SteeringAgent>().target = bannoTarget;
        _StateMachine.GetComponent<SteeringAgent>().behavior = SteeringBehavior.Seek;

        // Reducir necesidad de WC y aumentar higiene
        _DataAgent.WC.value -= Time.deltaTime * 0.5f;
        _DataAgent.Hygiene.value += Time.deltaTime * 0.4f;

        if (_DataAgent.WC.value < 0.1f && _DataAgent.Hygiene.value > 0.8f)
        {
            _StateMachine.ChangeState(TypeState.Jugar); // Retornar a jugar
        }

        base.Execute();
    }
    public override void Exit()
    {

    }
}
