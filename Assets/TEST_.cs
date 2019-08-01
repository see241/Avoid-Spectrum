using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_ : MonoBehaviour {
	public UnityEngine.UI.Text text;
	// Use this for initialization
	void Start () {
		text.text=AudioSettings.outputSampleRate.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log(AudioSettings.outputSampleRate.ToString());
	}
}
