using System.Collections.Generic;

namespace ProxFramework.Network
{
    public interface INetPackageDecoder
    {
        void Decode(RingBuffer ringBuffer, List<INetPackage> outNetPackages);
    }
}