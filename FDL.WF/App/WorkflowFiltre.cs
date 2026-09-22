namespace FDL.WF.App
{
    public sealed record WorkflowFiltre(
        int? IdUserAssigne = null,
        IReadOnlyList<int>? IdGroupesAssignes = null,
        bool? EstTermine = null);
}
