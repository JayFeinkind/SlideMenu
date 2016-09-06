using System;
using UIKit;

namespace SlideMenu
{
	internal interface IMenuConstraints
	{
		/// <summary>
		/// Adds the main constraints to the menu view
		/// </summary>
		void AddMainConstraints(UIView superView, bool addRoomForNavigationBar);

		/// <summary>
		/// Adds the chevron container constraints.
		/// </summary>
		/// <param name="menuView">Menu view.</param>
		/// <param name="chevronContainerView">Chevron container view.</param>
		void AddChevronContainerConstraints();


		/// <summary>
		/// Adds the chevron constraints.
		/// </summary>
		/// <param name="menuView">Menu view.</param>
		/// <param name="chevronView">Chevron view.</param>
		void AddChevronConstraints();

		/// <summary>
		/// Adds the content constraints. This is the menu table and selection label if used
		/// </summary>
		/// <param name="menuView">Menu view.</param>
		/// <param name="contentView">Content view.</param>
		void AddContentConstraints(UIView contentView);
	}
}

