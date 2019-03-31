using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;

namespace _3SumProblem
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            LoadInput = new RelayCommand(ExecuteLoadInput);
            SaveOutput = new RelayCommand(ExecuteSaveOutput);
        }

        public ICommand LoadInput { get; private set; }
        public ICommand SaveOutput { get; private set; }

        public IList<Triplet> Answer { get; private set; } = new BindingList<Triplet>();
        private string _inputText = "";
        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged();
            }
        }
        private bool _canSaveToFile = false;//Set here false;
        public bool CanSaveToFile
        {
            get => _canSaveToFile;
            set
            {
                _canSaveToFile = value;
                OnPropertyChanged();
            }
        }

        private void ExecuteLoadInput()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                InputText = File.ReadAllText(filePath);
                solve(InputText);
            }
        }

        private void solve(string input)
        {
            var separator = new string[1] {","};
            var stringNumbers = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            var intNumbers = new List<int>(stringNumbers.Length);
            foreach (var number in stringNumbers)
            {
                if (int.TryParse(number, out var num))
                {
                    intNumbers.Add(num);
                }
            }

            if (intNumbers.Count == 0)
                return;

            int target = 0;
            var answer = new List<Triplet>();
            var keys = new HashSet<string>();

            intNumbers.Sort();
            int size = intNumbers.Count;

            for (int i = 0; i < size - 2; ++i) 
            {
                int[] trialTriplet = new int[3];
                trialTriplet[0] = intNumbers[i];

                int newTarget = target - trialTriplet[0];
                int head = i + 1;
                int tail = size - 1;

                while (head < tail)
                {
                    trialTriplet[1] = intNumbers[head];
                    trialTriplet[2] = intNumbers[tail];

                    int twoSumValue = trialTriplet[1] + trialTriplet[2];

                    if (twoSumValue == newTarget)
                    {
                        string key = getKey(trialTriplet, 3);

                        if (!keys.Contains(key))
                        {
                            keys.Add(key);

                            IList<int> triplet = new List<int>();

                            for (int j = 0; j < 3; j++)
                                triplet.Add(trialTriplet[j]);

                            answer.Add(new Triplet()
                            {
                                FirstNumber = triplet[0],
                                SecondNumber = triplet[1],
                                ThirdNumber = triplet[2]
                            });
                        }

                        head++;
                        tail--;

                    }
                    else if (twoSumValue > newTarget)
                    {
                        tail--;
                    }
                    else if (twoSumValue < newTarget)
                    {
                        head++;
                    }
                }
            }

            Answer = answer;
            CanSaveToFile = true;
            OnPropertyChanged($"Answer");
        }
        private static string getKey(int[] arr, int len)
        {
            string key = "";

            for (int j = 0; j < 3; j++)
            {
                key += arr[j].ToString();
                key += ",";
            }

            return key;
        }

        private void ExecuteSaveOutput()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file(*.txt)| *.txt";

            if (saveFileDialog.ShowDialog() == true)
            {
                var filePath = saveFileDialog.FileName;
                string output = "";
                foreach (var triple in Answer)
                {
                    output += $"{triple.FirstNumber}  {triple.SecondNumber}  {triple.ThirdNumber}\n";
                }
                File.WriteAllText(filePath, output);
            }
                
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}