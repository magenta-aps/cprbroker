using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.DAL
{
    public static class Utilities
    {
        public static T[] AsArray<T>( object o) where T : class
        {
            if (o == null)
            {
                return new T[0];
            }
            else
            {
                return new T[] { o as T };
            }            
        }

    }
}
