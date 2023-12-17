using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace WorkflowAssembly
{
    public class WFLimiteInscricao : CodeActivity
    {
        #region Parameters
        [Input("Usuario")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> InputUser { get; set; }

        [Input("RegistroAlunoCurso")]
        [ReferenceTarget("ecs_cursosxalunos")]
        public InArgument<EntityReference> RecordContext { get; set; }

        [Output("Saida")]
        public OutArgument<string> Output { get; set; }
        #endregion

        protected override void Execute(CodeActivityContext activityContext)
        {
            IWorkflowContext context = activityContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = activityContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracer = activityContext.GetExtension<ITracingService>();

            tracer.Trace($"Guid Curso x Aluno: {context.PrimaryEntityId}");
            
            Entity entityAlunoCurso = service.Retrieve("ecs_cursosxalunos", context.PrimaryEntityId, new ColumnSet(true));
            Guid GuidAluno = ((EntityReference)entityAlunoCurso.Attributes["ecs_aluno"]).Id;

            tracer.Trace($"Nome do Curso: {entityAlunoCurso.Attributes["ecs_curso"]}");
            tracer.Trace($"Entidade Aluno: {GuidAluno}");

            string fetchXML = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                    <entity name='ecs_cursosxalunos'>
                                        <attribute name='ecs_cursosxalunosid'/>
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

            EntityCollection listCursosAluno = service.RetrieveMultiple(new FetchExpression(fetchXML));
            
            if (listCursosAluno.Entities.Count > 2)
            {
                Output.Set(activityContext, "Aluno não pode se matricular em mais de 2 cursos");
                tracer.Trace("Aluno não pode se matricular em mais de 2 cursos");
                throw new InvalidPluginExecutionException("Aluno não pode se matricular em mais de 2 cursos");
            }
        }
    }
}
