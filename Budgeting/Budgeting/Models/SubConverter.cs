using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.UI.Xaml.Data;
using Budgeting.ViewModels;

namespace Budgeting.Models
{
    public class SubConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            List<SubBudgetViewModel> mysubs = (List<SubBudgetViewModel>)value;
            List<string> SubBudgetNames = new List<string>();
            foreach (object o in mysubs)
            {
                SubBudgetModel mysub = (SubBudgetModel)o;
                SubBudgetNames.Add(mysub.Name);

                return SubBudgetNames;
            }
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
