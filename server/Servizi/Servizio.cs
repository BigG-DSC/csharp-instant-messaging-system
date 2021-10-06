using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Servizi
{
  /* Classe astratta */
  abstract class Servizio
  {
    /* Metodo astratto per fornire un determinato servizio */
    abstract public void fornisciServizio(Ricettore oggetto, List<string> stringa);
  }
}
