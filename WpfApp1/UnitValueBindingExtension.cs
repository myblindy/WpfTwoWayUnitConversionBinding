using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;

namespace WpfApp1
{
    public class UnitValueBindingExtension : MarkupExtension
    {
        public PropertyPath Path { get; set; }
        public string Unit { get; set; } = "m";
        public BindingMode Mode { get; set; }
        public string StringFormat { get; set; }
        public UpdateSourceTrigger UpdateSourceTrigger { get; set; }

        internal string SourceUnit { get; set; }

        public UnitValueBindingExtension() { }

        public UnitValueBindingExtension(PropertyPath path) => Path = path;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding { Converter = new UnitValueBindingExtensionConverter(this), Path = Path, Mode = Mode, StringFormat = StringFormat, UpdateSourceTrigger = UpdateSourceTrigger };
            var bindingexpression = (BindingExpression)binding.ProvideValue(serviceProvider);

            var target = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            var context = ((FrameworkElement)target.TargetObject).DataContext;
            var srcPI = GetProperty(Path.Path, context);
            UnitValue srcObject = null;

            void contextChangeDelegate(object context, PropertyChangedEventArgs e)
            {
                if (e.PropertyName != srcPI.Name) return;

                if (!(srcObject is null)) srcObject.PropertyChanged -= sourceObjectChangeDelegate;

                srcObject = (UnitValue)srcPI.GetValue(context);
                if (!(srcObject is null)) { srcObject.PropertyChanged += sourceObjectChangeDelegate; sourceObjectChangeDelegate(srcObject, new PropertyChangedEventArgs("Unit")); }
            }
            ((INotifyPropertyChanged)context).PropertyChanged += contextChangeDelegate;
            contextChangeDelegate(context, new PropertyChangedEventArgs(srcPI.Name));

            void sourceObjectChangeDelegate(object srcObject, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "Unit") SourceUnit = ((UnitValue)srcObject).Unit;
                bindingexpression.UpdateTarget();
            }

            return bindingexpression;
        }

        public static PropertyInfo GetProperty(string PropertyPath, object obj)
        {
            if (string.IsNullOrEmpty(PropertyPath))
                return null;

            var SourceProperties = PropertyPath.Split('.');
            var PropertyType = obj.GetType();
            var PropertyInfo = PropertyType.GetProperty(SourceProperties[0]);
            PropertyType = PropertyInfo.PropertyType;
            for (var x = 1; x < SourceProperties.Length; ++x)
            {
                PropertyInfo = PropertyType.GetProperty(SourceProperties[x]);
                PropertyType = PropertyInfo.PropertyType;
            }
            return PropertyInfo;
        }
    }

    internal class UnitValueBindingExtensionConverter : IValueConverter
    {
        private readonly UnitValueBindingExtension UnitValueBindingExtension;

        public UnitValueBindingExtensionConverter(UnitValueBindingExtension unitValueBindingExtension) => UnitValueBindingExtension = unitValueBindingExtension;

        static readonly Dictionary<string, double> MeterMultiplierTable = new Dictionary<string, double>
        {
            ["cm"] = 100,
            ["mm"] = 1000,
            ["m"] = 1,
            ["km"] = 0.0001,
            ["NM"] = 0.000539957,
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return null;
            var unitvalue = (UnitValue)value;
            return System.Convert.ToDouble(unitvalue.Value) / MeterMultiplierTable[unitvalue.Unit] * MeterMultiplierTable[UnitValueBindingExtension.Unit];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value is null || string.IsNullOrWhiteSpace((string)value) ? null :
            new UnitValue
            {
                Unit = UnitValueBindingExtension.SourceUnit,
                Value = System.Convert.ToDouble(value) * MeterMultiplierTable[UnitValueBindingExtension.SourceUnit] / MeterMultiplierTable[UnitValueBindingExtension.Unit],
            };
    }
}
