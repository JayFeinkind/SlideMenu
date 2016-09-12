using System;
using System.ComponentModel;
using System.Linq;
using Foundation;
using UIKit;

namespace SlideMenu
{
	[Register("SlideView"), DesignTimeVisible(true)]
	public class SlideView : UIView
	{
		UITapGestureRecognizer _menuTapGesture;
		UIPanGestureRecognizer _menuPanGesture;

		public event Action MenuClosedHandler;
		public event Action MenuOpenHandler;

		nfloat currentTouchOffset;

		public SlideView()
		{
			Initialize();
		}

		public SlideView(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		private void Initialize()
		{
			MenuOpen = false;
			TranslatesAutoresizingMaskIntoConstraints = false;
			_menuPanGesture = new UIPanGestureRecognizer(SlideMenuFromPan);
			_menuTapGesture = new UITapGestureRecognizer(ShowOrHideMenuFromTap);

			AddGestureRecognizer(_menuTapGesture);
			AddGestureRecognizer(_menuPanGesture);
		}

		[Export("requiresConstraintBasedLayout")]
		bool UseNewLayout()
		{
			return true;
		}

		private NSLayoutConstraint GetChangedConstraint()
		{
			NSLayoutConstraint constraint = null;

			switch (SlideDirection)
			{
				case SlideDirectionType.Up:
				case SlideDirectionType.Down:
					constraint = Constraints.FirstOrDefault(c => c.FirstItem == this && c.FirstAttribute == NSLayoutAttribute.Height);
					break;
				case SlideDirectionType.Left:
				case SlideDirectionType.Right:
					constraint = Constraints.FirstOrDefault(c => c.FirstItem == this && c.FirstAttribute == NSLayoutAttribute.Width);
					break;
			}

			if (constraint == null)
				throw new InvalidOperationException("Cannot animate menu with the appropriate height/width constraint");

			return constraint;
		}

		private void SlideMenuFromPan(UIPanGestureRecognizer panGesture)
		{
			var superviewTouchLocation = panGesture.LocationInView(Superview);
			var velocity = panGesture.VelocityInView(this);

			var constraint = GetChangedConstraint();

			if (panGesture.State == UIGestureRecognizerState.Began && panGesture.NumberOfTouches == 1)
			{
				switch (SlideDirection)
				{
					case SlideDirectionType.Up:
						currentTouchOffset = superviewTouchLocation.Y - constraint.Constant;
						break;
					case SlideDirectionType.Down:
						currentTouchOffset = constraint.Constant - superviewTouchLocation.Y;
						break;
					case SlideDirectionType.Right:
						currentTouchOffset = constraint.Constant - superviewTouchLocation.X;
						break;
					case SlideDirectionType.Left:
						currentTouchOffset = superviewTouchLocation.X - constraint.Constant;
						break;
				}
			}
			else if (panGesture.State == UIGestureRecognizerState.Changed && panGesture.NumberOfTouches == 1)
			{
				nfloat offset = 0;

				switch (SlideDirection)
				{
					case SlideDirectionType.Up:
						offset = currentTouchOffset - superviewTouchLocation.Y;
						currentTouchOffset = superviewTouchLocation.Y;
						break;
					case SlideDirectionType.Down:
						offset = superviewTouchLocation.Y - currentTouchOffset;
						currentTouchOffset = superviewTouchLocation.Y;
						break;
					case SlideDirectionType.Right:
						offset = currentTouchOffset - superviewTouchLocation.X;
						currentTouchOffset = superviewTouchLocation.X;
						break;
					case SlideDirectionType.Left:
						offset = superviewTouchLocation.X - currentTouchOffset;
						currentTouchOffset = superviewTouchLocation.X;
						break;
				}

				// instantly update height, do not use animation or it will not keep up with pan gesture
				constraint.Constant += offset;
				this.LayoutIfNeeded();
			}
			else if (panGesture.State == UIGestureRecognizerState.Ended)
			{
				int finalSize = 0;

				if (SlideDirection == SlideDirectionType.Up)
				{
					finalSize = velocity.Y < 0 ? ExpandedSize : CollapsedSize;
				}
				else if (SlideDirection == SlideDirectionType.Down)
				{
					finalSize = velocity.Y > 0 ? ExpandedSize : CollapsedSize;
				}
				else if (SlideDirection == SlideDirectionType.Right)
				{
					finalSize = velocity.X < 0 ? CollapsedSize : ExpandedSize;
				}
				else if (SlideDirection == SlideDirectionType.Left)
				{
					finalSize = velocity.X < 0 ? ExpandedSize : CollapsedSize;
				}

				// this will not actually change anything untill LayoutIfNeeded is called during animation
				constraint.Constant = finalSize;
				AnimateMenu(finalSize, null);
			}
		}

		private void ShowOrHideMenuFromTap()
		{
			var newHeight = MenuOpen ? CollapsedSize : ExpandedSize;
			AnimateMenu(newHeight, null);
		}

		private void AnimateMenu(int newHeight, UICompletionHandler completionBlock)
		{
			if (completionBlock == null)
			{
				completionBlock = (finished) => { };
			}

			var constraint = GetChangedConstraint();
			constraint.Constant = newHeight;

			AnimateNotify(
				duration: 1f,
				delay: 0,
				springWithDampingRatio: 0.5f,
				initialSpringVelocity: 0.8f, 
				options: UIViewAnimationOptions.CurveEaseInOut,
			    animations:() => {
				// must call LayoutIfNeeded on Superview or change will not animate
				if (Superview != null)
					Superview.LayoutIfNeeded(); 
				},
				completion: completionBlock);

			MenuOpen = newHeight == ExpandedSize;

			if (MenuOpen && MenuOpenHandler != null)
			{
				MenuOpenHandler();
			}
			else if (MenuOpen == false && MenuClosedHandler != null)
			{
				MenuClosedHandler();
			}
		}

		public void AnimateClosed(UICompletionHandler completionHandler)
		{
			AnimateMenu(CollapsedSize, completionHandler);
		}

		public void AnimateOpen(UICompletionHandler completionHandler)
		{
			AnimateMenu(ExpandedSize, completionHandler);
		}

		[Export("CollapsedSize"), Browsable(true)]
		public int CollapsedSize { get; set; }

		[Export("ExpandedSize"), Browsable(true)]
		public int ExpandedSize { get; set; }

		public bool MenuOpen { get; set; }

		[Export("SlideDirection"), Browsable(true)]
		public SlideDirectionType SlideDirection { get; set; }
	}

	public enum SlideDirectionType
	{
		Down,
		Up,
		Left,
		Right
	}
}

