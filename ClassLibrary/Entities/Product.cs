using ClassLibrary.Shared.Interfaces;
using Newtonsoft.Json;
using System;
using System.Text;

namespace ClassLibrary.Entities
{
    public class Product: IEntity
    {
        public Product(string sku, string description)
        {
            if (sku == null) throw new ArgumentNullException(nameof(sku));
            if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("Cant be empty", nameof(sku));

            if (description == null) throw new ArgumentNullException(nameof(description));
            if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Cant be empty", nameof(description));


            Id = Guid.NewGuid();
            Sku = sku;
            Description = description;
        }

        public string Sku { get; protected set; }
        public string Description { get; protected set; }
        public Guid Id { get; protected set; }

        public static Product FromCmsProduct(CmsProduct product) => new Product(product.Code, product.Name);

        public byte[] ToBytes()
        {
            var json = JsonConvert.SerializeObject(this);
            return Encoding.UTF8.GetBytes(json);
        }

        public static Product FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Product>(json);
        }
    }
}
