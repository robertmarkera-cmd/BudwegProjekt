using System;
using System.Windows.Input;

namespace Budweg.Commands
{
    public class CancelCommand : RelayCommand
    {
        public CancelCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
            : base(execute, canExecute)
        {
        }
    }
}
