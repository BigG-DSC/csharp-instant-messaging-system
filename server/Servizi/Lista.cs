using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Servizi
{
  class Lista : Servizio
  {
    /* Servizio per recuperare la lista di amici */
    override public void fornisciServizio(Ricettore client, List<string> lista)
    {
      /* Istanza di convertitore */
      Convertitore converti = new Convertitore();
      /* Variabile predisposta per contenere la lista degli amici */
      List<string> listaAmici = new List<string>();

      /* Recupero della lista di amici nel DB */
      lock (Database.Istanza)
        listaAmici = Database.Istanza.listaAmici(lista);

      /* Se la lista non è vuota */
      if (listaAmici.Count != 0)
      {
        /* Invia la lista di amici */
        Pacchetto messaggio = new Pacchetto("ListaAmici", converti.listaAStringa(listaAmici));
        client.InviaPacchetto(messaggio);
      }
      else // altrimenti è vuota
      {
        /* Invia messaggio di lista vuota */
        Pacchetto messaggio = new Pacchetto("ListaAmici", "Non hai ancora alcun amico!");
        client.InviaPacchetto(messaggio);
      }
    }
  }
}
