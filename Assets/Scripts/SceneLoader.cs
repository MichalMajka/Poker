using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour {

    public Text errorMessagePlaceholder;

    void OnLevelWasLoaded(int level) {
        errorMessagePlaceholder.text = SceneManager.GetLevelError(Application.loadedLevel);
    }
}
