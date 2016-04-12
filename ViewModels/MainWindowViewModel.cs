using Integrator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;


namespace Integrator.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        #region Commands
        public ICommand SimulateCommand { get; private set; }
        public ICommand DoubleClickCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        #endregion

        #region Input Properties
        public string[] CodingModes { get; set; }
        public int CodingIndex { get; set; }
        public string[] ModulationModes { get; set; }
        public int ModulationIndex { get; set; }

        public int FrameLength
        {
            get { return _frameLength; }
            set { _frameLength = value; OnPropertyChanged("FrameLength"); }
        }
        private int _frameLength;

        public int DecisionDepth
        {
            get { return _decisionDepth; }
            set { _decisionDepth = value; OnPropertyChanged("DecisionDepth"); }
        }
        private int _decisionDepth;

        public int SNR
        {
            get { return _snr; }
            set { _snr = value; OnPropertyChanged("SNR"); }
        }
        private int _snr;

        public string[] TransAuthors { get; set; }
        public string CurrTrans { get; set; }
        public string[] ReceivAuthors { get; set; }
        public string CurrReceiv { get; set; }
        #endregion

        #region Results Properties
        public KeyValuePair<string,double>[] Results { get; set; }

        private string ArchName { get; set; }
        public SimulationData MySimulationData { get; set; }
        public ObservableCollection<SimulationData> SimDataList { get; set; }
        public SimulationData SelectedData { get; set; }
        #endregion


        public MainWindowViewModel()
        {
            CodingModes = new string[3] { "Wersja 2", "Wersja 3", "Wersja 4" };
            CodingIndex = 0;
            ModulationModes = new string[2] { "8-PSK", "16-QAM" };
            ModulationIndex = 0;

            FrameLength = 2000;
            DecisionDepth = 15;
            SNR = 10;

            TransAuthors = new string[2] { "Jóźwiak - Frąckowiak", "Piekrasi - Kaszuba" };
            CurrTrans = TransAuthors[0];
            ReceivAuthors = new string[3] { "Kułacz - Zieliński", "Sienkiewicz - Knyrek", "Obuchowski - Szilke" };
            CurrReceiv = ReceivAuthors[0];

            ArchName = "SimulationArchive.xml";
            SimDataList = new ObservableCollection<SimulationData>();
            var _tempList = SimDataList;
            SimpleSerialization.MySerialization.Deserialize(ArchName, ref _tempList);
            SimDataList = _tempList;


            SimulateCommand = new RelayCommand(_ => Simulate());
            DoubleClickCommand = new RelayCommand(_ => { MySimulationData = SelectedData; UpdateUI(); });
            DeleteCommand = new RelayCommand(_ => { SimDataList.Remove(SelectedData); SerializeList(); });
        }

        private void RunTest()
        {
            var rnd = new Random();
            double srednia1 = 0;
            double srednia2 = 0;
            double wariancja1 = 0;
            double wariancja2 = 0;
            int count = 1000000;

            for (int i = 0; i < count; i++)
            {
                double x1, x2, w, y1, y2;
                do
                {
                    x1 = 2.0 * rnd.NextDouble() - 1.0;
                    x2 = 2.0 * rnd.NextDouble() - 1.0;
                    w = x1 * x1 + x2 * x2;
                } while (w >= 1.0);

                w = Math.Sqrt((-2.0 * Math.Log(w)) / w);
                y1 = x1 * w;
                y2 = x2 * w;

                srednia1 += y1 / count; srednia2 += y2 / count;
                wariancja1 += Math.Pow(y1, 2) / count;
                wariancja2 += Math.Pow(y2, 2) / count;
            }
            
        }

        public void Simulate()
        {
            MySimulationData = CreateSimulationData();
            var _supervisor = new Supervisor(MySimulationData);
            _supervisor.Simulate();

            SimDataList.Add(MySimulationData);
            SerializeList();
            UpdateUI();
        }

        private void UpdateUI()
        {
            Results = new KeyValuePair<string, double>[2];
            Results[0] = new KeyValuePair<string, double>("Błędne", MySimulationData.BitsLost);
            Results[1] = new KeyValuePair<string, double>("Poprawne", MySimulationData.BitsSend - MySimulationData.BitsLost);
            OnPropertyChanged("Results");
            OnPropertyChanged("MySimulationData");
        }

        private void SerializeList()
        {
            SimpleSerialization.MySerialization.Serialize(ArchName, SimDataList);
        }

        private SimulationData CreateSimulationData()
        {
            var tempSimulationData = new SimulationData
            {
                CodingMode = new SimulationData.Mode
                {
                    Name = CodingModes[CodingIndex],
                    Index = CodingIndex
                },
                ModulationMode = new SimulationData.Mode
                {
                    Name = ModulationModes[ModulationIndex],
                    Index = ModulationIndex
                },
                FrameLength = this.FrameLength,
                DecisionDepth = this.DecisionDepth,
                SNR = this.SNR,
                TransmitterAuthor = CurrTrans,
                ReceiverAuthor = CurrReceiv,
                BitsSend = 0,
                BitsLost = 0
            };


            return tempSimulationData;
        }

    }
}
