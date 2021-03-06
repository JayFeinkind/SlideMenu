using Foundation;
using System;
using UIKit;
using CoreGraphics;
using SlideMenu;
using System.Collections.Generic;
using System.Linq;

namespace TestMenu
{
	public partial class SlideOutMenuViewController : UIViewController
	{
		UITapGestureRecognizer _viewTapGesture;

		// Slide menu will keep track off whether it is open or not.  But since this
		//   classes gestures work simultaneously with Slide menu gestures keep
		//   track of this independently for animation.
		bool _menuOpen;

		public SlideOutMenuViewController(IntPtr handle) : base(handle)
		{
			_menuOpen = false;
		}

		#region Overrides

		public override void LoadView()
		{
			base.LoadView();

			_slideOutMenu.SlideDirection = SlideDirectionType.Up;

			_viewTapGesture = new UITapGestureRecognizer(CloseFromTap);
			View.AddGestureRecognizer(_viewTapGesture);

 			_viewTapGesture.CancelsTouchesInView = false;

    		// Hook into slide view's gesture recognizers
			_slideOutMenu.TapGestureRecognizer.AddTarget(AnimateFromTap);
			_slideOutMenu.PanGestureRecognizer.AddTarget(AnimateFromPan);

			// this will allow table view selections to superceed closing menu
			_slideOutMenu.TapGestureRecognizer.ShouldReceiveTouch += (recognizer, touch) =>
				(touch.View == _slideOutMenu || touch.View == _chevronContiainer || touch.View == _chevronView);

			_viewTapGesture.ShouldReceiveTouch += (recognizer, touch) => {
				var location = touch.LocationInView(View);

				bool inMenu = _slideOutMenu.Frame.Contains(location);

				return inMenu == false;
			};

			SetMenuLayer();

			_mainTableView.PanGestureRecognizer.AddTarget(CloseFromTap);

			_menuSelectionTableView.TableFooterView = new UIView();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			_menuSelectionTableView.Bounces = false;
			_menuSelectionTableView.Alpha = 0;
			_menuSelectionTableView.BackgroundColor = UIColor.Clear;

			_slideOutMenu.BackgroundColor = UIColor.White.ColorWithAlpha(0);

			_selectionLabel.Text = "0";

			var menuValues = Enumerable.Range(0, 6).Select(n => n.ToString());
			_menuSelectionTableView.Source = new SlideOutMenuTableViewSource(menuValues, SetSelectedLabel);
			_menuSelectionTableView.ReloadData();

			_mainTableView.RowHeight = UITableView.AutomaticDimension;
			_mainTableView.EstimatedRowHeight = 60;
			_mainTableView.Source = new TestTableViewSource();
			_mainTableView.ReloadData();
		}

		#endregion

		private void SetSelectedLabel(string label)
		{
			_selectionLabel.Text = label;
			_slideOutMenu.AnimateClosed(null);
			AnimateMenu(false);
		}

		private void SetMenuLayer()
		{
			_slideOutMenu.Layer.MasksToBounds = false;
			_slideOutMenu.Layer.CornerRadius = 10;
			_slideOutMenu.Layer.ShadowColor = UIColor.DarkGray.CGColor;
			_slideOutMenu.Layer.ShadowOpacity = 1.0f;
			_slideOutMenu.Layer.ShadowRadius = 6.0f;
			_slideOutMenu.Layer.ShadowOffset = new CGSize(0f, 3f);
		}

		private float GetChevronAngle(bool menuOpen)
		{
			return menuOpen ? (float)Math.PI : (float)Math.PI * 4;
		}

		#region Animation

		private void CloseFromTap()
		{
			if (_slideOutMenu.ViewExpanded)
			{
				_slideOutMenu.AnimateClosed(null);
				AnimateMenu(false);
			}
		}

		private void AnimateFromPan(NSObject gesture)
		{
			var panGesture = gesture as UIPanGestureRecognizer;
			var velocity = panGesture.VelocityInView(_slideOutMenu);

			if (panGesture.State == UIGestureRecognizerState.Changed && panGesture.NumberOfTouches == 1)
			{
				// subtract ten from height constaint since 10 pixels are off the edge
				var alpha = (nfloat)Math.Pow((MenuHeightConstraint.Constant)/ _slideOutMenu.ExpandedSize, 2);

				var degrees = (MenuHeightConstraint.Constant / _slideOutMenu.ExpandedSize) * 180;

				var radians = degrees  * (Math.PI / 180);

				_chevronView.Transform = CGAffineTransform.MakeRotation((nfloat)radians);
				_selectionLabel.Alpha = 1 - alpha;
				_menuSelectionTableView.Alpha = alpha;
				_slideOutMenu.BackgroundColor = _slideOutMenu.BackgroundColor.ColorWithAlpha(alpha);
			}
			else if (panGesture.State == UIGestureRecognizerState.Ended)
			{
				bool menuOpen = velocity.Y < 0;

				AnimateMenu(menuOpen);
			}
		}

		private void AnimateFromTap()
		{
			AnimateMenu(!_menuOpen);
		}

		private void AnimateMenu(bool open)
		{
			var chevronAngle = GetChevronAngle(open);
			var labelAlpha = open ? 0 : 1;
			var menuAlpha = open ? 1 : 0;
			var tableAlpha = open ? 1 : 0;

			UIView.AnimateNotify(
				_slideOutMenu.AnimationDuration,
				0,
				_slideOutMenu.SpringWithDampingRatio, 
				_slideOutMenu.InitialSpringVelocity, 
				UIViewAnimationOptions.CurveEaseInOut,
				() => { 
					_chevronView.Transform = CGAffineTransform.MakeRotation(chevronAngle);
					_selectionLabel.Alpha = labelAlpha;
					_slideOutMenu.BackgroundColor = _slideOutMenu.BackgroundColor.ColorWithAlpha(menuAlpha);
					_menuSelectionTableView.Alpha = tableAlpha;
				},
				null);

			_menuOpen = open;
		}

		#endregion
    }

	public class TestTableViewSource : UITableViewSource
	{
		string CellIdentifier = "DefaultCell";
		Lazy<UIImage> cellImage = new Lazy<UIImage>(() => UIImage.FromBundle("Elephant"));

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(CellIdentifier) ?? 
			                    new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);

			cell.ImageView.Image = cellImage.Value;

			string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
				"Vivamus placerat eros metus. Suspendisse mi nulla, euismod vitae tempus at, " +
				"consectetur vitae nulla. Phasellus imperdiet tortor ac facilisis aliquam. In vulputate " +
				"feugiat tortor, non lobortis augue molestie nec. Quisque in lectus sapien. Aenean quis metus " +
				"felis. Nullam vestibulum nisl in vulputate congue. Nunc aliquet convallis enim, nec blandit tortor";


			var rnd = new Random();

			text = text.Substring(0, rnd.Next(text.Length - 1));

			cell.TextLabel.Text = text;
			cell.TextLabel.Lines = 0;
			cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;

			return cell;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return 30;
		}
	}
}