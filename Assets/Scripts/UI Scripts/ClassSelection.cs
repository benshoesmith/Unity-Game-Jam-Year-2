using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassSelection : MonoBehaviour {

    [SerializeField]
    private GameObject classSelectionPanel = null;

    [SerializeField]
    private GameObject walls_ = null;

    private bool classSelected = false;

    [SerializeField]
    private Character player = null;

    // Use this for initialization
    void Start () {

		if(GlobalGame.Instance.Player.HasSaveFile())
        {
            Destroy(classSelectionPanel);
            Destroy(walls_);
            classSelected = true;
            Destroy(this);
            Destroy(classSelectionPanel);
        }

        classSelectionPanel.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OpenClassSelection()
    {
        classSelectionPanel.SetActive(true);

        classSelectionPanel.GetComponentInChildren<Button>().Select();

    }

    public void OnClassSelected(int characterClassSelected)
    {
        Destroy(classSelectionPanel);
        Destroy(walls_);
        classSelected = true;
        DialogHandler.Instance.EndConversation();
        player.CharacterClass = (Class)characterClassSelected;
        player.SaveCharacter();
    }

    public bool ClassSelected
    {
        get { return classSelected; }
    }



}
