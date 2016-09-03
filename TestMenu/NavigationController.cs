using Foundation;
using System;
using UIKit;
using SlideMenu;
using System.Linq;

namespace TestMenu
{
    public partial class NavigationController : UINavigationController
    {
		SlideOutMenu<string> _menu;

        public NavigationController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			_menu = new SlideOutMenu<string>(MenuPositionType.Bottom);
			//_menu.ChevronOffset = 100;
			_menu.AddMenuToSuperview(this.View, Enumerable.Range(0, 7).Select(n => n.ToString()), "0");
		}
    }
}