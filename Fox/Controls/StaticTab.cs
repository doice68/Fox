using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Fox.Core;

namespace Fox.Controls
{
    public class StaticTab : ContentControl
    {
        public static readonly DirectProperty<StaticTab, object> HeaderProperty =
            AvaloniaProperty.RegisterDirect<StaticTab, object>(
            nameof(Header),
            o => o.Header,
            (o, v) => o.Header = v);

        object header;
        public object Header 
        {
            get => header;
            set => this.SetAndRaise(HeaderProperty, ref header, value);
        }
    }
}
