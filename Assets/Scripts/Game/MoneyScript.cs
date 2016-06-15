using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class MoneyScript : MonoBehaviour {

    public Text changeValue;

    List<Text> changes = new List<Text>();
	
    public void SetMoney(string money) {
        if (changes.Count==0)
            foreach (Text child in gameObject.GetComponentsInChildren<Text>())
                if(child.name != "Label")
                    changes.Add(child);
        int i = 0;
        foreach (string value in money.Split(' '))
        {
            int difference = Int32.Parse(value) - Int32.Parse(changes[i].text);
            if(difference != 0)
            {
                Text animation = Instantiate(changeValue);
                animation.transform.SetParent(changes[i].transform.parent, false);
                animation.text = difference.ToString();
                if (difference > 0)
                    animation.text = "+" + animation.text;
                else
                    animation.color = Color.red;
            }
            changes[i++].text = value;
        }
    }

    public string GetMoney()
    {
        string ret = "";
        if (changes.Count == 0)
            foreach (Text child in gameObject.GetComponentsInChildren<Text>())
                if(child.name!="Label")
                    changes.Add(child);
        for(int i=0;i<changes.Count;i++)
        {
            ret += changes[i].text;
            if (i < changes.Count - 1)
                ret += " ";
        }
        return ret;
    }
}
