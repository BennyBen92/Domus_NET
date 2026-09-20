namespace FDL.WF.Domain
{
    public sealed record WorkflowTerminaison(int IdUserTermination, DateTime DateTermination)
    {
        public bool EstTermine => IdUserTermination > 0 && DateTermination != default;
    }
}
