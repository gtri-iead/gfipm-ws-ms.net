using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class UriUtil
    {
        static public bool TryCreateValidUri(string uriString, UriKind uriKind, out Uri result)
        {
            return Uri.TryCreate(uriString, uriKind, out result);
        }

        static public bool CanCreateValidUri(string uriString, UriKind uriKind)
        {
            Uri uri;
            return TryCreateValidUri(uriString, uriKind, out uri);
        }
    }
}
