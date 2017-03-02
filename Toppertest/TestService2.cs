using System;

namespace Toppertest
{
    class TestService2 : IDisposable
    {
        public TestService2()
        {
            throw new InvalidOperationException();
        }
        public void Dispose()
        {
        }
    }
}