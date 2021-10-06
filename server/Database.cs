using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
  class Database
  {
    /* VARIABILI */
    private static Database istanza; // Istanza Singleton Database
    private SQLiteConnection connessione; // Istanza della Connessione
    private int idMessaggi; // ID utilizzato per i record dei messaggi
    private int idAmicizie; // ID utilizzato per i record delle amicizie

    /* COSTRUTTORE */
    private Database()
    {
      /* Creazione DB se non esistente */
      if (!File.Exists("database.sqlite"))
      {
        try
        {
          SQLiteConnection.CreateFile("database.sqlite");
        }
        catch (Exception)
        {
          Interfaccia.stampaMessaggio(" <!> Impossibile creare il Database <!>\n");
          Interfaccia.stampaMessaggio(" <!> L'applicazione ha i permessi per farlo? <!>\n");
          Interfaccia.stampaMessaggio(" -> Chiusura automatica in 10 secondi");
          Thread.Sleep(10000);
          Environment.Exit(0);
        }
        /* Variabile per stabilire la connessione */
        connessione = new SQLiteConnection("Data Source = database.sqlite; Version=3;");
        /* Apertura connessione */
        connessione.Open();
        /* Generazione delle tabelle del DB */
        generaTabella("CREATE TABLE UTENTI (username VARCHAR(24) PRIMARY KEY, password VARCHAR(24))");
        generaTabella("CREATE TABLE MESSAGGI (id INT PRIMARY KEY, mittente VARCHAR(24), messaggio VARCHAR(250))");
        generaTabella("CREATE TABLE AMICIZIE (id INT PRIMARY KEY, username1 VARCHAR(24), username2 VARCHAR(24), confermaU1 INT, confermaU2 INT)");
        generaTabella("CREATE TABLE PENDING (username VARCHAR(24), idMessaggio INT, idAmicizia INT, FOREIGN KEY (idMessaggio) REFERENCES MESSAGGI(id), FOREIGN KEY (idAmicizia) REFERENCES AMICIZIE(id) )");
        /* Inserimento dell'User di Default *
          * USERNAME : Amministratore        *
          * PASSWORD : Password              */
        inserisci("INSERT INTO UTENTI (username, password) VALUES ('Amministratore','Password')");
      }

      /* Variabile per stabilire la connessione */
      connessione = new SQLiteConnection("Data Source = database.sqlite; Version=3;");
      /* Apertura connessione */
      connessione.Open();

      /* Impostazione del valore iniziale di idAmicizia */
      SQLiteDataReader reader = cerca("SELECT MAX(id) AS max FROM AMICIZIE");
      reader.Read();
      if (!DBNull.Value.Equals(reader["max"]))
        idAmicizie = Convert.ToInt32(reader["max"]);
      else
        idAmicizie = 0;

      /* Impostazione del valore iniziale di idMessaggi */
      reader = cerca("SELECT MAX(id) AS max FROM MESSAGGI");
      reader.Read();
      if (!DBNull.Value.Equals(reader["max"]))
        idMessaggi = Convert.ToInt32(reader["max"]);
      else
        idMessaggi = 0;
    }

    /* METODI */
    /* Metodo per creazione dell'istanza Singleton*/
    public static Database Istanza
    {
      get
      {
        if (istanza == null)
        {
          istanza = new Database();
        }
          return istanza;
      }
    }

    /* Crea la tabella Utente - | ID | Username | Password | */
    private void generaTabella(string query)
    {
      /* Ordine da interpretare */
      SQLiteCommand comando = new SQLiteCommand(query, connessione);
      /* Esecuzione dell'ordine */
      comando.ExecuteNonQuery();
    }

    /* Inserisce i dati nel DB sulla base di una query */
    public bool inserisci(string query)
    {
      try
      {
        /* Prova l'inserimento, in caso di errore ritorna false (eccezione) */
        SQLiteCommand comando = new SQLiteCommand(query, connessione);
        comando.ExecuteNonQuery();
      }
      catch (Exception)
      {
        return false;
      }

      return true;
    }

    /* Cerca i dati nel DB sulla base di una query */
    public SQLiteDataReader cerca(string query)
    {
      /* Esegui la ricerca */
      SQLiteCommand comando = new SQLiteCommand(query, connessione);
      SQLiteDataReader reader = comando.ExecuteReader();

      /* Ritorna il risultato (vuoto in caso di nessuna corrispondenza) */
      return reader;
    }

    /* Aggiorna i dati nel DB sulla base di una query */
    public int aggiorna(string query)
    {
      int righeAggiornate;

      try
      {
        /* Prova l'aggiornamento, in caso di eccezione segnala 0 righe aggiornate */
        SQLiteCommand comando = new SQLiteCommand(query, connessione);
        righeAggiornate = comando.ExecuteNonQuery();
      }
      catch (Exception)
      {
        righeAggiornate = 0;
      }

      /* Ritorna le righe aggiornate */
      return righeAggiornate;
    }

    /* Rimuovi dati da DB sulla base di una query */
    public bool rimuovi(string query)
    {
      try
      {
        /* Prova la rimozione, in caso di errore ritorna falso */ 
        SQLiteCommand comando = new SQLiteCommand(query, connessione);
        comando.ExecuteNonQuery();
      }
      catch (Exception)
      {
        return false;
      }

      return true;
    }

    /* Inserimento agile di una Pending Amicizia nel Database */
    public int inserisciAmicizia(List<string> dati)
    {
      /* Esito : -1 => Fallito l'inserimento *
       * Esito :  0 => Amicizia aggiornata   *
       * Esito :  1 => Amicizia inserita     */
      int esito;

      /* Controllo se l'amico da voler aggiungere esiste */
      string query = "SELECT * FROM UTENTI WHERE username = '" + dati[1] + "'";
      SQLiteDataReader reader = cerca(query);
            
      /* Se esiste */
      if(reader.Read())
      {
        /* Controllo che esista gia' l'amicizia */
        query = "SELECT * FROM AMICIZIE WHERE username1 = '" + dati[0] + "' AND username2 = '" + dati[1] + "'";                   
        reader = cerca(query);

        /* Se esiste */
        if (reader.Read())
        {
          /* Se risulta che l'amicizia non sia gia' instaurata, la instauro */
          if (reader["confermaU1"].ToString() != "1")
          {
            query = "UPDATE AMICIZIE SET confermaU1 = 1 WHERE id = '" + reader["id"].ToString() + "'";
            aggiorna(query);
            esito = 0;
          }
          else
            esito = -1;
        }
        else //altrimenti, non esiste, ma va verificato il reciproco
        {
          /* Controllo che esista gia' l'amicizia a ruoli invertiti */
          query = "SELECT * FROM AMICIZIE WHERE username1 = '" + dati[1] + "' AND username2 = '" + dati[0] + "'";
          reader = cerca(query);

          /* Se l'amicizia esiste */
          if (reader.Read())
          {
            /* Se risulta che l'amicizia non sia gia' instaurata, la instauro */
            if (reader["confermaU2"].ToString() != "1")
            {
              query = "UPDATE AMICIZIE SET confermaU2 = 1 WHERE id = '" + reader["id"].ToString() + "'";
              aggiorna(query);
              esito = 0;
            }
            else
              esito = -1;
          }
          else // altrimenti, non esiste, e' possibile aggiungerla
          {
            /* Aggiorno l'id corrente */
            idAmicizie++;
            
            /* Tentativo di inserimento */
            query = "INSERT INTO AMICIZIE (id, username1, username2, confermaU1, confermaU2) VALUES ('" + idAmicizie + "','" + dati[0] + "', '" + dati[1] + "', 1, 2)";
            bool inserito = inserisci(query);

            /* Se l'inserimento è andato a buon fine */
            if (inserito)
            {
              /* Tenta l'inserimento della pending */
              query = "INSERT INTO PENDING (username, idMessaggio, idAmicizia) VALUES ('" + dati[1] + "', null, '" + idAmicizie + "')";
              inserito = inserisci(query);

              /* Se l'inserimento va a buon fine */
              if (inserito)
                esito = 1;
              else // altrimenti
              {
                /* Rimuovi l'amicizia inserita in precedenza */
                query = "DELETE FROM AMICIZIE WHERE id = '" + idAmicizie + "'";
                rimuovi(query);

                /* Riporta l'idAmicizie allo stato precedente */
                if (idAmicizie > 0)
                    idAmicizie--;
                esito = -1;
              }
            }
            else // altrimenti non è andato a buon fine 
            {
              /* Riporta idAmicizie allo stato precedente */
              if (idAmicizie > 0)
                  idAmicizie--;
              esito = -1;
            }
          }
        }
      }
      else
          esito = -1;

      return esito;
    }

    /* Inserimento agile di una Pending Messaggio nel Database */
    public int inserisciMessaggio(List<string> dati)
    {
      /* Esito : -1 => Fallito l'inserimento *
       * Esito :  1 => Messaggio Inserito    */
      int esito;

      /* Controllo se l'amico a cui voler inviare il messaggio esiste */
      string query = "SELECT * FROM UTENTI WHERE username = '" + dati[1] + "'";
      SQLiteDataReader reader = cerca(query);

      /* Se esiste */
      if (reader.Read())
      {
        /* Aggiornamento variabile idMessaggi */
        idMessaggi++;

        query = "INSERT INTO MESSAGGI  (id, mittente, messaggio) VALUES ('" + idMessaggi + "','" + dati[0] + "', '" + dati[2] + "')";

        /* Se l'inserimento del messaggio va a buon fine */
        if (inserisci(query))
        {
          query = "INSERT INTO PENDING (username, idMessaggio, idAmicizia) VALUES ('" + dati[1] + "','" + idMessaggi + "', null)";

          /* Prova a inserire anche la pending */
          if (inserisci(query))
              esito = 1;
          else // a;trimenti si è verificato un errore
          {
            /* Rimuovi il messaggio precedentemente inserito */
            query = "DELETE FROM MESSAGGI WHERE id = '" + idMessaggi + "'";
            rimuovi(query);

            /* Riporta l'idMessaggi allo stato precedente */
            if (idMessaggi > 0)
                idMessaggi--;
            esito = -1;
          }
        }
        else // altrimenti, si è verificato un errore
        {
          /* Quindi porta idMessaggi allo stato precedente */
          if (idMessaggi > 0)
              idMessaggi--;
          esito = -1;
        }
      }
      else // altrimenti
        esito = -1;

      /* Ritorna l'esito del metodo */
      return esito;
    }

    /* Metodo per la restituzione della lista di amici di un determinato utente */
    public List<string> listaAmici(List<string> datiUtente)
    {
      /* Lista nella quale saranno posizionati gli amici */
      List<string> lista = new List<string>();

      /* Restituisci le amicizie dove username1 corrisponde allo username del richiedente */
      string query = "SELECT username2 FROM AMICIZIE WHERE username1 = '" + datiUtente[0] + "' AND confermaU1 = 1 AND confermaU2 = 1";
      SQLiteDataReader reader = cerca(query);

      /* Se c'è corrispondenza, inserisci gli elementi trovati nella lista */
      while (reader.Read())
          lista.Add(reader["username2"].ToString());

      /* Restituisci le amicizie dove username2 corrisponde allo username del richiedente */
      query = "SELECT username1 FROM AMICIZIE WHERE username2 = '" + datiUtente[0] + "' AND confermaU1 = 1 AND confermaU2 = 1";
      reader = cerca(query);

      /* Se c'è corrispondenza, inserisci gli elementi trovati nella lista */
      while (reader.Read())
          lista.Add(reader["username1"].ToString());

      /* Ritorna la lista */
      return lista;
    }

    /* Metodo per il prelievo di una amicizia dal DB tramite id */
    public string prelevaAmicizia(string id, string username)
    {
      /* Ricerca l'amicizia */
      string query = "SELECT username1, username2 FROM AMICIZIE WHERE id = '" + id + "'";
      SQLiteDataReader reader = cerca(query);

      /* Leggi gli utenti coinvolti */
      string utente1 = reader["username1"].ToString();
      string utente2 = reader["username2"].ToString();

      /* Restituisci l'utente non richidente */
      if (username == utente1)
          return utente2;

      if (username == utente2)
          return utente1;

      /* Se non c'è alcuna corrispondenza con il richiedente, null */
      return null;
    }
  }
}
