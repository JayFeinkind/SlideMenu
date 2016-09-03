using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using System.Linq;
using Foundation;

namespace SlideMenu
{
	public class SlideOutMenu<T> : UIView
		where T : class
	{
		#region Private Fields

		public event Action MenuClosedHandler;
		public event Action MenuOpenHandler;

		public event Action<T> ValueSelectedHandler;

		UITapGestureRecognizer _menuTapGesture;
		UIPanGestureRecognizer _menuPanGesture;

		int _minSize = 50;
		int _maxSize = 0;

		int _compactChevronSize = 18;

		nfloat? _firstTouchY;

		UITableView _expandedTableView;
		//UILabel _collapsedLabel;
		UIView _chevronImage;
		UIView _menuBorder;

		UIView _superView;

		MenuPositionType _position;

		IEnumerable<T> _values;

		NSLayoutConstraint _mainHeightConstraint;
		NSLayoutConstraint _chevronHeightConstraint;

		T _currentSelection;

		// 44 is the default row height for table view
		int _estimatedRowHeight = 44;

		#endregion

		[Export("requiresConstraintBasedLayout")]
		bool UseNewLayout()
		{
			return true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SlideMenu.SlideOutMenu"/> class.
		/// </summary>
		/// <param name="positiion">Positiion.</param>
		/// <param name="values">Values.</param>
		public SlideOutMenu(MenuPositionType positiion, IEnumerable<T> values = null)
		{
			_position = positiion;
			_values = values ?? new List<T>();

			TranslatesAutoresizingMaskIntoConstraints = false;
			ChevronOffset = 0;
		}

		#region Add Menu to View

		public void AddMenuToSuperview(UIView superView, IEnumerable<T> values, T presetValue = null)
		{
			_superView = superView;

			BackgroundColor = UIColor.White.ColorWithAlpha(0);

			// add 5 for the top constraint
			int estimatedHeight = values.Count() * _estimatedRowHeight + _compactChevronSize + 5;

			// If collapsed size is bigger then the expanded size and its bigger then the default 
			//		size it will be reset both.
			if (_minSize > _maxSize && _minSize > estimatedHeight)
			{
				_maxSize = estimatedHeight;
				_minSize = 75;
			}
			else if (_minSize > _maxSize)
			{
				_maxSize = estimatedHeight;
			}

			// check to see if there already is a superview before adding
			if (this.Superview == null)
			{
				_superView.AddSubview(this);
			}

			_values = values;

			SetupUI();

			SetTapGesture();

			_menuPanGesture = new UIPanGestureRecognizer(SlideMenuFromPan);
			//AddGestureRecognizer(_menuPanGesture);
			_chevronImage.AddGestureRecognizer(_menuPanGesture);

			_expandedTableView.Alpha = 0;
			_menuBorder.Alpha = 0;
			//_collapsedLabel.Alpha = 1;

			_chevronImage.UserInteractionEnabled = true;

			float angle = 0;

			if (_position == MenuPositionType.Bottom)
			{
				angle = (float)Math.PI * 4;
			}
			else if (_position == MenuPositionType.Top)
			{
				angle = (float)Math.PI;
			}

			_chevronImage.Transform = CGAffineTransform.MakeRotation(angle);
		}

		private void SetupUI()
		{
			_expandedTableView = GetExpandedMenuTable();
			//_collapsedLabel = GetCollapsedMenuLabel();
			_chevronImage = GetChevronView();
			_menuBorder = new MenuBoarder();

			AddSubview(_menuBorder);
			AddSubview(_expandedTableView);
			//AddSubview(_collapsedLabel);
			AddSubview(_chevronImage);

			AddBorderConstraints();
			AddChevronConstraints();
			AddMenuConstraints(_expandedTableView);
			//AddMenuConstraints(_collapsedLabel);
			AddMainConstraints();

		}

		private void SetTapGesture()
		{
			_menuTapGesture = new UITapGestureRecognizer(ShowOrHideMenuFromTap);
			_chevronImage.AddGestureRecognizer(_menuTapGesture);

			/// <summary>
			/// This will override whether or not to execute event.  It will let the user
			/// 	tap on the table once the menu is expanded.  Default behavior is to stall the table touch to ensure this
			///      is what the user wants.  By default user would have to press row on table for a couple seconds before it
			/// 	would react.
			/// 
			/// This will prevent the main view tap gesture
			/// 	from overriding the table view selection.
			/// </summary>
			_menuTapGesture.ShouldReceiveTouch += (recognizer, touch) => (touch.View == this || touch.View == _chevronImage);

			_menuTapGesture.CancelsTouchesInView = false;
		}

		#endregion

		#region Initialize Menu UI

		private UIView GetChevronView()
		{
			var view = new ChevronView();
			view.BackgroundColor = UIColor.Clear;
			view.TranslatesAutoresizingMaskIntoConstraints = false;

			return view;
		}

		private UITableView GetExpandedMenuTable()
		{
			var table = new UITableView();
			table.TableFooterView = new UIView();
			table.TranslatesAutoresizingMaskIntoConstraints = false;
			table.Source = new MenuTableViewSource<T>(ValueSelectedHandler, _values);
			table.UserInteractionEnabled = true;
			table.EstimatedRowHeight = 50;
			table.BackgroundColor = UIColor.Clear;
			table.RowHeight = UITableView.AutomaticDimension;
			table.Bounces = false;

			return table;
		}

		private UILabel GetCollapsedMenuLabel()
		{
			var label = new UILabel();

			label.Font = UIFont.BoldSystemFontOfSize(17);
			label.TextColor = UIColor.Black;
			label.Text = _currentSelection.ToString();
			label.TextAlignment = UITextAlignment.Center;
			label.TranslatesAutoresizingMaskIntoConstraints = false;

			return label;
		}

		#endregion

		#region Menu Interaction

		private void SlideMenuFromPan(UIPanGestureRecognizer panGesture)
		{
			var touchLocation = panGesture.LocationInView(this);

			if (_firstTouchY == null)
			{
				_firstTouchY = touchLocation.Y;
			}

			// offset represents height from buttom of screen
			var offset = _position == MenuPositionType.Bottom ? 
			                                          this.Frame.Height - touchLocation.Y + _firstTouchY : 
			                                          touchLocation.Y + _firstTouchY;

			if ((panGesture.State == UIGestureRecognizerState.Began ||
				 panGesture.State == UIGestureRecognizerState.Changed) &&
				(panGesture.NumberOfTouches == 1))
			{
				if (_chevronHeightConstraint.Constant != _compactChevronSize)
				{
					_chevronHeightConstraint.Constant = _compactChevronSize;
					_chevronImage.SetNeedsDisplay();
				}

				var alpha = Math.Pow((double)(offset / _maxSize), 2);

				// ensure alpha is between 0 and 1, only happens if dragged further then the bounds
				if (alpha > 1)
					alpha = 1;
				if (alpha < 0)
					alpha = 0;


				var degrees = (offset / _maxSize) * 180;
				var radians = degrees * (Math.PI / 180);

				_chevronImage.Transform = CGAffineTransform.MakeRotation((nfloat)radians);

				_expandedTableView.Alpha = (nfloat)alpha;
				//_collapsedLabel.Alpha = (nfloat)(1 - alpha);

				nfloat backgroundAlpha = _expandedTableView.Alpha > 0.9f ? 0.9f : _expandedTableView.Alpha;
				BackgroundColor = BackgroundColor.ColorWithAlpha(backgroundAlpha);

				// instantly update height, do not use animation or it will not keep up with pan gesture
				_mainHeightConstraint.Constant = offset.Value;
				this.LayoutIfNeeded();
			}
			else if (panGesture.State == UIGestureRecognizerState.Ended)
			{
				var velocity = panGesture.VelocityInView(this);

				// set final position based on swipe direction
				int finalSize = 0;

				if (_position == MenuPositionType.Bottom)
				{
					finalSize = velocity.Y < 0 ? ExpandedMenuSize : CollapsedMenuSize;
				}
				else if (_position == MenuPositionType.Top)
				{
					finalSize = velocity.Y > 0 ? ExpandedMenuSize : CollapsedMenuSize;
				}

				_mainHeightConstraint.Constant = finalSize;

				int compactViewAlpha = Convert.ToInt16(finalSize == CollapsedMenuSize);
				int expandedViewAlpha = Convert.ToInt16(finalSize == ExpandedMenuSize);

				_firstTouchY = null;

				AnimateMenu(finalSize, compactViewAlpha, expandedViewAlpha, null);
			}
		}

		private void ShowOrHideMenuFromTap()
		{
			var newHeight = _mainHeightConstraint.Constant == _minSize ? _maxSize : _minSize;
			int compactViewAlpha = Convert.ToInt16(newHeight == _minSize);
			int expandedViewAlpha = Convert.ToInt16(newHeight == _maxSize);

			AnimateMenu(newHeight, compactViewAlpha, expandedViewAlpha, null);
		}

		/// <summary>
		/// This will be called anytime the menu opens or closes
		/// </summary>
		/// <param name="newHeight">New height.</param>
		/// <param name="collapsedViewAlpha">Collapsed view alpha.</param>
		/// <param name="expandedViewAlpha">Expanded view alpha.</param>
		/// <param name="completionBlock">Completion block.</param>
		private void AnimateMenu(int newHeight, nfloat collapsedViewAlpha, nfloat expandedViewAlpha, UICompletionHandler completionBlock)
		{
			if (completionBlock == null)
			{
				completionBlock = (finished) => { };
			}

			bool menuOpen = newHeight == _maxSize;

			_mainHeightConstraint.Constant = newHeight;

			float angle = 0;

			if (_position == MenuPositionType.Bottom)
			{
				angle = menuOpen ? (float)Math.PI : (float)Math.PI * 4;
			}
			else if (_position == MenuPositionType.Top)
			{
				angle = !menuOpen ? (float)Math.PI : (float)Math.PI * 4;
			}

			nfloat backgroundAlpha = expandedViewAlpha > 0.9f ? 0.9f : expandedViewAlpha;

			// previously using 0.7 and 1.0 for sping values but it was a bit slow
			AnimateNotify(1f, 0, 0.50f, 0.8f, UIViewAnimationOptions.CurveEaseInOut, () =>
		   {
				_chevronImage.Transform = CGAffineTransform.MakeRotation(angle);
			   //_collapsedLabel.Alpha = collapsedViewAlpha;
			   _expandedTableView.Alpha = expandedViewAlpha;
			   _menuBorder.Alpha = expandedViewAlpha;
				BackgroundColor = BackgroundColor.ColorWithAlpha(backgroundAlpha);
				this.LayoutIfNeeded();
		   }, completionBlock);

			UpdateMenuLayout(menuOpen);
		}

		private void UpdateMenuLayout(bool menuOpen)
		{
			if (menuOpen)
			{
				_chevronImage.RemoveGestureRecognizer(_menuPanGesture);
				_chevronImage.RemoveGestureRecognizer(_menuTapGesture);
				AddGestureRecognizer(_menuTapGesture);
				AddGestureRecognizer(_menuPanGesture);

				_chevronHeightConstraint.Constant = _compactChevronSize;
			}
			else
			{
				_chevronImage.AddGestureRecognizer(_menuPanGesture);
				_chevronImage.AddGestureRecognizer(_menuTapGesture);
				RemoveGestureRecognizer(_menuTapGesture);
				RemoveGestureRecognizer(_menuPanGesture);
				_chevronHeightConstraint.Constant = 50;
			}

			_chevronImage.SetNeedsDisplay();

			if (menuOpen && MenuOpenHandler != null)
			{
				MenuOpenHandler();
			}
			else if (MenuClosedHandler != null)
			{
				MenuClosedHandler();
			}
		}

		#endregion

		#region Constraints

		private void AddMainConstraints()
		{
			if (_superView != null)
			{
				_mainHeightConstraint = NSLayoutConstraint.Create(this, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, _minSize);
				_superView.AddConstraint(_mainHeightConstraint);

				switch (_position)
				{
					case MenuPositionType.Bottom:
						_superView.AddConstraint(NSLayoutConstraint.Create(this, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, _superView, NSLayoutAttribute.Bottom, 1, 0));
						_superView.AddConstraint(NSLayoutConstraint.Create(this, NSLayoutAttribute.Right, NSLayoutRelation.Equal, _superView, NSLayoutAttribute.Right, 1, 0));
						_superView.AddConstraint(NSLayoutConstraint.Create(this, NSLayoutAttribute.Left, NSLayoutRelation.Equal, _superView, NSLayoutAttribute.Left, 1, 0));
						break;
					case MenuPositionType.Top:
						_superView.AddConstraint(NSLayoutConstraint.Create(this, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _superView, NSLayoutAttribute.Top, 1, 0));
						_superView.AddConstraint(NSLayoutConstraint.Create(this, NSLayoutAttribute.Right, NSLayoutRelation.Equal, _superView, NSLayoutAttribute.Right, 1, 0));
						_superView.AddConstraint(NSLayoutConstraint.Create(this, NSLayoutAttribute.Left, NSLayoutRelation.Equal, _superView, NSLayoutAttribute.Left, 1, 0));
						break;
				}
			}
		}

		private void AddBorderConstraints()
		{
			switch (_position)
			{
				case MenuPositionType.Bottom:
					AddConstraint(NSLayoutConstraint.Create(_menuBorder, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 1));
					AddConstraint(NSLayoutConstraint.Create(_menuBorder, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1, 0));
					AddConstraint(NSLayoutConstraint.Create(_menuBorder, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1, 0));
					AddConstraint(NSLayoutConstraint.Create(_menuBorder, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1, 0));
					break;
				case MenuPositionType.Top:
					AddConstraint(NSLayoutConstraint.Create(_menuBorder, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 1));
					AddConstraint(NSLayoutConstraint.Create(_menuBorder, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1, 0));
					AddConstraint(NSLayoutConstraint.Create(_menuBorder, NSLayoutAttribute.Left, NSLayoutRelation.Equal, this, NSLayoutAttribute.Left, 1, 0));
					AddConstraint(NSLayoutConstraint.Create(_menuBorder, NSLayoutAttribute.Right, NSLayoutRelation.Equal, this, NSLayoutAttribute.Right, 1, 0));
					break;
			}
		}

		private void AddChevronConstraints()
		{
			_chevronHeightConstraint = NSLayoutConstraint.Create(_chevronImage, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, _minSize);
			AddConstraint(_chevronHeightConstraint);
			AddConstraint(NSLayoutConstraint.Create(_chevronImage, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 30));
			AddConstraint(NSLayoutConstraint.Create(_chevronImage, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterX, 1, (nfloat)ChevronOffset));

			if (_position == MenuPositionType.Bottom)
			{
				AddConstraint(NSLayoutConstraint.Create(_chevronImage, NSLayoutAttribute.Top, NSLayoutRelation.Equal, this, NSLayoutAttribute.Top, 1, 5));
			}
			else if (_position == MenuPositionType.Top)
			{
				AddConstraint(NSLayoutConstraint.Create(_chevronImage, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, this, NSLayoutAttribute.Bottom, 1, 5));
			}
		}

		private void AddMenuConstraints(UIView newView)
		{
			if (_position == MenuPositionType.Bottom)
			{
				// anchor to bottom of arrow image
				AddConstraint(
					NSLayoutConstraint.Create(
						_chevronImage,
						NSLayoutAttribute.Bottom,
						NSLayoutRelation.Equal,
						newView,
						NSLayoutAttribute.Top,
						1,
						0));

				// bottom constraint
				AddConstraint(
					NSLayoutConstraint.Create(
						newView,
						NSLayoutAttribute.Bottom,
						NSLayoutRelation.GreaterThanOrEqual,
						this,
						NSLayoutAttribute.Bottom,
					1,
					1));
			}
			else if (_position == MenuPositionType.Top)
			{
				// anchor to bottom of arrow image
				AddConstraint(
					NSLayoutConstraint.Create(
						_chevronImage,
						NSLayoutAttribute.Top,
						NSLayoutRelation.Equal,
						newView,
						NSLayoutAttribute.Bottom,
						1,
						0));

				// bottom constraint
				AddConstraint(
					NSLayoutConstraint.Create(
						newView,
						NSLayoutAttribute.Top,
						NSLayoutRelation.GreaterThanOrEqual,
						this,
						NSLayoutAttribute.Top,
					1,
					1));
			}

			// width constraint
			newView.AddConstraint(
				NSLayoutConstraint.Create(
					newView,
					NSLayoutAttribute.Width,
					NSLayoutRelation.Equal,
					null,
					NSLayoutAttribute.NoAttribute,
					1,
					300));

			// center horizontally
			AddConstraint(
				NSLayoutConstraint.Create(
					this,
					NSLayoutAttribute.CenterX,
					NSLayoutRelation.Equal,
					newView,
					NSLayoutAttribute.CenterX,
					1,
					0));
		}

		#endregion

		#region Public Properties

		public double ChevronOffset { get; set; }

		public bool MenuOpen { get; set; }

		public int ExpandedMenuSize
		{
			get
			{
				return _maxSize;
			}
			set
			{
				_maxSize = value;
			}
		}

		public int CollapsedMenuSize
		{
			get
			{
				return _minSize;
			}
			set
			{
				if (value >= 50)
				{
					_minSize = value;
				}
			}
		}

		public UIPanGestureRecognizer MenuPanGesture
		{
			get
			{
				return _menuPanGesture;
			}
		}

		#endregion
	}
}

