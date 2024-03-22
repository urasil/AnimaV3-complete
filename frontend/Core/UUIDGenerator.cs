using System;

namespace dotnetAnima.Core {
    public static class UUIDGenerator {
        // Generate a V4 UUID identifier
        public static string NewUUID() { return Guid.NewGuid().ToString(); }
    }
}
