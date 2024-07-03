using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Translation_Organizer
{
    internal class ViewModel : INotifyPropertyChanged
    {
        //Variable Properties
        private string title;
        private string saveFilePath;
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
                    this.NotifyPropertyChanged(nameof(SelectedParagraph));
                    deleteParagraphCommand.InvokeCanExecuteChanged();
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
                    prevSentenceCommand.InvokeCanExecuteChanged();
                    nextSentenceCommand.InvokeCanExecuteChanged();
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
                prevSentenceCommand.InvokeCanExecuteChanged();
                nextSentenceCommand.InvokeCanExecuteChanged();
                addParagraphCommand.InvokeCanExecuteChanged();
                deleteParagraphCommand.InvokeCanExecuteChanged();
                exportCommand.InvokeCanExecuteChanged();
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
            set 
            { 
                paragraphs[paragraphIndex].JpSentences[sentenceIndex] = value;
                MakeProjectUnsaved();
            }
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
            set 
            { 
                paragraphs[paragraphIndex].RmjSentences[sentenceIndex] = value;
                MakeProjectUnsaved();
            }
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
            set 
            { 
                paragraphs[paragraphIndex].EnSentences[sentenceIndex] = value;
                MakeProjectUnsaved();
            }
        }
        public ObservableCollection<string> SelectedParagraph
        {
            get
            {
                if(paragraphs == null)
                {
                    return null;
                }
                return paragraphs[paragraphIndex].JpSentences;
            }
        }



        //Command Properties
        private CommandHandler blankNewCommand;
        private CommandHandler newCommand;
        private CommandHandler saveAsCommand;
        private CommandHandler saveCommand;
        private CommandHandler openCommand;
        private CommandHandler blankOpenCommand;
        private CommandHandler addSentenceCommand;
        private CommandHandler deleteSentenceCommand;
        private CommandHandler prevSentenceCommand;
        private CommandHandler nextSentenceCommand;
        private CommandHandler addParagraphCommand;
        private CommandHandler deleteParagraphCommand;
        private CommandHandler exportCommand;

        public ICommand BlankNewCommand
        {
            get { return blankNewCommand; }
        }
        public ICommand NewCommand
        {
            get { return newCommand; }
        }
        public ICommand SaveAsCommand
        {
            get { return saveAsCommand; }
        }
        public ICommand SaveCommand
        {
            get { return saveCommand; }
        }
        public ICommand OpenCommand
        {
            get { return openCommand; }
        }
        public ICommand BlankOpenCommand
        {
            get { return blankOpenCommand; }
        }
        public ICommand AddSentenceCommand
        {
            get { return addSentenceCommand; }
        }
        public ICommand DeleteSentenceCommand
        {
            get { return deleteSentenceCommand; }
        }
        public ICommand PrevSentenceCommand
        {
            get { return prevSentenceCommand; }
        }
        public ICommand NextSentenceCommand
        {
            get { return nextSentenceCommand; }
        }
        public ICommand AddParagraphCommand
        {
            get { return addParagraphCommand; }
        }
        public ICommand DeleteParagraphCommand
        {
            get { return deleteParagraphCommand; }
        }
        public ICommand ExportCommand
        {
            get { return exportCommand; }
        }

        //Constructor
        public ViewModel()
        {
            blankNewCommand = new CommandHandler(ExecuteNewCommand, CanExecuteNoProjectCommand);
            newCommand = new CommandHandler(ExecuteNewCommand);
            saveAsCommand = new CommandHandler(ExecuteSaveCommand, CanExecuteSaveAsCommand);
            saveCommand = new CommandHandler(ExecuteSaveCommand, CanExecuteSaveCommand);
            openCommand = new CommandHandler(ExecuteOpenCommand);
            blankOpenCommand = new CommandHandler(ExecuteOpenCommand, CanExecuteNoProjectCommand);
            addSentenceCommand = new CommandHandler(ExecuteAddSentenceCommand, CanExecuteIfProjectCommand);
            deleteSentenceCommand = new CommandHandler(ExecuteDeleteSentenceCommand, CanExecuteDeleteSentenceCommand);
            prevSentenceCommand = new CommandHandler(ExecutePrevSentenceCommand, CanExecutePrevSentenceCommand);
            nextSentenceCommand = new CommandHandler(ExecuteNextSentenceCommand, CanExecuteNextSentenceCommand);
            addParagraphCommand = new CommandHandler(ExecuteAddParagraphCommand, CanExecuteIfProjectCommand);
            deleteParagraphCommand = new CommandHandler(ExecuteDeleteParagraphCommand, CanExecuteDeleteParagraphCommand);
            exportCommand = new CommandHandler(ExecuteExportCommand, CanExecuteIfProjectCommand);
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
        //The Newcommand  condition that fires when there is no existing project
        private bool CanExecuteNoProjectCommand(object commandParameter)
        {
            if(paragraphs == null)
            {
                return true;
            }
            return false;
        }
        //The NewCommand that fires when there is an existing project
        private void ExecuteNewCommand(object commandParameter)
        {
            Title = "*";
            saveFilePath = "";
            ParagraphIndex = 0;
            Paragraphs = new ObservableCollection<ParagraphModel>() { new ParagraphModel() };
            Paragraphs[0].Init();
            ParagraphIndex = 0;
        }

        //Save Command Functions
        private bool CanExecuteSaveAsCommand(object commandParameter)
        {
            if (paragraphs == null || string.IsNullOrEmpty(saveFilePath) == false)  //saveFilePath check is for if the project has been saved before
            {
                return false;
            }
            return true;
        }
        //Save command will be the same for both saveAs and save commands just the difference of whether the title/file name is there or not
        private void ExecuteSaveCommand(object commandParameter)
        {
            //SaveAs command only
            if (commandParameter != null && string.IsNullOrEmpty(saveFilePath))
            {
                SaveFileDialog saveFileDialog = (SaveFileDialog)commandParameter;
                Title = saveFileDialog.SafeFileName;
                saveFilePath = saveFileDialog.FileName;
            }

            //Rest of save is shared by both save and saveAs commands
            using(TextWriter writer = new StreamWriter(saveFilePath))
            {
                XmlSerializer serializer = new XmlSerializer(Paragraphs.GetType());
                serializer.Serialize(writer, Paragraphs);
            }
            if (Title[0].Equals('*'))
            {
                Title = Title.Substring(1);
            }
        }
        private bool CanExecuteSaveCommand(object commandParameter)
        {
            //If there is an project then you can save, if not you cannot
            if (paragraphs == null)
            {
                return false;
            }
            if(title[0].Equals('*'))    //Only checks saves instead of save as so the title being empty won't affect condition check
            {
                return true;
            }
            return false;
        }
        //When user makes change to project and show that the project is in unsaved state
        private void MakeProjectUnsaved()
        {
            if (title[0].Equals('*') == false)
            {
                Title = "*" + Title;
            }
        }

        //Open Project Command
        private void ExecuteOpenCommand(object commandParameter)
        {
            OpenFileDialog openFileDialog = (OpenFileDialog)commandParameter;
            using(TextReader reader = new StreamReader(openFileDialog.FileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<ParagraphModel>));
                var temp = serializer.Deserialize(reader);
                if(temp != null)
                {
                    ParagraphIndex = 0;
                    Paragraphs = (ObservableCollection<ParagraphModel>)temp;
                    Title = openFileDialog.SafeFileName;
                    saveFilePath = openFileDialog.FileName;
                    ParagraphIndex = 0;
                }
            }
        }

        //Add sentence command
        public void ExecuteAddSentenceCommand(object commandParameter)
        {
            MakeProjectUnsaved();
            paragraphs[paragraphIndex].JpSentences.Insert(sentenceIndex+1, "");
            paragraphs[paragraphIndex].RmjSentences.Insert(sentenceIndex+1, "");
            paragraphs[paragraphIndex].EnSentences.Insert(sentenceIndex+1, "");
            SentenceIndex++;
        }

        //Delete Sentence command
        public void ExecuteDeleteSentenceCommand(object commandParameter)
        {
            MakeProjectUnsaved();
            if (sentenceIndex == paragraphs[paragraphIndex].JpSentences.Count - 1)
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
            if(paragraphs == null || paragraphs[ParagraphIndex].JpSentences.Count == 1)
            {
                return false;
            }
            return true;
        }
        //Previous/Next Sentence Commands
        public void ExecutePrevSentenceCommand(object commandParameter)
        {
            SentenceIndex--;
        }
        public bool CanExecutePrevSentenceCommand(object commandParameter)
        {
            if(paragraphs == null || sentenceIndex == 0)
            {
                return false;
            }
            return true;
        }
        public void ExecuteNextSentenceCommand(object commandParameter)
        {
            SentenceIndex++;
        }
        public bool CanExecuteNextSentenceCommand(object commandParameter)
        {
            if(paragraphs == null || sentenceIndex == (paragraphs[paragraphIndex].JpSentences.Count - 1))
            {
                return false;
            }
            return true;
        }
        //Add paragraph/ delete paragraph commands
        public void ExecuteAddParagraphCommand(object commandParameter)
        {
            MakeProjectUnsaved();
            paragraphs.Insert(paragraphIndex + 1, new ParagraphModel());
            Paragraphs[paragraphIndex + 1].Init();
            ParagraphIndex++;
        }
        public void ExecuteDeleteParagraphCommand(object commandParameter)
        {
            MakeProjectUnsaved();
            if (paragraphIndex == paragraphs.Count - 1)
            {
                paragraphs.RemoveAt(paragraphIndex);
                ParagraphIndex--;
                return;
            }
            paragraphs.RemoveAt(paragraphIndex);
            //To send the notifications of property changes
            ParagraphIndex = ParagraphIndex;
        }
        public bool CanExecuteDeleteParagraphCommand(object commandParameter)
        {
            if(paragraphs == null || paragraphs.Count == 1)
            {
                return false;
            }
            return true;
        }

        //Export to English command
        public void ExecuteExportCommand(object commandParameter)
        {
            string filePath = (string)commandParameter;
            ParagraphToTextConverter converter = new ParagraphToTextConverter();
            using(StreamWriter writer = File.CreateText(filePath))
            {
                foreach(ParagraphModel p in paragraphs)
                {
                    writer.WriteLine(converter.Convert(p.EnSentences, null, null, null));
                }
            }
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
