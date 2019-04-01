using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMinimapIcon : MonoBehaviour {

    [SerializeField]
    private Character character_ = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (character_)
        {

            float angle = Vector2.Angle(Vector2.up, character_.Direction);
            //Debug.Log(Vector2.Angle(Vector2.up, character_.Direction));
            if(character_.Direction.x > 0)
                angle = 360-angle;


            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
	}
}
