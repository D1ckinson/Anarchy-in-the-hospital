using System;
using System.Collections.Generic;
using System.Linq;

namespace Anarchy_in_the_hospital
{
    internal class Program
    {
        static void Main()
        {
            Console.CursorVisible = false;

            int cardsQuantity = 10;

            PatientCardFabrik cardFabrik = new PatientCardFabrik();
            List<PatientCard> patientCards = new List<PatientCard>();

            for (int i = 0; i < cardsQuantity; i++)
                patientCards.Add(cardFabrik.CreateCard());

            ActionBuilder actionBuilder = new ActionBuilder(patientCards);
            Menu menu = new Menu(actionBuilder.GiveActions());

            menu.Work();
        }
    }

    class ActionBuilder
    {
        private List<PatientCard> _patientCards;

        public ActionBuilder(List<PatientCard> patientCards)
        {
            _patientCards = patientCards;

            DrawCards();
        }

        public Dictionary<string, Action> GiveActions() =>
            new Dictionary<string, Action>
            {
                { "Отсортировать по имени", SortByName },
                { "Отсортировать по болезни", SortByDisease },
                { "Отсортировать по возрасту", SortByAge }
            };

        private void SortByName()
        {
            _patientCards = _patientCards.OrderBy(card => card.Name).ToList();

            DrawCards();
        }

        private void SortByDisease()
        {
            _patientCards = _patientCards.OrderBy(card => card.Disease).ToList();

            DrawCards();
        }

        private void SortByAge()
        {
            _patientCards = _patientCards.OrderBy(card => card.Age).ToList();

            DrawCards();
        }

        private void DrawCards()
        {
            int indent = 3;
            int spaceLineSize = 100;

            char space = ' ';

            Console.CursorTop = GiveActions().Count + indent;

            foreach (PatientCard card in _patientCards)
            {
                Console.Write(new string(space, spaceLineSize));
                Console.CursorLeft = 0;
                Console.WriteLine($"{card.Name}, возраст - {card.Age}, болезнь - {card.Disease}");
            }
        }
    }

    class PatientCard
    {
        public PatientCard(string name, int age, string disease)
        {
            Name = name;
            Age = age;
            Disease = disease;
        }

        public string Name { get; private set; }
        public int Age { get; private set; }
        public string Disease { get; private set; }

    }

    class PatientCardFabrik
    {
        private List<string> _names;
        private List<string> _surnames;
        private List<string> _diseases;

        private int[] _ageStats = { 20, 60 };

        private Random _random = new Random();

        public PatientCardFabrik()
        {
            FillNames();
            FillSurnames();
            FillDiseases();
        }

        public PatientCard CreateCard()
        {
            string name = _names[_random.Next(0, _names.Count)];
            string surname = _surnames[_random.Next(_surnames.Count)];
            string fullName = $"{name} {surname}";

            string disease = _diseases[_random.Next(_diseases.Count)];

            int age = _random.Next(_ageStats[0], _ageStats[1]);

            return new PatientCard(fullName, age, disease);
        }

        private void FillNames() =>
            _names = new List<string>
            {
                "Геннадий",
                "Дмитрий",
                "Максим",
                "Александр",
                "Валерий",
                "Михаил"
            };

        private void FillSurnames() =>
            _surnames = new List<string>
            {
                "Немичев",
                "Величко",
                "Андреев",
                "Кузнецов",
                "Емельянов",
                "Киррилов",
                "Мамонов"
            };

        private void FillDiseases() =>
            _diseases = new List<string>
            {
                "рак",
                "простуда",
                "оспа",
                "геморрой",
                "пневмоноультрамикроскопиксиликоволканокониоз"
            };
    }

    class Menu
    {
        private const ConsoleKey MoveSelectionUp = ConsoleKey.UpArrow;
        private const ConsoleKey MoveSelectionDown = ConsoleKey.DownArrow;
        private const ConsoleKey ConfirmSelection = ConsoleKey.Enter;

        private ConsoleColor _backgroundColor = ConsoleColor.White;
        private ConsoleColor _foregroundColor = ConsoleColor.Black;

        private int _itemIndex = 0;
        private bool _isRunning;
        private string[] _items;

        private Dictionary<string, Action> _actions = new Dictionary<string, Action>();

        public Menu(Dictionary<string, Action> actions)
        {
            _actions = actions;
            _actions.Add("Выход", Exit);
            _items = _actions.Keys.ToArray();
        }

        public void Work()
        {
            _isRunning = true;

            while (_isRunning)
            {
                DrawItems();

                ReadKey();
            }
        }

        private void SetItemIndex(int index)
        {
            int lastIndex = _items.Length - 1;

            if (index > lastIndex)
                index = lastIndex;

            if (index < 0)
                index = 0;

            _itemIndex = index;
        }

        private void ReadKey()
        {
            switch (Console.ReadKey(true).Key)
            {
                case MoveSelectionDown:
                    SetItemIndex(_itemIndex + 1);
                    break;

                case MoveSelectionUp:
                    SetItemIndex(_itemIndex - 1);
                    break;

                case ConfirmSelection:
                    _actions[_items[_itemIndex]].Invoke();
                    break;
            }
        }

        private void DrawItems()
        {
            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < _items.Length; i++)
                if (i == _itemIndex)
                    WriteColoredText(_items[i]);
                else
                    Console.WriteLine(_items[i]);
        }

        private void WriteColoredText(string text)
        {
            Console.ForegroundColor = _foregroundColor;
            Console.BackgroundColor = _backgroundColor;

            Console.WriteLine(text);

            Console.ResetColor();
        }

        private void Exit() =>
            _isRunning = false;
    }
}
