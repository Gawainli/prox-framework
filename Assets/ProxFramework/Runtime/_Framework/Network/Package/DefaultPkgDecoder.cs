using System.Collections.Generic;

namespace ProxFramework.Network
{
    public class DefaultPkgDecoder : INetPackageDecoder
    {
        public void Decode(RingBuffer ringBuffer, List<INetPackage> outNetPackages)
        {
            while (true)
            {
                if (ringBuffer.ReadableBytes < DefaultNetPackage.HeaderSize)
                {
                    break;
                }

                ringBuffer.MarkReaderIndex();

                var msgId = ringBuffer.ReadInt();
                var msgBodyLength = ringBuffer.ReadInt();

                if (ringBuffer.ReadableBytes < msgBodyLength)
                {
                    ringBuffer.ResetReaderIndex();
                    break;
                }

                if (msgBodyLength > DefaultNetPackage.BodyMaxSize)
                {
                    break;
                }

                var pkg = new DefaultNetPackage
                {
                    MsgId = msgId,
                    BodyBytes = ringBuffer.ReadBytes(msgBodyLength)
                };
                outNetPackages.Add(pkg);
            }

            ringBuffer.DiscardReadBytes();
        }

        public INetPackage Decode(byte[] bytes)
        {
            return null;
        }
    }
}