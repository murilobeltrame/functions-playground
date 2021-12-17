using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ClassLibrary.Entities
{
    public class LegacyProduct
    {
        public LegacyProduct(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public string Code { get; set; }
        public string Name { get; set; }

        public static async Task<LegacyProduct> FromStreamAsync(Stream productStream)
        {
            if (productStream == null) throw new ArgumentNullException(nameof(productStream));
            var productText = await new StreamReader(productStream).ReadToEndAsync();
            return JsonConvert.DeserializeObject<LegacyProduct>(productText);
        }
    }
}
