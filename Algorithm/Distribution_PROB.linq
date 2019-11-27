<Query Kind="Program" />

//https://exercicios.brasilescola.uol.com.br/exercicios-matematica/exercicios-sobre-probabilidade-condicional.htm#questao-1
void Main()
{
	var dice = 2; //Binomial distribution, dice = 2;
	var events = 10;
	var sampling = Math.Pow(dice, events);
	var cartesianProduct = dice.ToArrays(events).CartesianProduct();
	cartesianProduct.PrintGroup(events, dice);
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

		return result;
	}

	public static IEnumerable<List<int>> ToArrays(this int dice, int events)
	{
		var result = new List<List<int>>();
		for (int j = 1; j <= events; j++)
		{
			var array = new List<int>();
			for (int i = 1; i <= dice; i++)
				array.Add(i);
			
			result.Add(array);
		}

		return result;
	}
	
	public static void PrintGroup(this IEnumerable<IEnumerable<int>> list, int events, int dice)
	{
		var listCountDict = Enumerable.Range(1, dice * events).ToDictionary(x => x);
		Group(listCountDict, list);
		listCountDict.Dump("Values");
	}

	public static void Group(Dictionary<int, int> dict, IEnumerable<IEnumerable<int>> list)
	{
		foreach (var key in dict.Keys.ToList())
			dict[key] = 0;

		foreach (var item in list)
			dict[item.First()]++;

		var zeroKey = 0;
		foreach (var item in dict)
			if (item.Value == 0) 
				zeroKey = item.Key;
			else continue;

		for (int i = 1; i <= zeroKey; i++)
			dict.Remove(i);
	}
}
