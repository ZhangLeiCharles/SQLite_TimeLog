using hello_world;
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

        myTimer newTimer = new myTimer();

        Windows.UI.Xaml.DispatcherTimer timer0 = new DispatcherTimer();


        public MainPage()
        {
            this.InitializeComponent();
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db1.sqlite");
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            conn.CreateTable<MainTable>();
            Debug.WriteLine(path);

            //initialize timer
            newTimer.name = "new timer";
            newTimer.category = "default";
            newTimer.start = DateTime.Now;
            newTimer.end = DateTime.Now;
            newTimer.timeElapse = TimeSpan.Zero;
            newTimer.running = false;

            btn_0.Content = "Start";

            DispatcherTimerSetup();

        }

        public void DispatcherTimerSetup()
        {

            timer0.Tick += dispatcherTimer_Tick;
            timer0.Interval = new TimeSpan(0, 0, 1);
        }



        void dispatcherTimer_Tick(object sender, object e)
        {
            newTimer.end = DateTime.Now;
            newTimer.timeElapse = (newTimer.end - newTimer.start);
            TaskTimer_0.Text = newTimer.timeElapse.Hours.ToString() + ":" + newTimer.timeElapse.Minutes.ToString() + ":" + newTimer.timeElapse.Seconds.ToString();
        }


        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            //press button
            if (button.IsChecked == true)
            {
                //previous timer still running
                if (newTimer.running == true)
                {
                    newTimer.running = false;

                    store_data(newTimer);

                }

                //initialize new taks info
                newTimer.category = "default";
                newTimer.name = button.Content.ToString();
                newTimer.start = DateTime.Now;
                newTimer.running = true;

                btn_0.Content = "Running";

                //start counting, updata time info on main page
                timer0.Start();
            }
            //release button
            else
            {
                //prepare to save new timer data
                //newTimer.end = DateTime.Now;
                //newTimer.timeElapse = (newTimer.end - newTimer.start);
                newTimer.running = false;

                store_data(newTimer);
                btn_0.Content = "Start";
                TaskTimer_0.Text = "-:--:--";
                timer0.Stop();
            }

        }


        private void btn_show_Click(object sender, RoutedEventArgs e)
        {
            var query = conn.Table<MainTable>().Where(x => x.Id > 1);
            string result = String.Empty;
            foreach (var item in query)
            {
                result = String.Format("{0} : {1}, time span : {2}", item.Id, item.Name, item.TimeSpan.TotalMinutes);
                Debug.WriteLine(result);
            }
        }

        private void store_data(myTimer newTimer)
        {
            //filter too short timer
            if (newTimer.timeElapse.TotalMinutes > 0.01)
            {
                conn.Insert(new MainTable()
                {
                    Category = "Button",
                    Name = newTimer.name,
                    StartTime = newTimer.start,
                    EndTime = newTimer.end,
                    TimeSpan = newTimer.timeElapse

                });
            }
        }


    }
}
