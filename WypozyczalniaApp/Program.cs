using System;
using System.Collections.Generic;
using WypozyczalniaApp.Interfejsy;
using WypozyczalniaApp.Logika;
using WypozyczalniaApp.Modele;

class Program
{
    // Warstwa Prezentacji (UI) komunikuje się z Warstwą Logiki (DAL) wyłącznie przez interfejs.
    static IZarzadzanieBaza baza = new BazaDanych();

    // Główny punkt wejścia do aplikacji.
    static void Main(string[] args)

    {
        Console.Title = "Wypożyczalnia sprzętu";
        // Definiujemy opcje menu w tablicy
        string[] opcjeMenu = {
            "1. Zarządzanie KLIENTAMI",
            "2. Zarządzanie SPRZĘTEM",
            "3. WYPOŻYCZENIA i ZWROTY",
            "4. RAPORTY i STATYSTYKI",
            "0. Wyjście"
        };

        while (true)
        {
            // Menu Główne 
            int wybor = WyswietlMenuHybrydowe(opcjeMenu, "=== SYSTEM WYPOŻYCZALNI SPRZĘTU ===");

            switch (wybor)
            {
                case 0:
                    MenuKlienci(); // Przejście do zarządzania klientami
                    break;
                case 1:
                    MenuSprzet(); // Przejście do zarządzania sprzętem
                    break;
                case 2:
                    MenuTransakcje(); // Przejście do wypożyczeń i zwrotów
                    break;
                case 3:
                    MenuRaporty(); // Przejście do raportów i statystyk
                    break;
                case 4:
                    return; // Wyjście (opcja 0)
                case -1:
                    return; // Wyjście (klawisz ESC)
                default:
                    break; // Ignorujemy inne wartości
            }
        }
    }

    // ================= SEKCJA: METODY POMOCNICZE =================

    // Metoda wyświetlająca menu z obsługą strzałek i cyfr i ESC.
   
    static int WyswietlMenuHybrydowe(string[] opcje, string tytul, bool pokazStopke = false)
    {
        int aktualnyWybor = 0; // Indeks aktualnie zaznaczonej opcji

        while (true)
        {
            Console.Clear();
            Console.WriteLine(tytul + "\n");
            Console.WriteLine("Wybierz opcję:\n");

            for (int i = 0; i < opcje.Length; i++)  // Wyświetlenie wszystkich opcji menu
            {
                if (i == aktualnyWybor)
                {
                    // Podświetlenie wybranej opcji
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine(">> " + opcje[i] + " <<");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("   " + opcje[i]);
                }
            }

            // Wyświetlamy stopkę 
            if (pokazStopke)
            {
                Console.WriteLine("\n-----------------------------------");
                Console.WriteLine(" [ESC] - Powrót");
            }
            // Odczyt naciśniętego klawisza
            ConsoleKeyInfo klawisz = Console.ReadKey(true);

            // Obsługa nawigacji strzałkami
            if (klawisz.Key == ConsoleKey.UpArrow)
            {
                aktualnyWybor--;
                if (aktualnyWybor < 0)
                {
                    aktualnyWybor = opcje.Length - 1; // Zawijanie do końca listy

                }
            }
            else if (klawisz.Key == ConsoleKey.DownArrow)
            {
                aktualnyWybor++;
                if (aktualnyWybor >= opcje.Length)
                {
                    aktualnyWybor = 0; // Zawijanie do początku listy
                }
            }
            // Zatwierdzenie Enterem
            else if (klawisz.Key == ConsoleKey.Enter)
            {
                return aktualnyWybor;
            }
            // Obsługa skrótów klawiszowych (cyfry)
            else if (char.IsDigit(klawisz.KeyChar))
            {
                int cyfra = int.Parse(klawisz.KeyChar.ToString());
                // Logika: 1->Index 0, 2->Index 1...
                if (cyfra >= 1 && cyfra <= opcje.Length)
                {
                    return cyfra - 1; // Konwersja na indeks tablicy
                }
                // Obsługa "0" jako ostatniej opcji w menu głównym
                if (cyfra == 0 && opcje.Length > 0 && !pokazStopke)
                {
                    return opcje.Length - 1;
                }
            }
            // Wyjście ESC (zwracamy -1)
            else if (klawisz.Key == ConsoleKey.Escape)
            {
                return -1;
            }
        }
    }

