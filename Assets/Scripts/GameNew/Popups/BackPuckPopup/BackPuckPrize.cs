using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BackPuckPrize
{
	public enum PrizeType { Gold, Exp, VIP};
    public PrizeType prizeType;
    public int count;

    public BackPuckPrize(PrizeType prizeType, int count)
    {
        this.prizeType = prizeType;
        this.count = count;
    }
}