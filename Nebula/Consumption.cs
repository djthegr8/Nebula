using System.Diagnostics;
using System.Threading;

namespace Nebula
{
	internal class Consumption
	{
		public PerformanceCounter perfc = new PerformanceCounter("Processor", "% Processor Time", "_Total");

		public PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes", readOnly: true);

		public double Percentage { get; private set; }

		public Consumption(bool isCPU = true)
		{
			Percentage = CalculatePercentage(isCPU);
		}

		private double CalculatePercentage(bool isCPU = true)
		{
			if (isCPU)
			{
				perfc.NextValue();
				Thread.Sleep(300);
				return perfc.NextValue();
			}
			ramCounter.NextValue();
			Thread.Sleep(300);
			return (float)MainWindow.alpha - ramCounter.NextValue() / 1024f;
		}
	}
}
