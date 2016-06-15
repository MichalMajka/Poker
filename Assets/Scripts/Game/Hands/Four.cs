
public class Four : Hand
{
    int cardPower;
    int looseCardPower;
    public Four(int cardPower) : base(7)
    {
        this.cardPower=cardPower;
    }

    public int LooseCardPower
    {
        set
        {
            looseCardPower = value;
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
        if (cardPower > (second as Four).cardPower)
            return 1;
        else if(cardPower < (second as Four).cardPower)
            return -1;
        else
            return 0;
    }
}
