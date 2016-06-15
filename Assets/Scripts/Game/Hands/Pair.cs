using System;

public class Pair : Hand
{
    int cardPower;
    int[] looseCardsPower;
    public Pair(int cardPower) : base(1)
    {
        this.cardPower = cardPower;
    }

    public int CardPower
    {
        get
        {
            return cardPower;
        }
    }

    public int[] LooseCardsPower
    {
        set
        {
            looseCardsPower = value;
        }
    }


    override public int Compare(Hand second)
    {
        Pair secondHidden = second as Pair;
        if (cardPower > secondHidden.cardPower)
            return 1;
        else if (cardPower < secondHidden.cardPower)
            return -1;
        for (int i = 0; i < 3; i++)
        {
            if (looseCardsPower[i] > secondHidden.looseCardsPower[i])
                return 1;
            else if (looseCardsPower[i] < secondHidden.looseCardsPower[i])
                return -1;
        }
        return 0;
    }
}

