using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace WorkflowAssembly
{
    /// <summary>
    /// A workflow assembly that creates records of the entity "CalendarioAluno" whenever a student gets enrolled in a course.
    /// </summary>
    public class CreateStudentCalendar : CodeActivity
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
        protected override void Execute(CodeActivityContext activityContext)
        {
            // Default implementation of a plugin for Dynamics 365, slightly altered for a Workflow Assembly.
            IWorkflowContext context = activityContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = activityContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracer = activityContext.GetExtension<ITracingService>();

            tracer.Trace($"Guid Aluno Curso: {context.PrimaryEntityId}");

            // Retrieves all the columns from the Curso x Aluno record received in the context, in order to get the ID of the Aluno (student) record.
            Entity entityAlunoCurso = service.Retrieve("ecs_curso_aluno", context.PrimaryEntityId, new ColumnSet(true));
            string periodoCurso;

            // Gets the Guid of the course the student is getting enrolled to. Only proceed if there is actually a course.
            Guid guidCurso = ((EntityReference)entityAlunoCurso.Attributes["ecs_curso"]).Id;
            tracer.Trace($"Guid Curso: {guidCurso}");
            if (guidCurso == Guid.Empty)
            {
                return;
            }

            // Sets the text of the "periodo" field into a variable
            if (((OptionSetValue)entityAlunoCurso.Attributes["ecs_periodo"]).Value == 123450000)
            {
                periodoCurso = "Diurno";
            }
            else
            {
                periodoCurso = "Noturno";
            }
            tracer.Trace($"Período {periodoCurso}");

            // Checks if the field "datainicio" is filled
            if (entityAlunoCurso.Attributes.Contains("ecs_datainicio"))
            {
                // Gets the start date of the course
                DateTime temporaryDate = (DateTime)entityAlunoCurso["ecs_datainicio"];
                DateTime classDate = new DateTime(temporaryDate.Year, temporaryDate.Month, temporaryDate.Day);
                tracer.Trace($"Data de início: {classDate} {classDate:ddd}");

                // Retrieves the "duracao" field from the course the student is getting enrolled to.
                Entity entityCurso = service.Retrieve("ecs_cursos", guidCurso, new ColumnSet("ecs_duracaohoras"));
                int duracao = Convert.ToInt32(entityCurso.Attributes["ecs_duracaohoras"]);
                tracer.Trace($"Duração: {duracao}");

                // Calculates the amount of days needed to complete the course. This calculation considers every day will have 4 hours worth of classes.
                int diasNecessarios = duracao / 4;
                tracer.Trace($"Dias necessários: {diasNecessarios}");

                if (diasNecessarios > 0)
                {
                    // Iterates through every necessary day to create calendar entries for each of them.
                    for (int i = 0; i < diasNecessarios; i++)
                    {
                        // Skips Sunday from the calendar, since no classes happen on Sunday.
                        if (classDate.ToString("ddd") == "Sun")
                        {
                            classDate = classDate.AddDays(1);
                        }
                        // If the classes happen at night, skips Saturday too, as Saturday only has morning classes.
                        if (classDate.ToString("ddd") == "Sat" && periodoCurso == "Noturno")
                        {
                            classDate = classDate.AddDays(2);
                        }

                        // Creates an entry of the calendar.
                        Entity entityCalendario = new Entity("ecs_calendarioaluno");
                        entityCalendario["ecs_name"] = $"Aula {i + 1}";
                        entityCalendario["ecs_dataaula"] = classDate;
                        entityCalendario["ecs_curso_aluno"] = new EntityReference("ecs_curso_aluno", context.PrimaryEntityId);
                        service.Create(entityCalendario);

                        tracer.Trace($"Aula: {i + 1} Data: {classDate}");
                        classDate = classDate.AddDays(1); // Increments the date by one to continue building the calendar.
                    }
                }
            }
        }
    }
}
