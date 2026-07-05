using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProductMonitor.MVVMTemplate
{
    internal class RelayCommand : ICommand
    {
        private readonly Action _excute;
        private readonly Func<bool>? _canExcute;

        public RelayCommand(Action excute, Func<bool>? canExcute = null)
        {
            _excute = excute;
            _canExcute = canExcute;
        }
        public bool CanExecute(object? parameter)
        {
            if (_canExcute == null)
                return true;
            return _canExcute();
        }
        public void Execute(object? parameter) => _excute();
        public event EventHandler? CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}
