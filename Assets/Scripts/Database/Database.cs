using System;

namespace DatabaseLibrary
{
    public class Database
    {
        public double Lenght { get; set; }
        public double[] Conductivities { get; set; }
        public TimeSlice[] Measurings { get; set; }

        public Database()
        {
            Conductivities = Array.Empty<double>();
            Measurings = Array.Empty<TimeSlice>();
        }
    }
}