using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using CoreAnimation;
using ObjCRuntime;

namespace SlideMenu
{
	public class SlideOutMenu : UIView
	{
		#region Private Fields

		public event Action MenuClosedHandler;
		public event Action MenuOpenHandler;
		public event Action<MenuOptionModel> MenuOptionSelectedHandler;

		UITapGestureRecognizer _menuTapGesture;
		UIPanGestureRecognizer _menuPanGesture;

		int _minSize = 60;
		int _maxSize = 0;

		int _compactChevronSize = 18;

		nfloat? _bottomPositionY;
		nfloat? _topPositionY;

		UITableView _expandedTableView;
		UILabel _collapsedLabel;
		UIView _chevronContainer;

		UIView _chevronView;

		UITableViewSource _expandedTablaViewSource;

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
			UIPosition = ContentPositionType.Center;
			TranslatesAutoresizingMaskIntoConstraints = false;
			ShowCurrentSelection = true;
			HideMenuBackgroundOnCollapse = true;
			AddRoomForNavigationBar = false;
			ContentWidth = 300;
			MenuBackgroundColor = UIColor.White;
			MenuShouldFillScreen = true;
			ChevronColor = UIColor.Black;
			DisplayLabelColor = UIColor.Black;
			BorderColor = UIColor.Black;
			MaxBackgroundAlpha = 1;
			CloseMenuOnSelection = true;
			UsesSpringAnimation = true;
			HideCurrentSelectionFromMenu = false;
			DisplayLabelFont = UIFont.BoldSystemFontOfSize(16);
		}

		private void SetLayer()
		{
			Layer.MasksToBounds = false;
			Layer.CornerRadius = 10;
			Layer.ShadowColor = UIColor.DarkGray.CGColor;
			Layer.ShadowOpacity = 1.0f;
			Layer.ShadowRadius = 6.0f;
			Layer.ShadowOffset = new System.Drawing.SizeF(0f, 3f);
		}

		#region Add Menu to View

