using System;
using UIKit;

namespace SlideMenu
{
	internal class MenuConstraintModel
	{
		public UIView MenuView { get; set; }
		public UIView ChevronView { get; set; }
		public UIView ChevronContainer { get; set; }
		public int ContentWidth { get; set; }
		public int CollapsedSize { get; set; }
		public bool MenuShouldFillScreen { get; set; }
		public ContentPositionType ContentPosition { get; set; }
	}
}

