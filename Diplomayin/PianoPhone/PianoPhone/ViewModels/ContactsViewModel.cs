using Microsoft.Phone.Controls;
using PianoPhone.Models;
using PianoPhone.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace PianoPhone.Views
{
   public class ContactsViewModel:ViewModel
    {
       public ContactsViewModel()
       {
           LayoutMode = LongListSelectorLayoutMode.List;
           
       }

       public void Initialize(CancellationToken token, bool fromStore = true)
       {
           if (fromStore)
           {
               IsSelectionEnabled = true;
               LoadContactsFromContactStore(token);
           }
           else
           {
               IsSelectionEnabled =false;
               LoadContactsFromStorage(token);
           }

       }

       private async void LoadContactsFromStorage(CancellationToken token)
       {
           await FileManager.GetContactsAsync(token);
       }

        public async void LoadContactsFromContactStore(CancellationToken token)
        {
            await FileManager.GetContactsFromPhoneAsync(token);
        }

        public ObservableCollection<CollectionControlModel> Items
        {
            get;
            set;
        }


    }
}
