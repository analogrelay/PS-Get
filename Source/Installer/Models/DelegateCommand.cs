using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace PsGet.Installer.Models {
    public class DelegateCommand : ICommand {
        private bool _canExecute = true;

        public event EventHandler CanExecuteChanged;

        public Action<object> Action { get; private set; }
        public bool CommandCanExecute {
            get { return _canExecute; }
            set {
                if (_canExecute != value) {
                    _canExecute = value;
                    OnCanExecuteChanged(EventArgs.Empty);
                }
            }
        }

        public DelegateCommand(Action<object> action) : this(action, canExecute: true) { }

        public DelegateCommand(Action<object> action, bool canExecute) {
            Action = action;
            CommandCanExecute = canExecute;
        }

        protected virtual void OnCanExecuteChanged(EventArgs args) {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) {
                handler(this, args);
            }
        }

        public bool CanExecute(object parameter) {
            return CommandCanExecute;
        }

        public void Execute(object parameter) {
            Action(parameter);
        }
    }
}
