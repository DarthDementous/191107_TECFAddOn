using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Base abstract class of all transitions, objects that tell a state when to transition.
 * */
public abstract class ITransition : ScriptableObject {

	public IState transitionState;

    // Optional logic to be run upon entering the state to be transitioned from
    public virtual void Initialise(StateManager a_controller) { }

	// Run logic to determine whether to transition
	public abstract bool Decide(StateManager a_controller);

    // Optional logic to be run upon entering the transition state
    public virtual void Shutdown(StateManager a_controller) { }
}
