using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Servizi
{
  class Segreteria : Servizio
  {
    /* Servizio di Segreteria */
    public override void fornisciServizio(Ricettore client, List<string> lista)
    {
      SQLiteDataReader reader; // Variabile per la lettura di query
      List<string> pending = new List<string>(); // Lista per rendere leggibile il risultato della query

      /* Query - lista[0] contiene la username del richiedente del servizio */
      string query = "SELECT username, idMessaggio, idAmicizia FROM PENDING WHERE username = '" + lista[0] + "'";

      /* Ricerca della pending */
      lock (Database.Istanza)
          reader = Database.Istanza.cerca(query);

      /* Se presente la pending, memorizza i dati relativi nella lista */
      if (reader.Read())
      {
        pending.Add(reader["username"].ToString());
        pending.Add(reader["idMessaggio"].ToString());
        pending.Add(reader["idAmicizia"].ToString());
      }

      /* Se la lista non e' vuota */
      if(pending.Count != 0)
      {
        /* Amicizia in Pending (perchè idmessaggio è vuoto) */
        if ((pending[1].Length == 0) && pending[2].Length != 0)
        {
          string richiedente;

          /* Prelievo dell'username che ha chiesto l'amicizia - pending[2] contiene l'id dell'amicizia */
          lock (Database.Istanza)
              richiedente = Database.Istanza.prelevaAmicizia(pending[2],lista[0]);

          /* Invio della pending al client */
          Pacchetto messaggio = new Pacchetto("Segreteria", richiedente + " vuole essere tuo amico");
          client.InviaPacchetto(messaggio);

          /* Rimozione della pending */
          Database.Istanza.rimuovi("DELETE FROM PENDING WHERE idAmicizia = '" + pending[2] + "'");
        }

        /* Messaggio in Pending - (perchè idamicizia è vuoto) */
        if (pending[1].Length != 0 && pending[2].Length == 0)
        {
          List<string> listaMessaggio = new List<string>();
          /* Query prelievo messaggio - pending[1] contiene l'id del messaggio */
          query = "SELECT * FROM MESSAGGI WHERE id = '" + pending[1] + "'";

          /* Prelievo del messaggio */
          lock (Database.Istanza)
              reader = Database.Istanza.cerca(query);

          /* Inserimento dei dati relativi in una lista */
          if (reader.Read())
          {
            listaMessaggio.Add(reader["mittente"].ToString());
            listaMessaggio.Add(reader["messaggio"].ToString());
            listaMessaggio.Add(lista[0]);
          }

          /* Inoltra il messaggio al destinatario */
          Pacchetto messaggio = new Pacchetto("Segreteria", "[" + listaMessaggio[0] + "] " + listaMessaggio[1]);
          client.InviaPacchetto(messaggio);

          /* Rimozione della pending */
          Database.Istanza.rimuovi("DELETE FROM PENDING WHERE idMessaggio = '" + pending[1] + "'");
        }
      }
      else // altrimenti e' vuota
      {
        /* Invio messaggio segreteria vuota al client */
        Pacchetto messaggio = new Pacchetto("Segreteria", lista[0] + ", la tua segreteria è vuota");
        client.InviaPacchetto(messaggio);
      }
    }
  }
}
