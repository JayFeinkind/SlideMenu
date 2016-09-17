using System;
using System.Collections.Generic;
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

		UITapGestureRecognizer _closeMenuTap;
		SlideOutMenu _menu;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.

			SetMenu(ContentPositionType.Center, MenuPositionType.Bottom);

			_closeMenuTap = new UITapGestureRecognizer(CloseMenu);
			_closeMenuTap.ShouldReceiveTouch = (recognizer, touch) => {

				var location = touch.LocationInView(View);

				var inMenu = _menu.Frame.Contains(location);

				return inMenu == false;
			};

			View.AddGestureRecognizer(_closeMenuTap);

			View.BackgroundColor = UIColor.Green;
		}

		private void SetMenu(ContentPositionType position, MenuPositionType menuPosition)
		{
			_menu = new SlideOutMenu(menuPosition);

			//------ these properties are preset to these values during initialization ----
			_menu.UIPosition = position;
			_menu.ShowCurrentSelection = true;
			_menu.AddRoomForNavigationBar = false;
			_menu.MenuBackgroundColor = UIColor.White;
			_menu.ChevronColor = UIColor.Black;
			_menu.DisplayLabelColor = UIColor.Black;
			_menu.MaxBackgroundAlpha = 1;
			_menu.CloseMenuOnSelection = true;
			_menu.UsesSpringAnimation = true;
			_menu.HideCurrentSelectionFromMenu = true;
			_menu.DisplayLabelFont = UIFont.BoldSystemFontOfSize(16);
			_menu.MenuShouldFillScreen = true;

			// NOTE: shadow does not currently use a UIBezierPath because that would require seperate
			//    animation as menu changes frame.  Currently using UIView.Animate api which does not
			//    animate layer changes.  Thes does have a negative affect on performance.  Future update
			//    will use CAAnimation to animate both changes
			_menu.UsesShadow = true;
			//-----------------------------------------------------------------------------

			// change full screen to false and set menu width
			_menu.MenuShouldFillScreen = false;
			_menu.ContentWidth = 400;

			var models = new List<MenuOptionModel>();

			models.Add(new MenuOptionModel { 
				Data = "New Parts Order",
				DisplayName = "New Parts Order",
				DisplayColor = View.TintColor,
				MenuOptionSelected = ShowAlert
			});
			models.Add(new MenuOptionModel
			{
				Data = "Service Parts Catalog",
				DisplayName = "Service Parts Catalog",
				DisplayColor = View.TintColor,
				MenuOptionSelected = ShowAlert
			});
			models.Add(new MenuOptionModel
			{
				Data = "Order History By Date",
				DisplayName = "Order History By Date",
				DisplayColor = View.TintColor,
				MenuOptionSelected = ShowAlert
			});
			models.Add(new MenuOptionModel
			{
				Data = "Select New Option",
				DisplayName = "Select New Option",
				DisplayColor = View.TintColor,
				MenuOptionSelected = ShowAlert
			});
			models.Add(new MenuOptionModel
			{
				Data = "Menu option",
				DisplayName = "Menu option",
				DisplayColor = View.TintColor,
				MenuOptionSelected = ShowAlert
			});
			models.Add(new MenuOptionModel
			{
				Data = "Running out of ideas",
				DisplayName = "Running out of ideas",
				DisplayColor = View.TintColor,
				MenuOptionSelected = ShowAlert
			});

			_menu.AddMenuToView(superView: this.View, values: models, presetValue: models.First());
		}

		private async void ShowAlert(object message)
		{
			string str = message as string;
			UIAlertController alert = UIAlertController.Create("Menu option selected", str, UIAlertControllerStyle.Alert);
			alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
			await PresentViewControllerAsync(alert, true);
		}

		private void CloseMenu()
		{
			if (_menu.MenuOpen)
			{
				_menu.AnimateClosed(null);
			}
		}
	}
}

