#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class PlayerPrefsEditor
{

    [MenuItem("Tools/Clear Race Data")]
    private static void ClearRaceData()
    {
        PlayerPrefs.DeleteKey("AllGameData");
        PlayerPrefs.Save();
        Debug.Log("Race data cleared from PlayerPrefs.");
    }
}
#endif