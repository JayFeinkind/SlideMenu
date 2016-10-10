using Foundation;
using System;
using UIKit;
using CoreGraphics;
using SlideMenu;

namespace TestMenu
{
    public partial class DesignerTestViewController : UIViewController
    {
		UIPanGestureRecognizer _panGesture;
		UITapGestureRecognizer _tapGesture;
		UITapGestureRecognizer _menuTapGesture;
		UIView _square;

        public DesignerTestViewController (IntPtr handle) : base (handle)
        {
        }

		public override void LoadView()
		{
			base.LoadView();

			_menuTapGesture = new UITapGestureRecognizer(AnimateSquare);
			_panGesture = new UIPanGestureRecognizer(PanGestureHandler);
			_tapGesture = new UITapGestureRecognizer(CloseMenu);
			_slideMenu.SlideDirection = SlideDirectionType.Up;
			View.AddGestureRecognizer(_tapGesture);
			_slideMenu.AddGestureRecognizer(_panGesture);
			_slideMenu.AddGestureRecognizer(_menuTapGesture);

 			// this will alow both the menu pan gesture and this View's pan gesture to work simultaneously.  Otherwise
			//     _panGesture will superceed the menu pan gesturey
			_slideMenu.PanGestureRecognizer.ShouldRecognizeSimultaneously = (gestureRecognizer, otherGestureRecognizer) => {
				return otherGestureRecognizer == _panGesture;
			};

			_slideMenu.TapGestureRecognizer.ShouldRecognizeSimultaneously = (gestureRecognizer, otherGestureRecognizer) => {
				return otherGestureRecognizer == _menuTapGesture;
			};

 			// tap gesture should work with all other gestures simultaneously.y
			_tapGesture.ShouldRecognizeSimultaneously = (gestureRecognizer, otherGestureRecognizer) => true;

			// we don't want to close the menu when tapping on the menu
			_tapGesture.ShouldReceiveTouch = (recognizer, touch) => { 
				
				CGPoint location = touch.LocationInView(View);

				bool inMenu = _slideMenu.Frame.Contains(location);

				return inMenu == false;
			};
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			_square = new UIView(new CGRect(50, 300, 50, 50));
			_square.BackgroundColor = UIColor.Yellow;
			View.AddSubview(_square);
		}

		void AnimateSquare()
		{
			UIView.AnimateNotify(0.5f, () => {
				var center = _square.Center;

				if (_square.Frame.Y == 300)
				{
					center.Y += 250;
				}
				else
				{
					center.Y -= 250;
				}

				_square.Center = center;
			}, null);
		}

		void CloseMenu()
		{
			if (_slideMenu.ViewExpanded)
			{
				_slideMenu.AnimateClosed((finished) => { _displayLabel.Text = "Closed Menu"; });
			}
		}

		void PanGestureHandler(UIPanGestureRecognizer gesture)
		{
			var touchLocation = gesture.LocationInView(View);

			_displayLabel.Text = "Y: " + touchLocation.Y + "  X: " + touchLocation.X;
		}

		partial void NavigateButton_TouchUpInside(UIButton sender)
		{
			PerformSegue("SlideMenuSegue", null);
		}

		partial void UIButton1564_TouchUpInside(UIButton sender)
		{
			PerformSegue("PullSegue", null);
		}
	}
}