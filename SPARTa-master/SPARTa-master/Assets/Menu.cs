using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Menu : MonoBehaviour {

    public Texture background;
    public Texture menuBackground;
    public Texture emptyCheck;
    public Texture filledCheck;
    public bool openMenu = false;
    public bool debugUI=false;
    public bool feet = true;
    public bool meters = false;
    public Rect menuRect;
    public String filenamePref;
    public GUIStyle material;
    //public Dropdown dropdown;

    void Start()
    {
        menuRect = new Rect((Screen.width / 2) - 400, 400, 800, 600);
        //dropdown.options.Add(new Dropdown.OptionData("Feet, Inches"));
        //dropdown.options.Add(new Dropdown.OptionData("Meters"));
        if (PlayerPrefs.HasKey("Feet"))
        {
            feet = Convert.ToBoolean(PlayerPrefs.GetInt("Feet"));
            meters = !feet;
        }
        if (PlayerPrefs.HasKey("DebugUI"))
        {
            debugUI = Convert.ToBoolean(PlayerPrefs.GetInt("DebugUI"));
        }
        string pulledPref = PlayerPrefs.GetString("filenamePref");
        if(pulledPref == null || pulledPref == String.Empty)
        {
            filenamePref = "DynamicMesh";
        }
        else
        {
            filenamePref = PlayerPrefs.GetString("filenamePref");
        }

    }

    // Update is called once per frame
    void Update () {
	
	}

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background);
        GUIStyle centeredTextStyle = new GUIStyle("label");
        centeredTextStyle.alignment = TextAnchor.MiddleCenter;
        //GUI.Label(new Rect((Screen.width/2)-300, 100, 600, 150), string.Format("<size=100>{0}</size>", "Main Menu"), centeredTextStyle);
        if (GUI.Button(new Rect((Screen.width/2)-150, (Screen.height/2) - 200, 300, 100), string.Format("<size=30>{0}</size>", "Start Recording")))
        {
            PlayerPrefs.SetInt("DebugUI", Convert.ToInt32(debugUI));
            PlayerPrefs.SetInt("Feet", Convert.ToInt32(feet));
            
            SceneManager.LoadScene(1);
        }
        if (GUI.Button(new Rect((Screen.width / 2) - 150, (Screen.height / 2), 300, 100), string.Format("<size=30>{0}</size>", "Settings")))
        {
            openMenu = !openMenu;
        }
        if(openMenu)
        {
            menuRect = GUI.Window(0, menuRect, WindowFunction, menuBackground, "Settings");
        }
        
    }
    void WindowFunction(int windowID)
    {
        if (GUI.Button(new Rect(10, 290, 150, 80), string.Format("<size=40>{0}</size>", "Back")))
        {
            filenamePref = filenamePref.Replace(" ", "");
            PlayerPrefs.SetString("filenamePref", filenamePref);
            openMenu = !openMenu;
        }
        if (debugUI)
        {
            if (GUI.Button(new Rect(10, 10, 50, 50), filledCheck, material))
            {
                debugUI = false;
            }
        }
        if(!debugUI)
        {
            if (GUI.Button(new Rect(10, 10, 50, 50), emptyCheck, material))
            {
                debugUI = true;
            }
        }
        if (feet)
        {
            if (GUI.Button(new Rect(10, 100, 50, 50), filledCheck, material))
            {
                //feet = false;
                //meters = true;
            }
        }
        if (!feet)
        {
            if (GUI.Button(new Rect(10, 100, 50, 50), emptyCheck, material))
            {
                feet = true;
                meters = false;
            }
        }
        if (meters)
        {
            if (GUI.Button(new Rect(360, 100, 50, 50), filledCheck, material))
            {
                //feet = true;
                //meters = false;
            }
        }
        if (!meters)
        {
            if (GUI.Button(new Rect(360, 100, 50, 50), emptyCheck, material))
            {
                feet = false;
                meters = true;
            }
        }
        GUI.skin.textField.fontSize = 40;
        GUI.Label(new Rect(70, 10, 500, 50), string.Format("<size=40>{0}</size>", "Enable Debug UI"));
        GUI.Label(new Rect(70, 100, 500, 50), string.Format("<size=40>{0}</size>", "Feet & Inches"));
        GUI.Label(new Rect(420, 100, 500, 50), string.Format("<size=40>{0}</size>", "Meters"));
        GUI.Label(new Rect(10, 200, 200, 50), string.Format("<size=40>{0}</size>", "Filename:"));
        filenamePref = GUI.TextField(new Rect(220, 200, 500, 50), filenamePref, 20);
        // debugUI = GUILayout.Toggle(debugUI, string.Format("<size=40>{0}</size>", "Debug UI"));

        // dropdown.Show();


    }
}
