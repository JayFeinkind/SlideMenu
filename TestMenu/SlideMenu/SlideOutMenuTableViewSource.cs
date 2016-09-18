using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace TestMenu
{
	public class SlideOutMenuTableViewSource : UITableViewSource
	{
		const string DefaultCellIdentifier = "DefaultCell";
		readonly IEnumerable<string> _values;
		readonly Action<string> _rowSelected;

		public SlideOutMenuTableViewSource(IEnumerable<string> values, Action<string> rowSelected)
		{
			_rowSelected = rowSelected;
			_values = values;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(DefaultCellIdentifier)
						?? new UITableViewCell(UITableViewCellStyle.Default, DefaultCellIdentifier);

			cell.BackgroundColor = UIColor.Clear;

			var val = _values.ElementAt(indexPath.Row);

			cell.TextLabel.Text = val;
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

			// row selected handler is for menu notification
			if (_rowSelected != null)
			{
				_rowSelected(type);
			}
		}
	}
}
