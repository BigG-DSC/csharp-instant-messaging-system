using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Servizi
{
  class Messaggistica : Servizio
  {
    override public void fornisciServizio(Ricettore client, List<string> lista)
    {
      /* Lista utilizzata per memorizzare la lista di amici del mittente */
      List<string> listaAmici = new List<string>();

      /* Appropriazione istanza del Database */
      lock (Database.Istanza)
        /* Estraggo la lista di Amici associata al Client */
        listaAmici = Database.Istanza.listaAmici(lista);

      /* Se nella lista degli Amici e' presente l'amico a cui si vuole inviare il messaggio */
      if (listaAmici.Contains(lista[1]))
      {                
        /* Confermo l'invio del messaggio al mittente */
        Pacchetto messaggio = new Pacchetto("Messaggio", "OK");
        client.InviaPacchetto(messaggio);

        /* Variabile per la memorizzazione del client destinatario a cui inviare il messaggio */
        Ricettore destinatario;

        /* Ricerca se il Destinatario e' Online */
        lock (Connessione.Istanza)
          destinatario = Connessione.Istanza.cercaInDizionario(lista[1]);

        /* Se il destinatario e' online */
        if (destinatario != null)
        {
          /* Inoltra il messaggio al destinatario immediatamente */
          messaggio = new Pacchetto("InoltroMsg", lista[0] + "," + lista[2] + "," + lista[1]);
          destinatario.InviaPacchetto(messaggio);
        }
        else // altrimenti
        {
          /* Inserisci il messaggio nella tabella Pending del DB */
          lock (Database.Istanza)
            Database.Istanza.inserisciMessaggio(lista);
        }
      }
      else // Altrimenti, non e' fra gli amici
      {
        /* Invio l'errore relativo al Client */
        Pacchetto messaggio = new Pacchetto("Messaggio", "Impossibile inviare il messaggio. L'utente non e' tuo amico");
        client.InviaPacchetto(messaggio);
      }
    }
  }
}
