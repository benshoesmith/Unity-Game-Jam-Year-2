using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleViewUpdater : MonoBehaviour {

    //Public variables for the editor
    public GameObject consoleContainer;
    public Text logs;
    public InputField cmdInput;
    //Private ones
    private ConsoleController console = new ConsoleController();

    void Start() {
        //Add some functions to the consoleController events
        if (console != null)
        {
            console.visibilityChanged += onShowStatusChanged;
            console.logChanged += onLogChanged;
        }
        //Update the textLog
        console.appendLogLine("Starting Game....");
        console.appendLogLine("\"The Adventure of the Moose Legend\" started correctly!");
        updateLog(console.log);

        //Add Log callback so, Debus get printed to console too
        Application.logMessageReceived += this.HandleDebugLogMsg;
    }

    // Update is called once per frame
    void Update() { 
        //If tilt es pressed show the console
        if(Input.GetKeyUp("`"))
        {
            //Show console
            toggleShown();
        }
	}

    /*
     * Functions to control the status of the console, if it should be shown or not. 
     * 
     */
    #region Show/Hide console Function
    private void toggleShown()
    {
        setShown(!consoleContainer.activeSelf);
    }

    private void setShown(bool show)
    {
        consoleContainer.SetActive(show);
    }

    private void onShowStatusChanged(bool show)
    {
        setShown(show);
    }
    #endregion


    /*
     * Functions to take care to update the text of the console
     */
    #region Log Text update Functions

    void HandleDebugLogMsg(string logString, string stackTrace, LogType type)
    {
        this.console.appendLogLine(logString);
    }

    private void onLogChanged(string[] newLogContent)
    {
        //If the log changed in the console Controller update the text that is been displayed
        updateLog(newLogContent);
    }

    private void updateLog(string[] newLogcontent)
    {
        //Check if the content is null or empty, if so just reset the text
        if (newLogcontent != null && newLogcontent.Length > 0)
        {
            logs.text = string.Join("\n", newLogcontent);
        } else
        {
            logs.text = "";
        }
    }
    #endregion

    //Exposed function to allow us to run a command.
    public void runCommand()
    {
        //Pass the command to the console controller
        console.runCommandString(cmdInput.text);
        //Reset the input text
        cmdInput.text = "";
    }
}
