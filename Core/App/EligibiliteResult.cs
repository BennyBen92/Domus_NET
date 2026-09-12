using System;
using System.Collections.Generic;
using System.Text;

namespace FDL.Core.App
{
    public record EligibiliteResult(bool EstEligible, string motif)
    {
        public static EligibiliteResult Eligible(string motif) => new (true,motif);
        public static EligibiliteResult NonEligible(string motif) => new (false,motif);
    }
}
