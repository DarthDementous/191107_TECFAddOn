using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StateManager : MonoBehaviour {

	[System.Serializable]
	public struct StateNode {

		public string name;
		public IState state;

	}

	public StateNode[]	m_states;
	
	public IState		activeState;      // AI is in one state at a time

	public bool isDebug = true;			  // Whether to show color of active state

    private void Awake()
    {
        StateNode[] newStateList = new StateNode[m_states.Length];

        // Go through list and make instances of everything so it doesn't use the original object
        for (int i = 0; i < m_states.Length; ++i)
        {
            var currState = m_states[i];
            var copyState = new StateNode { name = currState.name, state = Instantiate(currState.state) };

            // Actions
            for (int a = 0; a < copyState.state.m_actions.Length; ++a)
            {
                var currAction = copyState.state.m_actions[a];
                currAction = Instantiate(currAction);

                copyState.state.m_actions[a] = currAction;
            }

            // Transitions
            for (int t = 0; t < copyState.state.m_transitions.Length; ++t)
            {
                var currTrans = copyState.state.m_transitions[t];
                currTrans = Instantiate(currTrans);

                copyState.state.m_transitions[t] = currTrans;
            }

            // Update activate state
            if (currState.state == activeState)
            {
                activeState = copyState.state;
            }

            /// Replace in state list
            m_states[i] = copyState;
        }
    }

    void Start() {
        // Initialise actions and transitions in current active state (if it has them)
        activeState.Initialise(this);

        if (activeState.m_transitions != null) {
            foreach (var transition in activeState.m_transitions) {

                transition.Initialise(this);

            }
        }

        if (activeState.m_actions != null) {
            foreach (var action in activeState.m_actions) {

                action.Initialise(this);

            }
        }


	}

    public string GetStateName(IState a_state)
    {
        // Look for the state
        foreach (var node in m_states)
        {
            if (node.state == a_state)
            {
                return node.name;
            }
        }

        return null;
    }

	public IState GetState(string a_stateName) {

		// Look for corresponding state
		foreach (var node in m_states) {

			if (node.name == a_stateName) {

				return node.state;

			}

		}

		Debug.Assert(false, "Attempted to get state that does not exist.");

		return null;

	}

    /**
     * @brief Shut down old state and initialise new state.
     * @param a_oldState is the state that WAS the active state.
     * @param a_newState is the state that will BECOME the active state.
     * @return void.
     * */
    void TransitionStates(IState a_oldState, IState a_newState) {
        // Run shutdown on actions and transitions in old state (if it has them)
        a_oldState.Shutdown(this);

        if (a_oldState.m_transitions != null) {
            foreach (var transition in a_oldState.m_transitions) {

                transition.Shutdown(this);

            }
        }

        if (a_oldState.m_actions != null) {
            foreach (var action in a_oldState.m_actions) {

                action.Shutdown(this);

            }
        }

        // Initialise actions and transitions in new state (if it has them)
        a_newState.Initialise(this);

        if (a_newState.m_transitions != null) {
            foreach (var transition in a_newState.m_transitions) {

                transition.Initialise(this);

            }
        }

        if (a_newState.m_actions != null) {
            foreach (var action in a_newState.m_actions) {

                action.Initialise(this);

            }
        }

        // Set new state
        activeState = a_newState;
    }

	public void SetState(IState a_state) {

		// Look for corresponding state
		foreach (var node in m_states) {

			// Node matches state, set as new active
			if (node.state == a_state) {
                TransitionStates(activeState, node.state);

                return;
			}

		}

		Debug.Assert(false, "Attempted to set state in controller that is not in the list of available states.");

	}

	public void SetState(string a_stateName) {

		// Look for corresponding state
		foreach (var node in m_states) {

			// Node matches state, set as new active
			if (node.name == a_stateName) {
                TransitionStates(activeState, node.state);

				return;
			}

		}

		Debug.Assert(false, "Attempted to set state with invalid name.");

	}
	
	void Update () {

		// Run logic for current set state
		Debug.Assert(activeState, "No active state assigned.");
		activeState.UpdateState(this);

		// DEBUG: Set color of entity to one representing the active state
		if (isDebug) {
			foreach (var renderer in GetComponentsInChildren<Renderer>()) {

                renderer.material.color = activeState.debugColor;

            }
		}

	}
}
