#region Using Directives

using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;

#endregion Using Directives

namespace PomoRig
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml 
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Constructors

        public MainWindow()
        {
            CaptionHeight = DefaultCaptionHeight;

            CloseCommand = new ActionCommand(Application.Current.Shutdown);
            HideCommand = new ActionCommand(HideMainWindow);
            StartCommand = new ActionCommand(StartTimer);
            StopCommand = new ActionCommand(StopTimer);

            InitializeComponent();

            StartTimer();
        }

        #endregion Constructors

        #region Public Properties

        public double CaptionHeight
        {
            get
            {
                return this.captionHeight;
            }
            private set
            {
                this.captionHeight = value;
                OnPropertyChanged("CaptionHeight");
            }
        }

        public Thickness BackgroundOffset
        {
            get
            {
                return this.backgroundOffset;
            }
            private set
            {
                this.backgroundOffset = value;
                OnPropertyChanged("BackgroundOffset");
            }
        }

        public string CurrentTime
        {
            get
            {
                return this.currentTimeText;
            }
            private set
            {
                this.currentTimeText = value;
                OnPropertyChanged("CurrentTime");
            }
        }

        public ICommand CloseCommand
        {
            get; private set;
        }

        public ICommand HideCommand
        {
            get; private set;
        }

        public ICommand StartCommand
        {
            get; private set;
        }

        public ICommand StopCommand
        {
            get; private set;
        }

        #endregion Public Properties

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Protected Methods

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (!ReferenceEquals(null, handler))
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowMainWindow();
        }

        private void ShowMainWindow()
        {
            this.isHidden = false;

            ResetDimensions();
            ResetLocation();

            Alert();
        }

        private void ResetDimensions()
        {
            ClearValue(HeightProperty);
            ClearValue(WidthProperty);

            CaptionHeight = DefaultCaptionHeight;
        }

        private void ResetLocation()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = Width;
            double windowHeight = Height;

            Top = (screenHeight / 2) - (windowHeight / 2);
            Left = (screenWidth / 2) - (windowWidth / 2);
        }

        private void Alert()
        {
            if (Dispatcher.CheckAccess())
            {
                AlertSafe();
                return;
            }

            Dispatcher.Invoke(AlertSafe);
        }

        private void AlertSafe()
        {
            Activate();

            if (this.isHidden)
            {
                ShowMainWindow();
                return;
            }

            Topmost = true;
            Topmost = false;
        }

        private void HideMainWindow()
        {
            SetUpHiddenMode();
            RefreshWidth();
        }

        private void SetUpHiddenMode()
        {
            this.isHidden = true;
            Height = 1;
            Topmost = true;
            Top = 0;
            Left = 0;
            CaptionHeight = 0;
        }

        private void RefreshWidth()
        {
            if (!this.isHidden)
            {
                return;
            }

            if (Dispatcher.CheckAccess())
            {
                RefreshWidthSafe();
                return;
            }

            Dispatcher.Invoke(RefreshWidthSafe);
        }

        private void RefreshWidthSafe()
        {
            double maxWidth = SystemParameters.PrimaryScreenWidth;
            Width = maxWidth - ((maxWidth * this.currentTime) / MaxTime);
        }

        private void StartTimer()
        {
            ResetCurrentTime();
            RefreshCurrentTimeText();
            ResetBackgroundOffset();

            InitTimer();
        }

        private void ResetCurrentTime()
        {
            this.currentTime = MaxTime;
        }

        private void RefreshCurrentTimeText()
        {
            uint time = this.currentTime;
            uint seconds = time % 60;
            uint minutes = time / 60;

            CurrentTime = string.Format("{0:00} {1:00}", minutes, seconds);
        }

        private void ResetBackgroundOffset()
        {
            BackgroundOffset = new Thickness(0, 0, 0, 0);
        }

        private void InitTimer()
        {
            if (!ReferenceEquals(null, this.timer))
            {
                return;
            }

            this.timer = new Timer(OnTick);
            this.timer.Change(1000, 1000);
        }

        private void OnTick(object state)
        {
            DecrementCurrentTime();
            RefreshCurrentTimeText();

            DecrementBackgroundOffset();
            RefreshWidth();

            if (0 >= this.currentTime)
            {
                StopTimer();
                Alert();
            }
        }

        private void DecrementCurrentTime()
        {
            this.currentTime--;
        }

        private void DecrementBackgroundOffset()
        {
            var localOffset = BackgroundOffset;
            localOffset.Left--;

            BackgroundOffset = localOffset;
        }

        private void StopTimer()
        {
            if (ReferenceEquals(null, this.timer))
            {
                return;
            }

            this.timer.Dispose();
            this.timer = null;
        }

        #endregion Private Methods

        #region Constants and Fields

        private const int DefaultCaptionHeight = 20;

        private const int MaxTime = 25 * 60;

        private Thickness backgroundOffset;

        private double captionHeight;

        private uint currentTime;

        private string currentTimeText;

        private bool isHidden;

        private Timer timer;

        #endregion Constants and Fields
    }
}