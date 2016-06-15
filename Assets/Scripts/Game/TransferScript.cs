using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TransferScript : MonoBehaviour {

    public TransferScript transferTo;
    public bool minimumValueIsZero;

    int value;

    public void TakeTransfer()
    {
        gameObject.GetComponentInChildren<Text>().text = (int.Parse(gameObject.GetComponentInChildren<Text>().text) + 1).ToString();
    }

    void OnMouseDown()
    {
        if (minimumValueIsZero)
        {
            if (int.Parse(gameObject.GetComponentInChildren<Text>().text) > 0)
            {
                transferTo.TakeTransfer();
                gameObject.GetComponentInChildren<Text>().text = (int.Parse(gameObject.GetComponentInChildren<Text>().text) - 1).ToString();
            }
        }
        else
        {
            if (int.Parse(gameObject.GetComponentInChildren<Text>().text) > value)
            {
                transferTo.TakeTransfer();
                gameObject.GetComponentInChildren<Text>().text = (int.Parse(gameObject.GetComponentInChildren<Text>().text) - 1).ToString();
            }
        }

    }

    public void RememberValue()
    {
        value = int.Parse(gameObject.GetComponentInChildren<Text>().text);
    }

    public void RecoverValue()
    {
        gameObject.GetComponentInChildren<Text>().text = value.ToString();
    }

    public void StartRising()
    {
        RememberValue();
        gameObject.GetComponent<Collider2D>().enabled = true;
    }

    public void StopRising()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
    }

    public void CancelRising()
    {
        RecoverValue();
        gameObject.GetComponent<Collider2D>().enabled = false;
    }
}
