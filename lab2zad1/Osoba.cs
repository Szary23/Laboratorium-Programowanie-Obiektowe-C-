using System;

///Napisz klasę Osoba, która będzie przechowywać informacje o imieniu, nazwisku oraz wieku osoby.
///Zaimplementuj konstruktor, który będzie przyjmował wszystkie trzy wartości.
///Użyj właściwości Imie, Nazwisko, Wiek, z walidacją:
///Imię i Nazwisko muszą mieć co najmniej 2 znaki.
///Wiek musi być liczbą dodatnią.
///Zaimplementuj metodę WyswietlInformacje(), która wyświetli informacje o osobie.

namespace lab2zad1
{
    internal class Osoba
    {

        private string imie, nazwisko;
        private int wiek;
        //zmienna imie
        public string Imie
        {
            get
            {
                return imie;
            }
            set
            {
                if (value.Length >= 2)
                {
                    imie = value;
                }
                else
                {
                    throw new ArgumentException("Imię musi mieć co najmniej 2 znaki.");
                }
            }
        }
        //zmienna nazwisko   
        public string Nazwisko
        {
            get
            {
                return nazwisko;
            }
            set
            {
                if (value.Length >= 2)
                {
                    nazwisko = value;
                }
                else
                {
                    throw new ArgumentException("Nazwisko musi mieć co najmniej 2 znaki.");
                }
            }
        }
        //zmienna wiek
        public int Wiek
        {
            get
            {
                return wiek;
            }
            set
            {
                if (value > 0)
                {
                    wiek = value;
                }
                else
                {
                    throw new ArgumentException("Wiek musi być liczbą dodatnią.");
                }
            }
        }

        public Osoba(string imie, string nazwisko, int wiek)
        {
            Imie = imie;
            Nazwisko = nazwisko;
            Wiek = wiek;
        }

        public void WyswietlInformacje()
        {
            Console.WriteLine($"Imię: {Imie}, Nazwisko: {Nazwisko}, Wiek: {Wiek}");
        }

    }
}
