using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Fox.Core;
using Fox.Core.Drawing;

namespace Fox.Controls
{
    public class PreviewCanvas : Control
    {
        public static readonly DirectProperty<PreviewCanvas, RenderTargetBitmap> BitmapProperty =
            AvaloniaProperty.RegisterDirect<PreviewCanvas, RenderTargetBitmap>(
            nameof(Bitmap),
            o => o.bitmap,
            (o, v) => o.bitmap = v,
            defaultBindingMode: BindingMode.OneWayToSource);

        RenderTargetBitmap bitmap;
        public RenderTargetBitmap Bitmap
        {
            get
            {
                return bitmap;
            }

            set
            {
                this.SetAndRaise(BitmapProperty, ref bitmap, value);
            }
        }

        public static readonly DirectProperty<PreviewCanvas, DrawCommandRegister> DrawCommandsProperty =
            AvaloniaProperty.RegisterDirect<PreviewCanvas, DrawCommandRegister>(
            nameof(DrawCommands),
            o => o.DrawCommands,
            (o, v) =>
            {
                o.DrawCommands = v;
                o.Init(v);
            },
            defaultBindingMode: BindingMode.OneWay);

        private DrawCommandRegister drawCommands;
        public DrawCommandRegister DrawCommands
        {
            get => drawCommands;
            set => this.SetAndRaise(DrawCommandsProperty, ref drawCommands, value);
        }

        public PreviewCanvas()
        {
            Bitmap = new RenderTargetBitmap(new PixelSize(1000, 1000));
        }

        void Init(DrawCommandRegister commands) 
        {
            if (commands == null) 
                return;

            commands.onCommandAdded += (command) =>
            {
                var lastBitmap = new RenderTargetBitmap(bitmap.PixelSize, bitmap.Dpi);
                
                using (var b = lastBitmap.CreateDrawingContext())
                b.DrawImage(bitmap, new Rect(bitmap.Size));
                
                using (var b = bitmap.CreateDrawingContext())
                { 
                    b.DrawImage(lastBitmap, new Rect(bitmap.Size));
                    command(b);
                }

                InvalidateVisual();
            };
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            context.DrawImage(bitmap, new Rect(0, 0, this.Bounds.Width, this.Bounds.Height));
        }
    }
}
