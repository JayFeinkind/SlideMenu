using System;
using UIKit;

namespace SlideMenu
{
	internal class BottomMenuConstraints : MenuConstraints
	{
		public BottomMenuConstraints(MenuConstraintModel model) : base(model)
		{
			
		}

		public override void AddChevronConstraints()
		{
			_menuView.AddConstraint(NSLayoutConstraint.Create(_chevronView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 18));
			_menuView.AddConstraint(NSLayoutConstraint.Create(_chevronView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 30));
			_menuView.AddConstraint(NSLayoutConstraint.Create(_chevronView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, _chevronContainer, NSLayoutAttribute.CenterX, 1, 0));
			_menuView.AddConstraint(NSLayoutConstraint.Create(_chevronView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _chevronContainer, NSLayoutAttribute.Top, 1, 5));

		}

		public override void AddChevronContainerConstraints()
		{
			_menuView.AddConstraint(NSLayoutConstraint.Create(_chevronContainer, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, _collapsedSize));
			_menuView.AddConstraint(NSLayoutConstraint.Create(_chevronContainer, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 30));
			_menuView.AddConstraint(NSLayoutConstraint.Create(_chevronContainer, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _menuView, NSLayoutAttribute.Top, 1, 5));
		}

		public override void AddContentConstraints(UIView contentView)
		{
			_menuView.AddConstraint(NSLayoutConstraint.Create(_chevronView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, contentView, NSLayoutAttribute.CenterX, 1, 0));

			// anchor to bottom of arrow image
			_menuView.AddConstraint(NSLayoutConstraint.Create(_chevronView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, contentView, NSLayoutAttribute.Top, 1, 0));
			// bottom constraint
			_menuView.AddConstraint(NSLayoutConstraint.Create(contentView, NSLayoutAttribute.Bottom, NSLayoutRelation.GreaterThanOrEqual, _menuView, NSLayoutAttribute.Bottom, 1, -10));

			// width constraint
			contentView.AddConstraint(NSLayoutConstraint.Create(contentView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, _contentWidth));

			if (_menuShouldFillScreen)
			{
				switch (_contentPosition)
				{
					case ContentPositionType.Center:
						_menuView.AddConstraint(NSLayoutConstraint.Create(_menuView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, contentView, NSLayoutAttribute.CenterX, 1, 0));
						break;
					case ContentPositionType.Right:
						_menuView.AddConstraint(NSLayoutConstraint.Create(contentView, NSLayoutAttribute.Right, NSLayoutRelation.Equal, _menuView, NSLayoutAttribute.Right, 1, 0));
						break;
					case ContentPositionType.Left:
						_menuView.AddConstraint(NSLayoutConstraint.Create(_menuView, NSLayoutAttribute.Left, NSLayoutRelation.Equal, contentView, NSLayoutAttribute.Left, 1, 0));
						break;
				}
			}
			else
			{
				_menuView.AddConstraint(NSLayoutConstraint.Create(_menuView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, contentView, NSLayoutAttribute.CenterX, 1, 0));
			}
		}

		public override void AddMainConstraints(UIView superView, bool addRoomForNavigationBar)
		{
			// tab bar is 50 high
			nfloat bottom = addRoomForNavigationBar ? 40 : -10;
			superView.AddConstraint(NSLayoutConstraint.Create(_menuView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, _collapsedSize));
			superView.AddConstraint(NSLayoutConstraint.Create(superView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, _menuView, NSLayoutAttribute.Bottom, 1, bottom));

			if (_menuShouldFillScreen)
			{
				superView.AddConstraint(NSLayoutConstraint.Create(_menuView, NSLayoutAttribute.Right, NSLayoutRelation.Equal, superView, NSLayoutAttribute.Right, 1, 0));
				superView.AddConstraint(NSLayoutConstraint.Create(_menuView, NSLayoutAttribute.Left, NSLayoutRelation.Equal, superView, NSLayoutAttribute.Left, 1, 0));
			}
			else
			{
				superView.AddConstraint(NSLayoutConstraint.Create(_menuView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, _contentWidth + 10));

				switch (_contentPosition)
				{
					case ContentPositionType.Center:
						superView.AddConstraint(NSLayoutConstraint.Create(_menuView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, superView, NSLayoutAttribute.CenterX, 1, 0));
						break;
					case ContentPositionType.Right:
						superView.AddConstraint(NSLayoutConstraint.Create(_menuView, NSLayoutAttribute.Right, NSLayoutRelation.Equal, superView, NSLayoutAttribute.Right, 1, -5));
						break;
					case ContentPositionType.Left:
						superView.AddConstraint(NSLayoutConstraint.Create(superView, NSLayoutAttribute.Left, NSLayoutRelation.Equal, _menuView, NSLayoutAttribute.Left, 1, -5));
						break;
				}
			}
		}
	}
}

