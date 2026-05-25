using System;

namespace Shared.Infra.Data.Providers
{
    public interface IUnityOfWork : IDisposable
    {
        void SaveChanges();
        void CancelChanges();

    }
}
