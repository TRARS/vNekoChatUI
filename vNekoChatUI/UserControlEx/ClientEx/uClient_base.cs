using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace vNekoChatUI.UserControlEx.ClientEx
{
    public class RelayCommand : ICommand
    {
        readonly Action<object?> _execute;
        readonly Func<object?, bool> _canExecute;

        public RelayCommand(Action<object?> execute) : this(execute, null!) { }
        public RelayCommand(Action<object?> execute, Func<object?, bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute; _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void Execute(object? parameter) { _execute(parameter); }
    }

    public class TextBoxPasteCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            string customText = parameter as string;
            var textBox = Keyboard.FocusedElement as TextBox;
            if (textBox != null)
            {
                int caretIndex = textBox.CaretIndex;

                textBox.Text = textBox.Text.Insert(caretIndex, customText);
                textBox.CaretIndex = caretIndex + customText.Length;
            }
        }
    }


}
