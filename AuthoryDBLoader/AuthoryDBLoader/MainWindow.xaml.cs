using System.Collections.Generic;
using System.Windows;

namespace AuthoryDBLoader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Effect> effects = new List<Effect>();
        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 7; i++)
            {
                effects.Add(new Effect());
            }
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
