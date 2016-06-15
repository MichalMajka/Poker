
using UnityEngine;

public class TwoPairs : Hand
{
    int[] cardPower = new int[3];
    public TwoPairs(Pair first, Pair second) : base(2)
    {
        if (first.CardPower > second.CardPower)
        {
            cardPower[0] = first.CardPower;
            cardPower[1] = second.CardPower;
        }
        else
        {
            cardPower[1] = first.CardPower;
            cardPower[0] = second.CardPower;
        }
    }

    public int LooseCardPower
    {
        set
        {
            cardPower[2] = value;
        }
    }

    override public int Compare(Hand second)
    {
        TwoPairs secondHidden = second as TwoPairs;
        for (int i = 0; i < 3; i++)
        {
            if (cardPower[i] > secondHidden.cardPower[i])
                return 1;
            else if (cardPower[i] < secondHidden.cardPower[i])
                return -1;
        }
        return 0;
    }
}
