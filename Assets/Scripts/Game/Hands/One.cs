using System;
using System.Collections.Generic;

public class One : Hand
{
    int[] highestCards;

    public One() : base(0)
    {
    }

    public int[] HighestCards
    {
        set
        {
            highestCards = value;
        }
    }

    override public int Compare(Hand second)
    {
        One secondHidden = second as One;
        for (int i = 0; i < 3; i++)
        {
            if (highestCards[i] > secondHidden.highestCards[i])
                return 1;
            else if (highestCards[i] < secondHidden.highestCards[i])
                return -1;
        }
        return 0;
    }
}
