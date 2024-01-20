using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GenshinLike.AP
{
    public static class ObjectExtensions
    {
        public static void Say(this object _object, string _message)
        {
            System.Diagnostics.Debug.WriteLine("[{0}] {1}", _object.GetType().Name, _message);
            Console.WriteLine("[{0}] {1}", _object.GetType().Name, _message);
        }

        public static void SayFormat(this object _object, string _format, params object[] _arguments)
        {
            System.Diagnostics.Debug.WriteLine("[{0}] {1}", _object.GetType().Name, string.Format(_format, _arguments));
            Console.WriteLine("[{0}] {1}", _object.GetType().Name, string.Format(_format, _arguments));
        }
    }
}
