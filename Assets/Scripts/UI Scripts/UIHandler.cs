using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {

    public Canvas PauseMenuCanvas;
    public Canvas InventoryCanvas;
    public Canvas SkillMenuCanvas;
    public Canvas CharacterInvCanvas;
    public Canvas DialogCanvas;
    public Canvas MiniMapCanvas;
    public Canvas NotificationCanvas;
    public Canvas QuestCanvas;
    public RectTransform ConsoleCanvas;

    private void Awake()
    {
        GlobalGame.Instance.GameStateSwitched += Instance_GameStateSwitched;
    } 

    private void Start()
    {
        Instance_GameStateSwitched();
    }

    private void Instance_GameStateSwitched()
    {
        switch(GlobalGame.Instance.CurrentGameState)
        {
            case GlobalGame.GameState.Normal:
                CloseAll();
                MiniMapCanvas.gameObject.SetActive(true);
                NotificationCanvas.gameObject.SetActive(true);
                break;
            case GlobalGame.GameState.InCombat:
                CloseAll();
                break;
            case GlobalGame.GameState.Paused:
                CloseAllButPauseMenu();
                NotificationCanvas.gameObject.SetActive(true);
                break;
            case GlobalGame.GameState.InDialog:
                CloseAll();
                OpenDialogUI();
                NotificationCanvas.gameObject.SetActive(true);
                break;
            case GlobalGame.GameState.InTransition:
                CloseAll();
                break;
        }
    }

    private void Update()
    {
        if (ConsoleCanvas.gameObject.activeSelf)
        {
            GlobalGame.Instance.CurrentPlayerState = GlobalGame.PlayerState.InMenu;
            return;
        }

        if (Input.GetKeyUp(KeyCode.I))
        {
            ToggleInventory();

        }
        else if (Input.GetKeyUp(KeyCode.P))
        {
            TogglePauseMenu();
        }
        else if (Input.GetKeyUp(KeyCode.K))
        {
            ToggleSkillMenu();
        } else if (Input.GetKeyUp(KeyCode.C))
        {
            ToggleCharacter();
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            ToggleQuestUI();
        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (GlobalGame.Instance.CurrentGameState == GlobalGame.GameState.Paused)
                GlobalGame.Instance.CurrentGameState = GlobalGame.GameState.Normal;

            CloseAll();
            MiniMapCanvas.gameObject.SetActive(true);
        }

        //If any ui is open then set player state to in menus. (which disables character movement.)
        if (AnyUIOpen())
            GlobalGame.Instance.CurrentPlayerState = GlobalGame.PlayerState.InMenu;
        else
            GlobalGame.Instance.CurrentPlayerState = GlobalGame.PlayerState.Normal;
  
    }

    private void TogglePauseMenu()
    {
        //Only toggle pause menu if in normal mode or paused mode.
        if (GlobalGame.Instance.CurrentGameState == GlobalGame.GameState.Normal || GlobalGame.Instance.CurrentGameState == GlobalGame.GameState.Paused)
        {

            PauseMenuCanvas.gameObject.SetActive(!PauseMenuCanvas.gameObject.activeSelf);

            if (PauseMenuCanvas.gameObject.activeSelf)
                GlobalGame.Instance.CurrentGameState = GlobalGame.GameState.Paused;
            else
                GlobalGame.Instance.CurrentGameState = GlobalGame.GameState.Normal;



        }
    }

    private void ToggleInventory()
    {
     
        //only toggle the inventory if in the normal game state/mode.
        if (GlobalGame.Instance.CurrentGameState == GlobalGame.GameState.Normal)
            InventoryCanvas.gameObject.SetActive(!InventoryCanvas.gameObject.activeSelf);

        //find first selectable button in inventory
        Toggle btn = InventoryCanvas.GetComponentInChildren<Toggle>();
        //if the inventory had just been set to active and there is a button that can be pressed then put it in focus.
        if (InventoryCanvas.gameObject.activeSelf)
        {
            if (OnInventoryOpen != null)
                OnInventoryOpen.Invoke();
            if (btn)
                btn.Select();
        }
    }

    private void ToggleQuestUI()
    {

        //only toggle the quest ui if in the normal game state/mode.
        if (GlobalGame.Instance.CurrentGameState == GlobalGame.GameState.Normal)
            QuestCanvas.gameObject.SetActive(!QuestCanvas.gameObject.activeSelf);

        if(QuestCanvas.gameObject.activeSelf)
        {
            QuestManagerUI qui = QuestCanvas.GetComponent<QuestManagerUI>();
            if(qui)
            {
                qui.UpdateQuestUI();
            }
        }

        //find first selectable button in quest ui
        Button btn = QuestCanvas.GetComponentInChildren<Button>();
        //if the quest ui had just been set to active and there is a button that can be pressed then put it in focus.
        if (QuestCanvas.gameObject.activeSelf && btn)
        {
            btn.Select();
        }
    }


    private void ToggleSkillMenu()
    {
        //only toggle the skillmenu if in the normal game state/mode.
        if (GlobalGame.Instance.CurrentGameState == GlobalGame.GameState.Normal)
            SkillMenuCanvas.gameObject.SetActive(!SkillMenuCanvas.gameObject.activeSelf);

        //find first selectable button in skill tree
        Button btn = SkillMenuCanvas.GetComponentInChildren<Button>();
        //if the skill tree had just been set to active and there is a button that can be pressed then put it in focus.
        if (SkillMenuCanvas.gameObject.activeSelf)
        {
            if (OnSkillTreeOpen != null)
                OnSkillTreeOpen.Invoke();

            if(btn)
                btn.Select();
        }
    }

    private void ToggleCharacter()
    {
        //only toggle the skillmenu if in the normal game state/mode.
        if (GlobalGame.Instance.CurrentGameState == GlobalGame.GameState.Normal)
            CharacterInvCanvas.gameObject.SetActive(!CharacterInvCanvas.gameObject.activeSelf);

        //find first selectable button in skill tree
        Toggle btn = CharacterInvCanvas.GetComponentInChildren<Toggle>();
        //if the skill tree had just been set to active and there is a button that can be pressed then put it in focus.
        if (CharacterInvCanvas.gameObject.activeSelf)
        {
            if (OnCharacterOpen != null)
                OnCharacterOpen.Invoke();

            if( btn)
                btn.Select();
        }
    }

    private void OpenDialogUI()
    {
        
        if (DialogCanvas && GlobalGame.Instance.CurrentGameState == GlobalGame.GameState.InDialog)
            DialogCanvas.gameObject.SetActive(!DialogCanvas.gameObject.activeSelf);

        if (!DialogCanvas)
            return;

        //find first selectable button in dialog
        Button btn = DialogCanvas.GetComponentInChildren<Button>();
        //if the DialogCanvas had just been set to active and there is a button that can be pressed then put it in focus.
        if (DialogCanvas.gameObject.activeSelf && btn)
            btn.Select();
    }

    private bool AnyUIOpen()
    {
        return (SkillMenuCanvas.gameObject.activeSelf || InventoryCanvas.gameObject.activeSelf || PauseMenuCanvas.gameObject.activeSelf || CharacterInvCanvas.gameObject.activeSelf || QuestCanvas.gameObject.activeSelf);
    }

    private void CloseAllButPauseMenu()
    {
        InventoryCanvas.gameObject.SetActive(false);
        SkillMenuCanvas.gameObject.SetActive(false);
        if(DialogCanvas)
            DialogCanvas.gameObject.SetActive(false);
        CharacterInvCanvas.gameObject.SetActive(false);
        NotificationCanvas.gameObject.SetActive(false);
        QuestCanvas.gameObject.SetActive(false);
    }

    private void CloseAll()
    {
        CloseAllButPauseMenu();
        PauseMenuCanvas.gameObject.SetActive(false);
        GlobalGame.Instance.CurrentPlayerState = GlobalGame.PlayerState.Normal;
        MiniMapCanvas.gameObject.SetActive(false);
    }

    public void exitGame()
    {
        Application.Quit();
    }


    public void continueGame()
    {
        if (GlobalGame.Instance.CurrentGameState == GlobalGame.GameState.Paused)
            PauseMenuCanvas.gameObject.SetActive(false);

        GlobalGame.Instance.CurrentGameState = GlobalGame.GameState.Normal;
    }

    public delegate void UIHandlerEventHandler();
    public event UIHandlerEventHandler OnInventoryOpen;
    public event UIHandlerEventHandler OnCharacterOpen;
    public event UIHandlerEventHandler OnSkillTreeOpen;
}
