<Query Kind="Program" />

//https://exercicios.brasilescola.uol.com.br/exercicios-matematica/exercicios-sobre-probabilidade-condicional.htm#questao-1

void Main()
{
	var dice = 2; //Binomial distribution, dice = 2;
	var events = 10;
	var sampling = Math.Pow(dice, events);
	var cartesianProduct = dice.ToArrays(events).CartesianProduct();//.ToList();
	//cartesianProduct.ToList().ForEach(x => string.Join(",", x).Dump());
	Memory.PrintSum(events, dice, cartesianProduct);
}


public static class CartesianProductContainer
{
	public static IEnumerable<IEnumerable<int>> CartesianProduct(this IEnumerable<IEnumerable<int>> sequences)
	{
		IEnumerable<IEnumerable<int>> emptyProduct = new[] { Enumerable.Empty<int>() };
		var result = sequences.Aggregate(
			emptyProduct,
			(accumulator, sequence) =>
				from accseq in accumulator
				from item in sequence
				select new[] { accseq.Concat(new[] { item }).Sum() });
				//select accseq.Concat(new[] { item })); //print var cartesianProduct 

		//result.Dump();

		return result;
	}

	public static IEnumerable<List<int>> ToArrays(this int dice, int events)
	{
		var result = new List<List<int>>();
		for (int j = 1; j <= events; j++)
		{
			var array = new List<int>();
			for (int i = 1; i <= dice; i++)
			{
				array.Add(i);
			}
			result.Add(array);
		}

		return result;
	}
}

public class Memory
{
	public int Num { get; set; }
	public string Txt { get; set; }
	public static void PrintSum(int events, int dice, IEnumerable<IEnumerable<int>> list)
	{
		var listCountDict = Enumerable.Range(1, dice * events).ToDictionary(x => x);
		foreach (var key in listCountDict.Keys.ToList())
		{
			listCountDict[key] = 0;
		}

		foreach (var item in list)
		{
			listCountDict[item.First()]++;
		}

		var zeroKey = 0;
		foreach (var item in listCountDict)
		{
			if (item.Value == 0) zeroKey = item.Key;
			else continue;
		}

		for (int i = 1; i <= zeroKey; i++)
		{
			listCountDict.Remove(i);
		}

		var firstListCount = listCountDict.First().Key - 1;
		var phiRight = (int)(Math.Round((listCountDict.Count() / 1.618), 0) + firstListCount);
		var phiLeft = listCountDict.First(x => x.Value == listCountDict[phiRight]).Key;

		double phiSum = 0;
		for (int i = (int)phiLeft; i <= (int)phiRight; i++)
		{
			phiSum += listCountDict[i];
		}
		var sum = listCountDict.Sum(x => (double)x.Value);
		double percent = phiSum / sum;

		percent.Dump("Percent PHI");
		listCountDict.First().Key.Dump("Left Range key");
		listCountDict.Last().Key.Dump("Right Range key");
		phiLeft.Dump("Left Range 1,618 key");
		phiRight.Dump("Right Range 1,618 key");
		((listCountDict.Count() / 1.618 + firstListCount) % 1).Dump("Approximation");
		listCountDict.Dump("Values");

	}

}