using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Automation.Peers;
using Avalonia.Automation;
using Avalonia.Controls.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia;

namespace Fox.Controls
{
    public class FitPanel : Panel
    {
        protected override Size ArrangeOverride(Size finalSize)
        {
            var result = new Size(Bounds.Size.Height, Bounds.Size.Height);

            foreach (var control in Children)
            {
                var s = Math.Min(finalSize.Width, finalSize.Height);
                control.Arrange(new Rect(-s / 2, -s / 2, s, s));
            }
            return result;
        }
    }
}
