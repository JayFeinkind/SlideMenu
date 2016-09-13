using Foundation;
using System;
using UIKit;

namespace TestMenu
{
    public partial class DesignerTestViewController : UIViewController
    {
		UIPanGestureRecognizer _panGesture;

        public DesignerTestViewController (IntPtr handle) : base (handle)
        {
        }

		public override void LoadView()
		{
			base.LoadView();

			_panGesture = new UIPanGestureRecognizer(PanGestureHandler);

			_slideMenu.AddGestureRecognizer(_panGesture);

			_slideMenu.PanGestureRecognizer.ShouldRecognizeSimultaneously = (gestureRecognizer, otherGestureRecognizer) => {
				return otherGestureRecognizer == _panGesture;
			};
		}

		void PanGestureHandler(UIPanGestureRecognizer gesture)
		{
			var touchLocation = gesture.LocationInView(View);

			_displayLabel.Text = "Y: " + touchLocation.Y + "  X: " + touchLocation.X;
		}
    }
}