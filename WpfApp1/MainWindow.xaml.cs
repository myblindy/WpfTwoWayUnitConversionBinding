using ReactiveUI;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ((VMType)DataContext).Length = new UnitValue { Value = 50.12, Unit = "cm", Type = MetricType.Distance };
            ((VMType)DataContext).Altitude = new UnitValue { Value = 2250.12, Unit = "cm", Type = MetricType.Altitude };
        }
    }

    class VMType : ReactiveObject
    {
        UnitValue length;
        public UnitValue Length { get => length; set => this.RaiseAndSetIfChanged(ref length, value); }
        UnitValue altitude;
        public UnitValue Altitude { get => altitude; set => this.RaiseAndSetIfChanged(ref altitude, value); }
    }
}
