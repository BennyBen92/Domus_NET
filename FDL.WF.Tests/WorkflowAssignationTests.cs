using FDL.Core.Domain;
using FDL.WF.Domain;
using Microsoft.Extensions.Time.Testing;

namespace FDL.WF.Tests
{
    public class WorkflowAssignationTests
    {
        private readonly DateTime dateTime = new(2026, 10, 20, 12, 0, 0);

        [Fact]
        public void WFAssignationUtilisateurInvalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Workflow.PourTache(
                ReferenceDossier.DepuisExistant(1500123, 1),
                new AuditInfo(1, dateTime),
                WorkflowAssignation.PourUtilisateur(0),
                "")
            );
        }

        [Fact]
        public void WFAssignationGroupeInvalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Workflow.PourTache(
                ReferenceDossier.DepuisExistant(1500123, 1),
                new AuditInfo(1, dateTime),
                WorkflowAssignation.PourGroupe(-1),
                "")
            );
        }
    }
}
