using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace vNekoChatUI.A.MVVM.Commands
{
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
