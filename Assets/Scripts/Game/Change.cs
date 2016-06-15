using UnityEngine;

public class Change
{
    private int value;

    public Change(int value)
    {
        this.value = value;
    }

    public int Value
    {
        get
        {
            return value;
        }
    }
}