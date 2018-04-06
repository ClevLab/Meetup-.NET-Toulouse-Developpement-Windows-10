using System;
using System.Collections.Generic;
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

namespace SamplesMeetup.Views
{
    public sealed partial class MainFrame : Page
    {
        #region [ Constructors ]
        public MainFrame()
        {
            this.InitializeComponent();
        }
        #endregion [ Constructors ]


        #region [ Events ]
        #region [ Events - Navigations ]
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(this.frameMaster?.Content == null)
            this.frameMaster?.Navigate(typeof(VisualStateManagerPage));
        }
        #endregion [ Events - Navigations ]


        #region [ Events - Controls ]
        private void buttonVisualStateManager_Click(object sender, RoutedEventArgs e)
        {
            this.frameMaster?.Navigate(typeof(VisualStateManagerPage));
        }

        private void buttonAnimations_Click(object sender, RoutedEventArgs e)
        {
            this.frameMaster?.Navigate(typeof(AnimationsPage));
        }

        private void buttonInk_Click(object sender, RoutedEventArgs e)
        {
            this.frameMaster?.Navigate(typeof(InkPage));
        }
        #endregion [ Events - Controls ]
        #endregion [ Events ]

    }
}
