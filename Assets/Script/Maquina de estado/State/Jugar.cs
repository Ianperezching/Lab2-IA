using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jugar : Humano
{
    public Transform jugarTarget; 
    private void Awake()
    {
        typestate = TypeState.Jugar;
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
        // Configurar movimiento hacia el área de juego
        _StateMachine.GetComponent<SteeringAgent>().target = jugarTarget;
        _StateMachine.GetComponent<SteeringAgent>().behavior = SteeringBehavior.Seek;
        // Si la energía es baja, ir a dormir
        if (_DataAgent.Energy.value < 0.25f)
        {
            _StateMachine.ChangeState(TypeState.Dormir);
        }
        // Si el hambre es alta, ir a comer
        else if (_DataAgent.Hunger.value > 0.8f)
        {
            _StateMachine.ChangeState(TypeState.Comer);
        }
        // Si la higiene es baja, ir al baño
        else if (_DataAgent.Hygiene.value < 0.2f || _DataAgent.WC.value > 0.8f)
        {
            _StateMachine.ChangeState(TypeState.Banno);
        }
        else
        {
            _DataAgent.DiscountEnergy();
            _DataAgent.Hunger.value += Time.deltaTime * 0.05f; // Jugar aumenta el hambre
        }

        base.Execute();
    }
    public override void Exit()
    {

    }
}
