public class Straight : Hand
{
    int highestCard;

    public Straight(int highestCard) : base(4)
    {
        this.highestCard = highestCard;
    }

    override public int Compare(Hand second)
    {
        Straight secondHidden = second as Straight;
        if (highestCard > secondHidden.highestCard)
            return 1;
        else if (highestCard < secondHidden.highestCard)
            return -1;
        else
            return 0;
    }
}
