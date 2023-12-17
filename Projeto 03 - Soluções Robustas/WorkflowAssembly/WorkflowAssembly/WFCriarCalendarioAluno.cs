using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace WorkflowAssembly
{
    public class WFCriarCalendarioAluno : CodeActivity
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

            Guid guidAlunoCurso = context.PrimaryEntityId;
            tracer.Trace($"Guid Aluno Curso: {guidAlunoCurso}");

            Entity entityAlunoCurso = service.Retrieve("ecs_cursosxalunos", context.PrimaryEntityId, new ColumnSet(true));
            string periodoCurso = String.Empty;
            DateTime dataInicio = new DateTime();

            Guid guidCurso = ((EntityReference)entityAlunoCurso.Attributes["ecs_curso"]).Id;
            tracer.Trace($"Guid Curso: {guidCurso}");

            if (entityAlunoCurso.Attributes.Contains("ecs_periodo"))
            {
                tracer.Trace($"Periodo: {((OptionSetValue)entityAlunoCurso.Attributes["ecs_periodo"]).Value}");
                if (((OptionSetValue)entityAlunoCurso.Attributes["ecs_periodo"]).Value == 12345000)
                {
                    periodoCurso = "Diurno";
                }
                else
                {
                    periodoCurso = "Noturno";
                }
                tracer.Trace($"Período {periodoCurso}");
            }

            if (entityAlunoCurso.Attributes.Contains("ecs_datadeinicio"))
            {
                DateTime data = (DateTime)entityAlunoCurso["ecs_datadeinicio"];
                dataInicio = new DateTime(data.Year, data.Month, data.Day);
                tracer.Trace($"Data de início: {dataInicio} {dataInicio:ddd}");

                if (guidCurso != Guid.Empty)
                {
                    Entity entityCurso = service.Retrieve("ecs_cursos", guidCurso, new ColumnSet("ecs_duracao"));
                    int duracao = 0;
                    if (entityCurso != null && entityCurso.Attributes.Contains("ecs_duracao"))
                    {
                        duracao = Convert.ToInt32(entityCurso.Attributes["ecs_duracao"]);
                    }
                    tracer.Trace($"Duração: {duracao}");

                    int diasNecessarios = duracao / 4;
                    tracer.Trace($"Dias necessários: {diasNecessarios}");

                    if (diasNecessarios > 0)
                    {
                        for (int i = 0; i < diasNecessarios; i++)
                        {
                            if (dataInicio.ToString("ddd") == "Sat" && periodoCurso == "Noturno")
                            {
                                dataInicio = dataInicio.AddDays(2);
                            }
                            if (dataInicio.ToString("ddd") == "Sun")
                            {
                                dataInicio = dataInicio.AddDays(1);
                            }

                            Entity entityCalendario = new Entity("ecs_calendarioaluno");
                            entityCalendario["ecs_name"] = $"Aula {i + 1}";
                            entityCalendario["ecs_datadaaula"] = dataInicio;
                            entityCalendario["ecs_cursoxaluno"] = new EntityReference("ecs_cursosxalunos", guidAlunoCurso);
                            service.Create(entityCalendario);

                            tracer.Trace($"Aula: {i + 1} Data: {dataInicio}");
                            dataInicio = dataInicio.AddDays(1);
                        }
                    }
                }
            }
        }
    }
}
