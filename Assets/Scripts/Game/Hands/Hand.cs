using System;
using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    protected int power = 0;

    public Hand()
    {

    }

    protected Hand(int power)
    {
        this.power = power;
    }

    public static int Compare(Hand first, Hand second)
    {
        if (first.GetType() != second.GetType())
        {
            if (first.power < second.power)
                return -1;
            else if (first.power > second.power)
                return 1;
            else
                return 0;
        }
        else
        {
            return first.Compare(second);
        }
    }

    public virtual int Compare(Hand second)
    {
        return 0;
    }
}
