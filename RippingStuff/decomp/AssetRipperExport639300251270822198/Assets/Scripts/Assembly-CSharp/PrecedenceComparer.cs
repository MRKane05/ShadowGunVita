using System.Collections.Generic;

internal class PrecedenceComparer : IComparer<GOAPAction>
{
	public int Compare(GOAPAction x, GOAPAction y)
	{
		return x.Precedence - y.Precedence;
	}
}
