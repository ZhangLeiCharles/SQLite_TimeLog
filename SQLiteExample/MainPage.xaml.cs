﻿using hello_world;
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

        myTimer newTimer = new myTimer();
        private void start_Click(object sender, RoutedEventArgs e)
        {
            newTimer.start = DateTime.Now;
            newTimer.running = true;
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            newTimer.end = DateTime.Now;
            TimeSpan ts = (newTimer.end - newTimer.start);
            timeElapse.Text = ts.Seconds.ToString();
            result_display.Text = ts.TotalMinutes.ToString();
            var add = conn.Insert(new Messsage() { Content = ts.TotalMinutes.ToString()});
            Debug.WriteLine(path);
        }

        private void btn_show_Click(object sender, RoutedEventArgs e)
        {
            var query = conn.Table<Messsage>().Where(x=>x.Id>1);
            string result = String.Empty;
            foreach(var item in query)
            {
                result =String.Format("{0} : {1}",item.Id, item.Content);
                Debug.WriteLine(result);
            }
        }

        //private void stop_Click(object sender, RoutedEventArgs e)
        //{

        //}
    }
}