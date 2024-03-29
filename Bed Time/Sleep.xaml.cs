﻿using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;

namespace BedTime
{
    public partial class Sleep : PhoneApplicationPage
    {
        private DateTime _wakeUpTime;

        public Sleep()
        {
            InitializeComponent();
            Loaded += LoadPage;
        }

        private void LoadPage(object sender, RoutedEventArgs e)
        {
            PickByWakeTimePanel.Visibility = Visibility.Visible;
        }
        private void FireTile(String text)
        {
            var uri = new Uri("Images/173x173.png", UriKind.Relative);
            var sri = Application.GetResourceStream(uri);

            var uriBack = new Uri("Images/173x173Back.png", UriKind.Relative);
            var sriBack = Application.GetResourceStream(uriBack);


            var wbm = new WriteableBitmap(173, 173);
            wbm.SetSource(sri.Stream);
            var wbmBack = new WriteableBitmap(173, 173);
            wbmBack.SetSource(sriBack.Stream);


            using (
                var stream =
                    IsolatedStorageFile.GetUserStoreForApplication().CreateFile(
                        "/Shared/ShellContent/tile.png"))
            {
                wbm.SaveJpeg(stream, 173, 173, 0, 100);
            }

            using (
    var streamBack =
        IsolatedStorageFile.GetUserStoreForApplication().CreateFile(
            "/Shared/ShellContent/tileBack.png"))
            {
                wbmBack.SaveJpeg(streamBack, 173, 173, 0, 100);
            }


            var currentTiles = ShellTile.ActiveTiles.First();
            var tilesUpdatedData = new StandardTileData
            {
                BackBackgroundImage =
                    new Uri("isostore:/Shared/ShellContent/tileBack.png",
                            UriKind.Absolute),
                BackContent = text,
                BackTitle = ""
            };
            if (currentTiles != null) currentTiles.Update(tilesUpdatedData);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SetTimes();
        }
        private string CheckForAlarms()
        {
            string tempstring = "";
            ScheduledAction temp = ScheduledActionService.Find("Bed Time Wake Up ID");
            if (temp != null)
            {
                FireTile("Alarm @ " + temp.BeginTime.ToShortDateString());
                tempstring = "Alarm set to: " + temp.BeginTime.ToShortDateString() + " at " +
                             temp.BeginTime.ToShortTimeString();
            }
            else
            {
                tempstring = "No alarm set.";
                FireTile("No alarm set.");
            }
            return tempstring;
        }

        private void SetTimes()
        {
            //Statusfield.Text = CheckForAlarms();
            //Statusfield2.Text = CheckForAlarms();
            timeButton1.Content = DateTime.Now.AddMinutes(104);
            timeButton2.Content = DateTime.Now.AddMinutes(194);
            timeButton3.Content = DateTime.Now.AddMinutes(284);
            timeButton4.Content = DateTime.Now.AddMinutes(374);
            timeButton5.Content = DateTime.Now.AddMinutes(464);
            timeButton6.Content = DateTime.Now.AddMinutes(554);
            timeButton7.Content = DateTime.Now.AddMinutes(644);
#if DEBUG
            timeButton1.Visibility = Visibility.Collapsed;
            timeButton2.Visibility = Visibility.Collapsed;
            timeButton3.Visibility = Visibility.Collapsed;
            timeButton4.Visibility = Visibility.Collapsed;
            timeButton8.Visibility = Visibility.Visible;
            timeButton8.Content = DateTime.Now.AddMinutes(1);
#endif
        }

        private void LoadTimes(object sender, RoutedEventArgs e)
        {
            SetTimes();
        }

        private void HandleAlarmSet(DateTime thetime)
        {
            try
            {
                ScheduledAction tempReminder = ScheduledActionService.Find("Go to sleep Reminder ID");
                if (tempReminder != null)
                {
                    ScheduledActionService.Remove("Go to sleep Reminder ID");
                }
                var alarm = new Alarm("Bed Time Wake Up ID")
                                {
                                    Content = "Wake Up",
                                    Sound = new Uri("Sounds/sleet.mp3", UriKind.RelativeOrAbsolute),
                                    BeginTime = thetime,
                                    RecurrenceType = RecurrenceInterval.None,
                                };

                ScheduledAction temp = ScheduledActionService.Find("Bed Time Wake Up ID");
                if (temp != null)
                {
                    ScheduledActionService.Remove("Bed Time Wake Up ID");
                    ScheduledActionService.Add(alarm);
                    FireTile("Alarm @ " + alarm.BeginTime.ToShortTimeString());
                }
                else
                {
                    ScheduledActionService.Add(alarm);
                    FireTile("Alarm @ " + alarm.BeginTime.ToShortTimeString());
                }

                MessageBox.Show("Alarm set for: " + alarm.BeginTime.ToShortDateString() + " at " +
                                alarm.BeginTime.ToShortTimeString());
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception caught: " + ex.Message);
            }
        }

