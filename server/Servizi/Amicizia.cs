using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Servizi
{
  class Amicizia : Servizio
  {
    /* Servizio per aggiungere un nuovo amico */
    override public void fornisciServizio(Ricettore client, List<string> lista)
    {
      /* Ricerca del ricettore a cui inviare l'amicizia *
        * e creazione della Pending */
      lock (Database.Istanza)
      {
        int esito;

        /* Se l'inserimento dell'amicizia va a buon fine */
        if((esito = Database.Istanza.inserisciAmicizia(lista)) != -1)
        {
          /* Avvisa il Client richiedente che e' andato tutto a buon fine */
          Pacchetto messaggio = new Pacchetto("AggiungiAmico", "OK");
          client.InviaPacchetto(messaggio);

          /* Controlla se il destinatario puo' essere avvisato Online */
          Ricettore destinatario = Connessione.Istanza.cercaInDizionario(lista[1]);

          /* Se e' Online */
          if (destinatario != null)
          {
            /* Se non e' un aggiornamento */
            if (esito == 1)
            {
              /* Inoltra la richiesta al destinatario */
              messaggio = new Pacchetto("InoltroAmicizia", "");
              destinatario.InviaPacchetto(messaggio);
            }
          }
        }
        else
        {
          /* Non e' stato possibile aggiungere l'amico*/
          Pacchetto messaggio = new Pacchetto("AggiungiAmico", "Non e' stato possibile aggiungere l'amico richiesto");
          client.InviaPacchetto(messaggio);
        }
      }
    }
  }
}
