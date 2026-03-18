using Votify.Modelos.Persistence.Interfaces;

namespace Votify.Modelos.BusinessLogic.Domain
{
    public class EventoAdministrador : EventoBase
    {
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Evento> _eventoRepo;
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Categorium> _categoriumRepo;
        public EventoAdministrador(Evento evento, Usuario usuario, IRepository<Evento> eventoRepo, IRepository<Proyecto> proyectoRepo, IRepository<Participante> participanteRepo, IRepository<Categorium> categoriumRepo, IRepository<Grupo> grupoRepo) : base(evento, usuario)
        {
            _proyectoRepo = proyectoRepo;
            _eventoRepo = eventoRepo;
            _participanteRepo = participanteRepo;
            _grupoRepo = grupoRepo;
            _categoriumRepo = categoriumRepo;
        }

        public async Task CambiarNombre(string nuevoNombre)
        {
            evento.Nombre = nuevoNombre;
            await _eventoRepo.UpdateAsync(evento);
        }

        public async Task CambiarDescripcion(string nuevaDescripcion)
        {
            evento.Descripcion = nuevaDescripcion;
            await _eventoRepo.UpdateAsync(evento);
        }

        public async Task<List<Categorium>> VerCategoria()
        {
            return await _categoriumRepo.FindAsync(c => c.IdEvento == evento.Id);
        } 


    }
}
