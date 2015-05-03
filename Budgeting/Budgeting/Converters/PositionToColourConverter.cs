using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Budgeting.Converters
{
    //Returns the budgetPage button colour based on the position in MyBudgets
    class PositionToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((int)value % 2 == 0)
            {
                return new SolidColorBrush(ColorHelper.FromArgb(255, 31, 97, 143));
            }
            else
            {
                return new SolidColorBrush(ColorHelper.FromArgb(255, 13, 151, 144));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
