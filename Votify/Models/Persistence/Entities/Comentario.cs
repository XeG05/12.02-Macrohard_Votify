using System;
using System.Collections.Generic;

namespace Votify.Models;

public partial class Comentario
{
    public long Id { get; set; }
    public long Id_proyecto { get; set; }
    public string CorreoRol { get; set; }
    public string Contenido { get; set; }
}
