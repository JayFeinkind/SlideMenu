using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using System.Linq;
using Foundation;

namespace SlideMenu
{
	public class SlideOutMenu : UIView
	{
		#region Private Fields

		public event Action MenuClosedHandler;
		public event Action MenuOpenHandler;

		UITapGestureRecognizer _menuTapGesture;
		UIPanGestureRecognizer _menuPanGesture;

		int _minSize = 50;
		int _maxSize = 0;

		int _compactChevronSize = 18;

		nfloat? _firstTouchY;

		UITableView _expandedTableView;
		//UILabel _collapsedLabel;
		UIView _chevronContainer;
		UIView _menuBorder;
		UIView _chevronView;
		UIView _superView;

		MenuPositionType _position;

		IEnumerable<MenuOptionModel> _values;

		NSLayoutConstraint _mainHeightConstraint;
		NSLayoutConstraint _chevronHeightConstraint;

		MenuOptionModel _currentSelection;

		IMenuConstraints _menuConstraints;

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
		public SlideOutMenu(MenuPositionType positiion, IEnumerable<MenuOptionModel> values = null)
		{
			_position = positiion;
			_values = values ?? new List<MenuOptionModel>();

			TranslatesAutoresizingMaskIntoConstraints = false;
			ChevronOffset = 0;
			ShowCurrentSelection = true;
			HideMenuBackgroundOnCollapse = true;
			AddRoomForNavigationBar = false;
		}

		#region Add Menu to View

		public void AddMenuToSuperview(UIView superView, IEnumerable<MenuOptionModel> values, MenuOptionModel presetValue = null)
		{
			_superView = superView;

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

			if (HideMenuBackgroundOnCollapse == false)
			{
				AddGestureRecognizer(_menuPanGesture);
			}
			else
			{
				_chevronContainer.AddGestureRecognizer(_menuPanGesture);
			}

			_expandedTableView.Alpha = 0;

			//_collapsedLabel.Alpha = 1;

			_chevronContainer.UserInteractionEnabled = true;

			BackgroundColor = UIColor.White.ColorWithAlpha(HideMenuBackgroundOnCollapse ? 0 : 1);
			_menuBorder.Alpha = HideMenuBackgroundOnCollapse ? 0 : 1;

			var angle = GetChevronAngle(false);

			_chevronView.Transform = CGAffineTransform.MakeRotation(angle);
		}

		private float GetChevronAngle(bool menuOpen)
		{
			float angle = 0;

			if (_position == MenuPositionType.Bottom)
			{
				angle = menuOpen ? (float)Math.PI : (float)Math.PI * 4;
			}
			else if (_position == MenuPositionType.Top)
			{
				angle = !menuOpen ? (float)Math.PI : (float)Math.PI * 4;
			}

			return angle;
		}

		private void SetupUI()
		{
			switch (_position)
			{
				case MenuPositionType.Bottom:
					_menuConstraints = new BottomMenuConstraints();
					break;
				case MenuPositionType.Top:
					_menuConstraints = new TopMenuConstraints();
					break;
			}

			_expandedTableView = GetExpandedMenuTable();
			//_collapsedLabel = GetCollapsedMenuLabel();
			_chevronContainer = GetChevronContainerView();
			_chevronView = GetChevronView();
			_menuBorder = new MenuBoarder();

			AddSubview(_menuBorder);
			AddSubview(_expandedTableView);
			//AddSubview(_collapsedLabel);
			AddSubview(_chevronContainer);
			AddSubview(_chevronView);


			_menuConstraints.AddBorderConstraints(this, _menuBorder);
			_menuConstraints.AddChevronContainerConstraints(this, _chevronContainer, ChevronOffset, _minSize);
			_menuConstraints.AddChevronConstraints(this, _chevronView);
			_menuConstraints.AddContentConstraints(this, _expandedTableView, _chevronContainer);
			_menuConstraints.AddMainConstraints(this, _superView, AddRoomForNavigationBar, _minSize);

			_mainHeightConstraint = _superView.Constraints.First(c => c.FirstItem == this && c.FirstAttribute == NSLayoutAttribute.Height);
			_chevronHeightConstraint = this.Constraints.First(c => c.FirstItem == _chevronContainer && c.FirstAttribute == NSLayoutAttribute.Height);
			//AddMenuConstraints(_collapsedLabel);

			BringSubviewToFront(_chevronContainer);
		}

