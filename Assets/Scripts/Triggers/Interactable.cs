using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour {


    [Header("Unity Events")]
    [SerializeField]
    protected UnityEvent onInteractEvents; //functions to call when being interact.

    private GameObject interacter_;

    public virtual void Interact(GameObject interacter)
    {
        onInteractEvents.Invoke();
    }

    public GameObject Interacter
    {
        get { return interacter_; }
    }

}
