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

        public int ModulationDepth
        {
            get { return _modulationDepth; }
            set { _modulationDepth = value; OnPropertyChanged("ModulationDepth"); }
        }
        private int _modulationDepth;

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
            CodingModes = new string[3] { "Coding Mode 1" , "Coding Mode 2" , "Coding Mode 3" };
            CodingIndex = 0;
            ModulationModes = new string[3] { "Modulation Mode 1", "Modulation Mode 2", "Modulation Mode 3" };
            ModulationIndex = 1;

            FrameLength = 200;
            ModulationDepth = 15;
            SNR = 10;

            TransAuthors = new string[2] { "Transmiter 1", "Transmiter 2" };
            CurrTrans = TransAuthors[0];
            ReceivAuthors = new string[3] { "Receiver 1", "Receiver 2", "Receiver 3" };
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
                ModulationDepth = this.ModulationDepth,
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