    // Podstawowe wczytywanie tekstu (obsługa ESC, Backspace)
    static string WczytajZKlawiatury()
    {
        string buffer = "";
        while (true)
        {
            ConsoleKeyInfo klawisz = Console.ReadKey(true);

            if (klawisz.Key == ConsoleKey.Escape)
            {
                return null; // Anulowanie wczytywania
            }
            else if (klawisz.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return buffer; // Zatwierdzenie tekstu
            }
            else if (klawisz.Key == ConsoleKey.Backspace)
            {
                if (buffer.Length > 0)
                {
                    buffer = buffer.Substring(0, buffer.Length - 1);
                    Console.Write("\b \b");
                }
            }
            else if (!char.IsControl(klawisz.KeyChar))
            {
                buffer += klawisz.KeyChar; // Dodanie znaku do bufora
                Console.Write(klawisz.KeyChar);
            }
        }
    }

    // Walidacja liczby całkowitej (ID, Dni)
    static int WczytajLiczbe()
    {
        while (true)
        {
            string tekst = WczytajZKlawiatury();
            if (tekst == null)
            {
                return -1; // ESC -> anulowanie
            }

            int liczba;
            if (int.TryParse(tekst, out liczba))
            {
                return liczba; // Poprawna liczba
            }
            else
            {
                Console.Write("To nie jest liczba! Wpisz poprawnie (lub ESC): ");
            }
        }
    }

    // Walidacja kwoty (Cena)
    static decimal WczytajDecimal()
    {
        while (true)
        {
            string tekst = WczytajZKlawiatury();
            if (tekst == null)
            {
                return -1; // ESC -> anulowanie
            }

            decimal wartosc;
            if (decimal.TryParse(tekst, out wartosc))
            {
                if (wartosc >= 0)
                {
                    return wartosc; // Poprawna kwota
                }
                else
                {
                    Console.Write("Cena nie może być ujemna! Wpisz ponownie: ");
                }
            }
            else
            {
                Console.Write("Błędna kwota! Wpisz poprawnie (np. 50,00): ");
            }
        }
    }

    // Walidacja Email
    static string WczytajEmail()
    {
        while (true)
        {
            string email = WczytajZKlawiatury();
            if (email == null)
            {
                return null;
            }

            if (email.Contains("@") && email.Contains("."))
            {
                return email;
            }
            Console.Write("Błędny adres email. Spróbuj ponownie: ");
        }
    }

    // Walidacja Telefonu
    static string WczytajTelefon()
    {
        while (true)
        {
            string tel = WczytajZKlawiatury();
            if (tel == null)
            {
                return null;
            }

            if (tel.Length == 9 && long.TryParse(tel, out _))
            {
                return tel;
            }
            Console.Write("Niepoprawny numer telefonu. Spróbuj ponownie: ");
        }
    }

    // ================= SEKCJA: LOGIKA MENU =================

