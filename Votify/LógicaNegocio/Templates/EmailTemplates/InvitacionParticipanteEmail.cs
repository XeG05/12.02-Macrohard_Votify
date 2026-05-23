namespace Votify.LógicaNegocio.Templates.EmailTemplates
{
    public class InvitacionParticipanteEmail : InvitacionEmailTemplate
    {
        public InvitacionParticipanteEmail(IConfiguration config)
        : base(config)
        {
        }

        protected override string ObtenerAsunto(string nombreEvento)
        {
            return $"Invitación al evento: {nombreEvento}";
        }

        protected override string ObtenerBody(
            string nombreEvento,
            string nombreCreador,
            string urlAceptacion)
        {
            return $"""
        Hola,

        {nombreCreador} te ha invitado a participar en "{nombreEvento}" en Votify.

        Acepta tu invitación aquí:
        {urlAceptacion}

        El enlace caduca en 7 días.
        """;
        }
    }


}

