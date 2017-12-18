using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Thruster_Test
{
    class SaveLoadHandler
    {
        string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Thruster Test Logs\\";

        public SaveLoadHandler()
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }

        public void Save(string comment, double[] time, double[] throttle, double[] thrust,
                         double[] current, double[] rpm, double[] voltage)
        {
            string path = folderPath + DateTime.Now.Month + "." + DateTime.Now.Day + "." + DateTime.Now.Year.ToString().Substring(2, 2) + " " +
                        DateTime.Now.Hour + "." + DateTime.Now.Minute + "." + DateTime.Now.Second + ".txt";
            try
            {
                var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    streamWriter.WriteLine(@"#THRUSTER TEST DATA LOG FILE. DO NOT EDIT#");
                    streamWriter.WriteLine(System.DateTime.Now.ToLongDateString());
                    streamWriter.WriteLine(System.DateTime.Now.ToLongTimeString());
                    streamWriter.WriteLine(comment);
                    streamWriter.WriteLine("#\tTime(s)\tThrottle(%)\tThrust(kg)\tCurrent(A)\tRPM\tVoltage(V)");
                    for (int i = 0; i < time.Count(); i++)
                    {
                        streamWriter.WriteLine((i + 1).ToString() + '\t' + time[i] + '\t' + throttle[i] + '\t' + thrust[i] + '\t'
                                                + current[i] + '\t' + "0" + '\t' + voltage[i]);
                    }
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("Error saving the log file.\nPlease make sure the file is not opened.", "Error",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
        }

        public Tuple<string, List<double>, List<double>, List<double>, List<double>, List<double>, List<double>> Load()
        {
            try
            {
                // Create OpenFileDialog 
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                // Set filter for file extension and default file extension 
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Data Log Files (*.txt)|*.txt";

                // Display OpenFileDialog by calling ShowDialog method 
                Nullable<bool> result = dlg.ShowDialog();
                string filename;

                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    filename = dlg.FileName;
                }
                else
                {
                    return null;
                }

                //Check if header is correct
                var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string header = streamReader.ReadLine();
                    if (header != @"#THRUSTER TEST DATA LOG FILE. DO NOT EDIT#")
                    {                        
                        return null;
                    }
                    string date = streamReader.ReadLine();
                    string time = streamReader.ReadLine();
                    string comment = streamReader.ReadLine();
                    List<double> times = new List<double>();
                    List<double> throttle = new List<double>();
                    List<double> thrust = new List<double>();
                    List<double> current = new List<double>();
                    List<double> rpm = new List<double>();
                    List<double> voltage = new List<double>();

                    string line = streamReader.ReadLine();                    
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        // process the line
                        try
                        {
                            string[] values = line.Split('\t');
                            times.Add(double.Parse(values[1]));
                            throttle.Add(double.Parse(values[2]));
                            thrust.Add(double.Parse(values[3]));
                            current.Add(double.Parse(values[4]));
                            rpm.Add(double.Parse(values[5]));
                            voltage.Add(double.Parse(values[6]));
                        }
                        catch
                        {
                            break;
                        }
                    }
                    return Tuple.Create(comment, times, throttle, thrust, current, rpm, voltage);
                }
            }
            catch
            {
                Console.WriteLine("Error loading file");
                return null;
            }
        }
    }
}
