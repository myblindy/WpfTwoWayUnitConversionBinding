using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{

    public class UnitValue : ReactiveObject
    {
        double value;
        public double Value { get => value; set => this.RaiseAndSetIfChanged(ref this.value, value); }
        string unit;
        public string Unit { get => unit; set => this.RaiseAndSetIfChanged(ref unit, value); }
        MetricType type;
        public MetricType Type { get => type; set => this.RaiseAndSetIfChanged(ref type, value); }
    }

    public enum MetricType
    {
        Altitude,
        Distance,
    }

    public class MetricSettings : ReactiveObject
    {
        private MetricSettings()
        {
            Units[MetricType.Distance] = this.WhenAny(x => x.MetricDistances, w => w.Value ? "m" : "ft").ToProperty(this, x => x.DistanceUnit, out distanceUnit);
            Units[MetricType.Altitude] = this.WhenAny(x => x.MetricAltitudes, w => w.Value ? "m" : "ft").ToProperty(this, x => x.AltitudeUnit, out altitudeUnit);
        }
        public static MetricSettings Instance { get; } = new MetricSettings();

        bool metricDistances, metricAltitudes;
        public bool MetricDistances { get => metricDistances; set => this.RaiseAndSetIfChanged(ref metricDistances, value); }
        public bool MetricAltitudes { get => metricAltitudes; set => this.RaiseAndSetIfChanged(ref metricAltitudes, value); }

        readonly ObservableAsPropertyHelper<string> distanceUnit;
        public string DistanceUnit => distanceUnit.Value;

        readonly ObservableAsPropertyHelper<string> altitudeUnit;
        public string AltitudeUnit => altitudeUnit.Value;

        public Dictionary<MetricType, ObservableAsPropertyHelper<string>> Units = new Dictionary<MetricType, ObservableAsPropertyHelper<string>>();
    }
}
