using Foundation;
using System;
using UIKit;

namespace TestMenu
{
    public partial class PullMenuViewController : UIViewController
    {
        public PullMenuViewController (IntPtr handle) : base (handle)
        {
        }

		public override void LoadView()
		{
			base.LoadView();

			_pullMenu.SlideDirection = SlideMenu.SlideDirectionType.Left;
		}
    }
}