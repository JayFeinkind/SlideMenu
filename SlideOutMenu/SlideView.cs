using System;
using System.ComponentModel;
using System.Linq;
using CoreGraphics;
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

		CGPoint firstTouchLocation;

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
			_menuPanGesture = new UIPanGestureRecognizer(SlideMenuFromPan);
			_menuTapGesture = new UITapGestureRecognizer(ShowOrHideMenuFromTap);
			AddGestureRecognizer(_menuTapGesture);
			AddGestureRecognizer(_menuPanGesture);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			Layer.CornerRadius = CornerRadius;
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
			var velocity = panGesture.VelocityInView(this);
			var constraint = GetChangedConstraint();
			var touchLocation = panGesture.LocationInView(this);

			if (panGesture.State == UIGestureRecognizerState.Began && panGesture.NumberOfTouches == 1)
			{
				firstTouchLocation = touchLocation;
			}
			else if (panGesture.State == UIGestureRecognizerState.Changed && panGesture.NumberOfTouches == 1)
			{
				// instantly update height, do not use animation or it will not keep up with pan gesture
				// NOTE: Slide direction refers to which way you would swipe to expand menu, not current slide direction
				switch (SlideDirection)
				{
					case SlideDirectionType.Up:
						constraint.Constant += firstTouchLocation.Y - touchLocation.Y;
						break;
					case SlideDirectionType.Down:
						constraint.Constant += (firstTouchLocation.Y - touchLocation.Y) * -1;
						break;
					case SlideDirectionType.Right:
						constraint.Constant += (firstTouchLocation.X - touchLocation.X) * -1;
						break;
					case SlideDirectionType.Left:
						constraint.Constant += firstTouchLocation.X - touchLocation.X;
						break;
				}

				// For whatever reason when slide direction is up animation works better if touch
				//    location is not updated
				if (SlideDirection != SlideDirectionType.Up)
				{
					firstTouchLocation = touchLocation;
				}

				this.LayoutIfNeeded();
			}
			else if (panGesture.State == UIGestureRecognizerState.Ended)
			{
				int finalSize = 0;

				switch (SlideDirection)
				{
					case SlideDirectionType.Up:
						finalSize = velocity.Y < 0 ? ExpandedSize : CollapsedSize;
						break;
					case SlideDirectionType.Down:
						finalSize = velocity.Y > 0 ? ExpandedSize : CollapsedSize;
						break;
					case SlideDirectionType.Right:
						finalSize = velocity.X < 0 ? CollapsedSize : ExpandedSize;
						break;
					case SlideDirectionType.Left:
						finalSize = velocity.X < 0 ? ExpandedSize : CollapsedSize;
						break;
				}

				// this will not actually change anything until LayoutIfNeeded is called during animation
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

			// if using spring animation extend duration because spring animation is included
			//    in duration
			AnimateNotify(
				duration: UseSpringAnimation ? 1f : 0.5f,
				delay: 0,
				springWithDampingRatio: UseSpringAnimation ? SpringWithDampingRatio : 1, //0.5
				initialSpringVelocity: UseSpringAnimation ? InitialSpringVelocity : 1, //0.8
				options: UIViewAnimationOptions.CurveEaseInOut,
			    animations:() => {
					// must call LayoutIfNeeded on Superview or change will not animate
					if (Superview != null) Superview.LayoutIfNeeded(); 
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

		#region Designer Properties

		[Export("CollapsedSize"), Browsable(true)]
		public int CollapsedSize { get; set; }

		[Export("ExpandedSize"), Browsable(true)]
		public int ExpandedSize { get; set; }

		/// <summary>
		/// NOTE: Setting this properly will only technically affect expanding direction while
		///    sliding.  The expanding animation is accomplished by modifying the height/width constraint.
		///    View will expand in the opposite direction as it's anchor. For example if view has a bottom
		///    constraint but no top constraint it will expand up.
		/// </summary>
		/// <value>The slide direction.</value>
		[Export("SlideDirection"), Browsable(true)]
		public SlideDirectionType SlideDirection { get; set; }

		[Export("CornerRadius"), Browsable(true)]
		public int CornerRadius { get; set; }

		[Export("UseSpringAnimation"), Browsable(true)]
		public bool UseSpringAnimation { get; set; }

		[Export("SpringWithDampingRatio"), Browsable(true)]
		public nfloat SpringWithDampingRatio { get; set; }

		[Export("InitialSpringVelocity"), Browsable(true)]
		public nfloat InitialSpringVelocity { get; set; }

		#endregion

		public bool MenuOpen { get; set; }

		public UIPanGestureRecognizer PanGestureRecognizer { get { return _menuPanGesture; } }
	}
}

