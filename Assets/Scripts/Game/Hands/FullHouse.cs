
public class FullHouse : Hand
{
    int triplePower;
    int pairPower;
    public FullHouse(Pair pair, Triple triple) : base(6)
    {
        triplePower = triple.CardPower;
        pairPower = pair.CardPower;
    }

    public FullHouse(Triple triple, Pair pair) : base(6)
    {
        triplePower = triple.CardPower;
        pairPower = pair.CardPower;
    }

    override public int Compare(Hand second)
    {
        FullHouse secondHidden = second as FullHouse;
        if (triplePower > secondHidden.triplePower)
            return 1;
        else if (triplePower < secondHidden.triplePower)
            return -1;
        if (pairPower > secondHidden.pairPower)
            return 1;
        else if (pairPower < secondHidden.pairPower)
            return -1;
        return 0;
    }
}
