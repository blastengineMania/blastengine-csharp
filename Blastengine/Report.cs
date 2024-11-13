using System;
namespace Blastengine
{
	public class Report : Base
	{
        public Report(long id) : base()
        {
			DeliveryId = id;
		}

        public new async Task<Job> Get()
        {
            var Path = $@"/v1/deliveries/{DeliveryId}/analysis/report";
            var obj = await Client!.PostText(Path, "");
            return new Job(obj!.JobId, "Report");
        }
    }
}

