using Budgeting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Budgeting.Converters
{
    //Converts the ARBG values to a solidcolorbrush
    public class ARGBToBrush : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                List<byte> chosenbytes = ((SubBudgetModel)value).chosencolorARGB;
                return new SolidColorBrush(ColorHelper.FromArgb(chosenbytes[0], chosenbytes[1], chosenbytes[2], chosenbytes[3]));
            }
            catch
            {
                return null;
            }
              
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
