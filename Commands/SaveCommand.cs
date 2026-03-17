using System;
using System.Windows.Input;
using Budweg.View_Models;
using Budweg.Models;

namespace Budweg.Commands
{
    public class SaveCommand : ICommand
    {
        private readonly CaliberRepository _repo;
        private readonly Caliper _cal;

        public SaveCommand(CaliberRepository repo, Caliper cal)
        {
            _repo = repo;
            _cal = cal;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            _repo.AddCaliber(_cal);
        }
    }
}