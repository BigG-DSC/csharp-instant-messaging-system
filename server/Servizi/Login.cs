using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Servizi
{
  class Login : Servizio
  {
    /* Metodo richiamato al momento del Login */
    override public void fornisciServizio(Ricettore client, List<string> lista)
    {
      bool esito = false; // Booleano per verifica query
      SQLiteDataReader risultato; // Contenitore risultato Select
      /* Query - lista[0] contiene la username del richiedente - lista[1] contiene la password del richiedente */
      string query = "SELECT * FROM UTENTI WHERE username = '" + lista[0] + "' AND password = '" + lista[1] + "'";

      /* Richiesta di Appropriazione dell'istanza della classe Database */
      lock (Database.Istanza) 
          risultato = Database.Istanza.cerca(query); // Esecuzione e risultato della Query

      /* Verifica che la query abbia dato risultati */
      while (risultato.Read())
          esito = true;

      /* Se la query ha dato risultati */
      if (esito)
      {
        /* Aggiorno il dizionario della connessione */
        esito = Connessione.Istanza.utenteOnline(client, lista[0]);

        if (esito)
        {
          /* Avviso l'utente che il Login e' confermato */
          Pacchetto messaggio = new Pacchetto("Login", "OK");
          client.InviaPacchetto(messaggio);
        }
        else
        {
          /* Invia pacchetto contenente il messaggio di errore al Client */
          Pacchetto messaggio = new Pacchetto("Login", "Il tuo username e' gia' loggato in un altra posizione");
          client.InviaPacchetto(messaggio);
        }
      }
      else // altrimenti, in tutti gli altri casi
      {
        /* Invia pacchetto contenente il messaggio di errore al Client */
        Pacchetto messaggio = new Pacchetto("Login", "I dati inseriti non sono corretti oppure non sei ancora registrato");
        client.InviaPacchetto(messaggio);
      }
    }
  }
}
