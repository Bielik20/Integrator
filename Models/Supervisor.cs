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
        const string transmitter = "transmitter.dll";
        [DllImport(transmitter, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunTransmitter(int[] inputData, int frameLength, double[] realData, double[] imagData, int modDepth, int codMode, int modMode);

        const string receiver = "receiver.dll";
        [DllImport(receiver, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RunReceiver(int[] outcomeData, int frameLength, double[] realData, double[] imagData, int modDepth, int codMode, int modMode);
        #endregion

        #region Properties
        private SimulationData MySimulationData { get; set; }
        private int[] InputData { get; set; }
        private int[] OutcomeData { get; set; }
        private double[] RealData { get; set; }
        private double[] ImagData { get; set; }
        private int SymbolLength { get; set; }
        #endregion

        public Supervisor(SimulationData MySimulationData)
        {
            this.MySimulationData = MySimulationData;
        }

        public void Simulate()
        {
            while (MySimulationData.BitsLost < 100)
            {
                GenerateData();
                //RunTransmitter(InputData, MySimulationData.FrameLength, RealData, ImagData, MySimulationData.ModulationDepth, MySimulationData.CodingMode.Index, MySimulationData.ModulationMode.Index);
                //RollEngine.RollNoise(RealData, ImagData, MySimulationData.SNR);
                //RunReceiver(OutcomeData, MySimulationData.FrameLength, RealData, ImagData, MySimulationData.ModulationDepth, MySimulationData.CodingMode.Index, MySimulationData.ModulationMode.Index);
                RollEngine.Roll_4(OutcomeData);
                UpdateData();
            }
        }

        private void UpdateData()
        {
            MySimulationData.BitsSend += MySimulationData.FrameLength * SymbolLength;
            for (int i = 0; i < InputData.Count(); i++)
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
            RealData = new double[MySimulationData.FrameLength + MySimulationData.ModulationDepth];
            ImagData = new double[MySimulationData.FrameLength + MySimulationData.ModulationDepth];

            switch (MySimulationData.CodingMode.Index)
            {
                case 0:
                    RollEngine.Roll_4(InputData);
                    SymbolLength = 2;
                    break;
                case 1:
                    RollEngine.Roll_16(InputData);
                    SymbolLength = 4;
                    break;
                case 2:
                    RollEngine.Roll_64(InputData);
                    SymbolLength = 6;
                    break;
                default:
                    break;
            }
        }
    }
}
