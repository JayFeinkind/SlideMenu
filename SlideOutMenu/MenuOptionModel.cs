using System;

namespace SlideMenu
{
	public class MenuOptionModel
	{
		public string DisplayName { get; set; }
		public object Data { get; set; }
		public Action<object> MenuOptionSelected { get; set; }
	}
}

