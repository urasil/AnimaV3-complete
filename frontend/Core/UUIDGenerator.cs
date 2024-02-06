using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnetAnima.Core
{
    public static class UUIDGenerator
    {
        // Generate a V4 UUID identifier
        public static string NewUUID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
