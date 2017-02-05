using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(DayNightCycleGradient))]
public class DayNightCycleGradientEditor : Editor
{

	DayNightCycleGradient script;

	void OnEnable ()
	{
		script = target as DayNightCycleGradient;
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();
		//base.OnInspectorGUI ();
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

		if (GUILayout.Button ("Create Background Plane")) {
			EditorUtility.SetDirty (script);
			script.MakePlaneBasedOnGradient ();
		}
	}

	void SecondsDurationGUI (string message, float seconds)
	{
		System.TimeSpan time = System.TimeSpan.FromSeconds (seconds);
		string str = string.Format ("{0:D2}h:{1:D2}m:{2:D2}s:{3:D2}ms", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
		EditorGUILayout.HelpBox (message + str, MessageType.None, true);
	}
}
