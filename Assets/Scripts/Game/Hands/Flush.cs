
public class Flush : Hand
{
    int highestCard;

    public Flush(int highestCard) : base(5)
    {
        this.highestCard = highestCard;
    }

    public int HighestCard
    {
        set
        {
            highestCard = value;
        }

        get
        {
            return highestCard;
        }
    }

    override public int Compare(Hand second)
    {
        Flush secondHidden = second as Flush;
        if (highestCard > secondHidden.highestCard)
            return 1;
        else if (highestCard < secondHidden.highestCard)
            return -1;
        else
            return 0;
    }
}
