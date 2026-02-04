namespace WypozyczalniaApp.Modele
{
    // Klasa abstrakcyjna definiująca wspólne cechy dla wszystkich osób w systemie (Klienci, Pracownicy).
    // Dziedziczenie po EncjaBazowa zapewnia dostęp do unikalnego identyfikatora (ID) bez konieczności jego redefiniowania.
    // Zastosowanie modyfikatora abstract uniemożliwia utworzenie instancji samej klasy Osoba – wymusza to tworzenie konkretnych obiektów pochodnych.
    public abstract class Osoba : EncjaBazowa
    {
        // Przechowywanie podstawowych danych personalnych, które występują zarówno w tabeli Klienci, jak i Pracownicy.
        public string Imie { get; set; }
        public string Nazwisko { get; set; }

        // Metoda wirtualna (virtual) stanowiąca podstawę do realizacji polimorfizmu.
        // Umożliwienie klasom pochodnym nadpisania (override) tej metody w celu dostosowania formatu wyświetlanych danych.
        // Implementacja bazowa zwraca standardowe połączenie imienia i nazwiska.
        public virtual string PrzedstawSie()
        {
            return $"{Imie} {Nazwisko}";
        }
    }
}