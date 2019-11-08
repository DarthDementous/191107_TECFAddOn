using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/**
 * @brief Base abstract class for all states, objects that hold onto complex functionality defined solely by actions and transitions.
 * */
public abstract class IState : ScriptableObject {

	public Color debugColor = Color.gray;			// Color to represent the state for debug purposes (gray by default) 

	public IAction[]		m_actions;
	public ITransition[]	m_transitions;

    public virtual void Initialise(StateManager a_controller) {

    }

    public virtual void Shutdown(StateManager a_controller) {

    }

	public void UpdateState(StateManager a_controller)
	{
		// Check transitions last to avoid acting after already transitioning
		ExecuteActions(a_controller);
		CheckTransitions(a_controller);
	}

	public virtual void ExecuteActions(StateManager a_controller)
	{

		if (m_actions != null) {

			// Perform all assigned actions 
			foreach (var action in m_actions) {
				action.Act(a_controller);
			}

		}

	}

	public virtual void CheckTransitions(StateManager a_controller) {

		if (m_transitions != null) {

			// Loop through and decide whether a transition should take place
			foreach (var tr in m_transitions) {

				// Transition conditions met, set new active state
				if (tr.Decide(a_controller) == true) a_controller.SetState(tr.transitionState);

			}

		}

	}
}
