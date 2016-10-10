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
    [Register ("PullMenuViewController")]
    partial class PullMenuViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        SlideMenu.SlideView _pullMenu { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (_pullMenu != null) {
                _pullMenu.Dispose ();
                _pullMenu = null;
            }
        }
    }
}