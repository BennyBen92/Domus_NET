namespace FDL.Core.Domain
{
    public sealed record AuditInfo
    {
        public int IdUser { get; }
        public DateTime Date { get; }

        public AuditInfo(int idUser, DateTime date)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(idUser, nameof(idUser));
            if (date == default)
                throw new ArgumentException("La date est obligatoire.", nameof(date));
            IdUser = idUser;
            Date = date;
        }
    }
}
