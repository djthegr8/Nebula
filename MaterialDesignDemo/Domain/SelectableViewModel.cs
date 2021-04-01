using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nebula.MaterialDesignDemo.Domain
{
	public class SelectableViewModel : INotifyPropertyChanged
	{
		private bool _isSelected;

		private string _name;

		private string _description;

		private char _code;

		private double _numeric;

		private string _food;

		public bool IsSelected
		{
			get
			{
				return _isSelected;
			}
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					OnPropertyChanged("IsSelected");
				}
			}
		}

		public char Code
		{
			get
			{
				return _code;
			}
			set
			{
				if (_code != value)
				{
					_code = value;
					OnPropertyChanged("Code");
				}
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if (!(_name == value))
				{
					_name = value;
					OnPropertyChanged("Name");
				}
			}
		}

		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				if (!(_description == value))
				{
					_description = value;
					OnPropertyChanged("Description");
				}
			}
		}

		public double Numeric
		{
			get
			{
				return _numeric;
			}
			set
			{
				if (!(Math.Abs(_numeric - value) < 0.01))
				{
					_numeric = value;
					OnPropertyChanged("Numeric");
				}
			}
		}

		public string Food
		{
			get
			{
				return _food;
			}
			set
			{
				if (!(_food == value))
				{
					_food = value;
					OnPropertyChanged("Food");
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
