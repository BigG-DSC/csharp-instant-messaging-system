using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Servizi
{
  class Registrazione : Servizio
  {
    /* Metodo richiamato al momento della Registrazione */
    override public void fornisciServizio(Ricettore client, List<string> lista)
    {
      bool risultato; // Risultato dell'inserimento dell'utente nel DataBase

      /* Query */
      string query = "INSERT INTO UTENTI (username, password) values ('" + lista[0] + "', '" + lista[1] + "')";

      /* Appropriazione dell'istanza del DataBase */
      lock (Database.Istanza)
        risultato = Database.Istanza.inserisci(query); // Inserimento e Risultato della query

      /* Se l'inserimento e' avvenuto con successo */
      if (risultato)
      {
        /* Messaggio positivo */
        Pacchetto messaggio = new Pacchetto("Registrazione", "OK");
        client.InviaPacchetto(messaggio);
      }
      else // altrimenti
      {
        /* Messaggio negativo */
        Pacchetto messaggio = new Pacchetto("Registrazione", "La Username immessa non e' disponibile");
        client.InviaPacchetto(messaggio);
      }
    }
  }
}
