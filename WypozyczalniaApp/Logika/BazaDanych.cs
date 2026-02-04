using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using WypozyczalniaApp.Interfejsy;
using WypozyczalniaApp.Modele;

namespace WypozyczalniaApp.Logika
{
    // Klasa odpowiedzialna za warstwę dostępu do danych (Data Access Layer).
    // Implementuje logikę połączenia z bazą SQL Server oraz operacje CRUD.
    public class BazaDanych : IZarzadzanieBaza
    {
        // Ciąg połączeniowy (Connection String) do lokalnej instancji SQL Server Express.
        // TrustServerCertificate=True pozwala na pracę w środowisku developerskim bez certyfikatu SSL.
        private string polaczenie = @"Server=DESKTOP-5S1274Q\SQLEXPRESS;Database=WypozyczalniaSport;Trusted_Connection=True;TrustServerCertificate=True;";

        // ================= SEKCJA: KLIENCI =================

        // Pobiera pełną listę klientów z bazy danych.
        // Wyniki zapytania są mapowane na listę obiektów klasy Klient.
        public List<Klient> PobierzWszystkichKlientow()
        {
            var lista = new List<Klient>();
            try
            {
                // Użycie bloku 'using' gwarantuje poprawne zamknięcie połączenia
                // i zwolnienie zasobów (Disposable) niezależnie od wyniku operacji.
                using (var conn = new SqlConnection(polaczenie))
                {
                    conn.Open();
                    string sql = "SELECT * FROM Klienci";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        var r = cmd.ExecuteReader();

                        // Iteracja po wierszach zwróconych przez bazę i tworzenie obiektów.
                        while (r.Read())
                        {
                            lista.Add(new Klient
                            {
                                Id = (int)r["id_klienta"],
                                Imie = r["imie"].ToString(),
                                Nazwisko = r["nazwisko"].ToString(),
                                Email = r["email"].ToString(),
                                Telefon = r["telefon"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd pobierania klientów: " + ex.Message);
            }
            return lista;
        }

        // Dodaje nowy rekord klienta do bazy.
        // Wykorzystuje parametryzację zapytania w celu ochrony przed atakami SQL Injection.
        public void DodajKlienta(Klient k)
        {
            try
            {
                using (var conn = new SqlConnection(polaczenie))
                {
                    conn.Open();
                    string sql = "INSERT INTO Klienci (imie, nazwisko, email, telefon) VALUES (@imie, @nazw, @email, @tel)";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        // Przypisanie wartości do parametrów zapytania.
                        cmd.Parameters.AddWithValue("@imie", k.Imie);
                        cmd.Parameters.AddWithValue("@nazw", k.Nazwisko);
                        cmd.Parameters.AddWithValue("@email", k.Email);
                        cmd.Parameters.AddWithValue("@tel", k.Telefon);

                        // ExecuteNonQuery używamy do operacji, które nie zwracają danych (INSERT, UPDATE, DELETE).
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Błąd dodawania: " + ex.Message); }
        }

        // Aktualizuje dane istniejącego klienta na podstawie jego ID.
        public void EdytujKlienta(Klient k)
        {
            try
            {
                using (var conn = new SqlConnection(polaczenie))
                {
                    conn.Open();
                    string sql = "UPDATE Klienci SET imie=@imie, nazwisko=@nazw, email=@email, telefon=@tel WHERE id_klienta=@id";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", k.Id);
                        cmd.Parameters.AddWithValue("@imie", k.Imie);
                        cmd.Parameters.AddWithValue("@nazw", k.Nazwisko);
                        cmd.Parameters.AddWithValue("@email", k.Email);
                        cmd.Parameters.AddWithValue("@tel", k.Telefon);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Błąd edycji: " + ex.Message); }
        }

        // Usuwa klienta z bazy danych.
        // Zawiera obsługę wyjątków naruszenia więzów integralności (np. gdy klient ma aktywne wypożyczenia).
        public bool UsunKlienta(int id)
        {
            try
            {
                using (var conn = new SqlConnection(polaczenie))
                {
                    conn.Open();
                    // Pamiętaj, aby nazwa tabeli i kolumny (id_klienta) pasowała do Twojej bazy!
                    string sql = "DELETE FROM Klienci WHERE id_klienta=@id";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0; // Zwraca TRUE tylko jak usunięto
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547) // Błąd relacji (klient ma wypożyczenia)
                {
                    Console.WriteLine("\n>> BŁĄD: Nie można usunąć klienta, bo ma historię wypożyczeń.");
                }
                else Console.WriteLine("Błąd SQL: " + ex.Message);

                return false; // Zwraca FALSE
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd: " + ex.Message);
                return false;
            }
        }
        // ================= SEKCJA: SPRZĘT =================

        // Pobiera pełną listę sprzętu dostępnego w systemie.
        public List<Sprzet> PobierzWszystkieSprzety()
        {
            var lista = new List<Sprzet>();
            try
            {
                using (var conn = new SqlConnection(polaczenie))
                {
                    conn.Open();
                    string sql = "SELECT * FROM Sprzet";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        var r = cmd.ExecuteReader();
                        while (r.Read())
                        {
                            lista.Add(new Sprzet
                            {
                                Id = (int)r["id_sprzetu"],
                                Nazwa = r["nazwa"].ToString(),
                                CenaZaDobe = (decimal)r["cena_za_dobe"],
                                Stan = r["stan_techniczny"].ToString(),
                                IdKategorii = (int)r["id_kategorii"]
                            });
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Błąd sprzetu: " + ex.Message); }
            return lista;
        }

        // Pobiera sprzęt przefiltrowany pod kątem dostępności do wypożyczenia.
        // Wyklucza sprzęt uszkodzony lub w serwisie.
        public List<Sprzet> PobierzDostepnySprzet()
        {
            var lista = new List<Sprzet>();
            try
            {
                using (var conn = new SqlConnection(polaczenie))
                {
                    conn.Open();
                    string sql = "SELECT * FROM Sprzet WHERE stan_techniczny = 'Sprawny' OR stan_techniczny = 'Nowy' OR stan_techniczny = 'Idealny'";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        var r = cmd.ExecuteReader();
                        while (r.Read())
                        {
                            lista.Add(new Sprzet
                            {
                                Id = (int)r["id_sprzetu"],
                                Nazwa = r["nazwa"].ToString(),
                                CenaZaDobe = (decimal)r["cena_za_dobe"],
                                Stan = r["stan_techniczny"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Błąd sprzetu: " + ex.Message); }
            return lista;
        }

        public void DodajSprzet(Sprzet s)
        {
            try
            {
                using (var conn = new SqlConnection(polaczenie))
                {
                    conn.Open();
                    string sql = "INSERT INTO Sprzet (id_kategorii, nazwa, marka, cena_za_dobe, stan_techniczny) VALUES (@kat, @nazwa, 'Inna', @cena, @stan)";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@kat", 1); // Domyślna kategoria dla uproszczenia
                        cmd.Parameters.AddWithValue("@nazwa", s.Nazwa);
                        cmd.Parameters.AddWithValue("@cena", s.CenaZaDobe);
                        cmd.Parameters.AddWithValue("@stan", s.Stan);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Błąd dodawania sprzętu: " + ex.Message); }
        }

        public void EdytujSprzet(Sprzet s)
        {
            try
            {
                using (var conn = new SqlConnection(polaczenie))
                {
                    conn.Open();
                    string sql = "UPDATE Sprzet SET nazwa=@nazwa, cena_za_dobe=@cena, stan_techniczny=@stan WHERE id_sprzetu=@id";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", s.Id);
                        cmd.Parameters.AddWithValue("@nazwa", s.Nazwa);
                        cmd.Parameters.AddWithValue("@cena", s.CenaZaDobe);
                        cmd.Parameters.AddWithValue("@stan", s.Stan);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Błąd edycji sprzętu: " + ex.Message); }
        }

        public bool UsunSprzet(int id)
        {
            try
            {
                using (var conn = new SqlConnection(polaczenie))
                {
                    conn.Open();
                    string sql = "DELETE FROM Sprzet WHERE id_sprzetu=@id";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547) // Błąd relacji (sprzęt jest w wypożyczeniach)
                {
                    Console.WriteLine("\n>> BŁĄD: Nie można usunąć sprzętu, bo jest w historii wypożyczeń.");
                }
                else Console.WriteLine("Błąd SQL: " + ex.Message);

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd: " + ex.Message);
                return false;
            }
        }
        // ================= SEKCJA: PRACOWNICY =================

        public List<Pracownik> PobierzPracownikow()
        {
            var lista = new List<Pracownik>();
            try
            {
                using (var conn = new SqlConnection(polaczenie))
                {
                    conn.Open();
                    string sql = "SELECT * FROM Pracownicy";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        var r = cmd.ExecuteReader();
                        while (r.Read())
                        {
                            lista.Add(new Pracownik
                            {
                                Id = (int)r["id_pracownika"],
                                Imie = r["imie"].ToString(),
                                Nazwisko = r["nazwisko"].ToString(),
                                Stanowisko = r["stanowisko"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Błąd: " + ex.Message); }
            return lista;
        }

        // ================= SEKCJA: TRANSAKCJE BIZNESOWE (WYPOŻYCZENIA) =================

        // Realizuje złożony proces wypożyczenia w ramach jednej Transakcji SQL.
        // Zapewnia to spójność danych (Atomowość) - albo wszystkie operacje się udadzą, albo żadna.
        public void DokonajWypozyczenia(int idKlienta, int idPracownika, int idSprzetu, int iloscDni)
        {
            using (var conn = new SqlConnection(polaczenie))
            {
                conn.Open();
                var transakcja = conn.BeginTransaction(); // Rozpoczęcie transakcji
                try
                {
                    // --- KROK 0: SPRAWDZENIE DOSTĘPNOŚCI (Blokada podwójnego wypożyczenia) ---
                    string sqlCheck = @"
                        SELECT COUNT(*) 
                        FROM Szczegoly_Wypozyczenia sw 
                        JOIN Wypozyczenia w ON sw.id_wypozyczenia = w.id_wypozyczenia 
                        WHERE sw.id_sprzetu = @idS AND w.status_zwrotu = 'W toku'";

                    using (var cmdCheck = new SqlCommand(sqlCheck, conn, transakcja))
                    {
                        cmdCheck.Parameters.AddWithValue("@idS", idSprzetu);
                        int zajety = (int)cmdCheck.ExecuteScalar();
                        if (zajety > 0) throw new Exception("Ten sprzęt jest już wypożyczony i nie został zwrócony!");
                    }
                    // ------------------------------------------------------------------------

                    // Krok 1: Pobranie ceny sprzętu z bazy
                    decimal cena = 0;
                    string sqlCena = "SELECT cena_za_dobe FROM Sprzet WHERE id_sprzetu = @idS";

                    using (var cmd = new SqlCommand(sqlCena, conn, transakcja))
                    {
                        cmd.Parameters.AddWithValue("@idS", idSprzetu);
                        var w = cmd.ExecuteScalar();
                        if (w == null) throw new Exception("Brak sprzętu!");
                        cena = (decimal)w;
                    }

                    // Krok 2: Logika biznesowa - obliczenie kosztu
                    decimal koszt = cena * iloscDni;
                    int idWyp = 0;

                    // Krok 3: Utworzenie rekordu wypożyczenia i pobranie jego nowego ID
                    string sqlWyp = @"INSERT INTO Wypozyczenia (id_klienta, id_pracownika, data_planowanego_zwrotu, status_zwrotu) 
                                      OUTPUT INSERTED.id_wypozyczenia 
                                      VALUES (@idK, @idP, @dataZ, 'W toku')";

                    using (var cmd = new SqlCommand(sqlWyp, conn, transakcja))
                    {
                        cmd.Parameters.AddWithValue("@idK", idKlienta);
                        cmd.Parameters.AddWithValue("@idP", idPracownika);
                        cmd.Parameters.AddWithValue("@dataZ", DateTime.Now.AddDays(iloscDni));
                        idWyp = (int)cmd.ExecuteScalar();
                    }

                    // Krok 4: Zapisanie szczegółów finansowych
                    string sqlSzcz = "INSERT INTO Szczegoly_Wypozyczenia (id_wypozyczenia, id_sprzetu, koszt_wypozyczenia) VALUES (@idW, @idS, @koszt)";
                    using (var cmd = new SqlCommand(sqlSzcz, conn, transakcja))
                    {
                        cmd.Parameters.AddWithValue("@idW", idWyp);
                        cmd.Parameters.AddWithValue("@idS", idSprzetu);
                        cmd.Parameters.AddWithValue("@koszt", koszt);
                        cmd.ExecuteNonQuery();
                    }

                    // Zatwierdzenie wszystkich zmian w bazie
                    transakcja.Commit();
                    Console.WriteLine($"Wypożyczono! Koszt: {koszt} PLN");
                }
                catch (Exception ex)
                {
                    // Wycofanie zmian w przypadku błędu (Rollback)
                    transakcja.Rollback();
                    Console.WriteLine("Błąd transakcji: " + ex.Message);
                }
            }
        }

        // Metoda raportowa wykorzystująca złączenia tabel (JOIN).
        // Pobiera czytelne dane (nazwiska, nazwy) zamiast surowych kluczy obcych.
        public List<string> PobierzAktywneWypozyczenia()
        {
            var lista = new List<string>();
            try
            {
                using (var conn = new SqlConnection(polaczenie))
                {
                    conn.Open();
                    string sql = @"
                        SELECT w.id_wypozyczenia, k.id_klienta, k.imie, k.nazwisko, s.nazwa, w.data_planowanego_zwrotu
                        FROM Wypozyczenia w
                        JOIN Klienci k ON w.id_klienta = k.id_klienta
                        JOIN Szczegoly_Wypozyczenia sw ON sw.id_wypozyczenia = w.id_wypozyczenia
                        JOIN Sprzet s ON sw.id_sprzetu = s.id_sprzetu
                        WHERE w.status_zwrotu = 'W toku'";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        var r = cmd.ExecuteReader();
                        while (r.Read())
                        {
                            string data = ((DateTime)r["data_planowanego_zwrotu"]).ToShortDateString();

                            lista.Add($"ID Wyp: {r["id_wypozyczenia"]} | Klient: [{r["id_klienta"]}] {r["imie"]} {r["nazwisko"]} | Sprzęt: {r["nazwa"]} | Termin: {data}");
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Błąd listy wypożyczeń: " + ex.Message); }
            return lista;
        }
        

        // Obsługuje zwrot sprzętu (aktualizacja statusu w bazie).
        public void ZwrocSprzet(int idWypozyczenia)
        {
           
            string connStr = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=WypozyczalniaSport;Integrated Security=True";

            using (var polaczenie = new SqlConnection(connStr))
            {
                polaczenie.Open();

               
                // Dodajemy warunek: AND status_zwrotu != 'Zakończone'
                // Dzięki temu nie można oddać sprzętu, który już jest oddany.
                string query = @"UPDATE Wypozyczenia 
                         SET status_zwrotu = 'Zakończone', 
                             data_zwrotu = GETDATE() 
                         WHERE id_wypozyczenia = @Id 
                         AND status_zwrotu != 'Zakończone'";

                using (var komenda = new SqlCommand(query, polaczenie))
                {
                    komenda.Parameters.AddWithValue("@Id", idWypozyczenia);

                    int zmienioneWiersze = komenda.ExecuteNonQuery();

                    if (zmienioneWiersze > 0)
                    {
                        // Udało się - status był inny niż 'Zakończone'
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n>> SUKCES: Zwrot został zaakceptowany.");
                        Console.ResetColor();
                    }
                    else
                    {
                        // Nie udało się - albo ID nie istnieje, albo już było 'Zakończone'
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n>> BŁĄD: Nie można zwrócić tego wypożyczenia.");
                        Console.WriteLine(">> (Powód: ID nie istnieje LUB sprzęt został już oddany wcześniej)");
                        Console.ResetColor();
                    }
                }
            }
        }

        // ================= RAPORTY =================

        // Raport 1: Stan magazynowy (wykorzystuje funkcje agregujące COUNT i GROUP BY).
        public List<string> GenerujRaportKategorii()
        {
            var raport = new List<string>();
            try
            {
                using (var conn = new SqlConnection(polaczenie))
                {
                    conn.Open();
                    string sql = @"
                        SELECT k.nazwa AS Kategoria, COUNT(s.id_sprzetu) AS Ilosc
                        FROM Sprzet s
                        JOIN Kategorie k ON s.id_kategorii = k.id_kategorii
                        GROUP BY k.nazwa";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        var r = cmd.ExecuteReader();
                        raport.Add("--------------------------------");
                        raport.Add(String.Format("| {0,-20} | {1,5} |", "KATEGORIA", "ILOŚĆ"));
                        raport.Add("--------------------------------");

                        while (r.Read())
                        {
                            string kat = r["Kategoria"].ToString();
                            int ilosc = (int)r["Ilosc"];
                            raport.Add(String.Format("| {0,-20} | {1,5} |", kat, ilosc));
                        }
                        raport.Add("--------------------------------");
                    }
                }
            }
            catch (Exception ex) { raport.Add("Błąd raportu: " + ex.Message); }
            return raport;
        }

        // Raport 2: Top 3 Klientów (sortowanie i limitowanie wyników).
        public List<string> GenerujRaportTopKlientow()
        {
            var raport = new List<string>();
            try
            {
                using (var conn = new SqlConnection(polaczenie))
                {
                    conn.Open();
                    string sql = @"
                        SELECT TOP 3 k.imie, k.nazwisko, COUNT(w.id_wypozyczenia) as Ilosc
                        FROM Klienci k
                        JOIN Wypozyczenia w ON k.id_klienta = w.id_klienta
                        GROUP BY k.imie, k.nazwisko
                        ORDER BY Ilosc DESC";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        var r = cmd.ExecuteReader();
                        raport.Add("--- TOP 3 KLIENTÓW ---");
                        while (r.Read())
                        {
                            int ilosc = (int)r["Ilosc"];
                            string odmiana;

                            // Prosta logika odmiany słowa 'wypożyczenie' w języku polskim.
                            if (ilosc == 1)
                            {
                                odmiana = "wypożyczenie";
                            }
                            else if (ilosc % 10 >= 2 && ilosc % 10 <= 4 && (ilosc % 100 < 10 || ilosc % 100 >= 20))
                            {
                                odmiana = "wypożyczenia";
                            }
                            else
                            {
                                odmiana = "wypożyczeń";
                            }

                            raport.Add($"{r["imie"]} {r["nazwisko"]} - {ilosc} {odmiana}");
                        }
                    }
                }
            }
            catch (Exception ex) { raport.Add("Błąd: " + ex.Message); }
            return raport;
        }

        // Raport 3: Finanse (agregacja SUM).
        public List<string> GenerujRaportFinansowy()
        {
            var raport = new List<string>();
            try
            {
                using (var conn = new SqlConnection(polaczenie))
                {
                    conn.Open();
                    string sql = "SELECT SUM(koszt_wypozyczenia) FROM Szczegoly_Wypozyczenia";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        var wynik = cmd.ExecuteScalar();
                        decimal suma = (wynik != DBNull.Value) ? (decimal)wynik : 0;

                        raport.Add("--- PODSUMOWANIE FINANSOWE ---");
                        raport.Add($"Całkowity przychód: {suma:C}");
                    }
                }
            }
            catch (Exception ex) { raport.Add("Błąd: " + ex.Message); }
            return raport;
        }
    }
}