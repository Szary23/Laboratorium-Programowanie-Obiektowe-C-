using System.Collections.Generic;
using WypozyczalniaApp.Modele;

namespace WypozyczalniaApp.Interfejsy
{
    public interface IZarzadzanieBaza
    {
        // --- KLIENCI ---
        List<Klient> PobierzWszystkichKlientow();
        void DodajKlienta(Klient klient);
        void EdytujKlienta(Klient klient);
        bool UsunKlienta(int id);

        // --- SPRZĘT ---
        List<Sprzet> PobierzWszystkieSprzety();
        List<Sprzet> PobierzDostepnySprzet();
        void DodajSprzet(Sprzet sprzet);
        void EdytujSprzet(Sprzet sprzet);
        bool UsunSprzet(int id);

        // --- PRACOWNICY ---
        List<Pracownik> PobierzPracownikow();

        // --- WYPOŻYCZENIA I ZWROTY ---
        void DokonajWypozyczenia(int idKlienta, int idPracownika, int idSprzetu, int iloscDni);
        void ZwrocSprzet(int idWypozyczenia);
        List<string> PobierzAktywneWypozyczenia();

        // --- RAPORTY ---
        // 1. Ilość sprzętu w kategoriach
        List<string> GenerujRaportKategorii();

        // 2. Najlepsi klienci (najwięcej wypożyczeń)
        List<string> GenerujRaportTopKlientow();

        // 3. Podsumowanie finansowe firmy
        List<string> GenerujRaportFinansowy();
    }
}