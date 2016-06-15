using UnityEngine;

public class Pile
{
    Change storedChange;
    int quantity;

    public Pile(Change storedChange, int quantity)
    {
        this.storedChange = storedChange;
        this.quantity = quantity;
    }

    public static Pile operator +(Pile firstAdditive, Pile secondAdditive)
    {
        Pile ret = new Pile(firstAdditive.storedChange, 0);
        ret.quantity = firstAdditive.quantity + secondAdditive.quantity;
        return ret;
    }
    public static Pile operator -(Pile firstAdditive, Pile secondAdditive)
    {
        Pile ret = new Pile(firstAdditive.storedChange, 0);
        ret.quantity = firstAdditive.quantity - secondAdditive.quantity;
        return ret;
    }

    public override string ToString()
    {
        return quantity.ToString();
    }

    public int Value()
    {
        return quantity*storedChange.Value;
    }

    public Change StoredChange
    {
        get
        {
            return storedChange;
        }
    }

    public int Quantity
    {
        get
        {
            return quantity;
        }
    }
}