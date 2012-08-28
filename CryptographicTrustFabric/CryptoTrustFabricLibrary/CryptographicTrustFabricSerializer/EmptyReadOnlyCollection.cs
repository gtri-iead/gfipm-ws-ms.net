using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace GfipmCryptographicTrustFabricSerializer
{
    internal static class EmptyReadOnlyCollection<T>
    {
        // Fields
        public static readonly ReadOnlyCollection<T> Instance;

        // Methods
        static EmptyReadOnlyCollection()
        {
            EmptyReadOnlyCollection<T>.Instance = new ReadOnlyCollection<T>(new T[0]);
        }
    }
}
