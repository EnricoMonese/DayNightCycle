using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor (typeof(DayNightCycle))]
public class DayNightCycleEditor : Editor {

	DayNightCycle script;

	void OnEnable () {
		script = target as DayNightCycle;
		script.EditorSetup ();
	}


	public override void OnInspectorGUI () {
		base.OnInspectorGUI ();
		SecondsDurationGUI ("Complete day duration: ", script.secondsInFullDay);
		SecondsDurationGUI ("Enviroment updates every: ", script.updateRateInSeconds);
		float currentSeconds = script.secondsInFullDay * script.currentTimeOfDay;
		SecondsDurationGUI ("Elapsed time from : ", currentSeconds);
		float hour = 86400f * script.currentTimeOfDay;
		SecondsDurationGUI ("Current in game time: ", hour);

		if (GUI.changed) {
			EditorUtility.SetDirty (script);
			script.UpdateEditor ();
		}
	}

	void SecondsDurationGUI (string message, float seconds) {
		System.TimeSpan time = System.TimeSpan.FromSeconds(seconds);
		string str = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D2}ms", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
		EditorGUILayout.HelpBox (message + str, MessageType.None, true);
	}
}
