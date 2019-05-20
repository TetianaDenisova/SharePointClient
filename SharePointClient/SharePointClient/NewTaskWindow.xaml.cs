using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SharePointClient
{
    /// <summary>
    /// Interaction logic for NewTaskWindow.xaml
    /// </summary>
    public partial class NewTaskWindow : Window
    {
        private Task editTask;
        public NewTaskWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var newTask = new Task();
            newTask.Title = tbTitle.Text;
            newTask.PercentComplete = tbComplete.Text != string.Empty ? double.Parse(tbComplete.Text) : 0.0;
            newTask.Description = tbDescription.Text;
            newTask.Priority = cbPriority.SelectedItem != null ?
                ((TextBlock)cbPriority.SelectedItem).Text
                : "Low";
            newTask.Status = cbStatus.SelectedItem != null ? ((TextBlock)cbStatus.SelectedItem).Text : "Not started";
            newTask.DueDate = dpicker.SelectedDate;
            editTask = newTask;
            this.DialogResult = true;
        }

        public Task EditTask
        {
            get
            {
                if (editTask == null)
                {
                    editTask = new Task("");
                }
                return editTask;
            }
            set
            {
                editTask = value;
                setModel(editTask);
            }
        }

        private void setModel(Task editTask)
        {
            tbTitle.Text = editTask.Title;
            tbComplete.Text = editTask.PercentComplete.ToString();
            tbDescription.Text = editTask.Description;
            cbPriority.SelectedItem = cbPriority.Items.OfType<TextBlock>().Where(p => ((p)).Text == editTask.Priority);
            cbStatus.SelectedItem = cbStatus.Items.OfType<TextBlock>().Where(p => ((p)).Text == editTask.Status);
            dpicker.SelectedDate = editTask.DueDate;
        }
    }
}
