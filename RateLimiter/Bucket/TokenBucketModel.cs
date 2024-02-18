using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter.Bucket
{
    public sealed class TokenBucketModel
    {
        public TokenBucketModel(int rate, int bucketSize)
        {
            Rate = rate;
            BucketSize = bucketSize;
        }

        public Guid BucketId { get; set; } = Guid.NewGuid();
        public int Rate { get; set; } = 10;
        public int BucketSize { get; set; } = 3;
        public DateTime LastRefillTime { get; set; } = DateTime.UtcNow;

    }
}
