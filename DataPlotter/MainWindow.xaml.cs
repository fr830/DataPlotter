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
using System.Threading;
using System.IO;
using OxyPlot;
using OxyPlot.Series;

namespace DataPlotter
{
    
    public class MainViewModel
    {
        public static Thread thread;
        private void updatePlot()
        {
            this.MyModel = new PlotModel { Title = "" };
            LineSeries series = new LineSeries();
            this.MyModel.Series.Add(series);
            bool first = true;
            while (true)
            {
                this.MyModel.InvalidatePlot(true);
                int i = 0;
                int length = 0;
                try
                {
                    using (var fs = new FileStream(System.IO.Directory.GetCurrentDirectory() + @"/data.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                    using (var reader = new StreamReader(fs))
                    {
                        while (!reader.EndOfStream)
                        {
                            length++;
                            var line = reader.ReadLine();
                        }
                    }
                    using (var fs = new FileStream(System.IO.Directory.GetCurrentDirectory() + @"/data.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                    using (var reader = new StreamReader(fs))
                    {
                        while (!reader.EndOfStream)
                        {
                            i++;
                            var line = reader.ReadLine();

                            if (!String.IsNullOrWhiteSpace(line) && i>length-10000)
                                try
                                {
                                    series.Points.Add(new DataPoint(i, Convert.ToDouble(line)));
                                    if (!first)
                                    {
                                        series.Points.RemoveAt(0);
                                    }
                                }
                                catch (FormatException)
                                {
                                    series.Points.Add(new DataPoint(i, 0));
                                    if (!first)
                                    {
                                        series.Points.RemoveAt(0);
                                    }
                                }
                        }
                    }
                }
                    catch (IOException)
                {
                    series.Points.Add(new DataPoint(i, 0));
                }
                Thread.Sleep(50);
                first = false;
            }
        }
        public MainViewModel()
        {
            thread = new Thread(new ThreadStart(updatePlot));
            thread.Start();
        }

        public PlotModel MyModel { get; private set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void WindowClose(object sender, EventArgs e)
        {
            MainViewModel.thread.Abort();
        }
    }
}
