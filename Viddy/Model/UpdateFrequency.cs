using System;
using System.Linq;
using System.Reflection;
using Viddy.Core.Extensions;

namespace Viddy.Model
{
    public enum UpdateFrequency
    {
        [Frequency(30)]
        ThirtyMinutes,
        [Frequency(60)]
        OneHour,
        [Frequency(360)]
        SixHours,
        [Frequency(720)]
        TwelveHours,
        [Frequency(1440)]
        OneDay
    }

    public class Frequency : Attribute
    {
        public int Minutes;

        public Frequency(int minutes)
        {
            Minutes = minutes;
        }
    }

    public static class FrequencyExtensions
    {
        public static int GetMinutes(this UpdateFrequency en)
        {
            var type = en.GetType();
            var memInfo = type.GetTypeInfo().DeclaredMembers;

            var attrs = memInfo.FirstOrDefault(x => x.Name == en.ToString());
            if (attrs != null)
            {
                var attribute = attrs.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(Frequency));
                if (attribute != null)
                {
                    var valueString = attribute.ConstructorArguments[0].Value.ToString();
                    int result;
                    return int.TryParse(valueString, out result) ? result : 0;
                }
            }

            return 0;
        }

        public static UpdateFrequency GetFrequency(this int minutes)
        {
            var list = Enum.GetValues(typeof (UpdateFrequency)).ToList<UpdateFrequency>();
            var result = list.FirstOrDefault(x => x.GetMinutes() == minutes);
            return result;
        }
    }
}
