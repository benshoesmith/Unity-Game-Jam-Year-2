using UnityEngine;

using System;
using System.Collections.Generic;
using System.Text;

public delegate void CommandHandler(string[] args);

public class ConsoleController
{

    #region Event declarations
    // Used to communicate with ConsoleView
    public delegate void LogChangedHandler(string[] log);
    public event LogChangedHandler logChanged;

    public delegate void VisibilityChangedHandler(bool visible);
    public event VisibilityChangedHandler visibilityChanged;
    #endregion

    /// <summary>
    /// Object to hold information about each command
    /// </summary>
    class CommandRegistration
    {
        public string command { get; private set; }
        public CommandHandler handler { get; private set; }
        public string help { get; private set; }

        public CommandRegistration(string command, CommandHandler handler, string help)
        {
            this.command = command;
            this.handler = handler;
            this.help = help;
        }
    }

    /// <summary>
    /// How many log lines should be retained?
    /// Note that strings submitted to appendLogLine with embedded newlines will be counted as a single line.
    /// </summary>
    const int scrollbackSize = 120;

    Queue<string> scrollback = new Queue<string>(scrollbackSize);
    List<string> commandHistory = new List<string>();
    Dictionary<string, CommandRegistration> commands = new Dictionary<string, CommandRegistration>(StringComparer.OrdinalIgnoreCase);

    public string[] log { get; private set; } //Copy of scrollback as an array for easier use by ConsoleView

    const string repeatCmdName = "!!"; //Name of the repeat command, constant since it needs to skip these if they are in the command history

    public ConsoleController()
    {
        //When adding commands, you must add a call below to registerCommand() with its name, implementation method, and help text.
        //registerCommand("babble", babble, "Example command that demonstrates how to parse arguments. babble [word] [# of times to repeat]");
        registerCommand("help", help, "Print this help.");
        registerCommand("hide", hide, "Hide the console.");
        registerCommand("addItem", addItem, "Adds an item with id to you inventory");
        registerCommand("addXP", addXP, "Adds the specified amount of xp to the player");
        registerCommand("setXP", setXP, "Sets the player experience to the given value");
        registerCommand("addSkillpoints", addSkilpoints, "Adds the specified amount of skillpoints to the player");
        registerCommand("setSkillpoints", setSkillpoints, "Sets the player skill points to the given value");
    }

    void registerCommand(string command, CommandHandler handler, string help)
    {
        commands.Add(command, new CommandRegistration(command, handler, help));
    }

    public void appendLogLine(string line)
    {
        if (scrollback.Count >= ConsoleController.scrollbackSize)
        {
            scrollback.Dequeue();
        }
        scrollback.Enqueue(line);

        log = scrollback.ToArray();
        if (logChanged != null)
        {
            logChanged(log);
        }
    }

    public void runCommandString(string commandString)
    {
        appendLogLine("$ " + commandString);

        string[] commandSplit = parseArguments(commandString);
        string[] args = new string[0];
        if (commandSplit.Length < 1)
        {
            appendLogLine(string.Format("Unable to process command '{0}'", commandString));
            return;

        }
        else if (commandSplit.Length >= 2)
        {
            int numArgs = commandSplit.Length - 1;
            args = new string[numArgs];
            Array.Copy(commandSplit, 1, args, 0, numArgs);
        }
        runCommand(commandSplit[0], args);
        commandHistory.Add(commandString);
    }

    public void runCommand(string command, string[] args)
    {
        CommandRegistration reg = null;
        if (!commands.TryGetValue(command, out reg))
        {
            appendLogLine(string.Format("Unknown command '{0}', type 'help' for list.", command));
        }
        else
        {
            if (reg.handler == null)
            {
                appendLogLine(string.Format("Unable to process command '{0}', handler was null.", command));
            }
            else
            {
                reg.handler(args);
            }
        }
    }

    static string[] parseArguments(string commandString)
    {
        LinkedList<char> parmChars = new LinkedList<char>(commandString.ToCharArray());
        bool inQuote = false;
        var node = parmChars.First;
        while (node != null)
        {
            var next = node.Next;
            if (node.Value == '"')
            {
                inQuote = !inQuote;
                parmChars.Remove(node);
            }
            if (!inQuote && node.Value == ' ')
            {
                node.Value = '\n';
            }
            node = next;
        }
        char[] parmCharsArr = new char[parmChars.Count];
        parmChars.CopyTo(parmCharsArr, 0);
        return (new string(parmCharsArr)).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
    }

    #region Command handlers

    void addItem(string[] args)
    {
        if (args.Length == 1)
        {
            InventoryHandler.Instance.AddItem(Int32.Parse(args[0]));
            appendLogLine("Added the Item with ID: " + args[0] + " to the inventory.");
        } else if(args.Length == 2)
        {
            int max =(Int32.Parse(args[1]));
            appendLogLine("Added " + max + " of the Item with ID: " + args[0] + " to the inventory.");
            for (int i = 0; i < max; i++)
            {
                InventoryHandler.Instance.AddItem(Int32.Parse(args[0]));

            }
        }
    }

    void addXP(string[] args)
    {
        if (args.Length == 1)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Character>().Xp += Int32.Parse(args[0]);
            appendLogLine("Added " + args[0] + " XP to the player.");
        }
    }

    void setXP(string[] args)
    {
        if (args.Length == 1)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Character>().Xp = Int32.Parse(args[0]);
            appendLogLine("Set the player XP to: " + args[0]);
        }
    }

    void addSkilpoints(string[] args)
    {
        if (args.Length == 1)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Character>().SkillPoints += Int32.Parse(args[0]);
        }
        appendLogLine("Added " + args[0] + " Skillpoints to the player.");
    }

    void setSkillpoints(string[] args)
    {
        if (args.Length == 1)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Character>().SkillPoints = Int32.Parse(args[0]);
            appendLogLine("Set the player Skillpoints to: " + args[0]);
        }
    }

    void help(string[] args)
    {
        foreach (CommandRegistration reg in commands.Values)
        {
            appendLogLine(string.Format("{0}: {1}", reg.command, reg.help));
        }
    }

    void hide(string[] args)
    {
        if (visibilityChanged != null)
        {
            visibilityChanged(false);
        }
    }
    #endregion
}