using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using LiveCharts.Wpf;
using MaterialDesignThemes.Wpf;

namespace Nebula
{
    public partial class MainWindow : Window
	{
		public class Processes
		{
			public string PID { get; set; }

			public string Process_Name { get; set; }

			public string RAMTaken { get; set; }
		}

		private string strData;

		private string str2data;

		public ObservableCollection<DataObject> listee;

		public string PID;

		public string Pname;

		public ObservableCollection<Processes> listp = new ObservableCollection<Processes>();

		public static long alpha = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory").Get().Cast<ManagementObject>().Sum((ManagementObject x) => Convert.ToInt64(x.Properties["Capacity"].Value)) / 1073741824;

        public MainWindow()
		{
			InitializeComponent();
			GaugeIOT.LabelFormatter = (double x) => x + "%";
			RAM.Text = alpha + " ";
			GaugeIOT2.ToValue = alpha;
			var obj = (from ManagementObject x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get()
				select x.GetPropertyValue("Caption")).FirstOrDefault();
			var text = ((obj != null) ? obj.ToString() : "Unknown");
			OSS_Copy1.Text = text.Split(' ')[0];
			OSS_Copy.Text = text.Replace(text.Split(' ')[0] + " ", "");
			OSS.Text = $"Version: {Environment.OSVersion.Version}";
			foreach (ManagementObject item in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor").Get())
			{
				Alfa.Text = item["Name"].ToString()!.Split(' ')[0];
				Alfa_Copy.Text = item["Name"].ToString()!.Replace(Alfa.Text, "");
			}
			if (Alfa.Text[0] != 'i' && Alfa.Text[0] != 'I')
			{
				intel.Visibility = Visibility.Hidden;
				amd.Visibility = Visibility.Visible;
			}
			PackIcon packIcon = osidek;
			packIcon.Kind = OSS_Copy.Text.Split(' ')[1][0] switch
			{
				'8' => PackIconKind.MicrosoftWindows, 
				'1' => PackIconKind.MicrosoftWindows, 
				_ => PackIconKind.MicrosoftWindowsClassic, 
			};
			GaugeIOT2.Sections = new List<AngularSection>
			{
				new AngularSection
				{
					FromValue = 0.0,
					ToValue = alpha,
					Fill = new LinearGradientBrush(new GradientStopCollection
					{
						new GradientStop
						{
							Offset = 0.0,
							Color = Color.FromRgb(199, 118, 221)
						},
						new GradientStop
						{
							Offset = 0.25,
							Color = Color.FromRgb(143, 85, 204)
						},
						new GradientStop
						{
							Offset = 0.5,
							Color = Color.FromRgb(101, 62, 178)
						},
						new GradientStop
						{
							Offset = 0.75,
							Color = Color.FromRgb(59, 41, 135)
						},
						new GradientStop
						{
							Offset = 1.0,
							Color = Color.FromRgb(18, 10, 99)
						}
					})
				}
			};
			DispatcherTimer dispatcherTimer = new DispatcherTimer();
			dispatcherTimer.Interval = TimeSpan.FromSeconds(0.3);
			dispatcherTimer.Tick += Timer_Tick;
			dispatcherTimer.Start();
		}

		private void CloseButtonClick(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void OnGridMouseDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void Timer_Tick(object sender, EventArgs eventArgs)
		{
			try
			{
				strData = new Consumption().Percentage.ToString();
				str2data = new Consumption(isCPU: false).Percentage.ToString();
				double value = Convert.ToDouble(strData);
				double value2 = Convert.ToDouble(str2data);
				GaugeIOT.Value = value;
				GaugeIOT2.Value = value2;
				Process[] prcs = Process.GetProcesses();
				Process[] array = prcs;
				foreach (Process process in array)
				{
					if (string.IsNullOrEmpty(process.MainWindowTitle))
					{
						continue;
					}
					if (listp.All(x => x.PID != process.Id.ToString()))
					{
						listp.Add(new Processes
						{
							PID = process.Id.ToString(),
							Process_Name = process.MainWindowTitle,
							RAMTaken = $"{process.WorkingSet64 / 1024:#,##0.##}"
						});
					}
					else if (listp.First((Processes x) => x.PID == process.Id.ToString()).Process_Name != process.MainWindowTitle)
					{
						listp.Remove(listp.First((Processes x) => x.PID == process.Id.ToString()));
						listp.Add(new Processes
						{
							PID = process.Id.ToString(),
							Process_Name = process.MainWindowTitle,
							RAMTaken = $"{process.WorkingSet64 / 1024:#,##0.##}"
						});
					}
				}
				if (listp.Any((Processes lel) => !prcs.Any((Process x) => x.Id.ToString() == lel.PID)))
				{
					listp.Remove(listp.First((Processes lel) => !prcs.Any((Process x) => x.Id.ToString() == lel.PID)));
				}
				DG1.ItemsSource = listp;
			}
			catch (Exception)
			{
			}
		}

		private void Made_Click_1(object sender, RoutedEventArgs e)
		{
			Process.Start("https://github.com/djthegr8");
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.MainWindow.WindowState = WindowState.Minimized;
		}

		private void DashButtonClick(object sender, RoutedEventArgs e)
		{
			ProcessPage.Visibility = Visibility.Collapsed;
			Dashboard.Visibility = Visibility.Visible;
		}

		private void ProcessButtonClick(object _, RoutedEventArgs __)
		{
			Dashboard.Visibility = Visibility.Collapsed;
			ProcessPage.Visibility = Visibility.Visible;
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			Processes processes = (Processes)((DataGrid)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget).SelectedCells[0].Item;
            var prc = Process.GetProcessById(int.Parse(processes.PID));
			prc.CloseMainWindow();
			prc.Close();
			listp.Remove(processes);
		}

		private void MenuItem_Click_1(object sender, RoutedEventArgs e)
		{
			try
			{
				listp.Clear();
				Timer_Tick(null, null);
			}
			catch
			{
			}
		}

    }
}
