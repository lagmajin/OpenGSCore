using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class LoadingGauge
    {
        public float Gauge { get; set; } = 0;

        public LoadingGauge(float gauge = 0.0f)
        {


            if (Gauge > 100.0f)
            {
                Gauge = 100.0f;

            }
            



        }

        public void SetPercent(float percent)
        {
            if (Gauge > 100f)
            {

            }
            else
            {
                Gauge = percent;
            }
        }
        public void Full()
        {

        }

        public bool IsComplete()
        {
            return Gauge > 100.0f;
        }

        public void Clear()
        {
            Gauge = 0.0f;

            
        }

        public string ToString()
        {
            return "Percent";
        }

        public static implicit  operator float (LoadingGauge gauge)
        {

            return gauge.Gauge;
        }

        public static implicit operator LoadingGauge(float gauge)
        {
            return new LoadingGauge(gauge);
        }

    }
}
