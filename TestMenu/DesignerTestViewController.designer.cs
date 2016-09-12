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
        SlideMenu.SlideView SlideMenu { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (SlideMenu != null) {
                SlideMenu.Dispose ();
                SlideMenu = null;
            }
        }
    }
}