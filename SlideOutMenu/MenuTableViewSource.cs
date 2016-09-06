using System;
using System.Collections.Generic;
using Foundation;
using System.Linq;
using UIKit;

namespace SlideMenu
{
	public class MenuTableViewSource : UITableViewSource
	{
		const string DefaultCellIdentifier = "DefaultCell";
		IEnumerable<MenuOptionModel> _values;
		Action<MenuOptionModel> _rowSelected;
		bool _hideCurrentSelection;
		MenuOptionModel _currentSelection;

		public MenuTableViewSource(IEnumerable<MenuOptionModel> values, Action<MenuOptionModel> rowSelected, bool hideCurrentSelection, MenuOptionModel currentSelection)
		{
			_hideCurrentSelection = hideCurrentSelection;
			_currentSelection = currentSelection;
			_rowSelected = rowSelected;
			_values = values;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(DefaultCellIdentifier)
						?? new UITableViewCell(UITableViewCellStyle.Default, DefaultCellIdentifier);

			cell.BackgroundColor = UIColor.Clear;

			cell.TextLabel.Text = Values.ElementAt(indexPath.Row).DisplayName;
			cell.TextLabel.TextColor = tableView.TintColor;
			cell.TextLabel.Lines = 0;
			cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
			cell.TextLabel.Font = UIFont.BoldSystemFontOfSize(17);
			cell.TextLabel.TextAlignment = UITextAlignment.Center;

			return cell;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return Values.Count();
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			tableView.BeginUpdates();
			tableView.DeselectRow(indexPath, true);
			tableView.EndUpdates();

			var type = Values.ElementAt(indexPath.Row);

			_currentSelection = type;

			// row selected handler is for menu notification.
			if (_rowSelected != null)
			{
				_rowSelected(type);
			}

			// invoke any method user added
			if (type.MenuOptionSelected != null)
			{
				type.MenuOptionSelected(type.Data);
			}
		}

		private IEnumerable<MenuOptionModel> Values
		{
			get
			{
				if (_hideCurrentSelection)
				{
					return _values.Where (v => v.DisplayName != _currentSelection.DisplayName);
				}

				return _values;
			}
			set
			{
				_values = value;
			}
		}
	}
}

