﻿namespace ProximityExample
{
    using ProximityExample.Data;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage 
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Unloaded += (o, e) => ((ViewModel)this.Resources["ViewModel"]).Dispose();
        }
    }
}