    // ================= SEKCJA: ZARZĄDZANIE KLIENTAMI =================
    // Obsługuje pełny proces CRUD dla encji Klient.
    static void MenuKlienci()
    {
        Console.Clear();
        Console.WriteLine("--- ZARZĄDZANIE KLIENTAMI ---");

        // Pobranie aktualnego stanu z bazy danych
        List<Klient> klienci = baza.PobierzWszystkichKlientow();
        foreach (Klient k in klienci)
        {
            Console.WriteLine("[" + k.Id + "] " + k.Imie + " " + k.Nazwisko + " (Tel: " + k.Telefon + ", Email: " + k.Email + ")");
        }

        Console.WriteLine("\nCo chcesz zrobić?");
        Console.WriteLine("D - Dodaj | E - Edytuj | U - Usuń | ESC - Wstecz");

        ConsoleKeyInfo klawisz = Console.ReadKey(true);
        if (klawisz.Key == ConsoleKey.Escape)
        {
            return; // Powrót do Menu Głównego
        }

        string opcja = klawisz.KeyChar.ToString().ToUpper();

        try
        {
            // --- DODAWANIE NOWEGO KLIENTA ---
            if (opcja == "D")
            {
                Console.WriteLine("\n(Wciśnij ESC, aby anulować)");

                // Pobieranie danych z walidacją (nie można przejść dalej z błędnym danymi)
                Console.Write("Imię: ");
                string imie = WczytajZKlawiatury(); if (imie == null) return; // Obsługa anulowania (ESC)

                Console.Write("Nazwisko: ");
                string nazw = WczytajZKlawiatury(); if (nazw == null) return;

                Console.Write("Email: ");
                string email = WczytajEmail(); if (email == null) return; // Dedykowana walidacja formatu email

                Console.Write("Telefon: ");
                string tel = WczytajTelefon(); if (tel == null) return; // Walidacja długości i znaków

                // Utworzenie obiektu domenowego i przekazanie do warstwy danych
                Klient nowyKlient = new Klient();
                nowyKlient.Imie = imie;
                nowyKlient.Nazwisko = nazw;
                nowyKlient.Email = email;
                nowyKlient.Telefon = tel;

                baza.DodajKlienta(nowyKlient);
                Console.WriteLine("Dodano klienta!");
            }
            // --- EDYCJA ISTNIEJĄCEGO KLIENTA ---
            else if (opcja == "E")
            {
                int id = -1;
                // Pętla wymuszająca podanie poprawnego, istniejącego ID
                while (true)
                {
                    Console.Write("Podaj ID klienta do edycji: ");
                    id = WczytajLiczbe();
                    if (id == -1) return;

                    // Sprawdzenie czy ID istnieje w pobranej wcześniej liście (lokalny cache)
                    if (klienci.Exists(k => k.Id == id)) break;
                    Console.WriteLine("Nie ma takiego ID. Spróbuj ponownie.");
                }

                // Pobieranie nowych danych (użytkownik musi wpisać wszystkie pola od nowa)
                Console.Write("Nowe Imię: ");
                string imie = WczytajZKlawiatury(); if (imie == null) return;

                Console.Write("Nowe Nazwisko: ");
                string nazw = WczytajZKlawiatury(); if (nazw == null) return;

                Console.Write("Nowy Email: ");
                string email = WczytajEmail(); if (email == null) return;

                Console.Write("Nowy Telefon: ");
                string tel = WczytajTelefon(); if (tel == null) return;

                Klient k = new Klient();
                k.Id = id;
                k.Imie = imie;
                k.Nazwisko = nazw;
                k.Email = email;
                k.Telefon = tel;

                baza.EdytujKlienta(k);
                Console.WriteLine("Zaktualizowano dane!");
            }
            // --- USUWANIE KLIENTA ---
            else if (opcja == "U")
            {
                int id = -1;
                while (true)
                {
                    Console.Write("Podaj ID klienta do usunięcia: ");
                    id = WczytajLiczbe();
                    if (id == -1) return;

                    
                    if (klienci.Exists(k => k.Id == id)) break;
                    else Console.WriteLine("Nie ma takiego ID. Spróbuj ponownie.");
                }

                Console.Write("Potwierdź (T/N): ");
                string potw = Console.ReadLine();
                if (potw != null && potw.ToUpper() == "T")
                {
                  
                    // Sprawdzamy, czy metoda zwróciła TRUE.
                    // Jeśli zwróciła FALSE (błąd), to NIE wchodzimy do środka i nie piszemy "Usunięto".
                    if (baza.UsunKlienta(id))
                    {
                        Console.WriteLine("Usunięto klienta.");
                    }
                }

            }
        }
        catch (Exception ex)
        {
            // Globalna obsługa błędów dla tego modułu (np. błąd połączenia z SQL)
            Console.WriteLine("\nBłąd: " + ex.Message);
        }

        // Pauza, aby użytkownik zdążył przeczytać komunikat sukcesu/błędu
        if (opcja == "D" || opcja == "E" || opcja == "U")
        {
            Console.WriteLine("\nNaciśnij dowolny klawisz...");
            Console.ReadKey();
        }
    }

