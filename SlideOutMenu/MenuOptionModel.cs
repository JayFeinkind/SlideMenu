using System;
using UIKit;

namespace SlideMenu
{
	public class MenuOptionModel
	{
		public string DisplayName { get; set; }
		public object Data { get; set; }
		public Action<object> MenuOptionSelected { get; set; }
		public UIColor DisplayColor { get; set; }
	}
}

