using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiter
{
    public sealed class TokenBucket
    {
        public TokenBucket(int rate = 10, int bucketSize = 3)
        {
            Rate = rate;
            BucketSize = bucketSize;
        }
        private Guid BucketId { get; set; } = Guid.NewGuid();
        private int Rate { get; set; }
        private int CurrentTokens { get; set; }
        private int BucketSize { get; set; }
        private DateTime LastRefillTime { get; set; }

        private readonly object _lock = new object();

        private readonly Dictionary<Guid, TokenBucket> _buckets = new Dictionary<Guid, TokenBucket>();

        public TokenBucket GetBucket(Guid bucketId)
        {
            if (_buckets.TryGetValue(bucketId, out var bucket))
                return bucket;
            _buckets.Add(bucketId, new TokenBucket());
            return _buckets[bucketId];
        }

        public void RefillBucket()
        {
            TimeSpan totalTimePassed = DateTime.UtcNow - LastRefillTime;
            double tokensToAdd = totalTimePassed.TotalSeconds * Rate;
            CurrentTokens = Convert.ToInt32(Math.Min(BucketSize, CurrentTokens + tokensToAdd));
            LastRefillTime = DateTime.UtcNow;
        }

        public bool IsValid(int tokens)
        {
            lock (_lock)
            {
                RefillBucket();

                if (CurrentTokens >= tokens)
                {
                    CurrentTokens -= tokens;
                    return true;
                }
                return false;
            }
        }
    }
}
