namespace FDL.WF.Domain
{
    public sealed record WorkflowTerminaison
    {
        public int IdUser { get; }
        public DateTime Date { get; }

        public WorkflowTerminaison(int idUser, DateTime date)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idUser, nameof(idUser));
            if (date == default)
                throw new ArgumentException("La date de terminaison est obligatoire.", nameof(date));
            IdUser = idUser;
            Date = date;
        }
    }
}
