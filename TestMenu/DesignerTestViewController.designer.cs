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
    [Register ("DesignerTestViewController")]
    partial class DesignerTestViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel _displayLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        SlideMenu.SlideView _slideMenu { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton NavigateButton { get; set; }

        [Action ("NavigateButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void NavigateButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (_displayLabel != null) {
                _displayLabel.Dispose ();
                _displayLabel = null;
            }

            if (_slideMenu != null) {
                _slideMenu.Dispose ();
                _slideMenu = null;
            }

            if (NavigateButton != null) {
                NavigateButton.Dispose ();
                NavigateButton = null;
            }
        }
    }
}