		private void SetTapGesture()
		{
			_menuTapGesture = new UITapGestureRecognizer(ShowOrHideMenuFromTap);
			_chevronContainer.AddGestureRecognizer(_menuTapGesture);

			if (HideMenuBackgroundOnCollapse == false)
			{
				AddGestureRecognizer(_menuTapGesture);
			}

			/// <summary>
			/// This will override whether or not to execute event.  It will let the user
			/// 	tap on the table once the menu is expanded.  Default behavior is to stall the table touch to ensure this
			///      is what the user wants.  By default user would have to press row on table for a couple seconds before it
			/// 	would react.
			/// 
			/// This will prevent the main view tap gesture
			/// 	from overriding the table view selection.
			/// </summary>
			_menuTapGesture.ShouldReceiveTouch += (recognizer, touch) => (touch.View == this || touch.View == _chevronContainer);

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

		private UIView GetChevronContainerView()
		{
			var view = new UIView();
			view.BackgroundColor = UIColor.Clear;
			view.TranslatesAutoresizingMaskIntoConstraints = false;

			return view;
		}

		private UITableView GetExpandedMenuTable()
		{
			var table = new UITableView();
			table.TableFooterView = new UIView();
			table.TranslatesAutoresizingMaskIntoConstraints = false;
			table.Source = new MenuTableViewSource(_values);
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
			var menuTouchLocation = panGesture.LocationInView(this);
			var velocity = panGesture.VelocityInView(this);

			if (_firstTouchY == null)
			{
				_firstTouchY = menuTouchLocation.Y;
			}

			if ((panGesture.State == UIGestureRecognizerState.Began ||
				 panGesture.State == UIGestureRecognizerState.Changed) &&
				(panGesture.NumberOfTouches == 1))
			{
				nfloat offset = 0;

				switch (_position)
				{
					case MenuPositionType.Bottom:
						offset = (nfloat)(Frame.Height - menuTouchLocation.Y + _firstTouchY);
						break;
					case MenuPositionType.Top:
						offset = menuTouchLocation.Y;
						break;
				}

				Console.WriteLine(offset);

				if (_chevronHeightConstraint.Constant != _compactChevronSize)
				{
					_chevronHeightConstraint.Constant = _compactChevronSize;
					_chevronContainer.SetNeedsDisplay();
				}

				var alpha = Math.Pow(_mainHeightConstraint.Constant / _maxSize, 2);

				// ensure alpha is between 0 and 1, only happens if dragged further then the bounds
				if (alpha > 1)
					alpha = 1;
				if (alpha < 0)
					alpha = 0;


				var degrees = (_mainHeightConstraint.Constant / _maxSize) * 180;
				var radians = degrees * (Math.PI / 180);

				_chevronView.Transform = CGAffineTransform.MakeRotation((nfloat)radians);

				_expandedTableView.Alpha = (nfloat)alpha;
				//_collapsedLabel.Alpha = (nfloat)(1 - alpha);

				nfloat backgroundAlpha = 1;

				if (HideMenuBackgroundOnCollapse == true)
				{
					backgroundAlpha = _expandedTableView.Alpha > 0.9f ? 0.9f : _expandedTableView.Alpha;
				}

				BackgroundColor = BackgroundColor.ColorWithAlpha(backgroundAlpha);

				// instantly update height, do not use animation or it will not keep up with pan gesture
				_mainHeightConstraint.Constant = offset;
				this.LayoutIfNeeded();
			}
			else if (panGesture.State == UIGestureRecognizerState.Ended)
			{
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

				// this will not actually change anything untill LayoutIfNeeded is called
				_mainHeightConstraint.Constant = finalSize;

				int compactViewAlpha = Convert.ToInt16(finalSize == CollapsedMenuSize);
				int expandedViewAlpha = Convert.ToInt16(finalSize == ExpandedMenuSize);

				// reset pan values
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

			float angle = GetChevronAngle(menuOpen);

			nfloat backgroundAlpha = 1;

			if (HideMenuBackgroundOnCollapse == true)
			{

				backgroundAlpha = expandedViewAlpha > 0.9f ? 0.9f : expandedViewAlpha;
			}

			// previously using 0.7 and 1.0 for sping values but it was a bit slow
			AnimateNotify(1f, 0, 0.50f, 0.8f, UIViewAnimationOptions.CurveEaseInOut, () =>
		   {
				_chevronView.Transform = CGAffineTransform.MakeRotation(angle);
			   //_collapsedLabel.Alpha = collapsedViewAlpha;
			   _expandedTableView.Alpha = expandedViewAlpha;
			   _menuBorder.Alpha = HideMenuBackgroundOnCollapse ? expandedViewAlpha : 1;
				BackgroundColor = BackgroundColor.ColorWithAlpha(backgroundAlpha);
				this.LayoutIfNeeded();
		   }, completionBlock);

			UpdateMenuLayout(menuOpen);
		}

		private void UpdateMenuLayout(bool menuOpen)
		{
			if (HideMenuBackgroundOnCollapse == true)
			{
				if (menuOpen)
				{
					_chevronContainer.RemoveGestureRecognizer(_menuPanGesture);
					_chevronContainer.RemoveGestureRecognizer(_menuTapGesture);
					AddGestureRecognizer(_menuTapGesture);
					AddGestureRecognizer(_menuPanGesture);
				}
				else
				{
					_chevronContainer.AddGestureRecognizer(_menuPanGesture);
					_chevronContainer.AddGestureRecognizer(_menuTapGesture);
					RemoveGestureRecognizer(_menuTapGesture);
					RemoveGestureRecognizer(_menuPanGesture);
				}
			}

			_chevronHeightConstraint.Constant = menuOpen ? _compactChevronSize : _minSize;
			_chevronContainer.SetNeedsDisplay();

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

		#region Public Properties

		public bool AddRoomForNavigationBar { get; set; }

		public bool HideMenuBackgroundOnCollapse { get; set; }

		public bool ShowCurrentSelection { get; set; }

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

