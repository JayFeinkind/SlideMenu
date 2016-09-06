using System;
using UIKit;

namespace SlideMenu
{
	internal abstract class MenuConstraints : IMenuConstraints
	{
		protected UIView _menuView;
		protected UIView _chevronView;
		protected UIView _chevronContainer;

		protected int _contentWidth;
		protected int _collapsedSize;
		protected bool _menuShouldFillScreen;
		protected ContentPositionType _contentPosition;

		protected MenuConstraints(MenuConstraintModel model)
		{
			_collapsedSize = model.CollapsedSize;
			_menuView = model.MenuView;
			_chevronView = model.ChevronView;
			_chevronContainer = model.ChevronContainer;
			_contentWidth = model.ContentWidth;
			_menuShouldFillScreen = model.MenuShouldFillScreen;
			_contentPosition = model.ContentPosition;
		}

		public abstract void AddMainConstraints(UIView superView, bool addRoomForNavigationBar);

		public abstract void AddChevronContainerConstraints();

		public abstract void AddChevronConstraints();

		public abstract void AddContentConstraints(UIView contentView);
	}
}

