﻿namespace HomeGroupExample
{
    using HomeGroupExample.Data;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += async (sender, args) =>
                {
                    var vm = new ViewModel();
                    this.DataContext = vm;
                    await vm.Initialize();
                };
        }
    }
}
