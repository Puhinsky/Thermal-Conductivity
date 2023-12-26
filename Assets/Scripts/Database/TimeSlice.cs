using System.Globalization;
using System.Linq;

namespace DatabaseLibrary
{
    public class TimeSlice
    {
        public double Time { get; private set; }
        public double[] Temperatures { get; private set; }

        public TimeSlice(double time, double[] temperatures)
        {
            Time = time;
            Temperatures = temperatures;
        }

        public override string ToString()
        {
            return string.Join(" ", Time.ToString(), string.Join(" ",Temperatures.Select(x => x.ToString(CultureInfo.InvariantCulture))));
        }
    }
}
