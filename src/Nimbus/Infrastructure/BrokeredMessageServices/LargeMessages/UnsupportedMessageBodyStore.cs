using System;
using System.Threading.Tasks;

namespace Nimbus.Infrastructure.BrokeredMessageServices.LargeMessages
{
    internal class UnsupportedMessageBodyStore : IMessageBodyStore
    {
        public const string FailureMessage = "Your message was too large. Large message support must be configured via BusBuilder.WithLargeMessageBodyStore(...) before use.";

        public Task Store(string id, byte[] bytes, DateTimeOffset expiresAfter)
        {
            throw new NotSupportedException(FailureMessage);
        }

        public Task<byte[]> Retrieve(string id)
        {
            throw new NotSupportedException(FailureMessage);
        }

        public Task Delete(string id)
        {
            throw new NotSupportedException(FailureMessage);
        }
    }
}