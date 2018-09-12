using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakacoinCore.Lib;

namespace VakacoinCore.Response.API
{
    public class ProducerSchedule : IEOAPI
    {
        public ProducerScheduleInner active { get; set; }
        public ProducerScheduleInner pending { get; set; }
        public ProducerScheduleInner proposed { get; set; }

        public VAKAAPIMetadata GetMetaData()
        {
            var meta = new VAKAAPIMetadata
            {
                uri = "v1/chain/get_producer_schedule"
            };

            return meta;
        }
    }

    public class ProducerScheduleInner
    {
        public int version;
        public List<ProducerScheduleProducerInfo> producers;
    }

    public class ProducerScheduleProducerInfo
    {
        public string producer_name;
        public string block_signing_key;
    }


}