        private void TimeButtonClick(object sender, RoutedEventArgs e)
        {
            HandleAlarmSet(Convert.ToDateTime(((Button)e.OriginalSource).Content));
            SetTimes();
            refreshbutton.IsEnabled = true;
            //ToggleSelectPanel(Toggles.Open);
            //ToggleSleepPanel(Toggles.Close);
            //ToggleWakePanel(Toggles.Close);
        }

        private void AboutClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            SetTimes();
        }

        private void ClearClick(object sender, RoutedEventArgs e)
        {
            ScheduledAction tempReminder = ScheduledActionService.Find("Go to sleep Reminder ID");
            if (tempReminder != null)
            {
                ScheduledActionService.Remove("Go to sleep Reminder ID");
            }
            ScheduledAction temp = ScheduledActionService.Find("Bed Time Wake Up ID");
            if (temp != null)
            {
                ScheduledActionService.Remove("Bed Time Wake Up ID");
                MessageBox.Show("Alarm cleared.");
                FireTile("No alarm set.");
            }
            else
            {
                //MessageBox.Show("No alarm found.");
            }
            //sleepButton1.Visibility = Visibility.Collapsed;
            //sleepButton2.Visibility = Visibility.Collapsed;
            //sleepButton3.Visibility = Visibility.Collapsed;
            //sleepButton4.Visibility = Visibility.Collapsed;
            //Dispatcher.BeginInvoke(() => { timePicker.Value = null; });

            //Statusfield.Text = CheckForAlarms();
            //Statusfield2.Text = CheckForAlarms();
            refreshbutton.IsEnabled = true;
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void SelectByWakeButtonClick(object sender, RoutedEventArgs e)
        {
            refreshbutton.IsEnabled = true;
            ToggleSelectPanel(Toggles.Close);
            ToggleSleepPanel(Toggles.Close);
            ToggleWakePanel(Toggles.Open);
        }

        private void SelectBySleepButtonClick(object sender, RoutedEventArgs e)
        {
            refreshbutton.IsEnabled = false;
            ToggleSelectPanel(Toggles.Close);
            ToggleSleepPanel(Toggles.Open);
            ToggleWakePanel(Toggles.Close);
        }

        private void ToggleSelectPanel(Toggles toggle)
        {
            //SelectButtonsPanel.Visibility = toggle == Toggles.Open ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ToggleWakePanel(Toggles toggle)
        {
            PickByWakeTimePanel.Visibility = toggle == Toggles.Open ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ToggleSleepPanel(Toggles toggle)
        {
            //PickBySleepTimePanel.Visibility = toggle == Toggles.Open ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TimePickerValueChanged(object sender, ValueChangedEventArgs<object> args)
        {
            //if (args.NewValue != null)
            //{
            //    _wakeUpTime = Convert.ToDateTime(args.NewValue);

            //    if (_wakeUpTime > DateTime.Now.Subtract(TimeSpan.FromMinutes(120)) ||
            //        _wakeUpTime < DateTime.Now.Subtract(TimeSpan.FromSeconds(30)))
            //    {
            //        if (_wakeUpTime < DateTime.Now.Subtract(TimeSpan.FromSeconds(30)))
            //        {
            //            _wakeUpTime = _wakeUpTime.AddHours(24);
            //        }
            //        sleepButton1.Content = _wakeUpTime.Subtract(TimeSpan.FromMinutes(360));
            //        sleepButton2.Content = _wakeUpTime.Subtract(TimeSpan.FromMinutes(450));
            //        sleepButton3.Content = _wakeUpTime.Subtract(TimeSpan.FromMinutes(540));
            //        sleepButton4.Content = _wakeUpTime.Subtract(TimeSpan.FromMinutes(630));
            //        sleepButton5.Content = _wakeUpTime.Subtract(TimeSpan.FromMinutes(720));

            //        sleepButton1.Visibility = Visibility.Visible;
            //        sleepButton2.Visibility = Visibility.Visible;
            //        sleepButton3.Visibility = Visibility.Visible;
            //        sleepButton4.Visibility = Visibility.Visible;
            //        HandleAlarmSet(_wakeUpTime);
            //        Statusfield2.Text = CheckForAlarms();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Please set time ahead at least two hours.");
            //    }
            //}
        }

        #region Nested type: Toggles

        private enum Toggles
        {
            Open,
            Close
        }

        #endregion
    }
}