using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint.Runtime;

namespace Fox.Core
{
    public static class JsExeptions
    {
        public static void IsValueInRange(int value, int min, int max) 
        {
            if (value < min || value > max)
                throw new JavaScriptException($"value was either too large or too small, value= {value}, min= {min}, max= {max}");
        }
        
        public static void IsColorValid(int a, int r, int g, int b) 
        {
            IsValueInRange(a, 0, 255);
            IsValueInRange(r, 0, 255);
            IsValueInRange(g, 0, 255);
            IsValueInRange(b, 0, 255);
        }
    }
}
