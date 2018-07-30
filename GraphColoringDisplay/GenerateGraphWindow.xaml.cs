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
using System.Windows.Shapes;

namespace GraphColoringDisplay
{
    /// <summary>
    /// Interaction logic for GenerateGraphWindow.xaml
    /// </summary>
    public partial class GenerateGraphWindow : Window
    {
        public GenerateGraphWindow()
        {
            InitializeComponent();
            this.Owner = App.Current.MainWindow;
        }

        public int NumberVertices
        {
            get
            {
                int numberVertices = Convert.ToInt32(txtNumberVertices.Text);
                return numberVertices;
            }
        }

        public double EdgeDensity
        {
            get
            {
                return Convert.ToDouble(txtEdgeDensity.Text);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
