using System.Collections.Generic;
using UnityEngine;
using KModkit;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;
using Rnd = UnityEngine.Random;

public class SimpleModuleScript : MonoBehaviour {

	public KMAudio audio;
	public KMBombInfo info;
	public KMNeedyModule module;
	public KMSelectable[] button;

	public int solveLim;

	public bool _isSolved = false;
	public bool needyActivated = false;

	public TextMesh[] screenTexts;

	public AudioSource correct;

	public string textFinder1;

	static int ModuleIdCounter;
	int ModuleId;

	void Awake()
	{
		ModuleId = ModuleIdCounter++;

		GetComponent<KMNeedyModule> ().OnNeedyActivation += OnNeedyActivation;
		GetComponent<KMNeedyModule> ().OnNeedyDeactivation += OnNeedyDeactivation;
		GetComponent<KMNeedyModule> ().OnTimerExpired += OnTimerExpired;
		GetComponent<KMNeedyModule> ().OnNeedyDeactivation += OnNeedySolved;

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

	public void OnNeedySolved()
	{
		textFinder1 = "Hey cool dude!";
		screenTexts[0].text = textFinder1;
	}

	public void OnTimerExpired()
	{
		textFinder1 = "Hey cool dude!";
		screenTexts[0].text = textFinder1;
		module.HandleStrike ();
		module.HandlePass ();
	}

	public void OnNeedyDeactivation()
	{
		correct.Play();
		textFinder1 = "GG";
		screenTexts[0].text = textFinder1;
		_isSolved = true;
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
}
