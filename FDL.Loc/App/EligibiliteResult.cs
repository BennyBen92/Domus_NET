namespace FDL.Loc.App
{
    public record EligibiliteResult(bool EstEligible, string Motif)
    {
        public static EligibiliteResult Eligible(string motif) => new (true,motif);
        public static EligibiliteResult NonEligible(string motif) => new (false,motif);
    }
}
