using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using System.Collections.ObjectModel;


namespace Budgeting.Models
{
    /*This is the main Budget class:
     *The Budget contains it's own information such as Name, Requirement for passwords, the starting and reset date, etc
     *However, the Budget ALSO carries every Sub-Budget and Event : This means that when accessing a specific budget the 
     *sub budgets and events are equally accessible through one instantiated form of the BudgetModel class
     *The last variables are for Terms: savedsubbudgets keeps a list of the collection of sub-budgets so users can review the
     *change in their spending based on their terms
     */
    [DataContract]
    public class BudgetModel
    {

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Boolean PwReq { get; set; }

        [DataMember]
        public Boolean Enabled { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public DateTimeOffset startdate { get; set; }

        [DataMember]
        public DateTimeOffset resetdate { get; set; }

        [DataMember]
        public List<SubBudgetModel> SubBudgets { get; set; }

        [DataMember]
        public List<EventModel> Events { get; set; }

        [DataMember]
        public int position { get; set;}

        [DataMember]
        public int Termnumber {get;set;}

        [DataMember]
        public List<List<SubBudgetModel>> savedsubbudgets { get; set; }
    

        public BudgetModel()
        {

        }

        public BudgetModel(string _Name, Boolean _PwReq, Boolean _Enabled, string _Password, DateTimeOffset _startdate, DateTimeOffset _resetdate, List<SubBudgetModel> ListSB, List<EventModel> ListEvents, List<List<SubBudgetModel>> _savedsubbudgets)
        {
            Name = _Name;
            PwReq = _PwReq;
            Enabled = _Enabled;
            Password = _Password;
            startdate = _startdate;
            resetdate = _resetdate;
            SubBudgets = ListSB;
            Events = ListEvents;

            savedsubbudgets = _savedsubbudgets;
        }


    }
}

