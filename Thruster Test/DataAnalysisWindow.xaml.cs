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
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using LiveCharts.Geared;

namespace Thruster_Test
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DataAnalysisWindow : Window
    {
        List<string> cbItems = new List<string> { "Thrust", "Throttle", "Voltage", "Current", "RPM", "Time" };

        SeriesCollection mSeriesCollection = new SeriesCollection();
        GScatterSeries mGScatterSeries;
        GColumnSeries mGColumnSeries;

        List<double> times, voltages, currents, thrusts, rpms, throttles;
        int chartType = 0; //0=scatterPlot, 1=histrogram

        public DataAnalysisWindow()
        {
            InitializeComponent();
            SetupPlot();
            RefreshData();
        }

        public void SetData(List<double> _times, List<double> _voltages, List<double> _currents,
                            List<double> _thrusts, List<double> _rpms, List<double> _throttles, string notes)
        {
            times = _times;
            voltages = _voltages;
            currents = _currents;
            thrusts = _thrusts;
            rpms = _rpms;
            throttles = _throttles;
            if (notes == "")
            {
                TBNotes.Text = "No notes to show!\nLazy guys ;)";
            }
            else
            {
                TBNotes.Text = notes;
            }
        }

        void ComputeStatistics(List<double> series)
        {
            int samples = series.Count;
            double average = Math.Round(series.Average(), 2);
            double max = series.Max();
            double min = series.Min();
            double range = Math.Round(max - min, 2);
            double squaredSum = 0;
            for (int i = 0; i < samples; i++)
            {
                squaredSum += Math.Pow(average - series[i], 2);
            }
            double rms = Math.Round(Math.Sqrt(squaredSum / samples), 2);

            LBLSamples.Content = samples.ToString();
            LblAvg.Content = average.ToString();
            LblMax.Content = max.ToString();
            LblMin.Content = min.ToString();
            LblRange.Content = range.ToString();
            LblSD.Content = rms.ToString();
        }

        void ResetMinMaxTBs(List<double> series)
        {
            TBStart.Text = series.Min().ToString();
            TBEnd.Text = series.Max().ToString();
        }

        void ShowPlot(List<double> seriesX, List<double> seriesY)
        {
            if (chartType == 0)
            {
                mGScatterSeries.Values.Clear();
                for (int i = 0; i < seriesX.Count; i++)
                {
                    mGScatterSeries.Values.Add(new ObservablePoint(seriesX[i], seriesY[i]));
                }
            }
            else
            {
                mGColumnSeries.Values.Clear();

                int[] freqDistribution = new int[10];
                List<double> classes = new List<double>();
                double classInterval;
                if (seriesY.Max() <= 1)
                {
                    classInterval = Math.Round(((seriesY.Max() - seriesY.Min())/9.0), 2);
                }
                else
                {
                    classInterval = (int)((seriesY.Max() - seriesY.Min()) / 9);
                }
  
                for (int i = 0; i < 10; i++)
                {
                    classes.Add(seriesY.Min() + classInterval * i);
                }
                
                for (int i = 0; i < seriesY.Count; i++)
                {
                    for (int j = 0; j < classes.Count; j++)
                    {
                        if (Math.Abs(seriesY[i] - classes[j]) < classInterval)
                        {
                            freqDistribution[j] += 1;
                            break;
                        }
                    }
                }

                for (int i = 0; i < freqDistribution.Length; i++)
                {
                    mGColumnSeries.Values.Add(new ObservablePoint(classes[i], freqDistribution[i]));
                }
            }
        }

        List<double> AssociateListFromName(string name)
        {
            name = name.ToLower();
            switch (name)
            {
                case "time":
                    return times;
                case "throttle":
                    return throttles;
                case "thrust":
                    return thrusts;
                case "voltage":
                    return voltages;
                case "current":
                    return currents;
                case "rpm":
                    return rpms;
                default:
                    return null;
            }
        }

        void AssociateTitlesFromName(string name)
        {
            name = name.ToLower();
            switch (name)
            {
                case "throttle":
                    mGScatterSeries.Title = "Throttle";
                    mGScatterSeries.LabelPoint = point => point.Y + "%";
                    break;
                case "thrust":
                    mGScatterSeries.Title = "Thrust";
                    mGScatterSeries.LabelPoint = point => point.Y + "kg";
                    break;
                case "voltage":
                    mGScatterSeries.Title = "Voltage";
                    mGScatterSeries.LabelPoint = point => point.Y + "V";
                    break;
                case "current":
                    mGScatterSeries.Title = "Current";
                    mGScatterSeries.LabelPoint = point => point.Y + "A";
                    break;
                case "rpm":
                    mGScatterSeries.Title = "RPM";
                    mGScatterSeries.LabelPoint = point => point.Y + "rpm";
                    break;
                case "time":
                    mGScatterSeries.Title = "Time";
                    mGScatterSeries.LabelPoint = point => point.Y + "s";
                    break;
                default:
                    break;
            }
        }

        void SetupPlot()
        {            
            mGScatterSeries = new GScatterSeries
            {
                Values = new GearedValues<ObservablePoint> { },
                PointGeometry = DefaultGeometries.Circle,
                MaxPointShapeDiameter = 7,
            };
            mGColumnSeries = new GColumnSeries
            {
                Title = "",
                Values = new GearedValues<ObservablePoint> { },
                LabelPoint = point => point.Y.ToString(),
                MaxColumnWidth = double.PositiveInfinity,
                ColumnPadding = 0,
            };
            mSeriesCollection.Add(mGScatterSeries);
            mChart.Series = mSeriesCollection;
        }

        private void CBX_DropDownClosed(object sender, EventArgs e)
        {
            ComboBoxItem selectedItemX = (ComboBoxItem)CBX.SelectedItem;
            string selectedContentX = (string)selectedItemX.Content;
            List<double> seriesX = AssociateListFromName(selectedContentX);
            if (seriesX.Count > 0)
            {
                TBStart.Text = seriesX.Min().ToString();
                TBEnd.Text = seriesX.Max().ToString();
                RefreshData();
            }
        }

        private void CBY_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshData();
        }

        private void CBX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> currentCbItems = new List<string>();
            for (int i = 0; i < cbItems.Count; i++)
            {
                currentCbItems.Add(cbItems[i]);
            }
            ComboBoxItem selectedItem = (ComboBoxItem)CBX.SelectedItem;
            string selectedContent = (string)selectedItem.Content;
            currentCbItems.Remove(selectedContent);

            if (CBY != null)
            {
                CBY.Items.Clear();
                for (int i = 0; i < currentCbItems.Count; i++)
                {
                    CBY.Items.Add(currentCbItems[i]);
                }
                CBY.SelectedIndex = 0;
                RefreshData();
            }
        }

        private void FindSeriesXandY(out List<double> outSeriesX, out List<double> outSeriesY)
        {
            outSeriesX = null;
            outSeriesY = null;

            ComboBoxItem selectedItemX = (ComboBoxItem)CBX.SelectedItem;
            string selectedContentX = (string)selectedItemX.Content;
            string selectedContentY = (string)CBY.SelectedItem;

            if (selectedContentX == null || selectedContentY == null)
            {
                return;
            }

            List<double> seriesX = AssociateListFromName(selectedContentX);
            List<double> seriesY = AssociateListFromName(selectedContentY);
            if (seriesX == null || seriesY == null)
            {
                return;
            }

            AssociateTitlesFromName(selectedContentY);

            try
            {
                double min = double.Parse(TBStart.Text);
                double max = double.Parse(TBEnd.Text);

                List<int> dataIndexi = new List<int>();
                for (int i = 0; i < seriesX.Count; i++)
                {
                    if (seriesX[i] >= min && seriesX[i] <= max)
                    {
                        dataIndexi.Add(i);
                    }
                }

                outSeriesX = new List<double>();
                outSeriesY = new List<double>();

                for (int i = 0; i < dataIndexi.Count; i++)
                {
                    outSeriesX.Add(seriesX[dataIndexi[i]]);
                    outSeriesY.Add(seriesY[dataIndexi[i]]);
                }
            }
            catch
            {
                return;
            }
        }

        private void RefreshData()
        {
            List<double> dataX, dataY;
            FindSeriesXandY(out dataX, out dataY);
            if (dataX != null && dataY != null && dataX.Count > 0 && dataY.Count > 0)
            {
                ComputeStatistics(dataY);
                ShowPlot(dataX, dataY);
            }
            else
            {
                ClearStatistics();
            }
        }

        private void ClearStatistics()
        {
            try
            {
                LBLSamples.Content = 0;
                LblAvg.Content = 0;
                LblMin.Content = 0;
                LblMax.Content = 0;
                LblRange.Content = 0;
                LblSD.Content = 0;
            }
            catch
            {
                //Handles for startup loading
            }
        }

        private void CBY_DropDownClosed(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CBX_SelectionChanged(this, null);
            ComboBoxItem selectedItemX = (ComboBoxItem)CBX.SelectedItem;
            string selectedContentX = (string)selectedItemX.Content;
            List<double> seriesX = AssociateListFromName(selectedContentX);
            if(seriesX != null){
                TBStart.Text = seriesX.Min().ToString();
                TBEnd.Text = seriesX.Max().ToString();
            }
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TBStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshData();
        }

        private void TBEnd_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshData();
        }

        private void CBChartType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CBChartType.SelectedIndex == 0)
            {
                mSeriesCollection.Clear();
                if (mGScatterSeries != null)
                {
                    mSeriesCollection.Add(mGScatterSeries);
                }
            }
            else
            {
                mSeriesCollection.Clear();
                if (mGColumnSeries != null)
                {
                    mSeriesCollection.Add(mGColumnSeries);
                }
            }
            chartType = CBChartType.SelectedIndex;
            RefreshData();
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            SaveLoadHandler loader = new SaveLoadHandler();
            Tuple<string, List<double>, List<double>, List<double>, List<double>, List<double>, List<double>> data = loader.Load();
            if (data != null)
            {
                SetData(data.Item2, data.Item7, data.Item5, data.Item4, data.Item6, data.Item3, data.Item1);
                //RefreshData();
            }
        }
    }
}
