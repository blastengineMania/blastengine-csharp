using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Blastengine
{
    public class IgnorePropertiesResolver : DefaultContractResolver
    {
        private readonly HashSet<string> _propertiesToIgnore;

        public IgnorePropertiesResolver(IEnumerable<string> propNamesToIgnore)
        {
            _propertiesToIgnore = new HashSet<string>(propNamesToIgnore);
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            return properties.Where(p => !_propertiesToIgnore.Contains(p.PropertyName)).ToList();
        }
    }
}

