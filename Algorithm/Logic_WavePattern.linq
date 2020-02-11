<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Windows.Forms.dll</Reference>
  <Namespace>System.Windows.Forms</Namespace>
  <Namespace>System.Drawing.Drawing2D</Namespace>
  <Namespace>System.Drawing</Namespace>
</Query>

//http://csharphelper.com/blog/2015/09/draw-a-simple-histogram-in-c/
//https://github.com/naudio/NAudio.WaveFormRenderer
//https://www.extendoffice.com/documents/excel/815-excel-remove-rows-based-on-cell-value.html
[STAThread]
void Main()
{
	Application.EnableVisualStyles();
	Application.Run(new MainForm());
}

public partial class MainForm : Form
{
	public MainForm()
	{
		InitializeComponent();
	}
	//###########################################################################
	private const int LENGHT = 15000;
	private const int GROUP = 10;
	private bool nestedHistogram = false;
	//###########################################################################
	private double m_dZoomscale = 1.0;
	public static double s_dScrollValue = .05;
	private Point MouseDownLocation;
	private Matrix transform = null;
	private NumbsOfCentralLimitTheorem.HistogramResult histogramResult = null;
	private bool printed = false;

	// Make random histogram data.
	private void MainForm_Load(object sender, EventArgs e)
	{
		histogramResult = GetHistogramOfCentralLimitTheorem(LENGHT, GROUP);

		// Make a transformation to the PictureBox.
		RectangleF data_bounds = new RectangleF(0, 0, histogramResult.Size, histogramResult.MaxValue * 2);
		PointF[] points =
		{
				new PointF(0, pictHistogram.ClientSize.Height),
				new PointF(pictHistogram.ClientSize.Width, pictHistogram.ClientSize.Height),
				new PointF(0, 0)
			};
		transform = new Matrix(data_bounds, points);
	}

	private void pictHistogram_Paint(object sender, PaintEventArgs e)
	{
		DrawHistogram(e.Graphics, pictHistogram.BackColor, histogramResult,
			pictHistogram.ClientSize.Width, pictHistogram.ClientSize.Height);
	}

	private void pictHistogram_Resize(object sender, EventArgs e)
	{
		pictHistogram.Refresh();
	}

	// Draw a histogram.
	private void DrawHistogram(Graphics gr, Color back_color,
		NumbsOfCentralLimitTheorem.HistogramResult histogramResult, int width, int height)
	{
		PrintResult();
		gr.Clear(back_color);
		gr.Transform = transform;
		gr.ScaleTransform((float)m_dZoomscale, (float)m_dZoomscale);
		FillRectangle(gr, Color.Black, histogramResult.Up, histogramResult.MaxValue, false);
		FillRectangle(gr, Color.Gray, histogramResult.Down, histogramResult.MaxValue, true);
	}
	
	private void PrintResult()
	{
		if (!printed)
		{
			printed = true;
			var listTuple = new List<(float x, float y, float z)>();
			float previousValueOfZ = 0;
			for (int i = 0; i < histogramResult.Up.Count(); i++)
			{
				if (histogramResult.Up[i] != 0.0001f && histogramResult.Down[i] != 0.0001f)
				{
					if (histogramResult.Up[i] % 1 == 0)
						previousValueOfZ = (int)(previousValueOfZ + 1f);
					else 
						previousValueOfZ += 0.1f;
					var tuple = (x: histogramResult.Up[i], y: histogramResult.Down[i], z: previousValueOfZ);
					listTuple.Add(tuple);
				}
			}
			Console.WriteLine("x,y,z");
			foreach (var tuple in listTuple)
				Console.WriteLine(tuple.x.ToString() + "," + tuple.y.ToString() + "," + tuple.z.ToString());
		}
	}

	protected void FillRectangle(Graphics gr, Color color, float[] arrayValues, float maxValue, bool down)
	{
		using (Pen thin_pen = new Pen(color, 0))
		{
			for (int i = 0; i < histogramResult.Down.Length; i++)
			{
				RectangleF rect;
				if (!down)
					rect = new RectangleF(i, maxValue, 1, arrayValues[i]);
				else
					rect = new RectangleF(i, maxValue - arrayValues[i], 1, arrayValues[i]);
				using (Brush the_brush = new SolidBrush(color))
				{
					gr.FillRectangle(the_brush, rect);
					gr.DrawRectangle(thin_pen, rect.X, rect.Y, rect.Width, rect.Height);
				}
			}
		}
	}

	protected void pictHistogram_OnMouseWheel(object sender, MouseEventArgs mea)
	{
		pictHistogram.Focus();
		if (pictHistogram.Focused == true && mea.Delta != 0)
			ZoomScroll(mea.Location, mea.Delta > 0);
	}

	private void ZoomScroll(Point location, bool zoomIn)
	{
		transform.Translate(-location.X, -location.Y);
		if (zoomIn)
			m_dZoomscale = m_dZoomscale + s_dScrollValue;
		else
			m_dZoomscale = m_dZoomscale - s_dScrollValue;
		transform.Translate(location.X, location.Y);
		pictHistogram.Invalidate();
	}

