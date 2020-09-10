using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System;
using LiveCharts;
using LiveCharts.Wpf;
using System.Net;
using System.IO;
using System.Windows.Threading;
using LiveCharts.Defaults;
using LiveCharts.Configurations;
using Microsoft;
using System.Windows.Media;
using System.Management;
using System.Linq;
using System.Windows.Controls;

namespace Nebula
{
    public partial class MainWindow : Window
    {
        private string strData;
        private string str2data;
        public static long alpha = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory").Get().Cast<ManagementObject>().Sum(x => Convert.ToInt64(x.Properties["Capacity"].Value))/(1024 * 1024 * 1024);
        public MainWindow()
        {
            InitializeComponent();
            GaugeIOT.LabelFormatter = (double x) => x.ToString() + '%';
            
            RAM.Text = alpha.ToString() + " ";
            GaugeIOT2.ToValue = alpha;
            var name = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                        select x.GetPropertyValue("Caption")).FirstOrDefault();
            var osn = name != null ? name.ToString() : "Unknown";
            OSS_Copy1.Text = osn.Split(' ')[0];
            OSS_Copy.Text = osn.Replace(osn.Split(' ')[0] + " ", "");
            OSS.Text = $"Version: {Environment.OSVersion.Version}";
            ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject mo in mos.Get())
            {
                Alfa.Text = (mo["Name"]).ToString().Split(' ')[0];
                Alfa_Copy.Text = (mo["Name"]).ToString().Replace(Alfa.Text, "");
            }
            if (Alfa.Text[0] != 'i' && Alfa.Text[0] != 'I')
            {
                intel.Visibility = Visibility.Hidden;
                amd.Visibility = Visibility.Visible;
            }
            osidek.Kind = (OSS_Copy.Text.Split(' ')[1][0]) switch
            {
                '8' => MaterialDesignThemes.Wpf.PackIconKind.MicrosoftWindows,
                '1' => MaterialDesignThemes.Wpf.PackIconKind.MicrosoftWindows,
                _ => MaterialDesignThemes.Wpf.PackIconKind.MicrosoftWindowsClassic,
            };
            GaugeIOT2.Sections = new List<AngularSection>
            {
                new AngularSection
                {
                    FromValue=0,
                    ToValue=Convert.ToInt32(alpha/3),
                    Fill = new SolidColorBrush(Color.FromRgb(187,134,252))
                },
                new AngularSection
                {
                    FromValue = Convert.ToInt32(alpha/3),
                    ToValue = 2 * Convert.ToInt32(alpha/3),
                    Fill = new SolidColorBrush(Color.FromRgb(55,0,179))
                },
                new AngularSection
                {
                    FromValue = 2 * Convert.ToInt32(alpha/3),
                    ToValue = alpha,
                    Fill = new SolidColorBrush(Color.FromRgb(207,102,121))
                }
            };
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.3)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnGridMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        void Timer_Tick(object sender, EventArgs eventArgs)
        {
            try
            {
                strData = new Consumption().Percentage.ToString();
                str2data = new Consumption(false).Percentage.ToString();
                var convertDouble = Convert.ToDouble(strData);
                var convertDouble2 = Convert.ToDouble(str2data);
                GaugeIOT.Value = convertDouble;
                GaugeIOT2.Value = convertDouble2;
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

        private void ProcessButtonClick(object sender, RoutedEventArgs e)
        {
            Dashboard.Visibility = Visibility.Collapsed;
            ProcessPage.Visibility = Visibility.Visible;
        }
    }
    internal class Consumption
    {
        public double Percentage { get; private set; }

        public PerformanceCounter perfc = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        public PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes", true);
        //public PerformanceCounter ramc;

        public Consumption(bool isCPU = true)
        {
            Percentage = CalculatePercentage(isCPU);
        }

        private double CalculatePercentage(bool isCPU = true)
        {
            if (isCPU)
            {
                perfc.NextValue();
                System.Threading.Thread.Sleep(300);
                return perfc.NextValue();
            }
            else
            {
                ramCounter.NextValue();
                System.Threading.Thread.Sleep(300);
                return MainWindow.alpha - ramCounter.NextValue() / 1024;
            }
        }
    }
}
