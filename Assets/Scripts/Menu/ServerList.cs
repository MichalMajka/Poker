using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ServerList : MonoBehaviour {
    public NetworkScript networkScript;
    public Button serverButton;
    List<Button> buttons = new List<Button>();
    HostData[] activeHosts;
    HostData chosenHost;
    void Update () {
        activeHosts = networkScript.GetServerList();
        if(activeHosts!=null)
        {
            foreach (HostData host in activeHosts)
            {
                bool isOnList = false;
                for (int i = 0; i < buttons.Count && !isOnList; i++)
                    if (buttons[i].name == host.gameName)
                        isOnList = true;
                if (!isOnList)
                {
                    Button newButton = Instantiate(serverButton);
                    newButton.transform.SetParent(gameObject.transform);
                    newButton.name = host.gameName;
                    newButton.GetComponentInChildren<Text>().text = host.gameName;
                    newButton.transform.localPosition = new Vector3(newButton.transform.position.x, newButton.transform.position.y, 0);
                    newButton.transform.localScale = new Vector3(1, 1, 1);
                    newButton.onClick.AddListener(() => ChoseServer(newButton, host));
                    buttons.Add(newButton);
                }
            }
            for (int j = 0; j < buttons.Count;)
            {
                bool stillOnLine = false;
                for (int i = 0; i < activeHosts.Length && !stillOnLine; i++)
                    if (buttons[j].name == activeHosts[i].gameName)
                        stillOnLine = true;
                if (!stillOnLine)
                {
                    Destroy(buttons[j].gameObject);
                    buttons.RemoveAt(j);
                    chosenHost = null;
                }
                else
                    j++;
            }
        }
    }

    public void ConnectToHost()
    {
        if(chosenHost != null)
            networkScript.ConnectToServer(chosenHost);
    }

    public void ChoseServer(Button button, HostData choose)
    {
        foreach (Button listButton in buttons)
            listButton.colors = serverButton.colors;
        ColorBlock myColors = new ColorBlock();
        myColors = button.colors;
        myColors.normalColor = button.colors.pressedColor;
        myColors.highlightedColor = button.colors.pressedColor;
        button.colors = myColors;
        chosenHost = choose;
    }
}
