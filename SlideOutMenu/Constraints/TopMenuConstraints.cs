using System;
using UIKit;

namespace SlideMenu
{
	internal class TopMenuConstraints : MenuConstraints
	{
		public TopMenuConstraints(MenuConstraintModel model) : base(model)
		{

		}

		public override void AddChevronConstraints()
		{
			_menuView.AddConstraint(NSLayoutConstraint.Create(_chevronView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 18));
			_menuView.AddConstraint(NSLayoutConstraint.Create(_chevronView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 30));
			_menuView.AddConstraint(NSLayoutConstraint.Create(_chevronView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, _chevronContainer, NSLayoutAttribute.CenterX, 1, 0));
			_menuView.AddConstraint(NSLayoutConstraint.Create(_chevronView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, _chevronContainer, NSLayoutAttribute.Bottom, 1, -5));
		}

		public override void AddChevronContainerConstraints()
		{
			// X position will be set based on content position in AddContentConstraints()
			_menuView.AddConstraint(NSLayoutConstraint.Create(_chevronContainer, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, _collapsedSize));
			_menuView.AddConstraint(NSLayoutConstraint.Create(_chevronContainer, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 30));
			_menuView.AddConstraint(NSLayoutConstraint.Create(_chevronContainer, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, _menuView, NSLayoutAttribute.Bottom, 1, -5));
		}

		public override void AddContentConstraints(UIView contentView)
		{

			_menuView.AddConstraint(NSLayoutConstraint.Create(_chevronView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, contentView, NSLayoutAttribute.CenterX, 1, 0));


			_menuView.AddConstraint(
				NSLayoutConstraint.Create(
					contentView,
					NSLayoutAttribute.Bottom,
					NSLayoutRelation.Equal,
					_chevronView,
					NSLayoutAttribute.Top,
					1,
					0));


			_menuView.AddConstraint(
				NSLayoutConstraint.Create(
					_menuView,
					NSLayoutAttribute.Top,
					NSLayoutRelation.GreaterThanOrEqual,
					contentView,
					NSLayoutAttribute.Top,
				1,
				-10));
			
			// width constraint
			contentView.AddConstraint(
				NSLayoutConstraint.Create(
					contentView,
					NSLayoutAttribute.Width,
					NSLayoutRelation.Equal,
					null,
					NSLayoutAttribute.NoAttribute,
					1,
					_contentWidth));


			switch (_contentPosition)
			{
				case ContentPositionType.Center:
					// center horizontally
					_menuView.AddConstraint(
						NSLayoutConstraint.Create(
							_menuView,
							NSLayoutAttribute.CenterX,
							NSLayoutRelation.Equal,
							contentView,
							NSLayoutAttribute.CenterX,
							1,
							0));
					break;
				case ContentPositionType.Right:
					_menuView.AddConstraint(
						NSLayoutConstraint.Create(
							contentView, 
							NSLayoutAttribute.Right, 
							NSLayoutRelation.Equal,
							_menuView, 
							NSLayoutAttribute.Right, 
							1, 
							0));
					break;
				case ContentPositionType.Left:
					_menuView.AddConstraint(
						NSLayoutConstraint.Create(
							_menuView,
							NSLayoutAttribute.Left,
							NSLayoutRelation.Equal,
							contentView,
							NSLayoutAttribute.Left,
							1,
							0));
					break;
			}
		}

		public override void AddMainConstraints(UIView superView, bool addRoomForNavigationBar)
		{
			// 64 is 44 for nav bar and 20 for status bar
			nfloat top = addRoomForNavigationBar ? 54 : -10;
			superView.AddConstraint(NSLayoutConstraint.Create(_menuView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, _collapsedSize));
			superView.AddConstraint(NSLayoutConstraint.Create(_menuView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, superView, NSLayoutAttribute.TopMargin, 1, top));

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

