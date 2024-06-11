using System;
using System.Linq;
using System.Windows;

namespace kalkulator
{
    //partial- kod podzielony jest na różne pliki xaml i xaml.cs
    //dziedziczy po klasie window, jest głownym oknem aplikacji wpf
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); //łączy xaml z kodem c#, przez o możemy wyświetlić plik xaml, tworząc UI, podłącza zdarzenia
        }

        // oblicza logarytm lub ciąg za pomocą użycia przycisku
        //private- funkcja jest prywatna i może być wywoływana tylko z wnętrza klasy MainWindow
        //void oznacza, że metoda ObliczButton_Click nie zwraca żadnej wartości, jest to typ zwracany przez metodę
        //sender- obiekt który wywoływał zdarzenie (przycisk)
        private void ObliczButton_Click(object sender, RoutedEventArgs e)
        {
            // pobranie tekstu z pola tekstowego
            string input = WejscieTextBox.Text;
            string wynik;

            // sprawdza, czy wejście zawiera logarytmy
            if (input.Contains("log"))
            {
                wynik = ObliczLogarytmy(input);
                //jeśli tekst zawiera frazę log, metoda wywołuje funkcję ObliczLogarytmy, aby obliczyć wartość logarytmów
            }
            // sprawdza, czy wejście zawiera ciągi
            else if (input.Contains("a_"))
            {
                wynik = ObliczCiag(input);
                //jeśli tekst zawiera frazę a_, metoda wywołuje funkcję ObliczCiag, aby obliczyć wartość ciągu
            }
            // niepoprawne wejście
            else
            {
                wynik = "Niepoprawne dane wejściowe";
            }

            // wyświetla wynik w polu tekstowym
            WynikTextBlock.Text = wynik;
        }

        // funkcja do działania logarytmu
        //metoda zwraca wartość typu string, która jest wynikiem obliczeń lub komunikatem o błędzie.
        private string ObliczLogarytmy(string input)
        {
            try
            {
                var logParser = new ParserLogarytmow(); //tworzenie nowej instancji klasy ParserLogarytmow, która jest odpowiedzialna za parsowanie i obliczanie wartości wyrażeń logarytmicznych.
                return logParser.Ocen(input).ToString();
            }
            catch (Exception ex)
            {
                return $"Błąd: {ex.Message}"; //jeśli podczas wykonywania metody Ocen wystąpi jakikolwiek wyjątek, zostanie on przechwycony przez blok catch i wyświetlony jako błąd
            }
        }

        // funkcja do działania ciągów
        private string ObliczCiag(string input)
        {
            try
            {
                var seqParser = new ParserCiagow();
                return seqParser.Ocen(input);
            }
            catch (Exception ex)
            {
                return $"Błąd: {ex.Message}";
            }
        }
    }

    // klasa do parsowania i obliczania wyrażeń logarytmicznych
    public class ParserLogarytmow
    {
        // funkcja do oceny wyrażenia logarytmicznego
        //metoda Ocen przyjmuje jeden parametr input typu string, który reprezentuje wyrażenie logarytmiczne.
        //metoda zwraca wartość typu double, która jest wynikiem obliczeń logarytmicznych.
        public double Ocen(string input)
        {
            // usunięcie spacji z wejścia
            input = input.Replace(" ", "");//usuwa spacje
            // podzielenie wejścia na części według operatorów matematycznych
            string[] części = input.Split(new char[] { '+', '-', '*', '/' });//podzielenie wejscia na czesci wg operatorow matematycznych
            double wynik = 0;

            // iteracja przez każdą część wyrażenia
            foreach (var część in części)//iteracja przez kazda czesc wyrazenia
            {
                // obliczenie logarytmów o podstawie różnej od 10
                if (część.StartsWith("log_")) //sprawdzenie, czy część wyrażenia zaczyna się od log_, co oznacza logarytm o podstawie innej niż 10.
                {
                    string[] logCzęści = część.Substring(4).TrimEnd(')').Split('('); //podzielenie części wyrażenia na podstawę logarytmu i liczbę.
                    int baza = int.Parse(logCzęści[0]); //parsowanie podstawy logarytmu.
                    double liczba = double.Parse(logCzęści[1]);
                    wynik += Math.Log(liczba, baza); //obliczenie wartości logarytmu i dodanie do wyniku.
                }
                // obliczenie logarytmów o podstawie 10
                else if (część.StartsWith("log"))
                {
                    double liczba = double.Parse(część.Substring(4).TrimEnd(')'));
                    wynik += Math.Log10(liczba);
                }
            }

            return wynik; //zwrocenie wyniku
        }
    }

    //ciąg to uporządkowana kolekcja elementów, które są ułożone w określonym porządku. Każdy element w ciągu jest oznaczony indeksem (numerem pozycji). Ciągi można opisać wzorem ogólnym, który określa każdy element ciągu w zależności od jego indeksu.
    //klasa do parsowania i oceny ciągów
    //klasa ParserCiagow w kodzie służy do parsowania i oceny ciągów
    //ciag arytmetyczny: a_1=2, a_2=5, a_3=8
    //różnica między kolejnymi elementami jest stała (różnica stała).
    //ciąg geometryczny: a_1=2, a_2=6, a_3=18
    //iloraz (stosunek) między kolejnymi elementami jest stały (iloraz stały).
    public class ParserCiagow
    {
        // funkcja do oceny ciągu
        public string Ocen(string input)
        {
            // podzielenie wejścia na części ciągu
            //dzieli ciąg wejściowy input na części w miejscach, gdzie znajduje się przecinek.
            //t.Trim(): usuwa białe znaki z początku i końca każdego elementu
            //t.Split('='): dzieli każdy element na części w miejscach, gdzie znajduje się znak równości =.
            //dla "a1=2.0", wynik będzie tablicą ["a1", "2.0"]
            //przekształca każdą tablicę uzyskaną w poprzednim kroku w parę klucz-wartość i tworzy słownik (dictionary) z tych par.
            var terminy = input.Split(',').Select(t => t.Trim().Split('=')).ToDictionary(t => int.Parse(t[0].Substring(2)), t => double.Parse(t[1]));
            //dla pierwszej części tablicy t[0] (np. "a1")
            //usuwa dwa pierwsze znaki, pozostawiając tylko cyfry jako ciąg znaków (np. "1").
            //konwertuje ten ciąg na liczbę całkowitą (np. 1).
            //dla drugiej części tablicy t[1] (np. "2.0"), konwertuje ten ciąg na liczbę zmiennoprzecinkową (np. 2.0).
            //.ToDictionary(...): tworzy słownik, gdzie klucz to indeks wyrazu jako liczba całkowita, a wartość to liczba zmiennoprzecinkowa.

            // obliczenie różnic między kolejnymi częściami tabeli
            var różnice = terminy.Values.Zip(terminy.Values.Skip(1), (a, b) => b - a).ToList();

            // sprawdzenie, czy ciąg jest arytmetyczny
            bool czyArytmetyczny = różnice.All(d => d == różnice.First());
            // sprawdzenie, czy ciąg jest geometryczny
            bool czyGeometryczny = różnice.All(d => Math.Abs(d / różnice.First()) < 0.0001);

            if (czyArytmetyczny)
            {   
                double r = różnice.First();
                return $"Ciąg arytmetyczny o różnicy {r}";
            }
            else if (czyGeometryczny)
            {
                double r = terminy.Values.ElementAt(1) / terminy.Values.ElementAt(0);
                return $"Ciąg geometryczny o ilorazie {r}";
            }
            else
            {
                return "Ciąg nie jest ani arytmetyczny, ani geometryczny";
            }
        }
    }
}
