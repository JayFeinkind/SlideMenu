using Foundation;
using System;
using UIKit;
using SlideMenu;
using System.Linq;

namespace TestMenu
{
    public partial class NavigationController : UINavigationController
    {
		//SlideOutMenu<string> _menu;

        public NavigationController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			//_menu = new SlideOutMenu<string>(MenuPositionType.Top);
			////_menu.ChevronOffset = 100;
			////_menu.HideMenuBackgroundOnCollapse = false;
			//_menu.CollapsedMenuSize = 30;
			//_menu.AddRoomForNavigationBar = true;
			//_menu.AddMenuToSuperview(this.View, Enumerable.Range(0, 7).Select(n => n.ToString()), "0");
		}
    }
}