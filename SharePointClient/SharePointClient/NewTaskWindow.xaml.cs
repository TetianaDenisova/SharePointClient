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
            var priorityIndex = cbPriority.Items.IndexOf(cbPriority.Items.OfType<TextBlock>().Single(p => p.Text == editTask.Priority));
            cbPriority.SelectedIndex = priorityIndex;

            var statusIndex = cbStatus.Items.IndexOf(cbStatus.Items.OfType<TextBlock>().Single(p => p.Text == editTask.Status));
            cbStatus.SelectedIndex = statusIndex;
            dpicker.SelectedDate = editTask.DueDate;
        }
    }
}
