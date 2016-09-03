using System;
using UIKit;

namespace SlideMenu
{
	public interface IMenuConstraints
	{
		/// <summary>
		/// Adds the main constraints to the menu view
		/// </summary>
		void AddMainConstraints(UIView menuView, UIView superView, bool addRoomForNavigationBar, int? initialHeight);

		/// <summary>
		/// Adds the constraints to keep the border anchored to the menu
		/// </summary>
		/// <param name="menuView">Menu view.</param>
		/// <param name="borderView">Border view.</param>
		void AddBorderConstraints(UIView menuView, UIView borderView);


		/// <summary>
		/// Adds the chevron container constraints.
		/// </summary>
		/// <param name="menuView">Menu view.</param>
		/// <param name="chevronContainerView">Chevron container view.</param>
		void AddChevronContainerConstraints(UIView menuView, UIView chevronContainerView, double chevronOffset, int? initialHeight);


		/// <summary>
		/// Adds the chevron constraints.
		/// </summary>
		/// <param name="menuView">Menu view.</param>
		/// <param name="chevronView">Chevron view.</param>
		void AddChevronConstraints(UIView menuView, UIView chevronView);

		/// <summary>
		/// Adds the content constraints. This is the menu table and selection label if used
		/// </summary>
		/// <param name="menuView">Menu view.</param>
		/// <param name="contentView">Content view.</param>
		void AddContentConstraints(UIView menuView, UIView contentView, UIView chevronView);
	}
}

