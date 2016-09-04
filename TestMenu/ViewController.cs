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

			_menu = new SlideOutMenu(MenuPositionType.Top);
			//_menu.ChevronOffset = 100;
			_menu.HideMenuBackgroundOnCollapse = false;
			_menu.AddRoomForNavigationBar = true;
			_menu.ExpandedMenuSize = 300;

			var values = Enumerable.Range(0, 7).Select(n => new MenuOptionModel { 
				Data = n,
				DisplayName = n.ToString(),
				MenuOptionSelected = null
			});


			_menu.AddMenuToSuperview(this.View, values, values.First());
		}

		public override void TouchesBegan(Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);

			var touch = touches.AnyObject as UITouch;

			var touchLocation = touch.LocationInView(_menu);

			if (touchLocation.X < 0 || touchLocation.Y > 0)
			{
				// Touch is outside of menu
			}
		}
	}
}

