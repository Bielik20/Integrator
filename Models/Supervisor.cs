using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Integrator.Models
{
    class Supervisor
    {
        #region Cpp stuff
        const string transmitter1 = "nadajnik_JF.dll";
        [DllImport(transmitter1, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunTransmitter_JF(int[] inputData, int frameLength, double[] realData, double[] imagData, int codMode, int modMode);

        const string transmitter2 = "nadajnik_PK.dll";
        [DllImport(transmitter2, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunTransmitter_PK(int[] inputData, int frameLength, double[] realData, double[] imagData, int codMode, int modMode);

        //----------------------------------------------------------------------------------------------------------------------------------------------------

        const string receiver1 = "odbiornik_KZ.dll";
        [DllImport(receiver1, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunReceiver_KZ(int[] outcomeData, int frameLength, double[] realData, double[] imagData, int decDepth, int codMode, int modMode);

        const string receiver2 = "odbiornik_SK.dll";
        [DllImport(receiver2, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunReceiver_SK(int[] outcomeData, int frameLength, double[] realData, double[] imagData, int decDepth, int codMode, int modMode);

        const string receiver3 = "odbiornik_OS.dll";
        [DllImport(receiver3, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunReceiver_OS(int[] outcomeData, int frameLength, double[] realData, double[] imagData, int decDepth, int codMode, int modMode);
        #endregion

        #region Delegates
        public delegate void TransmitterDelegate(int[] inputData, int frameLength, double[] realData, double[] imagData, int codMode, int modMode);
        public static TransmitterDelegate RunTransmitter;

        public delegate void ReceiverDelegate(int[] outcomeData, int frameLength, double[] realData, double[] imagData, int decDepth, int codMode, int modMode);
        public static ReceiverDelegate RunReceiver;
        #endregion

        #region Properties
        private SimulationData MySimulationData { get; set; }
        private int[] InputData { get; set; }
        private int[] OutcomeData { get; set; }
        private double[] RealData { get; set; }
        private double[] ImagData { get; set; }
        private int SymbolLength { get; set; }
        private int MaxValue { get; set; }
        #endregion

        public Supervisor(SimulationData MySimulationData)
        {
            this.MySimulationData = MySimulationData;
            SetParameters();
            SetTransmitter();
            SetReceiver();
        }

        private void SetReceiver()
        {
            switch (MySimulationData.ReceiverAuthor)
            {
                case "Kułacz - Zieliński":
                    RunReceiver = RunReceiver_KZ;
                    break;

                case "Sienkiewicz - Knyrek":
                    RunReceiver = RunReceiver_SK;
                    break;

                case "Obuchowski - Szilke":
                    RunReceiver = RunReceiver_OS;
                    break;

                default:
                    break;
            }
        }

        private void SetTransmitter()
        {
            switch (MySimulationData.TransmitterAuthor)
            {
                case "Jóźwiak - Frąckowiak":
                    RunTransmitter = RunTransmitter_JF;
                    break;

                case "Piekrasi - Kaszuba":
                    RunTransmitter = RunTransmitter_PK;
                    break;

                default:
                    break;
            }
        }

        private void SetParameters()
        {
            switch (MySimulationData.ModulationMode.Index)
            {
                case 0:
                    SymbolLength = 2;
                    MaxValue = 4;
                    break;
                case 1:
                    SymbolLength = 3;
                    MaxValue = 8;
                    break;
                default:
                    break;
            }
        }

        public void Simulate()
        {
            while (MySimulationData.BitsLost < 100)
            {
                GenerateData();
                    int[] _tempInputData = new int[MySimulationData.FrameLength]; InputData.CopyTo(_tempInputData, 0);
                RunTransmitter(_tempInputData, MySimulationData.FrameLength, RealData, ImagData, MySimulationData.CodingMode.Index, MySimulationData.ModulationMode.Index);
                RollEngine.RollNoise(RealData, ImagData, MySimulationData.SNR);
                RunReceiver(OutcomeData, MySimulationData.FrameLength, RealData, ImagData, MySimulationData.DecisionDepth, MySimulationData.CodingMode.Index, MySimulationData.ModulationMode.Index);
                UpdateData();
            }
        }

        private void UpdateData()
        {
            MySimulationData.BitsSend += (MySimulationData.FrameLength - 2*MySimulationData.DecisionDepth) * SymbolLength;
            for (int i = MySimulationData.DecisionDepth; i < InputData.Count() - MySimulationData.DecisionDepth; i++)
            {
                var temp = InputData[i] ^ OutcomeData[i];
                MySimulationData.BitsLost += CalculateHammingWeight(temp);
            }
        }

        private int CalculateHammingWeight(int i)
        {
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
        }


        private void GenerateData()
        {
            InputData = new int[MySimulationData.FrameLength];
            OutcomeData = new int[MySimulationData.FrameLength];
            RealData = new double[MySimulationData.FrameLength];
            ImagData = new double[MySimulationData.FrameLength];
            RollEngine.Roll(InputData, MaxValue);
        }
    }
}
