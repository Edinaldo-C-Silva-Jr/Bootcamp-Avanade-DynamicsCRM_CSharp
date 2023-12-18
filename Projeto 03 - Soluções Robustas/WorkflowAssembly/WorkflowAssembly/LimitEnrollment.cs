using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace WorkflowAssembly
{
    /// <summary>
    /// A Workflow assembly that checks how many courses the student is currently enrolled to.
    /// It prevents enrollment of the same student in 3 or more courses at the same time.
    /// </summary>
    public class LimitEnrollment : CodeActivity
    {
        // Defines the parameters used for the execution of the Workflow Assembly
        #region Parameters
        /// <summary>
        /// The user that executes the workflow.
        /// </summary>
        [Input("Usuario")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> InputUser { get; set; }

        /// <summary>
        /// The context of the record that is on screen when the workflow is activated.
        /// </summary>
        [Input("RegistroAlunoCurso")]
        [ReferenceTarget("ecs_curso_aluno")]
        public InArgument<EntityReference> RecordContext { get; set; }

        /// <summary>
        /// The output parameters sent back to the workflow from the assembly.
        /// </summary>
        [Output("Saida")]
        public OutArgument<string> Output { get; set; }
        #endregion

        /// <summary>
        /// Method Execute, which will be called from Dynamics 365 to run the Assembly.
        /// </summary>
        /// <param name="serviceProvider">The service provider that allows passing information to the plugin.</param>
        /// <exception cref="InvalidPluginExecutionException">In case the student tries to get enrolled into more than 2 courses at the same time.</exception>
        protected override void Execute(CodeActivityContext activityContext)
        {
            // Default implementation of a plugin for Dynamics 365, slightly altered for a Workflow Assembly.
            IWorkflowContext context = activityContext.GetExtension<IWorkflowContext>(); // The context for a Workflow, which replaces the context of a plugin on Workflow Assembly implementations.
            IOrganizationServiceFactory serviceFactory = activityContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracer = activityContext.GetExtension<ITracingService>();

            tracer.Trace($"Guid do Curso x Aluno: {context.PrimaryEntityId}");
            
            // Retrieves all the columns from the Curso x Aluno record received in the context, in order to get the ID of the Aluno (student) record.
            Entity entityAlunoCurso = service.Retrieve("ecs_curso_aluno", context.PrimaryEntityId, new ColumnSet(true));
            Guid GuidAluno = ((EntityReference)entityAlunoCurso.Attributes["ecs_aluno"]).Id;

            tracer.Trace($"Guid do Aluno: {GuidAluno}");

            // Fetches a list of all the courses the student is currently enrolled to.
            string fetchXML = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                    <entity name='ecs_curso_aluno'>
                                        <attribute name='ecs_curso_alunoid'/>
                                        <attribute name='ecs_curso'/>
                                        <attribute name='ecs_name'/>
                                        <attribute name='ecs_emcurso'/>
                                            <order attribute='ecs_name' descending='false'/>
                                        <filter type='and'>
                                            <condition attribute='ecs_aluno' operator='eq' uitype='ecs_alunos' value='{GuidAluno}'/>
                                            <condition attribute='ecs_emcurso' operator='eq' value='1'/>
                                        </filter>
                                    </entity>
                                </fetch>";

            EntityCollection listCursoAluno = service.RetrieveMultiple(new FetchExpression(fetchXML));
            
            // If student is already enrolled to 2 other courses, prevent a new enrollment from being made.
            if (listCursoAluno.Entities.Count > 2)
            {
                tracer.Trace("Aluno não pode se matricular em mais de 2 cursos");
                throw new InvalidPluginExecutionException("Aluno não pode se matricular em mais de 2 cursos");
            }
        }
    }
}
