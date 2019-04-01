using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class Trigger : MonoBehaviour
{


	[SerializeField] protected bool doesReset_ = false;//when true the trigger will reset if the condition isnt meant after being triggered.
	[SerializeField] protected bool triggered_ = false;//true if the trigger has been triggered
	public bool HasTriggered{get{return triggered_;}}
    public bool DoesReset { get { return doesReset_; } }

	[Header("Unity Events")]
	[SerializeField] protected UnityEvent onTriggerUnityEvents; //functions to call when on trigger.
	[SerializeField] protected UnityEvent onResetUnityEvents; //functions to call when on reset.

	/*
	 *  Events
	 */
	public delegate void Triggered();
	public event Triggered OnTriggered;

	public delegate void Reset();
	public event Reset OnReset;

	//when called it will call the OnTriggered event
	protected void CallOnTriggered()
    {
		if (!triggered_)
        {
			onTriggerUnityEvents.Invoke ();
			if (OnTriggered != null)
				OnTriggered ();
		}

		triggered_ = true;
	}

	//when called it will call the OnReset event
	protected void CallOnReset()
    {
		if (triggered_)
        {
			onResetUnityEvents.Invoke ();
			if (OnReset != null)
            {
				OnReset ();
			}
		}

		triggered_ = false;
	}

	//will only work in unity editor. use to override this trigger to always be on.
	public bool TriggerOverride()
    {

		#if (UNITY_EDITOR)
		CallOnTriggered();
		doesReset_=false;
		return true;
		#endif

		return false;
	}
		

}
	

