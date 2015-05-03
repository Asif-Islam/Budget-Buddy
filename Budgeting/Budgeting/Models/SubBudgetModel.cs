using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Windows.UI.Xaml.Media;

namespace Budgeting.Models
{
    /* This is SubBudgetModel, in the second level of the model heirarchy
     * This Model contains basic information such as it's Name, Limit, Remainder (which equates to the Limit on instantiation)
     * Note that the colour scheme of the Sub Budget is refleced as bytes --> This is once more such that it can be JSON serialized 
     * as Windows.UI.Colors cannot be as easily serialized
     */ 
    [DataContract]
    public class SubBudgetModel
    {
        [DataMember]
        public string Name { get; set; }
        
        [DataMember]
        public bool LimitBool { get; set; }
        
        [DataMember]
        public double SBLimit { get; set; }

        [DataMember]
        public double Remainder { get; set; }

        [DataMember]
        public List<byte> chosencolorARGB { get; set;}
        
       
        public SubBudgetModel()
        {

        }

        public SubBudgetModel(string _Name, bool _LimitBool, double _SBLimit, double _Remainder, List<byte> _chosencolorARGB)
        {
            Name = _Name;
            LimitBool = _LimitBool;
            SBLimit = _SBLimit;
            Remainder = _Remainder;
            chosencolorARGB = _chosencolorARGB;
        }

 

    }
}
