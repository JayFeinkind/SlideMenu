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

		public MenuTableViewSource(IEnumerable<MenuOptionModel> values)
		{
			_values = values;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(DefaultCellIdentifier)
						?? new UITableViewCell(UITableViewCellStyle.Default, DefaultCellIdentifier);

			cell.BackgroundColor = UIColor.Clear;

			cell.TextLabel.Text = _values.ElementAt(indexPath.Row).DisplayName;
			cell.TextLabel.TextColor = tableView.TintColor;
			cell.TextLabel.Lines = 0;
			cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
			cell.TextLabel.Font = UIFont.BoldSystemFontOfSize(17);
			cell.TextLabel.TextAlignment = UITextAlignment.Center;

			return cell;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return _values.Count();
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			tableView.BeginUpdates();
			tableView.DeselectRow(indexPath, true);
			tableView.EndUpdates();

			var type = _values.ElementAt(indexPath.Row);

			if (type.MenuOptionSelected != null)
			{
				type.MenuOptionSelected(type.Data);
			}
		}
	}
}

