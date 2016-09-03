using System;
using CoreGraphics;
using UIKit;

namespace SlideMenu
{
	public class MenuBoarder : UIView
	{
		public MenuBoarder()
		{
			TranslatesAutoresizingMaskIntoConstraints = false;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			this.BackgroundColor = UIColor.DarkGray;
			var shadowPath = UIBezierPath.FromRect(this.Bounds);
			this.Layer.MasksToBounds = false;
			this.Layer.ShadowColor = UIColor.DarkGray.CGColor;
			this.Layer.ShadowOpacity = 0.50f;
			this.Layer.ShadowRadius = 1.0f;
			this.Layer.ShadowOffset = new CGSize(0.0f, -1.0f);
			this.Layer.ShadowPath = shadowPath.CGPath;
		}
	}
}

