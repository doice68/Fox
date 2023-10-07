using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;

namespace Fox.Core.Drawing
{
    public class DrawCommandRegister
    {
        public Action<Action<DrawingContext>> onCommandAdded = (d) => { };

        public void Add(Action<DrawingContext> command)
        {
            onCommandAdded(command);
        }
    }
}
