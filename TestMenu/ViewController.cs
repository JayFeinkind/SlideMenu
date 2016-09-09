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

			//_menuThree = GetMenu(ContentPositionType.Left, MenuPositionType.Top);
			_menu = GetMenu(ContentPositionType.Center, MenuPositionType.Bottom);
			//_menuTwo = GetMenu(ContentPositionType.Left, MenuPositionType.Bottom);

		}

		private SlideOutMenu GetMenu(ContentPositionType position, MenuPositionType menuPosition)
		{
			var menu = new SlideOutMenu(menuPosition);
			menu.UIPosition = ContentPositionType.Right;
			//menu.UIPosition = position;
			//menu.ExpandedMenuSize = 150;
			//menu.HideCurrentSelectionFromMenu = true;
			menu.MenuShouldFillScreen = false;
			menu.ContentWidth = 400;
			List<MenuOptionModel> models = new List<MenuOptionModel>();

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

			menu.AddMenuToView(this.View, models, models.First());
			return menu;
		}

		private void CloseMenu()
		{
			_menu.AnimateClosed(null);
			//_menuTwo.AnimateClosed(null);
			//_menuThree.AnimateClosed(null);
		}
	}
}

