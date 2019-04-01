using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeBtn : MonoBehaviour {

    public GameObject menu;
    public GameObject panel;


	// Update is called once per frame
	void Update ()
    { 
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.SetActive(true);
            panel.SetActive(false);
        }

}
}
