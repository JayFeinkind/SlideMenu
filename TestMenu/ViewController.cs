using System;
using System.Linq;
using SlideMenu;
using UIKit;

namespace TestMenu
{
	public partial class ViewController : UIViewController
	{
		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		SlideOutMenu _menu;
		SlideOutMenu _menuTwo;
		SlideOutMenu _menuThree;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.

			View.BackgroundColor = UIColor.Green;

			//_menuThree = GetMenu(ContentPositionType.Left, MenuPositionType.Top);
			_menu = GetMenu(ContentPositionType.Right, MenuPositionType.Bottom);
			//_menuTwo = GetMenu(ContentPositionType.Left, MenuPositionType.Bottom);

		}

		private SlideOutMenu GetMenu(ContentPositionType position, MenuPositionType menuPosition)
		{
			var menu = new SlideOutMenu(menuPosition);

			menu.HideMenuBackgroundOnCollapse = false;
			menu.ContentWidth = 300;
			menu.MenuShouldFillScreen = false;

			menu.UIPosition = position;
			//menu.ExpandedMenuSize = 150;
			menu.HideCurrentSelectionFromMenu = true;
			var values = Enumerable.Range(0, 7).Select(n => new MenuOptionModel
			{
				Data = n,
				DisplayName = n.ToString(),
				MenuOptionSelected = null
			});

			menu.AddMenuToView(this.View, values, null);
			return menu;
		}

		private void CloseMenu()
		{
			_menu.AnimateClosed(null);
			//_menuTwo.AnimateClosed(null);
			//_menuThree.AnimateClosed(null);
		}

		public override void TouchesBegan(Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);

			var touch = touches.AnyObject as UITouch;

			bool touchedOutside = true;

			if (touch.View == _menu) //|| touch.View == _menuTwo || touch.View == _menuThree)
				touchedOutside = false;

			foreach (var view in _menu.Subviews)
			{
				if (touch.View == view)
				{
					touchedOutside = false;
				}
			}

			//foreach (var view in _menuTwo.Subviews)
			//{
			//	if (touch.View == view)
			//	{
			//		touchedOutside = false;
			//	}
			//}

			//foreach (var view in _menuThree.Subviews)
			//{
			//	if (touch.View == view)
			//	{
			//		touchedOutside = false;
			//	}
			//}

			if (touchedOutside)
				CloseMenu();
		}
	}
}

