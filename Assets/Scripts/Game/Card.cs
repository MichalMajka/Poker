using UnityEngine;
using System.Collections;
using System;

public class Card
{
    int hierarchy;
    int colour;
    bool isUncovered = false;

    public Card(int colour, int hierarchy)
    {
        this.hierarchy = hierarchy;
        this.colour = colour;
    }

    public bool IsUncovered
    {
        get { return isUncovered; }
    }

    internal void Cover()
    {
        isUncovered = false;
    }

    internal void Uncover()
    {
        isUncovered = true;
    }


    public static int CompareByHierarchy(Card first, Card second)
    {
        int ret = 0;
        if (first.hierarchy > second.hierarchy) ret = -1;
        else if (first.hierarchy < second.hierarchy) ret = 1;
        return ret;
    }
    public static int CompareByColour(Card first, Card second)
    {
        int ret = 0;
        if (first.colour == second.colour)
        {
            if (first.hierarchy > second.hierarchy) ret = -1;
            else if (first.hierarchy < second.hierarchy) ret = 1;
        }
        else
        {
            if(first.colour > second.colour) ret = -1;
            else if (first.colour < second.colour) ret = 1;
        }
        return ret;
    }

    override public string ToString()
    {
        return hierarchy.ToString() + colour.ToString();
    }

    public int Hierarchy
    {
        get
        {
            return hierarchy;
        }
    }

    public int Colour
    {
        get
        {
            return colour;
        }
    }
}