    // ================= SEKCJA: ZARZĄDZANIE SPRZĘTEM =================
    // Analogiczna logika CRUD dla zasobów wypożyczalni.
    static void MenuSprzet()
    {
        Console.Clear();
        Console.WriteLine("--- ZARZĄDZANIE SPRZĘTEM ---");

        List<Sprzet> sprzet = baza.PobierzWszystkieSprzety();
        foreach (Sprzet s in sprzet)
        {
            Console.WriteLine("[" + s.Id + "] " + s.Nazwa + " | Cena: " + s.CenaZaDobe + " PLN | Stan: " + s.Stan);
        }

        Console.WriteLine("\nCo chcesz zrobić?");
        Console.WriteLine("D - Dodaj | E - Edytuj | U - Usuń | ESC - Wstecz");

        ConsoleKeyInfo klawisz = Console.ReadKey(true);
        if (klawisz.Key == ConsoleKey.Escape) return;

        string opcja = klawisz.KeyChar.ToString().ToUpper();

        try
        {
            // --- DODAWANIE SPRZĘTU ---
            if (opcja == "D")
            {
                Console.WriteLine("\n(ESC anuluje)");

                Console.Write("Nazwa sprzętu: ");
                string nazwa = WczytajZKlawiatury(); if (nazwa == null) return;

                Console.Write("Cena za dobę: ");
                // Użycie typu decimal 
                decimal cena = WczytajDecimal(); if (cena == -1) return;

                Console.Write("Stan: ");
                string stan = WczytajZKlawiatury(); if (stan == null) return;

                Sprzet nowySprzet = new Sprzet();
                nowySprzet.Nazwa = nazwa;
                nowySprzet.CenaZaDobe = cena;
                nowySprzet.Stan = stan;

                baza.DodajSprzet(nowySprzet);
                Console.WriteLine("Dodano sprzęt!");
            }
            // --- EDYCJA SPRZĘTU ---
            else if (opcja == "E")
            {
                int id = -1;
                while (true)
                {
                    Console.Write("Podaj ID sprzętu do edycji: ");
                    id = WczytajLiczbe();
                    if (id == -1) return;

                    if (sprzet.Exists(s => s.Id == id)) break;
                    else Console.WriteLine("Błąd: Nie znaleziono sprzętu.");
                }

                Console.Write("Nowa Nazwa: ");
                string nazwa = WczytajZKlawiatury(); if (nazwa == null) return;

                Console.Write("Nowa Cena: ");
                decimal cena = WczytajDecimal(); if (cena == -1) return;

                Console.Write("Nowy Stan: ");
                string stan = WczytajZKlawiatury(); if (stan == null) return;

                Sprzet s = new Sprzet();
                s.Id = id;
                s.Nazwa = nazwa;
                s.CenaZaDobe = cena;
                s.Stan = stan;

                baza.EdytujSprzet(s);
                Console.WriteLine("Zaktualizowano!");
            }
            // --- USUWANIE SPRZĘTU ---
            else if (opcja == "U")
            {
                int id = -1;
                while (true)
                {
                    Console.Write("\nPodaj ID sprzętu do usunięcia: ");
                    id = WczytajLiczbe();
                    if (id == -1) return;

                    if (sprzet.Exists(s => s.Id == id)) break;
                    else Console.WriteLine("Błąd: Nie ma takiego ID.");
                }

                Console.Write("Potwierdź (T/N): ");
                string potw = Console.ReadLine();
                if (potw != null && potw.ToUpper() == "T")
                {
                    // Usuwanie sprzętu tylko jeśli operacja w bazie się powiodła

                    if (baza.UsunSprzet(id))
                    {
                        Console.WriteLine("Usunięto sprzęt.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("\nBłąd: " + ex.Message);
        }

        if (opcja == "D" || opcja == "E" || opcja == "U")
        {
            Console.WriteLine("Naciśnij dowolny klawisz...");
            Console.ReadKey();
        }
    }

    // --- MENU TRANSAKCJE ---
    static void MenuTransakcje()
    {
        string[] opcje = {
            "1. Nowe wypożyczenie",
            "2. Zwrot sprzętu",
            "3. Pokaż aktywne wypożyczenia"
        };

        while (true)
        {
            // Przekazujemy 'true' jako trzeci parametr
            int wybor = WyswietlMenuHybrydowe(opcje, "--- WYPOŻYCZENIA I ZWROTY ---", true);

            if (wybor == 0) // Nowe wypożyczenie
            {
                Console.Clear();
                Console.WriteLine("\n\n-- KLIENCI --");
                foreach (Klient k in baza.PobierzWszystkichKlientow())
                {
                    Console.WriteLine("[" + k.Id + "] " + k.Imie + " " + k.Nazwisko);
                }

                Console.WriteLine("\n-- DOSTĘPNY SPRZĘT --");
                foreach (Sprzet s in baza.PobierzDostepnySprzet())
                {
                    Console.WriteLine("[" + s.Id + "] " + s.Nazwa + " (" + s.CenaZaDobe + " PLN)");
                }

                try
                {
                    Console.WriteLine("\n(ESC anuluje)");

                    Console.Write("ID Klienta: ");
                    int idK = WczytajLiczbe(); if (idK == -1) continue;

                    Console.Write("ID Pracownika: ");
                    int idP = WczytajLiczbe(); if (idP == -1) continue;

                    Console.Write("ID Sprzętu: ");
                    int idS = WczytajLiczbe(); if (idS == -1) continue;

                    Console.Write("Ile dni: ");
                    int dni = WczytajLiczbe(); if (dni == -1) continue;

                    baza.DokonajWypozyczenia(idK, idP, idS, dni);
                }
                catch
                {
                    Console.WriteLine("Błąd danych!");
                }

                Console.WriteLine("Naciśnij klawisz...");
                Console.ReadKey();
            }
            else if (wybor == 1) // Zwrot sprzętu
            {
                Console.Clear();
                List<string> lista = baza.PobierzAktywneWypozyczenia();
                foreach (string item in lista)
                {
                    Console.WriteLine(item);
                }

                Console.Write("\nPodaj ID Wypożyczenia do zwrotu (ESC anuluje): ");
                int idW = WczytajLiczbe();

                if (idW != -1)
                {
                    try
                    {
                        baza.ZwrocSprzet(idW);
                    }
                    catch
                    {
                        Console.WriteLine("Błąd ID");
                    }
                }

                Console.WriteLine("Naciśnij klawisz...");
                Console.ReadKey();
            }
            else if (wybor == 2) // Pokaż aktywne wypożyczenia
            {
                Console.Clear();
                List<string> lista = baza.PobierzAktywneWypozyczenia();
                foreach (string item in lista)
                {
                    Console.WriteLine(item);
                }

                Console.WriteLine("\nNaciśnij klawisz...");
                Console.ReadKey();
            }
            else if (wybor == -1) // Tylko ESC wychodzi z tego menu
            {
                return;
            }
        }
    }

    // --- MENU RAPORTY ---
    static void MenuRaporty()
    {
        string[] opcje = {
            "1. Stan magazynowy wg kategorii",
            "2. Top 3 Klientów",
            "3. Raport finansowy"
        };

        while (true)
        {
            // Przekazujemy 'true' jako trzeci parametr
            int wybor = WyswietlMenuHybrydowe(opcje, "--- RAPORTY I STATYSTYKI ---", true);

            if (wybor == -1) // Tylko ESC wychodzi z tego menu
            {
                return;
            }

            Console.Clear();
            Console.WriteLine("Generowanie raportu...\n");
            List<string> wynik = new List<string>();

            if (wybor == 0)
            {
                wynik = baza.GenerujRaportKategorii();
            }
            else if (wybor == 1)
            {
                wynik = baza.GenerujRaportTopKlientow();
            }
            else if (wybor == 2)
            {
                wynik = baza.GenerujRaportFinansowy();
            }

            foreach (string linia in wynik)
            {
                Console.WriteLine(linia);
            }

            Console.WriteLine("\nNaciśnij klawisz...");
            Console.ReadKey();
        }
    }
}