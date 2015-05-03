using System;
using System.Collections.Generic;
using System.Windows;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budgeting.Models;
using Windows.Storage;
using System.Runtime.Serialization.Json;
using Windows.Storage.Streams;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Budgeting.ViewModels
{
    /* The BudgetViewModel is the binding class of the entire application: All saved information goes through this all encompassing ViewModel
     * In hindsight, the BudgetViewModel could have been distinguished into smaller viewmodel classes for the SubBudget and Event but soon
     * became complicated when considering the different ways SubBudgets and Events were used for saving, editing, deleting, searching and graphing
     * 
     * The BudgetViewModel contains three important public variable members: It has one Observable Collection of ALL the user's budgets,
     * A BudgetModel that represents the specific budget being examined by the user, A SubBudgetModel that represents the specific sub budget
     * being examined, and an EventModel that represents a specific Event being examined (i.e. at EventDetails page)
     * 
     * The Purpose of the BudgetViewModel is to connect the logic and information of the Budget/SubBudget/Event model heirachy to our user
     * interface through Data Binding. This means that in the BudgetHomePage for example, a new BudgetViewModel will be instatiated, it will execute
     * a JSON Deserialization function and then populate "CurrentBudget" with the relevant information of the chosen Budget: All information is now 
     * RELATIVE to the contents of Current Budget
     */ 

    [DataContract]
    public class BudgetViewModel
    {
        [DataMember]
        public ObservableCollection<BudgetModel> MyBudgets { get; set; }

        [DataMember]
        public BudgetModel CurrentBudget {get;set;}

        [DataMember]
        public SubBudgetModel CurrentSubBudget { get; set; }

        [DataMember]
        public EventModel CurrentEvent { get; set; }

        Windows.Storage.ApplicationDataContainer savedbudgetsettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        

        //Serializes all Budgets in the application into a JSON file
        public async Task SaveBudget()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<BudgetModel>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync("MyBudgets.json", CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, MyBudgets);
            }
            
        }

        //Deserializes the MyBudget.json file and converts the information into the viewmodel's observable collection of budgets
        public async Task GetSavedBudgets() {

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ObservableCollection<BudgetModel>));

            using (var myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("MyBudgets.json"))
            {

                MyBudgets = (ObservableCollection<BudgetModel>)serializer.ReadObject(myStream);
            }

       }

        //Serializes the specifically chosen budget
        public async Task SaveCurrentBudget()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(BudgetModel));

            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync("CurrentBudget.json", CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, CurrentBudget);
            }
        }

        //Deserializes information about the specifically chosen budget
        public async Task GetCurrentBudget()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(BudgetModel));

            using (var myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("CurrentBudget.json"))
            {
                CurrentBudget = (BudgetModel)serializer.ReadObject(myStream);
            }

        }

        //Serializes information about a specifically chosen event
        public async Task SaveCurrentEvent()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(EventModel));

            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync("CurrentEvent.json", CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, CurrentEvent);
            }
        }

        //Deserializes information about a specifically chosen event
        public async Task GetCurrentEvent()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(EventModel));

            using (var myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("CurrentEvent.json"))
            {
                CurrentEvent = (EventModel)serializer.ReadObject(myStream);
            }

        }

        }

        


        }
    
