using System;
using System.Windows.Input;

namespace Budweg.Commands
{
    public class SaveCommand : RelayCommand
    {
        public SaveCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
            : base(execute, canExecute)
        {
        }
    }
}
