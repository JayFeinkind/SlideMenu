using System;
using System.Drawing;
using CoreGraphics;
using UIKit;

namespace SlideMenu
{
	public class ChevronView : UIView
	{
		UIColor _color;

		public ChevronView(UIColor color)
		{
			_color = color;
		}

		public override void Draw(CGRect rect)
		{
			base.Draw(rect);

			var context = UIGraphics.GetCurrentContext();

			context.SetLineWidth(2);
			_color.SetStroke();
			UIColor.Clear.SetFill();

			var topPath = new CGPath();
			var bottomPath = new CGPath();

			topPath.AddLines(new CGPoint[]{
				new CGPoint((rect.Width / 2) - 10, 11),
				new CGPoint(rect.Width / 2, 3),
				new CGPoint((rect.Width / 2) + 10, 11)
			});

			bottomPath.AddLines(new CGPoint[]{
				new CGPoint((rect.Width / 2) - 10, 16),
				new CGPoint(rect.Width / 2, 8),
				new CGPoint((rect.Width / 2) + 10, 16)
			});

			context.AddPath(topPath);
			context.AddPath(bottomPath);
			context.DrawPath(CGPathDrawingMode.FillStroke);
		}
	}
}

