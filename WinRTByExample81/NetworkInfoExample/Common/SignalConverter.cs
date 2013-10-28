using System;

namespace NetworkInfoExample.Common
{
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;

    public class SignalConverter : IValueConverter
    {
        private static readonly Symbol[] Symbols = {
                                                       Symbol.ZeroBars, Symbol.OneBar, Symbol.TwoBars,
                                                       Symbol.ThreeBars, Symbol.FourBars
                                                   };        
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            short signal;
            if (value == null)
            {
                signal = 0;
            }
            else if (!short.TryParse(value.ToString(), out signal))
            {
                signal = 0;
            }
            if (signal > 4)
            {
                signal = 4;                
            }
            return new SymbolIcon { Symbol = Symbols[signal] };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
