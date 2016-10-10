// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace TestMenu
{
    [Register ("SlideOutMenuViewController")]
    partial class SlideOutMenuViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView _chevronContiainer { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        TestMenu.ChevronView _chevronView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView _mainTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView _menuSelectionTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel _selectionLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        SlideMenu.SlideView _slideOutMenu { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint MenuHeightConstraint { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (_chevronContiainer != null) {
                _chevronContiainer.Dispose ();
                _chevronContiainer = null;
            }

            if (_chevronView != null) {
                _chevronView.Dispose ();
                _chevronView = null;
            }

            if (_mainTableView != null) {
                _mainTableView.Dispose ();
                _mainTableView = null;
            }

            if (_menuSelectionTableView != null) {
                _menuSelectionTableView.Dispose ();
                _menuSelectionTableView = null;
            }

            if (_selectionLabel != null) {
                _selectionLabel.Dispose ();
                _selectionLabel = null;
            }

            if (_slideOutMenu != null) {
                _slideOutMenu.Dispose ();
                _slideOutMenu = null;
            }

            if (MenuHeightConstraint != null) {
                MenuHeightConstraint.Dispose ();
                MenuHeightConstraint = null;
            }
        }
    }
}