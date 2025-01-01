using System.Collections.Generic;
using UnityEngine;
using KModkit;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;
using Rnd = UnityEngine.Random;
using System.Collections;

public class SimpleModuleScript : MonoBehaviour {

	public KMAudio audio;
	public KMBombInfo info;
	public KMNeedyModule module;
	public KMSelectable[] button;

	public int solveLim;

	public bool _isSolved = false;
	public bool needyActivated = false;

	public TextMesh[] screenTexts;

	public string textFinder1;

	static int ModuleIdCounter;
	int ModuleId;

	void Awake()
	{
		ModuleId = ModuleIdCounter++;

		GetComponent<KMNeedyModule> ().OnNeedyActivation += OnNeedyActivation;
		GetComponent<KMNeedyModule> ().OnNeedyDeactivation += OnNeedyDeactivation;
		GetComponent<KMNeedyModule> ().OnTimerExpired += OnTimerExpired;

		foreach (KMSelectable button in button)
		{
			KMSelectable pressedButton = button;
			button.OnInteract += delegate () { buttonPress(pressedButton); return false; };
		}
	}

	void Start ()
	{
		textFinder1 = "Hey cool dude!";
		screenTexts[0].text = textFinder1;
	}

	public void OnNeedyActivation()
	{
		solveLim = info.GetSolvedModuleNames().ToList().Count;
		textFinder1 = "Solve a module";
		screenTexts[0].text = textFinder1;
		needyActivated = true;
	}

    public void OnNeedyDeactivation()
    {
        textFinder1 = "GG";
        screenTexts[0].text = textFinder1;
        _isSolved = true;
    }

    public void OnTimerExpired()
	{
		textFinder1 = "Hey cool dude!";
		screenTexts[0].text = textFinder1;
		module.HandleStrike ();
		module.HandlePass ();
	}

    public void OnNeedySolved()
    {
        textFinder1 = "Hey cool dude!";
        screenTexts[0].text = textFinder1;
    }

    public void buttonPress(KMSelectable pressedButton)
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.ButtonPress, transform);
		int buttonPosition = new int();
		for(int i = 0; i < button.Length; i++)
		{
			if (pressedButton == button[i])
			{
				buttonPosition = i;
				break;
			}
		}

		if (_isSolved == false) 
		{
			if (needyActivated == true) 
			{
				switch (buttonPosition) 
				{
				case 0:
					if (solveLim < info.GetSolvedModuleNames ().ToList ().Count) 
					{
						module.HandlePass ();
						Invoke ("OnNeedySolved", 0);
						Log ("Button Press Good");
					}
					else 
					{
						Invoke ("OnTimerExpired", 0);
						Log ("Button Press Bad");
					}
					break;
				}
			}
		}

	}



	void Log(string message)
	{
		Debug.LogFormat("[Speedrun #{0}] {1}", ModuleId, message);
	}

	#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press [Presses the button]";
	#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
	{
		if (command != "press") yield return "sendtochaterror Invalid, or too many arguments!";
		else button[0].OnInteract(); yield return null;
	}
	IEnumerator TwitchHandleForcedSolve() { module.HandlePass(); yield return null; }
}
