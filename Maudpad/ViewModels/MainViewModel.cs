using ICSharpCode.AvalonEdit.Document;
using Maudpad.MVVM;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Maudpad.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Document Options
        private bool _hasDocumentChanged = false;
        public bool HasDocumentChanged
        {
            get
            {
                return _hasDocumentChanged;
            }
            set
            {
                _hasDocumentChanged = value;
                OnPropertyChanged(nameof(TitleText));
            }
        }

        private string _openedFilePath = null;
        public string OpenedFilePath
        {
            get
            {
                return _openedFilePath;
            }
            set
            {
                _openedFilePath = value;
                OnPropertyChanged(nameof(OpenedFileName));
                OnPropertyChanged(nameof(TitleText));
            }
        }

        public string OpenedFileName
        {
            get
            {
                if (OpenedFilePath == null)
                {
                    return "[Untitled]";
                }
                else
                {
                    return Path.GetFileName(OpenedFilePath);
                }
            }
        }

        private TextDocument _textContent = new TextDocument();
        public TextDocument TextContent
        {
            get { return _textContent; }
            set { _textContent = value; OnPropertyChanged(nameof(TextContent)); }
        }

        #endregion

        #region Appearance Options
        private int _editorFontSize = 25;
        public int EditorFontSize
        {
            get { return _editorFontSize; }
            set { _editorFontSize = value; OnPropertyChanged(nameof(EditorFontSize)); }
        }

        public string TitleText
        {
            get
            {
                string title = $"{OpenedFileName} - maudpad";
                if (HasDocumentChanged)
                    title = '*' + title;
                return title;
            }
        }
        #endregion

        public ICommand ZoomInOutCommand { get; }
        public ICommand HelpCommand { get; }
        public ICommand OpenFileCommand { get; }
        public ICommand SaveFileCommand { get; }
        public ICommand SaveFileAsCommand { get; }
        public ICommand DocumentChangedCommand { get; }

        public MainViewModel()
        {
            ZoomInOutCommand = new RelayCommand(ZoomInOut);
            HelpCommand = new RelayCommand(Help);
            OpenFileCommand = new RelayCommand(OpenFile);
            SaveFileCommand = new RelayCommand(SaveFile, (param) => HasDocumentChanged);
            SaveFileAsCommand = new RelayCommand(SaveFileAs);
            DocumentChangedCommand = new RelayCommand(DocumentChanged);
        }

        private void ZoomInOut(object param)
        {
            var delta = (int?)param;

            if (delta < 0)
            {
                if (EditorFontSize > 5)
                {
                    EditorFontSize -= 5;
                }
            }
            else
            {
                EditorFontSize += 5;
            }
        }

        private void Help(object param)
        {
            MessageBox.Show("maudpad tries to be an enhanced version of Windows' notepad\n" +
                            "It was made with love by Monsieur Puel\n\n"+
                            "https://github.com/Jomtek/maudpad",
                            "maudpad - Help",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenFile(object param)
        {
            if (HasDocumentChanged)
            {
                var dialog = MessageBox.Show($"Do you want to save changes to {OpenedFileName}?", "Save - maudpad", MessageBoxButton.YesNoCancel);
                if (dialog == MessageBoxResult.Cancel)
                {
                    return;
                }
                else if (dialog == MessageBoxResult.Yes)
                {
                    // TODO
                }
            }

            var ofd = new OpenFileDialog()
            {
                CheckFileExists = true,
                Title = "Open a file",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (ofd.ShowDialog().GetValueOrDefault())
            {
                // TODO: manage exceptions
                var content = File.ReadAllText(ofd.FileName);
                TextContent.Text = content;

                HasDocumentChanged = false;

                OpenedFilePath = ofd.FileName;
            }
        }

        private void SaveFile(object param)
        {
            if (OpenedFilePath == null)
            {
                SaveFileAs(param);
                return;
            }

            // TODO: manage exceptions
            File.WriteAllText(OpenedFilePath, TextContent.Text);

            HasDocumentChanged = false;
        }

        private void SaveFileAs(object param)
        {
            var sfd = new SaveFileDialog()
            {
                Title = $"Save '{OpenedFileName}'",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            // TODO: manage exceptions
            if (sfd.ShowDialog().GetValueOrDefault())
            {
                File.WriteAllText(sfd.FileName, TextContent.Text);
                OpenedFilePath = sfd.FileName;
                HasDocumentChanged = false;
            }
        }

        private void DocumentChanged(object param)
        {
            HasDocumentChanged = true;
            OnPropertyChanged(nameof(TitleText));
        }
    }
}