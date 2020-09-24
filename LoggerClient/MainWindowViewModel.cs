using LoggerClient.Command;
using LoggerClient.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;

namespace LoggerClient
{
    class MainWindowViewModel : ViewModelBase, LoadedListener
    {
        #region Fields
        private ObservableCollection<Train> _listTrain;
        private MainWindow mainWindow = Application.Current.Windows[0] as MainWindow;
        //private Point startPos;
        #endregion

        #region Properties
        public ObservableCollection<Train> ListTrain
        {
            get { return _listTrain; }
            set
            {
                _listTrain = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public ICommand SelectCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand MainTabCommand { get; set; }
        public ICommand ConfigTabCommand { get; set; }
        #endregion

        #region Constructors
        public MainWindowViewModel()
        {
            _listTrain = new ObservableCollection<Train>();

            SelectCommand = new SimpleCommand(Select);
            AddCommand = new SimpleCommand(Add);
            UpdateCommand = new SimpleCommand(Update);
            DeleteCommand = new SimpleCommand(Delete);
            MainTabCommand = new SimpleCommand(MainTab);
            ConfigTabCommand = new SimpleCommand(ConfigTab);

            // 28개 리스트에 추가
            for (int i = 0; i < 28; i++)
            {
                Train train1to28 = new Train();
                train1to28.Time = i;
                ListTrain.Add(train1to28);
            }
            Thread thread = new Thread(Count10);
            thread.Start();
            //Select();

        }
        #endregion

        #region Methods
        public void afterLoadedEvent()
        {
            Select();
        }
        public void Select()
        {
            Console.WriteLine("select");

            /*************그리드 초기화************/
            mainWindow.MainGrid.Children.Clear();
            mainWindow.MainGrid.RowDefinitions.Clear();

            mainWindow.ConfigGrid.Children.Clear();
            mainWindow.ConfigGrid.RowDefinitions.Clear();

            /************행 및 버튼 생성************/
            int trainCnt = 0;
            foreach (Train train in ListTrain)
            {

                // 5개씩 줄바꿈
                if (trainCnt % 5 == 0)
                {
                    StringBuilder rowSb = new StringBuilder();
                    rowSb.Append(@"<RowDefinition xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
                               xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' Height='1 * '/>");
                    RowDefinition rowDefinition = (RowDefinition)XamlReader.Parse(rowSb.ToString());
                    mainWindow.MainGrid.RowDefinitions.Add(rowDefinition);
                    RowDefinition rowDefinition2 = (RowDefinition)XamlReader.Parse(rowSb.ToString());
                    mainWindow.ConfigGrid.RowDefinitions.Add(rowDefinition2);
                }

                StringBuilder mainBtnSb = new StringBuilder();
                mainBtnSb.Append(@"<Button Width='345' Height='161' Margin='6, 4.5, 6, 4.5' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
                               xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ");
                mainBtnSb.Append(@"Grid.Column='" + (trainCnt % 5) + "' Grid.Row='" + (trainCnt / 5) + "' ");
                mainBtnSb.Append(@"Content='{Binding ListTrain[" + trainCnt + "].Time}' />");
                Button myButton = (Button)XamlReader.Parse(mainBtnSb.ToString());
                mainWindow.MainGrid.Children.Add(myButton);

                StringBuilder configBtnSb = new StringBuilder();
                configBtnSb.Append(@"<Button xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
                               xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ");
                configBtnSb.Append(@"Grid.Column='" + (trainCnt % 5) + "' Grid.Row='" + (trainCnt / 5) + "' ");
                configBtnSb.Append(@"Content='{Binding ListTrain[" + trainCnt + "].Time}' />");
                Button myButton2 = (Button)XamlReader.Parse(configBtnSb.ToString());
                mainWindow.ConfigGrid.Children.Add(myButton2);

                trainCnt++;
            }
        }
        private void Add()
        {
            Console.WriteLine("add");

            Train train = new Train();
            train.Time = 0;
            ListTrain.Add(train);

            Select();
        }
        private void Update()
        {
            Console.WriteLine("update");

            foreach (var train in ListTrain)
            {
                train.Time = 0;
            }
        }
        private void Delete()
        {
            ListTrain.Clear();
            Select();
        }
        private void MainTab()
        {
            mainWindow.ConfigView.Visibility = Visibility.Collapsed;
            mainWindow.MainView.Visibility = Visibility.Visible;

            mainWindow.btnMainTab_text.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0080b2"));
            mainWindow.btnConfigTab_text.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#525252"));

            mainWindow.btnMainTab.IsEnabled = false;
            mainWindow.btnConfigTab.IsEnabled = true;
        }
        private void ConfigTab()
        {
            mainWindow.MainView.Visibility = Visibility.Collapsed;
            mainWindow.ConfigView.Visibility = Visibility.Visible;

            mainWindow.btnConfigTab_text.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0080b2"));
            mainWindow.btnMainTab_text.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#525252"));

            mainWindow.btnConfigTab.IsEnabled = false;
            mainWindow.btnMainTab.IsEnabled = true;
        }
        //private void System_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        if (this.WindowState == WindowState.Maximized && Math.Abs(startPos.Y - e.GetPosition(null).Y) > 2)
        //        {
        //            var point = PointToScreen(e.GetPosition(null));

        //            this.WindowState = WindowState.Normal;

        //            this.Left = point.X - this.ActualWidth / 2;
        //            this.Top = point.Y - border.ActualHeight / 2;
        //        }
        //        DragMove();
        //    }
        //}
        private void Count10()
        {   
            while (true)
            {
                try
                {
                    if (ListTrain.Count > 0)
                    {
                        foreach (var item in ListTrain)
                        {
                            mainWindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                            {
                                if (item.Time == 50)
                                {
                                    item.Time = 0;
                                }
                                item.Time++;
                            }));
                        }
                    }
                    Thread.Sleep(1000);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        #endregion
    }

    interface LoadedListener
    {
        void afterLoadedEvent();
    }
}
