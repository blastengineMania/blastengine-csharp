using Newtonsoft.Json;
using System;
namespace Blastengine
{
    public class BlastengineObject
    {
        [JsonProperty("delivery_id")]
        public long DeliveryId;

        public BlastengineObject()
        {
        }
    }
}

