using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Models
{
    [DataContract]
    class SimulationData
    {
        [DataContract]
        public struct Mode
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public int Index { get; set; }
        }

        [DataMember]
        public Mode CodingMode { get; set; }
        [DataMember]
        public Mode ModulationMode { get; set; }
        [DataMember]
        public int FrameLength { get; set; }
        [DataMember]
        public int DecisionDepth { get; set; }
        [DataMember]
        public int SNR { get; set; }
        [DataMember]
        public string TransmitterAuthor { get; set; }
        [DataMember]
        public string ReceiverAuthor { get; set; }
        [DataMember]
        public int BitsSend { get; set; }
        [DataMember]
        public int BitsLost { get; set; }
        public double ErrorRate { get { return Math.Round(System.Convert.ToDouble(BitsLost) / System.Convert.ToDouble(BitsSend),9); } }


    }
}
