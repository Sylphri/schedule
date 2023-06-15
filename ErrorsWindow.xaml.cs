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

namespace schedule
{
    /// <summary>
    /// Логика взаимодействия для ErrorsWindow.xaml
    /// </summary>
    public partial class ErrorsWindow : Window
    {
        public ErrorsWindow()
        {
            InitializeComponent();

            this.Closing += (object sender, System.ComponentModel.CancelEventArgs e) =>
            {
                e.Cancel = true;
                this.Hide();
            };
        }

        internal void ShowErrors(List<ScheduleCheckResult> errors)
        {
            mainGrid.RowDefinitions.Clear();
            mainGrid.Children.Clear();

            // Add headers

            TextBox[] headersTextBoxes = new TextBox[2];
            headersTextBoxes[0] = new TextBox();
            headersTextBoxes[0].Text = "Назва помилки";
            headersTextBoxes[0].IsReadOnly = true;
            headersTextBoxes[1] = new TextBox();
            headersTextBoxes[1].Text = "Інформація";
            headersTextBoxes[1].IsReadOnly = true;

            RowDefinition headerRowDefinition = new RowDefinition();
            headerRowDefinition.Height = new System.Windows.GridLength(24);
            mainGrid.RowDefinitions.Add(headerRowDefinition);

            for (int i = 0; i < 2; ++i)
            {
                Grid.SetColumn(headersTextBoxes[i], i);
                mainGrid.Children.Add(headersTextBoxes[i]);
            }

            var errorsArr = errors.ToArray();
            for (int i = 0; i<errors.Count; ++i)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new System.Windows.GridLength(24);
                mainGrid.RowDefinitions.Add(rowDefinition);


                TextBox errorNameTextBox = new TextBox();
                errorNameTextBox.IsReadOnly = true;
                errorNameTextBox.Text = errors[i].ErrorName;
                Grid.SetRow(errorNameTextBox, i+1);
                Grid.SetColumn(errorNameTextBox, 0);
                mainGrid.Children.Add(errorNameTextBox);

                TextBox messageTextBox = new TextBox();
                messageTextBox.IsReadOnly = true;
                messageTextBox.Text = errors[i].Message;
                Grid.SetRow(messageTextBox, i+1);
                Grid.SetColumn(messageTextBox, 1);
                mainGrid.Children.Add(messageTextBox);
            }
        }
    }
}
