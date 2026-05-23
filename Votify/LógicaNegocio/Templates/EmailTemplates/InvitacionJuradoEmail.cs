namespace Votify.LógicaNegocio.Templates.EmailTemplates
{
    public class InvitacionJuradoEmail : InvitacionEmailTemplate
    {
        private readonly string _nombreCategoria;

        public InvitacionJuradoEmail(
            IConfiguration config,
            string nombreCategoria)
            : base(config)
        {
            _nombreCategoria = nombreCategoria;
        }

        protected override string ObtenerAsunto(string nombreEvento)
        {
            return $"Invitación como jurado: {nombreEvento}";
        }

        protected override string ObtenerBody(
            string nombreEvento,
            string nombreCreador,
            string urlAceptacion)
        {
            return $"""
        Hola,

        {nombreCreador} te ha invitado a participar como jurado
        en "{nombreEvento}" para la categoría "{_nombreCategoria}".

        Acepta tu invitación aquí:
        {urlAceptacion}

        El enlace caduca en 7 días.
        """;
        }
    }

}
