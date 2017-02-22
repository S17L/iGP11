using System.Collections.Generic;
using System.Linq;

namespace iGP11.Library.Component
{
    public class TokenReplacer : ITokenReplacer
    {
        private readonly IEnumerable<ITokenReplacingPolicy> _policies;

        public TokenReplacer(params ITokenReplacingPolicy[] policies)
        {
            _policies = policies?.ToArray() ?? new ITokenReplacingPolicy[0];
        }

        public string Replace(string expression)
        {
            return _policies.Aggregate(expression, (current, policy) => policy.Apply(current));
        }
    }
}