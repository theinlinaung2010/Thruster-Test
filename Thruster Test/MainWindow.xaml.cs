using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using LiveCharts.Geared;

namespace Thruster_Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        Bridge theBridge = new Bridge();
        CommandParser cp = new CommandParser();
        CommandInterpreter ci;

        SeriesCollection mSeriesCollection = new SeriesCollection();
        GLineSeries voltageSeries, currentSeries, thrustSeries, rpmSeries, throttleSeries;

        List<double> times, voltages, currents, thrusts, rpms, throttles;

        SaveLoadHandler logger = new SaveLoadHandler();

        bool isConnected = false;
        bool isRunning = false;
        double time = 0;

        List<string> commandLines;

        MaterialDesignThemes.Wpf.SnackbarMessageQueue sq = new
                    MaterialDesignThemes.Wpf.SnackbarMessageQueue(new TimeSpan(0, 0, 0, 0, 1000));

        public MainWindow()
        {
            SetupDataSeries();
            InitializeComponent();
            theBridge.TimeData += theBridge_TimeData;
            theBridge.ThrustData += theBridge_ThrustData;
            theBridge.VoltageData += theBridge_VoltageData;
            theBridge.CurrentData += theBridge_CurrentData;
            theBridge.RpmData += theBridge_RPMData;
            theBridge.ThrottleData += theBridge_ThrottleData;
            mChart.Series = mSeriesCollection;
            mSnackBar.MessageQueue = sq;

        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning == false)
            {
                StoreCommandLines();
                BtnStart.Content = FindResource("StopIcon");
                isRunning = true;
                BtnData.IsEnabled = false;
            }
            else
            {
                ci.Stop();
                BtnStart.Content = FindResource("StartIcon");
                theBridge.SendMotorSpeed(0);
                theBridge.SendStop();
                isRunning = false;
                BtnData.IsEnabled = true;
                return;
            }

            //Renew lists
            times = new List<double>();
            voltages = new List<double>();
            currents = new List<double>();
            thrusts = new List<double>();
            rpms = new List<double>();
            throttles = new List<double>();

            //Clear chart area
            voltageSeries.Values.Clear();
            currentSeries.Values.Clear();
            thrustSeries.Values.Clear();
            rpmSeries.Values.Clear();
            throttleSeries.Values.Clear();

            mChart.AxisX[0].MinValue = 0;
            mChart.AxisX[0].MaxValue = 15;

            //Connect to Arduino if not connected
            //Retrive the commands from parser
            //Send commands to Arduino

            cp.InputString = CmdTB.Text;
            if (cp.ParseInputString() == true)
            {
                if (isConnected)
                {
                    theBridge.SendStart();
                    ExecuteNextCommand();
                }
                else
                {
                    //Connect to Arduino
                    if (theBridge.Initialize() == true)
                    {
                        theBridge.SendStart();
                        //SetupDataSeries();
                        ExecuteNextCommand();
                    }
                    else
                    {
                        BtnStart.Content = FindResource("StartIcon");
                        isRunning = false;
                        BtnData.IsEnabled = true;
                        sq.Enqueue("Failed to connect");
                    }
                }
            }
            else
            {
                BtnStart.Content = FindResource("StartIcon");
                isRunning = false;
                BtnData.IsEnabled = true;
                int errorLineNumber = cp.GetErrorLineNumber();
                sq.Enqueue("Error interpreting line : " + errorLineNumber);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ci = new CommandInterpreter(theBridge);
            ci.CommandFinished += ci_CommandFinished;
            if (theBridge.Initialize() == true)
            {
                isConnected = true;
                sq.Enqueue("Connection established");
            }
            else
            {
                sq.Enqueue("Failed to connect");
            };
        }

        private void StoreCommandLines()
        {
            commandLines = new List<string>();
            commandLines.Add(""); //Empty string to account for early removal
            char[] inputChars = CmdTB.Text.ToArray();
            string s = "";

            for (int i = 0; i < inputChars.Length; i++)
            {
                if (inputChars[i] == '\r')
                {
                    commandLines.Add(s);
                    s = "";
                }
                else if (inputChars[i] != '\n')
                {
                    s += inputChars[i];
                }
            }
            commandLines.Add(s);
            s = "";
        }

        private void theBridge_TimeData(uint t)
        {
            time = Math.Round((t / 1000.0), 2);
            times.Add(time);
            if (time > 10 && time % 5 < 0.1)
            {
                this.Dispatcher.Invoke((Action)(() =>
            {
                mChart.AxisX[0].MinValue = (int)(time) - 4;
                mChart.AxisX[0].MaxValue = (int)(time) + 5;
            }));
            }
        }

        private void theBridge_ThrustData(double e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                double value = Math.Round(e, 2);
                thrustSeries.Values.Add(new ObservablePoint(time, value));
                TBThrust.Text = value.ToString();
                thrusts.Add(value);
            }));
        }

        private void theBridge_VoltageData(double e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                double value = Math.Round(e, 2);
                voltageSeries.Values.Add(new ObservablePoint(time, value));
                TBVoltage.Text = value.ToString();
                voltages.Add(value);
            }));
        }

        private void theBridge_CurrentData(double e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                double value = Math.Round(e, 2);
                currentSeries.Values.Add(new ObservablePoint(time, value));
                TBCurrent.Text = value.ToString();
                currents.Add(value);
            }));
        }

        private void theBridge_RPMData(double e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                double value = Math.Round(e, 2);
                rpmSeries.Values.Add(new ObservablePoint(time, value));
                TBrpm.Text = value.ToString();
                rpms.Add(value);
            }));
        }

        private void theBridge_ThrottleData(double e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                double value = Math.Round(e, 2);
                throttleSeries.Values.Add(new ObservablePoint(time, value));
                TBThrottle.Text = value.ToString();
                throttles.Add(value);
            }));
        }

        private void ExecuteNextCommand()
        {
            string nextCommand;
            List<int> paras;
            if (cp.NextCommand(out nextCommand, out paras) == true)
            {
                ci.NextCommand(nextCommand, paras);

                //Remove next line from Textbox
                this.Dispatcher.Invoke((Action)(() =>
                {
                    commandLines.RemoveAt(0);
                    CmdTB.Text = "";
                    for (int i = 0; i < commandLines.Count; i++)
                    {
                        CmdTB.Text += commandLines[i];
                        if (commandLines.Count - i != 1)
                        {
                            CmdTB.Text += "\r";
                        }
                    }
                }));
            }
            else
            {
                //Finished all commands
                System.Threading.Thread.Sleep(110);
                theBridge.SendMotorSpeed(0);                
                theBridge.SendStop();
                this.Dispatcher.Invoke((Action)(() =>
                {
                    BtnStart.Content = FindResource("StartIcon");
                    BtnData.IsEnabled = true;
                    CmdTB.Text = "";
                    logger.Save(TBNotes.Text, times.ToArray(), throttles.ToArray(), thrusts.ToArray(),
                                currents.ToArray(), rpms.ToArray(), voltages.ToArray());
                }));
                isRunning = false;
                sq.Enqueue("Finished all commands");
            }
        }

        private void ci_CommandFinished()
        {
            ExecuteNextCommand();
        }

        void SetupDataSeries()
        {
            //current voltage throttle thrust - Y axis indexes
            currentSeries = new GLineSeries
            {
                Values = new GearedValues<ObservablePoint> { },
                Title = "Current",
                LabelPoint = point => point.Y + "A",
                PointGeometry = DefaultGeometries.None,
                Stroke = (Brush)FindResource("CurrentBrush"),
                Fill = Brushes.Transparent,
                ScalesYAt = 0,
            };
            voltageSeries = new GLineSeries
            {
                Values = new GearedValues<ObservablePoint> { },
                Title = "Voltage",
                LabelPoint = point => point.Y + "V",
                PointGeometry = DefaultGeometries.None,
                Stroke = (Brush)FindResource("VoltageBrush"),
                Fill = Brushes.Transparent,
                ScalesYAt = 1,
            };
            throttleSeries = new GLineSeries
            {
                Values = new GearedValues<ObservablePoint> { },
                Title = "Throttle",
                LabelPoint = point => point.Y + "%",
                PointGeometry = DefaultGeometries.None,
                Stroke = (Brush)FindResource("ThrottleBrush"),
                Fill = Brushes.Transparent,
                ScalesYAt = 2,
                LineSmoothness = 0.1,
            };
            thrustSeries = new GLineSeries
            {
                Values = new GearedValues<ObservablePoint> { },
                Title = "Thrust",
                LabelPoint = point => point.Y + "kg",
                PointGeometry = DefaultGeometries.None,
                Stroke = (Brush)FindResource("ThrustBrush"),
                Fill = Brushes.Transparent,
                ScalesYAt = 3,
            };
            rpmSeries = new GLineSeries
            {
                Values = new GearedValues<ObservablePoint> { },
                Title = "Thrust",
                LabelPoint = point => point.Y + "kg",
                PointGeometry = DefaultGeometries.None,
                Stroke = (Brush)FindResource("RpmBrush"),
                Fill = Brushes.Transparent,
                ScalesYAt = 3,
            };
            mSeriesCollection.Add(voltageSeries);
            mSeriesCollection.Add(currentSeries);
            mSeriesCollection.Add(thrustSeries);
            mSeriesCollection.Add(throttleSeries);
            mSeriesCollection.Add(rpmSeries);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                theBridge.SendStop();
                theBridge.Exit();
            }
            catch
            {
            }
        }

        private void CBThrottle_Click(object sender, RoutedEventArgs e)
        {
            if (CBThrottle.IsChecked == true && CBThrottle.IsInitialized)
            {
                throttleSeries.Visibility = Visibility.Visible;
            }
            else
            {
                throttleSeries.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void BtnData_Click(object sender, RoutedEventArgs e)
        {
            DataAnalysisWindow dataWindow = new DataAnalysisWindow();
            dataWindow.SetData(times, voltages, currents, thrusts, rpms, throttles, TBNotes.Text);
            try
            {
                dataWindow.ShowDialog();
            }
            catch
            {
                dataWindow.Close();
            }
        }

        private void CBThrust_Checked(object sender, RoutedEventArgs e)
        {
            thrustSeries.Visibility = Visibility.Visible;
        }

        private void CBThrust_Unchecked(object sender, RoutedEventArgs e)
        {
            thrustSeries.Visibility = Visibility.Hidden;
        }

        private void CBCurrent_Checked(object sender, RoutedEventArgs e)
        {
            currentSeries.Visibility = Visibility.Visible;
        }

        private void CBrpm_Checked(object sender, RoutedEventArgs e)
        {
            rpmSeries.Visibility = Visibility.Visible;
        }

        private void CBVoltage_Checked(object sender, RoutedEventArgs e)
        {
            voltageSeries.Visibility = Visibility.Visible;
        }

        private void CBCurrent_Unchecked(object sender, RoutedEventArgs e)
        {
            currentSeries.Visibility = Visibility.Hidden;
        }

        private void CBrpm_Unchecked(object sender, RoutedEventArgs e)
        {
            rpmSeries.Visibility = Visibility.Hidden;
        }

        private void CBVoltage_Unchecked(object sender, RoutedEventArgs e)
        {
            voltageSeries.Visibility = Visibility.Hidden;
        }
    }
}
