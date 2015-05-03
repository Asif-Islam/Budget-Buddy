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
    [DataContract]
    public class SubBudgetViewModel
    {
        //public ObservableCollection<List<SubBudgetModel>> MySubBudgets = new ObservableCollection<List<SubBudgetModel>>();
        
        [DataMember]
        public List<SubBudgetModel> MySBudgets = new List<SubBudgetModel>();
 
        private const string JSONNAME = "myfile.json";




        public async Task Save()
        {
           

           
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<SubBudgetModel>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync("Myfile.json", CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, MySBudgets);
            }
            
          
            
            
       
            
       //     savedsubbudgetsettings.Values[currentbud] = MySBudgets;
  
        }
        public async Task GetSubBudgets()
        {
            
            
            

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<SubBudgetModel>));
       
            using (var myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("Myfile.json"))
            {

                MySBudgets = (List<SubBudgetModel>)serializer.ReadObject(myStream);
            }
            
            
           //}
           //catch (NullReferenceException ex) {
               
            //}

            //savedsettings to tell me if there are any saved budgets: If true - GetSubBudgets and then test, if not then give default value;
        }
        

    }
}
