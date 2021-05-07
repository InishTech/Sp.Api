using System;
using System.Linq;

namespace Sp.Test.Helpers
{
    public static class StringExtensions
    {
        public static string EnsureTrailingSlash( this string input )
        {
            if ( !input.EndsWith( "/" ) )
            {
                return input + "/";
            }

            return input;
        }
    }
}
