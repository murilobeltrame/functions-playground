using ClassLibrary.Entities;
using System;
using Xunit;

namespace Tests.ClassLibrary
{
    public class ProductShould
    {
        [Fact]
        public void BeInstantiated()
        {
            var product = new Product("some", "description");
            Assert.NotNull(product);
            Assert.NotEqual(Guid.Empty, product.Id);
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenPassingNullSku()
        {
            static void act() => new Product(null, "description");
            var ex = Assert.Throws<ArgumentNullException>(act);
            Assert.Equal("sku", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWhenPassingEmptySku()
        {
            static void act() => new Product(string.Empty, "description");
            var ex = Assert.Throws<ArgumentException>(act);
            Assert.Equal("sku", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentNullExceptionWhenPassingNullDescription()
        {
            static void act() => new Product("some", null);
            var ex = Assert.Throws<ArgumentNullException>(act);
            Assert.Equal("description", ex.ParamName);
        }

        [Fact]
        public void ThrowArgumentExceptionWhenPassingWhitespacesToDescription()
        {
            static void act() => new Product("some", "   ");
            var ex = Assert.Throws<ArgumentException>(act);
            Assert.Equal("description", ex.ParamName);
        }

        [Fact]
        public void BeCreatedFromAValidCmsProduct()
        {
            var cmsProduct = new CmsProduct("code", "name");
            var product = Product.FromCmsProduct(cmsProduct);
            Assert.NotEqual(Guid.Empty, product.Id);
            Assert.Equal(cmsProduct.Code, product.Sku);
            Assert.Equal(cmsProduct.Name, product.Description);
        }

        [Fact]
        public void BeCreatedFromAValidJson()
        {
            var sku = "some";
            var description = "description";
            var json = $"{{'sku':'{sku}', 'description':'{description}'}}";
            var product = Product.FromJson(json);
            Assert.NotEqual(Guid.Empty, product.Id);
            Assert.Equal(sku, product.Sku);
            Assert.Equal(description, product.Description);
        }
    }
}
