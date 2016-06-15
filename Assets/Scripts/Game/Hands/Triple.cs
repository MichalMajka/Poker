
public class Triple : Hand
{
    int cardPower;
    int[] looseCardsPower;
    public Triple(int cardPower) : base(3)
    {
        this.cardPower = cardPower;
    }

    public int[] LooseCardsPower
    {
        set
        {
            looseCardsPower = value;
        }
    }

    public int CardPower
    {
        get
        {
            return cardPower;
        }
    }


    override public int Compare(Hand second)
    {
        Triple secondHidden = second as Triple;
        if (cardPower > secondHidden.cardPower)
            return 1;
        else if (cardPower < secondHidden.cardPower)
            return -1;
        for (int i = 0; i < 2; i++)
        {
            if (looseCardsPower[i] > secondHidden.looseCardsPower[i])
                return 1;
            else if (looseCardsPower[i] < secondHidden.looseCardsPower[i])
                return -1;
        }
        return 0;
    }
}
