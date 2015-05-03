using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Windows.UI.Xaml.Media.Imaging;

namespace Budgeting.Models
{
    /*This is the model for an Event in the third level of the model heirachy: The Event Model contains the information for the user's events
     * This includes the name, sub-budget, location, price, paymethod, taglist, imagefilepath (for the icon) and photonames
     * It is important to note that the EventModel class only saves the name of the photos that users take with their camera - this is because strings
     * are naturally serializable for JSON and can easily be accessible by searching the filename when opening the localfolder
     */
    [DataContract]
    public class EventModel
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public SubBudgetModel SubBudgetLocation { get; set; }

        [DataMember]
        public string SubBudgetName { get; set; }

        [DataMember]
        public string EventLocation { get; set; }

        [DataMember]
        public double Price { get; set; }

        [DataMember]
        public string Pricestring { get; set; }

        [DataMember]
        public bool isDeducted { get; set; }

        [DataMember]
        public string PayMethod { get; set; }

        [DataMember]
        public List<string> Tags { get; set; }

        [DataMember]
        public DateTimeOffset EventDate { get; set; }

        [DataMember]
        public List<string> photonames {get;set;}

        [DataMember]
        public string imagefilepath { get; set; }




        public EventModel()
        {

        }

        public EventModel(string _Name, SubBudgetModel _SubBudgetLocation, string _SubBudgetName, string _EventLocation, double _Price, string _Pricestring, bool _isDeducted, string _PayMethod, List<string> _Tags, DateTimeOffset _EventDate, List<string> _photnames, string _imagefilepath)
        {
            Name = _Name;
            SubBudgetLocation = _SubBudgetLocation;
            SubBudgetName = _SubBudgetName;
            EventLocation = _EventLocation;
            Price = _Price;
            Pricestring = _Pricestring;
            isDeducted = _isDeducted;
            PayMethod = _PayMethod;
            Tags = _Tags;
            EventDate = _EventDate;
            photonames = _photnames;
            imagefilepath = _imagefilepath;

        }
    }

}
