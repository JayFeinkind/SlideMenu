using Foundation;
using System;
using UIKit;
using CoreGraphics;

namespace TestMenu
{
    public partial class DesignerTestViewController : UIViewController
    {
		UIPanGestureRecognizer _panGesture;
		UITapGestureRecognizer _tapGesture;

        public DesignerTestViewController (IntPtr handle) : base (handle)
        {
        }

		public override void LoadView()
		{
			base.LoadView();

			_panGesture = new UIPanGestureRecognizer(PanGestureHandler);
			_tapGesture = new UITapGestureRecognizer(CloseMenu);

			View.AddGestureRecognizer(_tapGesture);
			_slideMenu.AddGestureRecognizer(_panGesture);

 			// this will alow both the menu pan gesture and this View's pan gesture to work simultaneously.  Otherwise
			//     _panGesture will superceed the menu pan gesturey
			_slideMenu.PanGestureRecognizer.ShouldRecognizeSimultaneously = (gestureRecognizer, otherGestureRecognizer) => {
				return otherGestureRecognizer == _panGesture;
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

		void CloseMenu()
		{
			if (_slideMenu.MenuOpen)
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
	}
}