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

        myTimer currentTimer = new myTimer();


        Windows.UI.Xaml.DispatcherTimer timer0 = new DispatcherTimer();


        public MainPage()
        {
            this.InitializeComponent();
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db1.sqlite");
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            conn.CreateTable<MainTable>();
            Debug.WriteLine(path);

            //initialize timer
            currentTimer.name = "new timer";
            currentTimer.category = "default";
            currentTimer.start = DateTime.Now;
            currentTimer.end = DateTime.Now;
            currentTimer.timeElapse = TimeSpan.Zero;
            currentTimer.running = false;
            currentTimer.tag = 0;

            DispatcherTimerSetup();

        }

        //time cnt info refresh 
        public void DispatcherTimerSetup()
        {
            timer0.Tick += dispatcherTimer_Tick;
            timer0.Interval = new TimeSpan(0, 0, 1);
        }
        //update time elapse 1Hz, and refresh textblock
        void dispatcherTimer_Tick(object sender, object e)
        {
            currentTimer.end = DateTime.Now;
            currentTimer.timeElapse = (currentTimer.end - currentTimer.start);
            fresh_task_info_running();

        }


        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            //button down
            if (button.IsChecked == true)
            {
                //if has task running, end a task first
                if (currentTimer.running == true)
                {
                    task_end();
                    fresh_task_info_end();
                    TaskBtn_0.IsChecked = false;
                }

                //start a new task
                task_start(button.Tag.ToString());
                fresh_task_info_start();
            }
            //button up
            else
            {
                //button release, stop a task
                task_end();
            }

        }

        private void fresh_task_info_start()
        {
            switch (currentTimer.tag)
            {
                case 0:
                    TaskBtn_0.Content = "Running";
                    TaskCnt_0.Text = "0:0:0";
                    break;
                case 1:
                    TaskBtn_1.Content = "Running";
                    TaskCnt_1.Text = "0:0:0";
                    break;
                case 2:
                    TaskBtn_2.Content = "Running";
                    TaskCnt_2.Text = "0:0:0";
                    break;
                default:
                    break;
            }

        }

        private void fresh_task_info_running()
        {
            switch (currentTimer.tag)
            {
                case 0:
                    TaskCnt_0.Text = currentTimer.timeElapse.Hours.ToString() + ":" + currentTimer.timeElapse.Minutes.ToString() + ":" + currentTimer.timeElapse.Seconds.ToString();
                    break;
                case 1:
                    TaskCnt_1.Text = currentTimer.timeElapse.Hours.ToString() + ":" + currentTimer.timeElapse.Minutes.ToString() + ":" + currentTimer.timeElapse.Seconds.ToString();
                    break;
                case 2:
                    TaskCnt_2.Text = currentTimer.timeElapse.Hours.ToString() + ":" + currentTimer.timeElapse.Minutes.ToString() + ":" + currentTimer.timeElapse.Seconds.ToString();
                    break;
                default:
                    break;
            }

        }

        private void fresh_task_info_end()
        {
            switch (currentTimer.tag)
            {
                case 0:
                    //reset display zone
                    TaskBtn_0.Content = "Start";
                    TaskCnt_0.Text = "-:--:--";
                    //display last timer's data
                    TaskHist_0.Text = currentTimer.timeElapse.Hours.ToString() + ":" + currentTimer.timeElapse.Minutes.ToString() + ":" + currentTimer.timeElapse.Seconds.ToString();
                    break;
                case 1:
                    TaskBtn_1.Content = "Start";
                    TaskCnt_1.Text = "-:--:--";
                    TaskHist_1.Text = currentTimer.timeElapse.Hours.ToString() + ":" + currentTimer.timeElapse.Minutes.ToString() + ":" + currentTimer.timeElapse.Seconds.ToString();
                    break;
                case 2:
                    TaskBtn_2.Content = "Start";
                    TaskCnt_2.Text = "-:--:--";
                    TaskHist_2.Text = currentTimer.timeElapse.Hours.ToString() + ":" + currentTimer.timeElapse.Minutes.ToString() + ":" + currentTimer.timeElapse.Seconds.ToString();
                    break;
                default:
                    break;
            }

        }

        private void task_end()
        {
            timer0.Stop();
            store_data(currentTimer);

            currentTimer.running = false;
            fresh_task_info_end();
        }

        /**************************************************
        *task action button press
        ***************************************************/
        private void task_start(string taskTag)
        {
            //initialize new timer
            currentTimer.category = "default";
            currentTimer.name = task_get_name(taskTag);
            currentTimer.start = DateTime.Now;
            currentTimer.end = DateTime.Now;
            currentTimer.timeElapse = TimeSpan.Zero;
            currentTimer.running = true;
            currentTimer.tag = int.Parse(taskTag);

            //start counting, updata time info on main page
            timer0.Start();
            fresh_task_info_start();
        }

        private string task_get_name(string taskTag)
        {
            switch (taskTag)
            {
                case "0":
                    return TaskName_0.Text;
                case "1":
                    return TaskName_1.Text;
                case "2":
                    return TaskName_2.Text;
                default:
                    return "unknow";
                    
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

        private void store_data(myTimer currentTimer)
        {
            //filter too short timer
            if (currentTimer.timeElapse.TotalMinutes > 0.01)
            {
                conn.Insert(new MainTable()
                {
                    Category = "Button",
                    Name = currentTimer.name,
                    StartTime = currentTimer.start,
                    EndTime = currentTimer.end,
                    TimeSpan = currentTimer.timeElapse
                });
            }
        }
        
    }
}
