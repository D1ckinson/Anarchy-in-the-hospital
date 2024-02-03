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

            ActionBuilder actionBuilder = new ActionBuilder(patientCards, cardFabrik.GiveDiseases());
            MainMenu menu = new MainMenu(actionBuilder.GiveActions());

            menu.Work();
        }
    }

    class ActionBuilder
    {
        private List<PatientCard> _patientCards;
        private List<string> _diseases;

        public ActionBuilder(List<PatientCard> patientCards, List<string> diseases)
        {
            _patientCards = patientCards;
            _diseases = diseases;

            DrawCards(_patientCards);
        }

        public Dictionary<string, Action> GiveActions() =>
            new Dictionary<string, Action>
            {
                { "Отсортировать по имени", SortByName },
                { "Отсортировать по болезни", SortByDisease },
                { "Отсортировать по возрасту", SortByAge }
            };

        private void SortByName() =>
            DrawCards(_patientCards.OrderBy(card => card.Name));

        private void SortByAge() =>
            DrawCards(_patientCards.OrderBy(card => card.Age));

        private void SortByDisease()
        {
            string disease = new DiseaseMenu(_diseases).Work();

            DrawCards(_patientCards.FindAll(card => card.Disease == disease));

            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey(true);

            DrawCards(_patientCards);
        }

        private void DrawCards(IEnumerable<PatientCard> cards)
        {
            int indent = 3;
            int spaceLineSize = 100;
            int cursorPositionY = GiveActions().Count + indent;

            char space = ' ';

            Console.CursorTop = cursorPositionY;

            for (int i = 0; i < _patientCards.Count; i++)
                Console.WriteLine(new string(space, spaceLineSize));

            Console.SetCursorPosition(0, cursorPositionY);

            foreach (PatientCard card in cards)
                Console.WriteLine($"{card.Name}, возраст - {card.Age}, болезнь - {card.Disease}");
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
            _diseases = GiveDiseases();
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

        public List<string> GiveDiseases() =>
            new List<string>
            {
                "рак",
                "простуда",
                "оспа",
                "геморрой",
                "пневмоноультрамикроскопиксиликоволканокониоз"
            };

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
    }

    class MainMenu : Menu
    {
        private Dictionary<string, Action> _actions = new Dictionary<string, Action>();

        public MainMenu(Dictionary<string, Action> actions)
        {
            _actions = actions;
            _actions.Add("Выход", Exit);
            _items = _actions.Keys.ToArray();
        }

        protected override void ConfirmActionSelection()
        {
            base.ConfirmActionSelection();

            _actions[_items[_itemIndex]].Invoke();
        }
    }

    class DiseaseMenu : Menu
    {
        public DiseaseMenu(IEnumerable<string> diseases) =>
            _items = diseases.ToArray();

        public new string Work()
        {
            base.Work();

            return _items[_itemIndex];
        }

        protected override void ConfirmActionSelection()
        {
            base.ConfirmActionSelection();

            Exit();
        }
    }

    abstract class Menu
    {
        private const ConsoleKey MoveSelectionUp = ConsoleKey.UpArrow;
        private const ConsoleKey MoveSelectionDown = ConsoleKey.DownArrow;
        private const ConsoleKey ConfirmSelection = ConsoleKey.Enter;

        private ConsoleColor _backgroundColor = ConsoleColor.White;
        private ConsoleColor _foregroundColor = ConsoleColor.Black;

        protected string[] _items;
        protected int _itemIndex = 0;

        private bool _isRunning;

        public void Work()
        {
            _isRunning = true;

            while (_isRunning)
            {
                DrawItems();

                ReadKey();
            }
        }

        protected virtual void ConfirmActionSelection() =>
            EraseText();

        protected void Exit() =>
            _isRunning = false;

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
                    ConfirmActionSelection();
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

        private void EraseText()
        {
            int spaceLineSize = 60;
            char space = ' ';

            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < _items.Length; i++)
                Console.WriteLine(new string(space, spaceLineSize));
        }
    }
}
