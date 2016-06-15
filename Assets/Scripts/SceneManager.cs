using System;
using System.Collections.Generic;
using UnityEngine;


public static class SceneManager {
    static private Dictionary<int,string> descriptonOfProblem = new Dictionary<int, string>();

    static public void LoadLevelWithError(int levelIndex, string errorMessage) {
        if(descriptonOfProblem.ContainsKey(levelIndex))
            descriptonOfProblem[levelIndex] = errorMessage;
        else
            descriptonOfProblem.Add(levelIndex, errorMessage);
        Application.LoadLevel(levelIndex);
    }

    static public string GetLevelError(int levelIndex) {
        if(descriptonOfProblem.ContainsKey(levelIndex)) {
            string valueToReturn = descriptonOfProblem[levelIndex];
            descriptonOfProblem[levelIndex] = "";
            return valueToReturn;
        }
        else {
            return "";
        }
    }
}

