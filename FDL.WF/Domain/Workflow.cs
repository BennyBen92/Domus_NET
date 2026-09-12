namespace FDL.WF.Domain
{
    public enum EWorkflowType
    {
        None = 0,
        Document = 1,
        Note = 2,
        Task = 3,
    }
    public enum EWorkflowAction
    {
        None = 0,
        ToSign = 1,
        ToValidate = 2,
        ToCheck = 3,
        ToCorrect = 4,
        ToRead = 5
    }

    public class Workflow
    {
        public int IdWorkflow { get; private set; }
        public int IdDocument { get; set; }
        public string Message { get; set; }
        public EWorkflowType Type{ get; set; }
        public EWorkflowAction Action { get; set; }
        public int IdUserAssigned { get; set; }
        public int IdGroupAssigned { get; set; }

        public int IdUserTermination { get; private set; }
        public DateTime DateTermination { get; private set; }

        public int IdUserCreation { get; private set; }
        public DateTime DateCreation { get; private set; }
        public int IdUserUpdate { get; private set; }
        public DateTime DateUpdate { get; private set; }

        public Workflow()
        {
            IdUserCreation = 0;
            DateCreation = DateTime.Now;
            IdUserUpdate = 0;
            DateUpdate = DateTime.Now;

            Message = string.Empty;
            Type = EWorkflowType.None;
            Action = EWorkflowAction.None;
            IdUserAssigned = 0;
            IdGroupAssigned = 0;
            
            IdUserTermination = 0;
            DateTermination = DateTime.MinValue;

        }


    }
}