		public void AddMenuToView(UIView superView, IEnumerable<MenuOptionModel> values, MenuOptionModel presetValue = null)
		{
			if (superView == null) throw new ArgumentNullException(nameof(superView), "Super View can't be null");
			if (values == null) throw new ArgumentNullException(nameof(values), "Must supply at least one value");

			SetLayer();

			var valueCount = HideCurrentSelectionFromMenu ? values.Count () - 1 : values.Count ();
			int estimatedHeight = valueCount * _estimatedRowHeight + _compactChevronSize + 20;
			estimatedHeight = Math.Max(80, estimatedHeight);

			// If collapsed size is bigger then the expanded size and its bigger then the default 
			//		size it will be reset both.
			if (_minSize > _maxSize && _minSize > estimatedHeight)
			{
				_maxSize = estimatedHeight;
				_minSize = 60;
			}
			else if (_minSize > _maxSize)
			{
				_maxSize = estimatedHeight;
			}

			// check to see if there already is a superview before adding
			if (this.Superview == null)
			{
				superView.AddSubview(this);
			}

			_currentSelection = presetValue;

			_values = values;

			SetupUI(Superview);

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
			_collapsedLabel.Alpha = 1;

			_chevronContainer.UserInteractionEnabled = true;
			_chevronView.UserInteractionEnabled = false;

			if (HideMenuBackgroundOnCollapse)
			{
				BackgroundColor = MenuBackgroundColor.ColorWithAlpha(0);
			}
			else
			{
				BackgroundColor = MenuBackgroundColor;
			}

			_chevronView.Transform = CGAffineTransform.MakeRotation(GetChevronAngle(menuOpen: false));
			_expandedTableView.ReloadData();
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

		private void SetupUI(UIView superView)
		{
			// Instantaite the menu UI
			_expandedTableView = GetExpandedMenuTable();
			_collapsedLabel = GetCollapsedMenuLabel();
			_chevronContainer = GetChevronContainerView();
			_chevronView = GetChevronView();

			// add everything to view before applying constraints
			AddSubview(_collapsedLabel);
			AddSubview(_chevronContainer);
			AddSubview(_chevronView);
			AddSubview(_expandedTableView);

			var model = new MenuConstraintModel
			{
				CollapsedSize = _minSize,
				ChevronView = _chevronView,
				ChevronContainer = _chevronContainer,
				ContentWidth = ContentWidth,
				MenuView = this,
				MenuShouldFillScreen = MenuShouldFillScreen,
				ContentPosition = UIPosition
			};

			switch (_position)
			{
				case MenuPositionType.Bottom:
					_menuConstraints = new BottomMenuConstraints(model);
					break;
				case MenuPositionType.Top:
					_menuConstraints = new TopMenuConstraints(model);
					break;
			}

			_menuConstraints.AddChevronContainerConstraints();
			_menuConstraints.AddChevronConstraints();
			_menuConstraints.AddContentConstraints(_expandedTableView);
			_menuConstraints.AddMainConstraints(superView, AddRoomForNavigationBar);
			_menuConstraints.AddContentConstraints(_collapsedLabel);

			// retrieve height constraints
			_mainHeightConstraint = superView.Constraints.First(c => c.FirstItem == this && c.FirstAttribute == NSLayoutAttribute.Height);
			_chevronHeightConstraint = this.Constraints.First(c => c.FirstItem == _chevronContainer && c.FirstAttribute == NSLayoutAttribute.Height);

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

		private UIView GetCornerView()
		{
			var view = new UIView();
			view.BackgroundColor = UIColor.Clear;
			view.TranslatesAutoresizingMaskIntoConstraints = false;

			return view;
		}

		private UIView GetChevronView()
		{
			var view = new ChevronView(ChevronColor);
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
			table.Source = ExpandedTableViewSource;
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

			label.Font = DisplayLabelFont;
			label.TextColor = DisplayLabelColor;
			label.Text = _currentSelection?.DisplayName ?? _values.FirstOrDefault()?.DisplayName;
			label.TextAlignment = UITextAlignment.Center;
			label.TranslatesAutoresizingMaskIntoConstraints = false;
			return label;
		}

		#endregion

		#region Menu Interaction

		private void RotateChevron()
		{
			var degrees = (_mainHeightConstraint.Constant / _maxSize) * 180;
			if (_position == MenuPositionType.Top) degrees += 180;

			var radians = degrees * (Math.PI / 180);

			_chevronView.Transform = CGAffineTransform.MakeRotation((nfloat)radians);
		}

		private void SlideMenuFromPan(UIPanGestureRecognizer panGesture)
		{
			var menuTouchLocation = panGesture.LocationInView(this);
			var velocity = panGesture.VelocityInView(this);

			if (panGesture.State == UIGestureRecognizerState.Began && panGesture.NumberOfTouches == 1)
			{
				_bottomPositionY = menuTouchLocation.Y;
				_topPositionY = menuTouchLocation.Y;
			}
			else if (panGesture.State == UIGestureRecognizerState.Changed && panGesture.NumberOfTouches == 1)
			{
				nfloat offset = 0;

				// get offset from current touch to menu to create smoothe pan animation
				if (_position == MenuPositionType.Bottom)
				{
					offset = (_bottomPositionY ?? 0) - menuTouchLocation.Y;
				}
				if (_position == MenuPositionType.Top)
				{
					offset = (_topPositionY ?? 0) - menuTouchLocation.Y;
					offset *= -1;
				}

				var alpha = (nfloat)Math.Pow(_mainHeightConstraint.Constant / _maxSize, 2);

				// ensure alpha is between 0 and 1, only happens if dragged further then the bounds
				if (alpha > 1)
					alpha = 1;
				if (alpha < 0)
					alpha = 0;

				// only update chevron if it needs it.  This will cause a re-draw
				if (_chevronHeightConstraint.Constant != _compactChevronSize)
				{
					_chevronHeightConstraint.Constant = _compactChevronSize;
					_chevronContainer.SetNeedsDisplay();
				}

				RotateChevron();

				// alpha is calculated for expanded view so take opposite for compact
				_expandedTableView.Alpha = alpha;
				_collapsedLabel.Alpha = 1 - alpha;

				nfloat backgroundAlpha = 1;

				// if the menu is going to be hidden in compact mode change alpha of main background
				if (HideMenuBackgroundOnCollapse == true)
				{
					backgroundAlpha = _expandedTableView.Alpha > MaxBackgroundAlpha ? MaxBackgroundAlpha : _expandedTableView.Alpha;
					BackgroundColor = BackgroundColor.ColorWithAlpha(backgroundAlpha);
				}

				// instantly update height, do not use animation or it will not keep up with pan gesture
				_mainHeightConstraint.Constant += offset;
				this.LayoutIfNeeded();

				// save change in location, only used when menu is on top
				_topPositionY = menuTouchLocation.Y;
			}
			else if (panGesture.State == UIGestureRecognizerState.Ended)
			{
				// set final position based on swipe direction
				int finalSize = 0;

				if (_position == MenuPositionType.Bottom)
				{
					finalSize = velocity.Y < 0 ? _maxSize : _minSize;
				}
				else if (_position == MenuPositionType.Top)
				{
					finalSize = velocity.Y > 0 ? _maxSize : _minSize;
				}

				// this will not actually change anything untill LayoutIfNeeded is called
				_mainHeightConstraint.Constant = finalSize;

				// get final alpha values for animation
				int compactViewAlpha = Convert.ToInt16(finalSize == _minSize);
				int expandedViewAlpha = Convert.ToInt16(finalSize == _maxSize);

				AnimateMenu(finalSize, compactViewAlpha, expandedViewAlpha, null);
			}
		}

		private void ShowOrHideMenuFromTap()
		{
			var newHeight = MenuOpen ? _minSize : _maxSize;
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

			nfloat backgroundAlpha = expandedViewAlpha > MaxBackgroundAlpha ? MaxBackgroundAlpha : expandedViewAlpha;

			AnimateNotify(
				// duration
				UsesSpringAnimation ? 1f : 0.5f,
				// delay
	            0, 
				// spring ratio
	            UsesSpringAnimation ? 0.5f : 1, 
				// initial spring velocity
	            UsesSpringAnimation ? 0.8f : 1, 
	            UIViewAnimationOptions.CurveEaseInOut, 
				() =>
			   {
					_chevronView.Transform = CGAffineTransform.MakeRotation(GetChevronAngle(menuOpen));
				   _collapsedLabel.Alpha = collapsedViewAlpha;
				   _expandedTableView.Alpha = expandedViewAlpha;
				   BackgroundColor = BackgroundColor.ColorWithAlpha( HideMenuBackgroundOnCollapse ? backgroundAlpha : 1);
				   
					this.LayoutIfNeeded();
		   	   }, completionBlock);

			UpdateMenuLayout(menuOpen);

			if (menuOpen && MenuOpenHandler != null)
			{
				MenuOpenHandler();
			}
			else if (MenuClosedHandler != null)
			{
				MenuClosedHandler();
			}

			MenuOpen = menuOpen;
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
		}

		#endregion

		#region Public Methods

		public void AnimateClosed(UICompletionHandler completionHandler)
		{
			AnimateMenu(_minSize, 1, 0, completionHandler);
		}

		public void AnimateOpen(UICompletionHandler completionHandler)
		{
			AnimateMenu(_maxSize, 0, 1, completionHandler);
		}

		public void SetDisplayLabel(string str)
		{
			_collapsedLabel.Text = str;
		}

		#endregion

		#region Menu Selection Handlers

		private void MenuSelectionHandler(MenuOptionModel model)
		{
			if (MenuOptionSelectedHandler != null)
			{
				MenuOptionSelectedHandler(model);
			}

			SetDisplayLabel(model.DisplayName);
			_expandedTableView.ReloadData ();

			if (CloseMenuOnSelection)
			{
				AnimateClosed(null);
			}
		}

		private Action<MenuOptionModel> GetMenuSelectionHandler()
		{
			return new Action<MenuOptionModel>(MenuSelectionHandler);
		}

		#endregion

		#region Public Properties

		public UIFont DisplayLabelFont { get; set; }

		public UITableViewSource ExpandedTableViewSource
		{
			get
			{
				return _expandedTablaViewSource ?? new MenuTableViewSource(_values, GetMenuSelectionHandler(), HideCurrentSelectionFromMenu, _currentSelection);
			}
			set
			{
				_expandedTableView.Source = _expandedTablaViewSource = value ?? new MenuTableViewSource(_values,  GetMenuSelectionHandler(), HideCurrentSelectionFromMenu, _currentSelection);
				_expandedTableView.ReloadData();
			}
		}

		public UITableView ExpandedTableView
		{
			get
			{
				return _expandedTableView;
			}
		}

		public bool HideCurrentSelectionFromMenu { get; set; }

		public bool UsesSpringAnimation { get; set; }

		public bool CloseMenuOnSelection { get; set; }

		public nfloat MaxBackgroundAlpha { get; set; }

		public UIColor ChevronColor { get; set; }

		public UIColor DisplayLabelColor { get; set; }

		public UIColor BorderColor { get; set; }

		public bool MenuShouldFillScreen { get; set; }

		public UIColor MenuBackgroundColor { get; set; }

		public ContentPositionType UIPosition { get; set; }

		public bool AddRoomForNavigationBar { get; set; }

		public bool HideMenuBackgroundOnCollapse { get; set; }

		public bool ShowCurrentSelection { get; set; }

		public bool MenuOpen { get; set; }

		public int ContentWidth { get; set; }

		public int ExpandedMenuSize
		{
			get
			{
				return _maxSize - 10;
			}
			set
			{
				_maxSize = value + 10;
			}
		}

		public int CollapsedMenuSize
		{
			get
			{
				return _minSize - 10;
			}
			set
			{
				if (value >= 50)
				{
					_minSize = value + 10; 
				}
			}
		}

		#endregion
	}
}

