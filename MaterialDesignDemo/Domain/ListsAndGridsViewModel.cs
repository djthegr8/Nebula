using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Nebula.MaterialDesignDemo.Domain
{
	public class ListsAndGridsViewModel : INotifyPropertyChanged
	{
		public bool? IsAllItems1Selected
		{
			get
			{
				List<bool> list = Items1.Select((SelectableViewModel item) => item.IsSelected).Distinct().ToList();
				if (list.Count != 1)
				{
					return null;
				}
				return list.Single();
			}
			set
			{
				if (value.HasValue)
				{
					SelectAll(value.Value, Items1);
					OnPropertyChanged("IsAllItems1Selected");
				}
			}
		}

		public ObservableCollection<SelectableViewModel> Items1 { get; }

		public ObservableCollection<SelectableViewModel> Items2 { get; }

		public ObservableCollection<SelectableViewModel> Items3 { get; }

		public IEnumerable<string> Foods => new string[4] { "Burger", "Fries", "Shake", "Lettuce" };

		public event PropertyChangedEventHandler PropertyChanged;

		public ListsAndGridsViewModel()
		{
			Items1 = CreateData();
			Items2 = CreateData();
			Items3 = CreateData();
			foreach (SelectableViewModel item in Items1)
			{
				item.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
				{
					if (args.PropertyName == "IsSelected")
					{
						OnPropertyChanged("IsAllItems1Selected");
					}
				};
			}
		}

		private static void SelectAll(bool select, IEnumerable<SelectableViewModel> models)
		{
			foreach (SelectableViewModel model in models)
			{
				model.IsSelected = select;
			}
		}

		private static ObservableCollection<SelectableViewModel> CreateData()
		{
			return new ObservableCollection<SelectableViewModel>
			{
				new SelectableViewModel
				{
					Code = 'M',
					Name = "Material Design",
					Description = "Material Design in XAML Toolkit"
				},
				new SelectableViewModel
				{
					Code = 'D',
					Name = "Dragablz",
					Description = "Dragablz Tab Control",
					Food = "Fries"
				},
				new SelectableViewModel
				{
					Code = 'P',
					Name = "Predator",
					Description = "If it bleeds, we can kill it"
				}
			};
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
