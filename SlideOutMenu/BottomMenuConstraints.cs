using System;
using UIKit;

namespace SlideMenu
{
	public class BottomMenuConstraints : IMenuConstraints
	{
		public void AddBorderConstraints(UIView menuView, UIView borderView)
		{
			menuView.AddConstraint(NSLayoutConstraint.Create(borderView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 1));
			menuView.AddConstraint(NSLayoutConstraint.Create(borderView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, menuView, NSLayoutAttribute.Top, 1, 0));
			menuView.AddConstraint(NSLayoutConstraint.Create(borderView, NSLayoutAttribute.Left, NSLayoutRelation.Equal, menuView, NSLayoutAttribute.Left, 1, 0));
			menuView.AddConstraint(NSLayoutConstraint.Create(borderView, NSLayoutAttribute.Right, NSLayoutRelation.Equal, menuView, NSLayoutAttribute.Right, 1, 0));
		}

		public void AddChevronConstraints(UIView menuView, UIView chevronView)
		{
			menuView.AddConstraint(NSLayoutConstraint.Create(chevronView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 18));
			menuView.AddConstraint(NSLayoutConstraint.Create(chevronView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 30));
			menuView.AddConstraint(NSLayoutConstraint.Create(chevronView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, menuView, NSLayoutAttribute.CenterX, 1, 0));
			menuView.AddConstraint(NSLayoutConstraint.Create(chevronView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, menuView, NSLayoutAttribute.Top, 1, 5));

		}

		public void AddChevronContainerConstraints(UIView menuView, UIView chevronView, double chevronOffset, int? initialHeight)
		{
			menuView.AddConstraint(NSLayoutConstraint.Create(chevronView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, initialHeight ?? 0));
			menuView.AddConstraint(NSLayoutConstraint.Create(chevronView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 30));
			menuView.AddConstraint(NSLayoutConstraint.Create(chevronView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, menuView, NSLayoutAttribute.CenterX, 1, (nfloat)chevronOffset));
			menuView.AddConstraint(NSLayoutConstraint.Create(chevronView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, menuView, NSLayoutAttribute.Top, 1, 5));
		}

		public void AddContentConstraints(UIView menuView, UIView contentView, UIView chevronView)
		{
			// anchor to bottom of arrow image
			menuView.AddConstraint(
				NSLayoutConstraint.Create(
					chevronView,
					NSLayoutAttribute.Bottom,
					NSLayoutRelation.Equal,
					contentView,
					NSLayoutAttribute.Top,
					1,
					0));

			// bottom constraint
			menuView.AddConstraint(
				NSLayoutConstraint.Create(
					contentView,
					NSLayoutAttribute.Bottom,
					NSLayoutRelation.GreaterThanOrEqual,
					menuView,
					NSLayoutAttribute.Bottom,
				1,
				1));
			

			// width constraint
			contentView.AddConstraint(
				NSLayoutConstraint.Create(
					contentView,
					NSLayoutAttribute.Width,
					NSLayoutRelation.Equal,
					null,
					NSLayoutAttribute.NoAttribute,
					1,
					300));

			// center horizontally
			menuView.AddConstraint(
				NSLayoutConstraint.Create(
					menuView,
					NSLayoutAttribute.CenterX,
					NSLayoutRelation.Equal,
					contentView,
					NSLayoutAttribute.CenterX,
					1,
					0));
		}

		public void AddMainConstraints(UIView menuView, UIView superView, bool addRoomForNavigationBar, int? initialHeight)
		{
			nfloat bottom = addRoomForNavigationBar ? 50 : 0;
			superView.AddConstraint(NSLayoutConstraint.Create(menuView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, (nfloat)initialHeight));
			superView.AddConstraint(NSLayoutConstraint.Create(menuView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, superView, NSLayoutAttribute.Bottom, 1, -bottom));
			superView.AddConstraint(NSLayoutConstraint.Create(menuView, NSLayoutAttribute.Right, NSLayoutRelation.Equal, superView, NSLayoutAttribute.Right, 1, 0));
			superView.AddConstraint(NSLayoutConstraint.Create(menuView, NSLayoutAttribute.Left, NSLayoutRelation.Equal, superView, NSLayoutAttribute.Left, 1, 0));
		}
	}
}

