using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
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
            ShowAboutWindow = new RelayCommand(ExecuteShowAboutWindow);
        }

        public ICommand LoadInput { get; private set; }
        public ICommand SaveOutput { get; private set; }
        public ICommand ShowAboutWindow { get; private set; }

        /// <summary>
        /// Represents answer for problem
        /// </summary>
        public IList<Triplet> Answer { get; private set; } = new BindingList<Triplet>();
       
        /// <summary>
        /// Shows is solution ready or not
        /// </summary>
        public bool CanSaveToFile
        {
            get => _canSaveToFile;
            set
            {
                _canSaveToFile = value;
                OnPropertyChanged();
            }
        }
        private string _inputText = "";
        private bool _canSaveToFile = false;

        private void Solve(string input)
        {

            var numbers = ReadInput(input);

            if (numbers.Count == 0)
                return;

            var answer = new List<Triplet>();

            numbers.Sort();
            int size = numbers.Count;

            for (var i = 0; i < size - 2; ++i) 
            {
                if (i > 0 && numbers[i] == numbers[i-1])
                    continue;

                int newTarget = -numbers[i];
                int head = i + 1;
                int tail = size - 1;

                while (head < tail)
                {
                    int twoSumValue = numbers[head] + numbers[tail];

                    if (twoSumValue == newTarget)
                    {
                        answer.Add(new Triplet()
                            {
                                FirstNumber = numbers[i],
                                SecondNumber = numbers[head],
                                ThirdNumber = numbers[tail]
                            }
                        );
                        while (head < tail && numbers[head] == numbers[head+1])
                            ++head;

                        while (head < tail && numbers[tail] == numbers[tail-1])
                            --tail;
                        
                        ++head;
                        --tail;
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
        private string GetKey(Triplet triplet)
        {
            string key = $"{triplet.FirstNumber.ToString()},{triplet.SecondNumber.ToString()}," +
                         $"{triplet.ThirdNumber.ToString()}";
            return key;
        }
        private List<int> ReadInput(string input)
        {
            var separator = new string[1] { "," };
            var stringNumbers = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            var intNumbers = new List<int>(stringNumbers.Length);
            foreach (var number in stringNumbers)
            {
                if (int.TryParse(number, out var num))
                {
                    intNumbers.Add(num);
                }
            }

            return intNumbers;
        }
        private void ExecuteLoadInput()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                _inputText = File.ReadAllText(filePath);
                Solve(_inputText);
            }
        }
        private void ExecuteSaveOutput()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Text file(*.txt)| *.txt"
            };

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
        private void ExecuteShowAboutWindow()
        {
            var message = "Your can open file that have input data. " +
                          "It should be .txt file with integers number separated by comma. For example:\n10,4,-3,0,4-1,21\n" + 
                          "And after that you can save solution to file.";
            MessageBox.Show( message, "About");
        }

        /// <summary>
        /// Fires whether property change value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Property changed event handler
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}