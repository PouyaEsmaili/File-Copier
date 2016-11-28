using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Threading;
using WPFFolderBrowser;

namespace CopyApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.NotifyIcon ni;
        Thread trd;
        public MainWindow()
        {
            InitializeComponent();
            trd = new Thread(Copy_Run);

            #region InitializeForm
            DirectoryCpoier DC;
            DC = Config.Directory_Read();
            if (DC != null)
            {
                txtSource.Text = DC.SourcePath;
                btnDeleteConfig.IsEnabled = true;
                foreach (string item in DC.DestnationPathList)
                {
                    AddDestination(item);
                }

                if (DC.ScheduledCopyType != DirectoryCpoier.ScheduledCopyTypes.Watching)
                {
                    rbtnScheduledCopy.IsChecked = true;
                    switch (DC.ScheduledCopyType.ToString())
                    {
                        case "Hourly":
                            cmb.SelectedIndex = 0;
                            MinuteOfHour.SelectedIndex = DC.CopyTime.Minute;
                            break;
                        case "Daily":
                            cmb.SelectedIndex = 1;
                            DateTime dt1 =
                                new DateTime(2016, 1, 1, DC.CopyTime.Hour, DC.CopyTime.Minute, 0);
                            tpDaily.Value = dt1;
                            break;
                        case "Weekly":
                            cmb.SelectedIndex = 2;
                            DateTime dt2 =
                                new DateTime(2016, 1, 1, DC.CopyTime.Hour, DC.CopyTime.Minute, 0);
                            tpWeekly.Value = dt2;
                            DayOfWeekSelector.SelectedValue = (object)DC.DayOfWeek;
                            break;
                        case "Monthly":
                            cmb.SelectedIndex = 3;
                            DateTime dt3 =
                                new DateTime(2016, 1, 1, DC.CopyTime.Hour, DC.CopyTime.Minute, 0);
                            tpMonthly.Value = dt3;
                            DayOfMonthSelector.SelectedIndex = DC.DayOfMonth - 1;
                            break;
                    }
                }
                else if (DC.ScheduledCopyType == DirectoryCpoier.ScheduledCopyTypes.Watching)
                {
                    rbtnWatching.IsChecked = true;
                }
            }
            else
                AddDestination();
            #endregion

            ni = new System.Windows.Forms.NotifyIcon();
            Icon icon = new Icon(SystemIcons.Application, 40, 40);
            ni.Icon = icon;
            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = System.Windows.WindowState.Normal;
                };
            trd.Start();
        }
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }

        public void Copy_Run()
        {
            Copy C = new Copy();
            C.Main();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ni.Visible = false;
        }

        private void btnAddDestination_Click(object sender, RoutedEventArgs e)
        {
            AddDestination();
        }

        #region AddDestination
        private void AddDestination()
        {
            AddDestination("");
        }

        private void AddDestination(string Destination)
        {
            int Count = grdDestination.RowDefinitions.Count + 1;
            RowDefinition rd = new RowDefinition
            { Height = new GridLength(45), Name = "Row_" + Convert.ToString(Count) };
            grdDestination.RowDefinitions.Add(rd);

            //TextBox Creator
            TextBox txt = new TextBox();
            txt.Name = "txtDestination_" + Convert.ToString(Count);
            txt.IsReadOnly = true;
            txt.Text = Destination;
            txt.Width = 392;
            txt.Height = 21;
            Grid.SetRow(txt, Count - 1);
            Grid.SetColumn(txt, 0);
            grdDestination.Children.Add(txt);

            //Remove Button Creator
            Button rmv = new Button();
            rmv.Name = "btnRemove_" + Convert.ToString(Count);
            rmv.Content = "Remove";
            rmv.Width = 52;
            rmv.Height = 25;
            Grid.SetRow(rmv, Count - 1);
            Grid.SetColumn(rmv, 1);
            grdDestination.Children.Add(rmv);
            rmv.Click += Remove_Click;

            //Set Destination Button Creator
            Button set = new Button();
            set.Name = "btnSetDestination_" + Convert.ToString(Count);
            set.Content = "...";
            set.Width = 22;
            set.Height = 25;
            Grid.SetRow(set, Count - 1);
            Grid.SetColumn(set, 2);
            grdDestination.Children.Add(set);
            set.Click += Set_Click;
        }
        #endregion

        private void btnSource_Click(object sender, RoutedEventArgs e)
        {
            WPFFolderBrowserDialog fbd = new WPFFolderBrowserDialog("Choose Source Folder");
            if (fbd.ShowDialog() == true)
            {
                txtSource.Text = fbd.FileName;
            }
        }

        private void Set_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            WPFFolderBrowserDialog fbd = new WPFFolderBrowserDialog("Choose Destination Folder");
            if (fbd.ShowDialog() == true)
            {
                grdDestination.Children.OfType<TextBox>().
                    Single(Child => Child.Name == "txtDestination_" + btn.Name.Split('_')[1]).Text =
                    fbd.FileName;
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            RowDefinition rd;
            for (int i = 0; i < grdDestination.RowDefinitions.Count; i++)
            {
                rd = grdDestination.RowDefinitions[i];
                if (rd != null)
                {
                    if (rd.Name == "Row_" + btn.Name.Split('_')[1] &&
                        grdDestination.Children.OfType<TextBox>().Count() > 1)
                    {
                        grdDestination.RowDefinitions[i].Height = new GridLength(0);

                        grdDestination.Children.Remove(grdDestination.Children.OfType<TextBox>().
                            Single(Child => Child.Name == "txtDestination_" + btn.Name.Split('_')[1]));
                        grdDestination.Children.Remove(grdDestination.Children.OfType<Button>().
                            Single(Child => Child.Name == "btnRemove_" + btn.Name.Split('_')[1]));
                        grdDestination.Children.Remove(grdDestination.Children.OfType<Button>().
                           Single(Child => Child.Name == "btnSetDestination_" + btn.Name.Split('_')[1]));
                        break;
                    }
                }
            }
        }


        private void rbtnWatching_Checked(object sender, RoutedEventArgs e)
        {
            btnSet.Content = "Set";
            btnSet.IsEnabled = true;
            DisableGrids();
            cmb.IsEnabled = false;
        }

        private void rbtnCopyNow_Checked(object sender, RoutedEventArgs e)
        {
            btnSet.Content = "Start";
            btnSet.IsEnabled = true;
            DisableGrids();
            cmb.IsEnabled = false;
        }

        private void rbtnScheduledCopy_Checked(object sender, RoutedEventArgs e)
        {
            btnSet.Content = "Set";
            if (cmb.SelectedIndex < 0)
                btnSet.IsEnabled = false;
            cmb.IsEnabled = true;
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            DirectoryCpoier DC = new DirectoryCpoier(txtSource.Text);
            foreach (TextBox item in grdDestination.Children.OfType<TextBox>())
            {
                if (item.Text != "")
                    DC.AddDestination(item.Text);
                else
                {
                    MessageBox.Show("Check Destinations");
                    return;
                }
            }

            if (DC.SourcePath != "" && DC.DestnationPathList.Length > 0)
            {
                if (cmb.SelectedItem != null && rbtnScheduledCopy.IsChecked == true)
                {
                    DirectoryCpoier.Time t;
                    switch (((ComboBoxItem)cmb.SelectedItem).Content.ToString())
                    {
                        case "Hourly":
                            DC.ScheduledCopyType = DirectoryCpoier.ScheduledCopyTypes.Hourly;
                            t.Minute = MinuteOfHour.SelectedIndex;
                            t.Hour = 0;
                            DC.CopyTime = t;
                            break;
                        case "Daily":
                            DC.ScheduledCopyType = DirectoryCpoier.ScheduledCopyTypes.Daily;
                            t.Hour = tpDaily.Value.Value.Hour;
                            t.Minute = tpDaily.Value.Value.Minute;
                            DC.CopyTime = t;
                            break;
                        case "Weekly":
                            DC.ScheduledCopyType = DirectoryCpoier.ScheduledCopyTypes.Daily;
                            t.Hour = tpWeekly.Value.Value.Hour;
                            t.Minute = tpWeekly.Value.Value.Minute;
                            DC.CopyTime = t;
                            DC.DayOfWeek = (DayOfWeek)DayOfWeekSelector.SelectedIndex - 1;
                            break;
                        case "Monthly":
                            DC.ScheduledCopyType = DirectoryCpoier.ScheduledCopyTypes.Monthly;
                            t.Hour = tpMonthly.Value.Value.Hour;
                            t.Minute = tpMonthly.Value.Value.Minute;
                            DC.CopyTime = t;
                            DC.DayOfMonth = DayOfMonthSelector.SelectedIndex + 1;
                            break;
                    }
                }
                else if (rbtnWatching.IsChecked == true)
                    DC.ScheduledCopyType = DirectoryCpoier.ScheduledCopyTypes.Watching;
                else if (rbtnCopyNow.IsChecked == true)
                {
                    DC.StartCopy();
                    return;
                }
                btnDeleteConfig.IsEnabled = true;
                Config.Directory_Save(DC);
                MessageBox.Show("Saved");
            }
            else
                MessageBox.Show("Check Source, Destinations");
        }

        #region cmb_Selected_Events
        private void cmbHourly_Selected(object sender, RoutedEventArgs e)
        {
            DisableGrids();
            grdHourly.Visibility = Visibility.Visible;
            btnSet.IsEnabled = true;
        }

        private void cmbDaily_Selected(object sender, RoutedEventArgs e)
        {
            DisableGrids();
            grdDaily.Visibility = Visibility.Visible;
            btnSet.IsEnabled = true;
        }

        private void cmbWeekly_Selected(object sender, RoutedEventArgs e)
        {
            DisableGrids();
            grdWeekly.Visibility = Visibility.Visible;
            btnSet.IsEnabled = true;
        }

        private void cmbMonthly_Selected(object sender, RoutedEventArgs e)
        {
            DisableGrids();
            grdMonthly.Visibility = Visibility.Visible;
            btnSet.IsEnabled = true;
        }
        private void DisableGrids()
        {
            grdHourly.Visibility = Visibility.Hidden;
            grdDaily.Visibility = Visibility.Hidden;
            grdWeekly.Visibility = Visibility.Hidden;
            grdMonthly.Visibility = Visibility.Hidden;
        }
        #endregion

        private void btnDeleteConfig_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("Are you sure you want to delete Config file?", "Deleting Config File", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.Yes)
            {
                Config.Delete_Config();
                MainWindow mw = new MainWindow();
                mw.Left = this.Left;
                mw.Margin = this.Margin;
                this.Close();
                mw.Show();
            }
        }
    }
}