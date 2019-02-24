using System;

namespace SharpUtilities.Tests
{
    public class DisposableAction : IDisposable
    {
        private Action action;

        public DisposableAction(Action action)
        {
            this.action = action;
        }

        public void Dispose()
        {
            action();
        }
    }
}
