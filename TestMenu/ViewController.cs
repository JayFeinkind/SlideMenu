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
		//SlideOutMenu _menuTwo;
		//SlideOutMenu _menuThree;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.

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
			_menu.MenuShouldFillScreen = false;
			_menu.ContentWidth = 400;
			var models = new List<MenuOptionModel>();

			models.Add(new MenuOptionModel { 
				Data = "New POS Order",
				DisplayName = "New POS Order"
			});
			models.Add(new MenuOptionModel
			{
				Data = "Service History Catalog",
				DisplayName = "Service History Catalog"
			});
			models.Add(new MenuOptionModel
			{
				Data = "Order History By Date",
				DisplayName = "Order History By Date"
			});
			models.Add(new MenuOptionModel
			{
				Data = "Select New Option",
				DisplayName = "Select New Option"
			});

			_menu.AddMenuToView(this.View, models, models.First());
		}

		private void CloseMenu()
		{
			_menu.AnimateClosed(null);
			//_menuTwo.AnimateClosed(null);
			//_menuThree.AnimateClosed(null);
		}
	}
}