	private void pictHistogram_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button == System.Windows.Forms.MouseButtons.Left)
			MouseDownLocation = e.Location;
	}

	private void pictHistogram_MouseMove(object sender, MouseEventArgs e)
	{
		if (e.Button == System.Windows.Forms.MouseButtons.Left)
		{
			transform.Translate((e.Location.X - MouseDownLocation.X)
				/ 40, (e.Location.Y - MouseDownLocation.Y) / 40, MatrixOrder.Append);
			this.Refresh();
		}
	}

	private NumbsOfCentralLimitTheorem.HistogramResult GetHistogramOfCentralLimitTheorem(int length, int group)
	{
		var numbsOfCentralLimitTheorem = new NumbsOfCentralLimitTheorem();
		numbsOfCentralLimitTheorem.NestedHistogram = nestedHistogram;
		numbsOfCentralLimitTheorem.RandomResult(length);
		return numbsOfCentralLimitTheorem.GenerateHistogram(group);
	}
}

partial class MainForm
{
	private System.ComponentModel.IContainer components = null;

	protected override void Dispose(bool disposing)
	{
		if (disposing && (components != null))
			components.Dispose();
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.pictHistogram = new System.Windows.Forms.PictureBox();
		((System.ComponentModel.ISupportInitialize)(this.pictHistogram)).BeginInit();
		this.SuspendLayout();

		// pictHistogram
		this.pictHistogram.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top
					| System.Windows.Forms.AnchorStyles.Bottom)
					| System.Windows.Forms.AnchorStyles.Left)
					| System.Windows.Forms.AnchorStyles.Right)));
		this.pictHistogram.BackColor = System.Drawing.Color.White;
		this.pictHistogram.Cursor = System.Windows.Forms.Cursors.Cross;
		this.pictHistogram.Location = new System.Drawing.Point(8, 6);
		this.pictHistogram.Name = "pictHistogram";
		this.pictHistogram.Size = new System.Drawing.Size(550, 250);
		this.pictHistogram.TabIndex = 1;
		this.pictHistogram.TabStop = false;
		this.pictHistogram.Resize += new System.EventHandler(this.pictHistogram_Resize);
		this.pictHistogram.Paint += new System.Windows.Forms.PaintEventHandler(this.pictHistogram_Paint);
		this.pictHistogram.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictHistogram_OnMouseWheel);
		this.pictHistogram.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictHistogram_MouseDown);
		this.pictHistogram.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictHistogram_MouseMove);

		// MainForm
		this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.ClientSize = new System.Drawing.Size(563, 262);
		this.Controls.Add(this.pictHistogram);
		this.Name = "MainForm";
		this.Text = "Logic_WavePattern";
		this.Load += new System.EventHandler(this.MainForm_Load);
		((System.ComponentModel.ISupportInitialize)(this.pictHistogram)).EndInit();
		this.ResumeLayout(false);
	}

	internal System.Windows.Forms.PictureBox pictHistogram;
}

public class NumbsOfCentralLimitTheorem
{
	public float[] ResultList { get; set; }
	public int ResultLength { get; set; }
	public float[] LastList { get; set; }
	public float[] CurrentList { get; set; }
	public int SizeLastList { get; set; }
	public Dictionary<int, float> Histogram { get; set; }
	public bool NestedHistogram { get; set; }
	private int nestedCountDown = 2;

	public NumbsOfCentralLimitTheorem()
	{
		NestedHistogram = false;
		SizeLastList = 2;
		StartLastList();
		StartCurrentList();
	}

	public float[] RandomResult(int length)
	{
		ResultLength = length;
		ResultList = new float[length];
		Random rnd = new Random();
		for (int x = 0; x < length; x++)
		{
			float lineSum = 0;
			for (int i = 1; i < SizeLastList; i++)
			{
				var lastValueLeft = LastList[i - 1];
				var lastValueRight = LastList[i];
				var rndValue = (float)rnd.NextDouble(lastValueLeft, lastValueRight);
				lineSum = lineSum + (rndValue - lastValueLeft);
				CurrentList[i] = rndValue;
			}
			if (lineSum != 0)
				ResultList[x] = lineSum;
			SizeLastList++;
			LastList = CurrentList;
			StartCurrentList();
		}
		return ResultList;
	}

	public HistogramResult GenerateHistogram(int group)
	{
		Histogram = new Dictionary<int, float>();
		var minValue = ResultList.Min();
		var maxValue = ResultList.Max();
		var rangeValue = maxValue - minValue;
		var amountOfGroups = ResultLength / group;
		var intervalValue = rangeValue / amountOfGroups;
		foreach (var value in ResultList)
		{
			int key = (int)(value / intervalValue);
			if (!Histogram.ContainsKey(key))
				Histogram[key] = 0;
			Histogram[key]++;
		}
		var histogramResult = HistogramResult.Get(Histogram);
		if (NestedHistogram)
			printMaxInterval(histogramResult, Histogram, intervalValue, group);
		return histogramResult;
	}

