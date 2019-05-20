using System.Windows;

namespace SharePointClient
{
    /// <summary>
    /// Interaction logic for ModalWindow.xaml
    /// </summary>
    public partial class ModalWindow : Window
    {
        public ModalWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string Result
        {
            get { return textBox.Text; }
        }

        public string Info
        {
            get
            {
                return tbInfo.Text;
            }
            set
            {
                tbInfo.Text = value;
            }
        }
    }
}
