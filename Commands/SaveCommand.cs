using Budweg.Models;
using Budweg.View_Models;
using System;
using System.Windows.Input;

namespace Budweg.Commands
{
    public class AddCaliperCommand : ICommand
    {
        private readonly CaliberRepository _repository;

        public AddCaliperCommand(CaliberRepository repository)
        {
            _repository = repository;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            // Hvis du vil validere input senere, gøres det her.
            return parameter is Caliper;
        }

        public void Execute(object? parameter)
        {
            if (parameter is Caliper caliper)
            {
                _repository.AddCaliber(caliper);
            }
        }
    }
}
