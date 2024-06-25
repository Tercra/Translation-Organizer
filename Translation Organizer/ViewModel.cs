using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Translation_Organizer
{
    internal class ViewModel : INotifyPropertyChanged
    {
        //Variable Properties
        private string title;
        private int paragraphIndex;
        private int sentenceIndex;
        private ObservableCollection<ParagraphModel> paragraphs;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                this.NotifyPropertyChanged(nameof(title));
            }
        }
        public int ParagraphIndex 
        {
            get { return paragraphIndex; }
            set 
            { 
                paragraphIndex = value; 
                this.NotifyPropertyChanged(nameof(paragraphIndex));
            }
        }
        public int SentenceIndex
        {
            get { return sentenceIndex; }
            set 
            { 
                sentenceIndex = value; 
                this.NotifyPropertyChanged(nameof(sentenceIndex));
            }
        }
        public ObservableCollection<ParagraphModel> Paragraphs
        {
            get { return paragraphs; }
        }


        //Command Properties
        private ICommand newCommand;
        private ICommand saveCommand;

        public ICommand NewCommand
        {
            get
            {
                return newCommand;
            }
        }
        public ICommand SaveCommand
        {
            get
            {
                return saveCommand;
            }
        }

        //Constructor
        public ViewModel()
        {
            newCommand = new CommandHandler(ExecuteNewCommand);
            saveCommand = new CommandHandler(ExecuteSaveCommand, CanExecuteSaveCommand);
        }

        private class CommandHandler : ICommand
        {
            private readonly Action<object> executeAction;
            private readonly Func<object, bool> canExecuteAction;

            public CommandHandler(Action<object> executeActionPar)
            {
                executeAction = executeActionPar;
            }
            public CommandHandler(Action<object> executeActionPar, Func<object, bool> canExecuteActionPar)
            {
                executeAction = executeActionPar;
                canExecuteAction = canExecuteActionPar;
            }

            public bool CanExecute(object parameter) => canExecuteAction?.Invoke(parameter) ?? true;

            public event EventHandler? CanExecuteChanged;

            public void Execute(object parameter) => executeAction(parameter);
        }

        //NewCommand Functions
        private void ExecuteNewCommand(object commandParameter)
        {
            // Check if there is an ongoing project. If not just create the new project
            if(paragraphs == null)
            {
                paragraphIndex = 0;
                sentenceIndex = 0;
                paragraphs = new ObservableCollection<ParagraphModel>();
                paragraphs.Add(new ParagraphModel());
                this.NotifyPropertyChanged(nameof(paragraphs));
                return;
            }

            //If yes ask the user if they want to save the current project or cancel
            MessageBoxResult result = MessageBox.Show("Would you like to save the current project?", "Save Before Starting Anew", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    //Save before starting new project
                    break;
                case MessageBoxResult.No:
                    //Start new project without saving
                    paragraphIndex = 0;
                    sentenceIndex = 0;
                    paragraphs = new ObservableCollection<ParagraphModel>();
                    paragraphs.Add(new ParagraphModel());
                    this.NotifyPropertyChanged(nameof(paragraphIndex));
                    this.NotifyPropertyChanged(nameof(sentenceIndex));
                    this.NotifyPropertyChanged(nameof(paragraphs));
                    break;
                case MessageBoxResult.Cancel:
                    //Nothing happens
                    break;
            }
        }

        //Save Command Functions
        private void ExecuteSaveCommand(object commandParameter)
        {
            /*
             * Open up the save dialogue box
             * Check the result of the box
             * if there is a file location then check for duplicate
             * if duplicate ask user if they would like to overwrite
             */
        }

        private bool CanExecuteSaveCommand(object commandParameter)
        {
            //If there is an project then you can save, if not you cannot
            if (paragraphs == null)
            {
                return false;
            }
            return true;
        }


        //Notify Property Changes
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
