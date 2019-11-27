<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Numerics.dll</Reference>
  <Namespace>System.Numerics</Namespace>
</Query>

//https://www.mathsisfun.com/data/quincunx-explained.html
void Main()
{
	BinomailDistribuition.Possibilities = 10;
	var results = new List<BigDecimal>();
	results.Load();
	results.Print(true); //send false to print Table 1.
}

public static class BinomailDistribuition
{
	public static int Possibilities = 0;
	static int middleLeft = 0;
	static int middleRight = 0;
	static int resultCount = 0;
	
	public static void Load(this List<BigDecimal> results)
	{
		for (int i = 0; i <= Possibilities; i++)
		{
			var fatorLeft = Fatorial(Possibilities);
			var fatorRight = BigInteger.Multiply(Fatorial(i), Fatorial(Possibilities - i));
			BigInteger fat = BigInteger.Divide(fatorLeft, fatorRight);
			var powLeft = new BigDecimal(1, 0, 1000000000);
			var powRight = new BigDecimal(1, 0, 1000000000);
			if (i != 0)
				powLeft = BigDecimal.Pow(new BigDecimal(5, 1, 1000000000), i);
			if (i != Possibilities)
				powRight = BigDecimal.Pow(new BigDecimal(5, 1, 1000000000), (Possibilities - i));
			var prob = new BigDecimal(fat) * powLeft * powRight;
			results.Add(prob);
		}
	}
	
	public static BigInteger Fatorial(int value)
	{
		BigInteger fatorial = 1;
		for (int n = 1; n <= value; n++)
		{
			fatorial *= n;
		}
		return fatorial;
	}
	
	public static void Print(this List<BigDecimal> results, bool printTableProbability)
	{
		if (!printTableProbability)
		{
			var sum = results.Sum();
			var middle = (middleRight - middleLeft) / 2;
			var middlePercent = ((middleRight - middleLeft) * 14) / 100;
			var list = results.Where((x, i) => i >= middleLeft && i <= middleRight).ToList();
			var listPareto = list.Where((x, i) => i >= (middle - middlePercent) && i <= (middle + middlePercent)).ToList();
			var percentOfSum = (middleRight - middleLeft) * 100 / resultCount;
			var sumPercent = sum * new BigDecimal(100, 0, 1000000000);
			var paretoResult = new BigDecimal(0, 0, 1000000000);
			listPareto.ForEach(x => { paretoResult = paretoResult + x; });

			sumPercent.Dump("sum");
			middleLeft.Dump("middleLeft");
			middleRight.Dump("middleRight");
			(middleRight - middleLeft).Dump("itens of sum");
			percentOfSum.Dump("percent of sum");
			resultCount.Dump("total");
			paretoResult.Dump("20/80");
		}
		else
		{
			results.Dump(); //Valid Binomial distribution	
		}
	}
	
	public static BigDecimal Sum(this List<BigDecimal> results)
	{
		resultCount = results.Count;
		middleLeft = resultCount / 2;
		middleRight = middleLeft * 2 < resultCount ? middleLeft + 1 : middleLeft;

		var sum = middleLeft != middleRight ? results[middleLeft] + results[middleRight] : results[middleRight];
		while ((sum * new BigDecimal(100, 0, 1000000000)) < new BigDecimal(9999, 2, 1000000000))
		{
			middleLeft--;
			middleRight++;
			if (middleLeft >= 0)
				sum = sum + results[middleLeft];
			if (middleRight <= Possibilities)
				sum = sum + results[middleRight];
		}
		return sum;
	}
}

//Exemple of BigDecimal class - https://github.com/dparker1/BigDecimal/blob/
//3e0a4f1ba4c72c0b28d6571fcc6259558be104bd/BigDecimal/BigDecimal.cs
