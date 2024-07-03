using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Translation_Organizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ViewModel viewModel = new ViewModel();
            DataContext = viewModel;
            InitializeComponent();
        }

        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as ViewModel;
            if(viewModel == null)
            {
                return;
            }
            if(viewModel.BlankNewCommand.CanExecute(null))
            {
                viewModel.BlankNewCommand.Execute(null);
                return;
            }

            if (viewModel.SaveAsCommand.CanExecute(null) == false && viewModel.SaveCommand.CanExecute(null) == false)
            {
                viewModel.NewCommand.Execute(null);
                return;
            }
            MessageBoxResult result = MessageBox.Show("Would you like to save the current project?", "Save Before Starting Anew", MessageBoxButton.YesNoCancel);
            if(result == MessageBoxResult.Yes)
            {
                if (viewModel.SaveAsCommand.CanExecute(null))
                {
                    //Get result from savedialoguebox and give it to SaveAsCommand
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "Project";
                    saveFileDialog.DefaultExt = ".txt";
                    saveFileDialog.Filter = "TO Created Text Files (*.txt)|*.txt";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        viewModel.SaveAsCommand.Execute(saveFileDialog);
                    }
                    else { return; }    //Does not make a new project if the save dialogue returns false
                }
                else if (viewModel.SaveCommand.CanExecute(null))    //else if here and not in save event handler because it was handled by returned and cannot be here
                {
                    viewModel.SaveCommand.Execute(null);
                }
            }
            if(result != MessageBoxResult.Cancel)
            {
                viewModel.NewCommand.Execute(null);
            }
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as ViewModel;
            if(viewModel == null)
            {
                return;
            }
            if(viewModel.SaveAsCommand.CanExecute(null))
            {
                //Get result from savedialoguebox and give it to SaveAsCommand
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = "Project";
                saveFileDialog.DefaultExt = ".txt";
                saveFileDialog.Filter = "TO Created Text Files (*.txt)|*.txt";
                if (saveFileDialog.ShowDialog() == true)
                {
                    viewModel.SaveAsCommand.Execute(saveFileDialog);
                }
                return;
            }
            if (viewModel.SaveCommand.CanExecute(null))
            {
                viewModel.SaveCommand.Execute(null);
            }
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as ViewModel;
            if (viewModel == null)
            {
                return;
            }
            if(viewModel.BlankOpenCommand.CanExecute(null))
            {
                //Opening Dialogue box
                OpenFilePrompt(viewModel);
                return;
            }

            //Should you save before opening?
            //If there is no need to save then no need to ask
            if(viewModel.SaveAsCommand.CanExecute(null) == false && viewModel.SaveCommand.CanExecute(null) == false)
            {
                OpenFilePrompt(viewModel);
                return;
            }
            MessageBoxResult result = MessageBox.Show("Would you like to save the current project?", "Save Before Opening", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes)
            {
                if (viewModel.SaveAsCommand.CanExecute(null))
                {
                    //Get result from savedialoguebox and give it to SaveAsCommand
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "Project";
                    saveFileDialog.DefaultExt = ".txt";
                    saveFileDialog.Filter = "TO Created Text Files (*.txt)|*.txt";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        viewModel.SaveAsCommand.Execute(saveFileDialog);
                    }
                    else { return; }    //Does not make a new project if the save dialogue returns false
                }
                else if (viewModel.SaveCommand.CanExecute(null))    //else if here and not in save event handler because it was handled by returned and cannot be here
                {
                    viewModel.SaveCommand.Execute(null);
                }
            }
            if (result == MessageBoxResult.Cancel)
            {
                return;
            }

            //OpenFile Dialogue Section
            OpenFilePrompt(viewModel);
        }

        private void OpenFilePrompt(ViewModel viewModel)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "TO Created Text Files (*.txt)|*.txt";
            if(openFileDialog.ShowDialog() == true)
            {
                try
                {
                    viewModel.OpenCommand.Execute(openFileDialog);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("File was not a TO created text file or an error occured.", "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                    //MessageBox.Show(ex.Message);
                }
            }
        }

        private void ExportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as ViewModel;
            if (viewModel == null)
            {
                return;
            }
            if (!viewModel.ExportCommand.CanExecute(null))
            {
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "ExportProject";
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                viewModel.ExportCommand.Execute(saveFileDialog.FileName);
            }
        }
    }
}