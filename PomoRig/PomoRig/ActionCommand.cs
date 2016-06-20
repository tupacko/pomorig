#region Using Directives

using System;
using System.Windows.Input;

#endregion Using Directives

namespace PomoRig
{
	internal class ActionCommand : ICommand
	{
		#region Constructors

		public ActionCommand(Action action)
		{
			this.action = action;
		}

		#endregion Constructors

		#region Public Events

		public event EventHandler CanExecuteChanged;

		#endregion Public Events

		#region Public Methods

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			this.action();
		}

		#endregion Public Methods

		#region Constants and Fields

		private readonly Action action;

		#endregion Constants and Fields
	}
}