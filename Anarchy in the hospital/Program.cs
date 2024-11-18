using System;
using System.Collections.Generic;
using System.Linq;

namespace Anarchy_in_the_hospital
{
    internal class Program
    {
        static void Main()
        {
            HospitalFactory hospitalFactory = new HospitalFactory();
            Hospital hospital = hospitalFactory.Create();

            hospital.Work();
        }
    }

    class HospitalFactory
    {
        private PatientFactory _patientFactory = new PatientFactory();

        public Hospital Create() =>
            new Hospital(_patientFactory.Create(), _patientFactory.GetDiseases());
    }

    class Hospital
    {
        private List<Patient> _patients;
        private List<string> _diseases;

        public Hospital(List<Patient> patients, List<string> diseases)
        {
            _patients = patients;
            _diseases = diseases;
        }

        public void Work()
        {
            const string SortByNameCommand = "1";
            const string SortByAgeCommand = "2";
            const string SortByDiseaseCommand = "3";
            const string ExitCommand = "4";

            bool isWork = true;
            List<Patient> patients = _patients.ToList();

            while (isWork)
            {
                ShowPatientsInfo(patients);

                Console.WriteLine($"\n" +
                    $"{SortByNameCommand} - отсортировать больных по имени\n" +
                    $"{SortByAgeCommand} - отсортировать больных по возрасту\n" +
                    $"{SortByDiseaseCommand} - отсортировать больных по болезни\n" +
                    $"{ExitCommand} - выйти\n");

                switch (UserUtils.ReadString("Введите команду:"))
                {
                    case SortByNameCommand:
                        patients = _patients.OrderBy(patient => patient.Name).ToList();
                        break;

                    case SortByAgeCommand:
                        patients = _patients.OrderBy(patient => patient.Age).ToList();
                        break;

                    case SortByDiseaseCommand:
                        patients = SortByDisease();
                        break;

                    case ExitCommand:
                        isWork = false;
                        break;

                    default:
                        Console.WriteLine("Такой команды нет");
                        break;
                }
            }
        }

        private void ShowPatientsInfo(List<Patient> patients)
        {
            Console.WriteLine("\nПациенты:");
            patients.ForEach(patient => patient.ShowInfo());
        }

        private List<Patient> SortByDisease()
        {
            ShowDiseases();
            string disease = UserUtils.ReadString("Напишите название болезни:").ToLower();

            IEnumerable<Patient> patients = _patients.Where(patient => patient.Disease.ToLower() == disease);

            if (patients.Count() == 0)
            {
                Console.WriteLine("Пациентов с такой болезнью нет");

                return _patients;
            }

            return patients.ToList();
        }

        private void ShowDiseases()
        {
            Console.WriteLine("\nБолезни пациентов:");
            _diseases.ForEach(disease => Console.WriteLine(disease));
            Console.WriteLine();
        }
    }

    class Patient
    {
        public Patient(string name, int age, string disease)
        {
            Name = name;
            Age = age;
            Disease = disease;
        }

        public string Name { get; }
        public int Age { get; }
        public string Disease { get; }

        public void ShowInfo()
        {
            const int ShortOffset = -5;
            const int LongOffset = -20;

            Console.WriteLine($"{Name,LongOffset}Возраст: {Age,ShortOffset} Заболевание:{Disease}");
        }
    }

    class PatientFactory
    {
        public List<Patient> Create()
        {
            int patientsQuantity = 10;
            int[] ageStats = { 20, 60 };
            List<Patient> patients = new List<Patient>();

            for (int i = 0; i < patientsQuantity; i++)
            {
                string fullName = GetFullName();
                string disease = UserUtils.GenerateRandomValue(GetDiseases());
                int age = UserUtils.GenerateStat(ageStats);

                patients.Add(new Patient(fullName, age, disease));
            }

            return patients;
        }

        public List<string> GetDiseases() =>
            new List<string>
            {
                "рак",
                "простуда",
                "оспа",
                "геморрой"
            };

        private string GetFullName()
        {
            string name = UserUtils.GenerateRandomValue(GetNames());
            string surname = UserUtils.GenerateRandomValue(GetSurnames());

            return $"{name} {surname}";
        }

        private List<string> GetNames() =>
            new List<string>
            {
                "Геннадий",
                "Дмитрий",
                "Максим",
                "Александр",
                "Валерий",
                "Михаил"
            };

        private List<string> GetSurnames() =>
            new List<string>
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

    static class UserUtils
    {
        private static Random s_random = new Random();

        public static string ReadString(string text)
        {
            Console.Write(text);

            return Console.ReadLine();
        }

        public static int ReadInt(string parameter)
        {
            int number;
            string input;

            do
            {
                input = ReadString(parameter);
            }
            while (int.TryParse(input, out number) == false);

            return number;
        }

        public static T GenerateRandomValue<T>(IEnumerable<T> values)
        {
            int index = s_random.Next(values.Count());

            return values.ElementAt(index);
        }

        public static int GenerateStat(int[] stats)
        {
            int maxLength = 2;

            if (stats.Length != maxLength)
            {
                throw new ArgumentException("Массив stats должен содержать ровно 2 элемента.");
            }

            return s_random.Next(stats[0], stats[1] + 1);
        }

        public static bool IsIndexInRange(int index, int maxIndex) =>
            index >= 0 && index <= maxIndex;
    }
}
