using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
                this.NotifyPropertyChanged(nameof(Title));
            }
        }
        public int ParagraphIndex 
        {
            get { return paragraphIndex; }
            set 
            { 
                if(value >= 0)
                {
                    //Any changes in paragraph would naturally change the sentenceIndex
                    paragraphIndex = value;
                    SentenceIndex = 0;
                    this.NotifyPropertyChanged(nameof(ParagraphIndex));
                }
            }
        }
        public int SentenceIndex
        {
            get { return sentenceIndex; }
            set 
            { 
                if(value >= 0)
                {
                    sentenceIndex = value;
                    this.NotifyPropertyChanged(nameof(SentenceIndex));
                    this.NotifyPropertyChanged(nameof(SelectedJpSentence));
                    this.NotifyPropertyChanged(nameof(SelectedRmjSentence));
                    this.NotifyPropertyChanged(nameof(SelectedEnSentence));
                    deleteSentenceCommand.InvokeCanExecuteChanged();
                }
            }
        }
        public ObservableCollection<ParagraphModel> Paragraphs
        {
            get { return paragraphs; }
            set 
            { 
                paragraphs = value;
                this.NotifyPropertyChanged(nameof(Paragraphs));
                addSentenceCommand.InvokeCanExecuteChanged();
                deleteSentenceCommand.InvokeCanExecuteChanged();
            }
        }
        //Wrapper Properties (In order to bind to selected items)
        public string SelectedJpSentence
        {
            get 
            {
                if(paragraphs == null)
                {
                    return "";
                }
                return paragraphs[paragraphIndex].JpSentences[sentenceIndex]; 
            }
            set { paragraphs[paragraphIndex].JpSentences[sentenceIndex] = value; }
        }
        public string SelectedRmjSentence
        {
            get 
            {
                if (paragraphs == null)
                {
                    return "";
                }
                return paragraphs[paragraphIndex].RmjSentences[sentenceIndex]; 
            }
            set { paragraphs[paragraphIndex].RmjSentences[sentenceIndex] = value; }
        }
        public string SelectedEnSentence
        {
            get 
            {
                if (paragraphs == null)
                {
                    return "";
                }
                return paragraphs[paragraphIndex].EnSentences[sentenceIndex]; 
            }
            set { paragraphs[paragraphIndex].EnSentences[sentenceIndex] = value; }
        }



        //Command Properties
        private CommandHandler newCommand;
        private CommandHandler saveCommand;
        private CommandHandler addSentenceCommand;
        private CommandHandler deleteSentenceCommand;

        public ICommand NewCommand
        {
            get { return newCommand; }
        }
        public ICommand SaveCommand
        {
            get { return saveCommand; }
        }
        public ICommand AddSentenceCommand
        {
            get { return addSentenceCommand; }
        }
        public ICommand DeleteSentenceCommand
        {
            get { return deleteSentenceCommand; }
        }

        //Constructor
        public ViewModel()
        {
            newCommand = new CommandHandler(ExecuteNewCommand);
            saveCommand = new CommandHandler(ExecuteSaveCommand, CanExecuteIfProjectCommand);
            addSentenceCommand = new CommandHandler(ExecuteAddSentenceCommand, CanExecuteIfProjectCommand);
            deleteSentenceCommand = new CommandHandler(ExecuteDeleteSentenceCommand, CanExecuteDeleteSentenceCommand);
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

            public void InvokeCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        //The command canexecution function that will only work if a project is open/exists
        private bool CanExecuteIfProjectCommand(object commandParameter)
        {
            //If there is an project then you can save, if not you cannot
            if (paragraphs == null)
            {
                return false;
            }
            return true;
        }

        //NewCommand Functions
        private void ExecuteNewCommand(object commandParameter)
        {
            // Check if there is an ongoing project. If not just create the new project
            if(paragraphs == null)
            {
                ParagraphIndex = 0;
                Paragraphs = new ObservableCollection<ParagraphModel>() { new ParagraphModel() };
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
                    ParagraphIndex = 0;
                    Paragraphs = new ObservableCollection<ParagraphModel>() { new ParagraphModel() };
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

        //Add sentence command
        public void ExecuteAddSentenceCommand(object commandParameter)
        {
            paragraphs[paragraphIndex].JpSentences.Insert(sentenceIndex+1, "");
            paragraphs[paragraphIndex].RmjSentences.Insert(sentenceIndex+1, "");
            paragraphs[paragraphIndex].EnSentences.Insert(sentenceIndex+1, "");
            SentenceIndex++;
        }

        //Delete Sentence command
        public void ExecuteDeleteSentenceCommand(object commandParameter)
        {
            if(sentenceIndex == paragraphs[paragraphIndex].JpSentences.Count - 1)
            {
                paragraphs[paragraphIndex].JpSentences.RemoveAt(sentenceIndex);
                paragraphs[paragraphIndex].RmjSentences.RemoveAt(sentenceIndex);
                paragraphs[paragraphIndex].EnSentences.RemoveAt(sentenceIndex);
                SentenceIndex--;
                return;
            }
            paragraphs[paragraphIndex].JpSentences.RemoveAt(sentenceIndex);
            paragraphs[paragraphIndex].RmjSentences.RemoveAt(sentenceIndex);
            paragraphs[paragraphIndex].EnSentences.RemoveAt(sentenceIndex);
            //In order to call the property change notification and the set off the can execute properties
            SentenceIndex = SentenceIndex;
        }
        public bool CanExecuteDeleteSentenceCommand(object commandParameter)
        {
            if(paragraphs == null)
            {
                return false;
            }
            if(paragraphs[ParagraphIndex].JpSentences.Count == 1)
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