	public Dictionary<int, float> GenerateHistogram(List<float> ResultList, KeyValuePair<int, float> keyValue, int group)
	{
		Histogram = new Dictionary<int, float>();
		var minValue = ResultList.Min();
		var maxValue = ResultList.Max();
		var rangeValue = maxValue - minValue;
		var amountOfGroups = ResultList.Count / group;
		var intervalValue = rangeValue / amountOfGroups;
		foreach (var value in ResultList)
		{
			int key = (int)(value / intervalValue);
			if (!Histogram.ContainsKey(key))
				Histogram[key] = keyValue.Value;
			Histogram[key]-= (1.0F / group);
		}
		return Histogram;
	}

	private void printMaxInterval(HistogramResult histogramResult, Dictionary<int, float> histogram, float intervalValue, int group)
	{
		var histogramOrdered = histogram.OrderBy(x => x.Key);
		var middle = histogram.Count / 2;
		var valueUp = histogramOrdered.ElementAt(middle - nestedCountDown);
		var valueDown = histogramOrdered.ElementAt(middle + nestedCountDown);
		var listUp = GenerateList(valueUp, intervalValue);
		var listDown = GenerateList(valueDown, intervalValue);
		var histogramUp = GenerateHistogram(listUp, valueUp, group);
		var histogramDown = GenerateHistogram(listDown, valueDown, group);
		listUp = histogramUp.Values.ToList();
		listDown = histogramDown.Values.ToList();
		var listCountMin = listUp.Count > listDown.Count ? listDown.Count : listUp.Count;
		histogramResult.Up = RefreshArray(histogramResult.Up, listUp, listCountMin);
		histogramResult.Down = RefreshArray(histogramResult.Down, listDown, listCountMin);
	}
	
	private float[] RefreshArray(float[] array, List<float> newItens, int listCountMin)
	{
		var newArray = new float[array.Count() + listCountMin];
		var rangeValueListMin = newArray.Count() - nestedCountDown - listCountMin;
		var rangeValueListMax = newArray.Count() - nestedCountDown;
		for (int i = 1; i <= nestedCountDown; i++)
			newArray[newArray.Count() - i] = array[array.Count() - i];
		for (int i = 0; i < newArray.Count() - nestedCountDown; i++)
		{
			if (i >= rangeValueListMin && i <= rangeValueListMax)
				newArray[i] = newItens[i - rangeValueListMin];
			else
				newArray[i] = array[i];
		}
		return (float[])newArray.Clone();
	}

	private List<float> GenerateList(KeyValuePair<int, float> keyValue, float intervalValue)
	{
		var minValueInterval = keyValue.Key * intervalValue;
		var maxValueInterval = minValueInterval + intervalValue;
		var internalList = new List<float>();
		foreach (var value in ResultList)
		{
			if (value >= minValueInterval && value <= maxValueInterval)
				internalList.Add(value);
		}
		return internalList;
	}

	private void StartCurrentList()
	{
		var sizeCurrentList = SizeLastList + 1;
		CurrentList = new float[sizeCurrentList];
		CurrentList[0] = 0;
		CurrentList[sizeCurrentList - 1] = float.MaxValue / 2;
	}

	private void StartLastList()
	{
		LastList = new float[SizeLastList];
		LastList[0] = 0;
		LastList[SizeLastList - 1] = float.MaxValue / 2;
	}

	public class HistogramResult
	{
		public int Size { get; set; }
		public float MaxValue { get; set; }
		public float[] Up { get; set; }
		public float[] Down { get; set; }

		public static HistogramResult Get(Dictionary<int, float> histogram)
		{
			var histogramOrdered = histogram.OrderBy(k => k.Key);
			var result = new HistogramResult();
			var lengthOdd = histogram.Count % 2 > 0;
			var middle = histogram.Count / 2;
			var middleValue = histogramOrdered.ElementAt(middle).Key;
			result.Size = middleValue;
			result.MaxValue = histogram.OrderBy(k => k.Value).Last().Value;
			result.Up = ArrangeArray(new float[middleValue]);
			result.Down = ArrangeArray(new float[middleValue]);
			for (int i = 0; i < middle; i++)
			{
				var keyValue = histogramOrdered.ElementAt(i);
				result.Up[keyValue.Key] = keyValue.Value;
			}
			for (int i = lengthOdd ? middle + 2 : middle + 1; i < histogram.Count; i++)
			{
				var totalValue = middleValue * 2;
				var keyValue = histogramOrdered.ElementAt(i);
				result.Down[totalValue - keyValue.Key] = keyValue.Value;
			}
			return result;
		}

		private static float[] ArrangeArray(float[] array)
		{
			for (int i = 0; i < array.Length; i++)
				array[i] = 0.0001F;
			return array;
		}
	}
}

public static class rndExtension
{
	public static double NextDouble(this Random rng, double minimum, double maximum)
	{
		return rng.NextDouble() * (maximum - minimum) + minimum;
	}
}