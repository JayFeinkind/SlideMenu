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

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.

			View.BackgroundColor = UIColor.Green;

			_menu = new SlideOutMenu(MenuPositionType.Bottom);

			_menu.HideMenuBackgroundOnCollapse = true;
			_menu.ContentWidth = 150;
			_menu.MenuShouldFillScreen = false;
			_menu.UIPosition = ContentPositionType.Center;

			var values = Enumerable.Range(0, 7).Select(n => new MenuOptionModel
			{
				Data = n,
				DisplayName = n.ToString(),
				MenuOptionSelected = null
			});

			_menu.AddMenuToView(this.View, values, values.First());
		}

		private void CloseMenu()
		{
			_menu.AnimateClosed(null);
		}

		public override void TouchesBegan(Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);

			var touch = touches.AnyObject as UITouch;

			bool touchedOutside = true;

			if (touch.View == _menu)
				touchedOutside = false;

			foreach (var view in _menu.Subviews)
			{
				if (touch.View == view)
				{
					touchedOutside = false;
				}
			}

			if (touchedOutside)
				CloseMenu();
		}
	}
}

