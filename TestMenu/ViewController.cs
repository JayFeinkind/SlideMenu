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

		SlideOutMenu<string> _menu;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.

			View.BackgroundColor = UIColor.Green;

			_menu = new SlideOutMenu<string>(MenuPositionType.Bottom);
			//_menu.ChevronOffset = 100
			//_menu.HideMenuBackgroundOnCollapse = false
			_menu.AddRoomForNavigationBar = false;
			_menu.AddMenuToSuperview(this.View, Enumerable.Range(0, 7).Select(n => n.ToString()), "0");
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

