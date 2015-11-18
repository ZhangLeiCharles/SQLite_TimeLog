using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SQLiteExample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        string path;
        SQLite.Net.SQLiteConnection conn;
        

        public MainPage()
        {
            this.InitializeComponent();
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.sqlite");
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            conn.CreateTable<Messsage>();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var add = conn.Insert(new Messsage() { Content = textBox.Text });
            Debug.WriteLine(path);
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            var query = conn.Table<Messsage>().Where(x=>x.Id>1);
            string result = String.Empty;
            foreach(var item in query)
            {
                result =String.Format("{0} : {1}",item.Id, item.Content);
                Debug.WriteLine(result);
            }
        }
    }
}
