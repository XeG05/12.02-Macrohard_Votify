using Votify.Modelos.BusinessLogic.Interfaces;
using Votify.Modelos.Persistence.Interfaces;
using Votify.Modelos.BusinessLogic.Domain;
using Votify.Modelos.BusinessLogic.Factory;

namespace Votify.Modelos.BusinessLogic.Services
{
    public class Evento_Service : Service<Evento>, I_Evento_Service
    {
        private readonly IRepository<Evento> _evento_repo;
        private readonly IRepository<Usuario> _usuario_repo;
        private readonly IRepository<Proyecto> _proyectoRepo;
        private readonly IRepository<Participante> _participanteRepo;
        private readonly IRepository<Grupo> _grupoRepo;
        private readonly IRepository<Categorium> _categoriumRepo;
        public Evento_Service(IRepository<Evento> eventoRepo, IRepository<Usuario> usuarioRepo, IRepository<Participante> participanteRepo, IRepository<Proyecto> proyectoRepo, IRepository<Grupo> grupoRepo, IRepository<Categorium> categoriumRepo) : base(eventoRepo)
        {
            _evento_repo = eventoRepo;
            _usuario_repo = usuarioRepo;
            _participanteRepo = participanteRepo;
            _proyectoRepo = proyectoRepo;
            _categoriumRepo = categoriumRepo;
            _grupoRepo = grupoRepo;
        }

        public async Task<EventoBase> EntrarEvento(string correo, int eventoId)
        {
            var usuario = (await _usuario_repo.FindAsync(u => u.Correo == correo)).FirstOrDefault();

            if (usuario == null) { throw new Exception("usuario no encontrado"); }

            var evento = await _evento_repo.GetByIdAsync(eventoId);

            if (evento == null) { throw new Exception("Evento no encontrado"); }

            //DETECTAR ROL, DE MOMENTO APAÑADO HASTA TENERLO BIEN
            string rol = "ADMIN";

            //FACTORY
            EventoCreator creator;

            switch(rol)
            {
                case "ADMIN":
                    creator = new EventoAdministradorCreator(_evento_repo, _proyectoRepo, _participanteRepo, _grupoRepo, _categoriumRepo);
                    break;

                default:
                    throw new Exception("rol no válido");
            }

            //CREAR OBJETO DE DOMINIO
            return creator.CrearEvento(evento, usuario);
        }
    }
}
