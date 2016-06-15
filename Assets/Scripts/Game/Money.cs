using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Money
{
    Pile[] changes = new Pile[5];
    Change[] blueprints;

    public Money(string owner, Change[] blueprints)
    {
        this.blueprints = blueprints;
        if (owner == "Player")
        {
            changes[0] = new Pile(blueprints[0], 80);
            changes[1] = new Pile(blueprints[1], 40);
            changes[2] = new Pile(blueprints[2], 20);
            changes[3] = new Pile(blueprints[3], 10);
            changes[4] = new Pile(blueprints[4], 5);
        }
        else if(owner== "Empty")
        {
            for (int i = 0; i < 5; i++)
                changes[i] = new Pile(blueprints[i], 0);
        }
        else
        {
            string[] values = owner.Split(' ');
            for(int i=0;i<5;i++)
            changes[i] = new Pile(blueprints[i], Int32.Parse(values[i]));
        }
    }

    public static Money operator +(Money firstAdditive, Money secondAdditive)
    {
        Money ret = new Money("Empty", firstAdditive.blueprints);
        for (int i = 0; i < firstAdditive.changes.Length; i++)
            ret.changes[i] = firstAdditive.changes[i] + secondAdditive.changes[i];
        return ret;
    }
    public static Money operator -(Money firstAdditive, Money secondAdditive)
    {
        Money ret = new Money("Empty", firstAdditive.blueprints);
        for (int i = 0; i < firstAdditive.changes.Length; i++)
        {
            if (firstAdditive.changes[i].Value() - secondAdditive.changes[i].Value() < 0)
            {
                firstAdditive.changes[i + 1] = new Pile(firstAdditive.changes[i + 1].StoredChange, firstAdditive.changes[i + 1].Quantity - 1);
                firstAdditive.changes[i] = new Pile(firstAdditive.changes[i].StoredChange, firstAdditive.changes[i].Quantity + firstAdditive.changes[i + 1].StoredChange.Value/firstAdditive.changes[i].StoredChange.Value);
            }
            ret.changes[i] = firstAdditive.changes[i] - secondAdditive.changes[i];
        }
        return ret;
    }

    public Money SplitAward(int count)
    {
        return CountChanges(Value() / count);
    }

    public override string ToString()
    {
        string value = "";
        for(int i=0;i<changes.Length-1;i++)
            value += changes[i].ToString() + ' ';
        value += changes[changes.Length - 1].ToString();
        return value;
    }

    public int Value()
    {
        int ret = 0;
        for (int i = 0; i < changes.Length; i++)
            ret += changes[i].Value();
        return ret;
    }


    public Money CountChanges(int target)
    {
        Money ret = new Money("Empty", blueprints);
        for(int i=4;i>=0;i--)
        {
            int temp = (int)(target / ret.changes[i].StoredChange.Value);
            if(temp>changes[i].Quantity)
            {
                temp = changes[i].Quantity;
            }
            target-=temp* ret.changes[i].StoredChange.Value;
            ret.changes[i] = new Pile(ret.changes[i].StoredChange, temp);
        }
        return ret;
    }
}