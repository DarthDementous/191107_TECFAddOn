using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Base abstract class of all actions, modular pieces of code that contain specific functionality that is run only when the state
 * they are on is active.
 * */
public abstract class IAction : ScriptableObject {      // ScriptableObject = does not need to be attached to an object as a component

	/**
	 * @brief Initialise values in action scriptable objects.
	 * NOTE: Scriptable objects tend to hold onto global variable values as of last running so this is very necessary
	 * @return void.
	 * */
	public virtual void Initialise(StateManager a_controller) { }  // virtual keyword = Designed to be over-written but not required since already defined as empty in base class

	/**
	 * @brief Reset values in action scriptable objects and perform any other clean-up routines before switching states
	 * @return void.
	 * */
	public virtual void Shutdown(StateManager a_controller) { }

    /**
	 * @brief Run processes based on performing the action. NOTE: Can be over-written by sub-classes
	 * @return void.
	 * */
    public virtual void Act(StateManager a_controller) { }
}
